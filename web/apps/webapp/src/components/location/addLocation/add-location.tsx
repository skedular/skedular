import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { Loading } from '@/components/loading';
import { locationFeatureImageHeight, locationFeatureImageWidth } from '@/components/location';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags } from '@/components/organization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addLocation_addLocationMutation } from '@/queries/__generated__/addLocation_addLocationMutation.graphql';
import type { addLocation_rootQuery } from '@/queries/__generated__/addLocation_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import Image from 'next/image';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';

const RootQuery = graphql`
  query addLocation_rootQuery($organizationId: String!, $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]) {
    organization(id: $organizationId) {
      type {
        type
      }
    }
    ...multipleChoicesLocationTags_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<addLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  onAdded: (locationId: string) => void;
  onCancel: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  locationTagIds: string[];
  contactEmail: string | null;
  contactPhone: string | null;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  locationTagIds: array().nullable(),
  contactEmail: string()
    .nullable()
    .email(({ value }) => `${value} is not a valid email`),
  contactPhone: string().nullable(),
});

const AddLocation = ({ queryReference, onReloadRequired, organizationId, onAdded, onCancel, addLabel, showDismiss }: Props) => {
  const rootData = usePreloadedQuery<addLocation_rootQuery>(RootQuery, queryReference);

  const [commitAddLocation] = useMutation<addLocation_addLocationMutation>(graphql`
    mutation addLocation_addLocationMutation($input: AddLocationInput!) @raw_response_type {
      addLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          contactEmail
          contactPhone
          primaryFeatureImage {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
          locationTags {
            uniqueId
            name
            color
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse>();

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleLocationAddClick = ({ name, about, timezone, contactEmail, contactPhone, locationTagIds }: LocationDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding location '${name}'...`} />, infoNotificationOptions);
    const finalPrimaryFeatureImage = primaryFeatureImage
      ? {
          original: primaryFeatureImage.original
            ? { url: primaryFeatureImage.original.url, height: primaryFeatureImage.original.height, width: primaryFeatureImage.original.width }
            : null,
          thumbnail: primaryFeatureImage.thumbnail
            ? { url: primaryFeatureImage.thumbnail.url, height: primaryFeatureImage.thumbnail.height, width: primaryFeatureImage.thumbnail.width }
            : null,
        }
      : null;

    commitAddLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          name,
          about,
          organizationId,
          timezone,
          contactEmail,
          contactPhone,
          primaryFeatureImage: finalPrimaryFeatureImage,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new location '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location ${name} added.`} />,
        });

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new location '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addLocation: {
          location: {
            id,
            name,
            about,
            timezone,
            contactEmail,
            contactPhone,
            primaryFeatureImage: finalPrimaryFeatureImage,
            locationTags: [],
          },
        },
      },
    });
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setPrimaryFeatureImage(response);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Location">
          <Form
            onSubmit={handleLocationAddClick}
            initialValues={{
              name: '',
              about: '',
              timezone: '',
              locationTagIds: [],
              contactEmail: '',
              contactPhone: '',
            }}
            validate={validateLocationDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Location Setup" />
                  <BodyIconTypography label="Add your location name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Feature image">
                    <StackColumn>
                      {primaryFeatureImage?.thumbnail && primaryFeatureImage.original.height && primaryFeatureImage.original.width && (
                        <Image src={primaryFeatureImage.original.url} height={primaryFeatureImage.original.height} width={primaryFeatureImage.original.width} alt="" />
                      )}
                      <ImageFileUploaderWithCropper
                        defaultAspectRatio={locationFeatureImageWidth / locationFeatureImageHeight}
                        previewImageHeight={locationFeatureImageHeight}
                        previewImageWidth={locationFeatureImageWidth}
                        onUploadCompleted={handleFeatureImageUploadCompleted}
                      />
                    </StackColumn>
                  </FormFieldLabel>

                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Timezone">
                    <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                  </FormFieldLabel>

                  {rootData.organization?.type.type === 'MARKETPLACE' && (
                    <FormFieldLabel label="Location Tags">
                      <MultipleChoicesLocationTags rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} organizationId={organizationId} />
                    </FormFieldLabel>
                  )}

                  <SectionIconTypography label="Contact Details" />
                  <BodyIconTypography label="Edit your location contact details" />
                  <Divider />

                  <FormFieldLabel label="Email">
                    <TextField name="contactEmail" required={requiredFields.contactEmail} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Phone Number">
                    <TextField name="contactPhone" required={requiredFields.contactPhone} />
                  </FormFieldLabel>
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <StackRow>
                    {showDismiss && (
                      <Button variant="contained" sx={defaultButtonStyle} onClick={handleCloseClick}>
                        <BodyIconTypography label="Dismiss" invertDefaultColor={paletteMode === 'dark'} />
                      </Button>
                    )}
                    <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                      <BodyIconTypography label={addLabel ?? 'Add'} invertDefaultColor={paletteMode === 'dark'} />
                    </Button>
                  </StackRow>
                </StackColumn>
              </FormStackColumn>
            )}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

const MemoAddLocation = memo(AddLocation);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  onAdded: (locationId: string) => void;
  onCancel: () => void;
  addLabel?: string;
  showDismiss: boolean;
};

const AddLocationWithRelay = ({ onReloadRequired, organizationId, onAdded, onCancel, addLabel, showDismiss }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        multipleChoicesLocationTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddLocation
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        onAdded={onAdded}
        onCancel={onCancel}
        addLabel={addLabel}
        showDismiss={showDismiss}
      />
    </ErrorBoundary>
  );
};

export default memo(AddLocationWithRelay);
