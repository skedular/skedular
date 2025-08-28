import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, PushToRight, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { Loading } from '@/components/loading';
import { locationFeatureImageHeight, locationFeatureImageWidth } from '@/components/location';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags } from '@/components/organization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { FeatureBox, LeftSidePanel, RightSidePanel, TwoSideVerticalWizard } from '@/components/wizard';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addMarketplaceLocation_addLocationMutation } from '@/queries/__generated__/addMarketplaceLocation_addLocationMutation.graphql';
import type { addMarketplaceLocation_rootQuery } from '@/queries/__generated__/addMarketplaceLocation_rootQuery.graphql';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import GridViewIcon from '@mui/icons-material/GridView';
import LocalCafeIcon from '@mui/icons-material/LocalCafe';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
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
  query addMarketplaceLocation_rootQuery($organizationUniqueAlphanumericName: String!, $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]) {
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      type {
        type
      }
    }
    ...multipleChoicesLocationTags_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<addMarketplaceLocation_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelLabel?: string;
  createLabel?: string;
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

const AddMarketplaceLocation = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = usePreloadedQuery<addMarketplaceLocation_rootQuery>(RootQuery, queryReference);

  const [commitAddLocation] = useMutation<addMarketplaceLocation_addLocationMutation>(graphql`
    mutation addMarketplaceLocation_addLocationMutation($input: AddLocationInput!) @raw_response_type {
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
          organizationUniqueAlphanumericName,
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
    <TwoSideVerticalWizard>
      <LeftSidePanel
        title="Manage Your Co-Working Space Locations"
        description="Set up your physical locations so members can easily find, book, and interact with your spaces. Whether you have one hub or multiple branches, this helps streamline resource scheduling and visibility across your co-working network."
      >
        <FeatureBox
          icon={<MeetingRoomIcon sx={{ color: '#4CAF50', fontSize: 40 }} />}
          title="Flexible Room Booking"
          subtitle="Enable members to reserve meeting rooms, hot desks, or private offices."
        />
        <FeatureBox
          icon={<AccessTimeIcon sx={{ color: '#FF9800', fontSize: 40 }} />}
          title="Operating Hours"
          subtitle="Set the daily open and close hours to control when members can access the space."
        />
        <FeatureBox
          icon={<LocalCafeIcon sx={{ color: '#795548', fontSize: 40 }} />}
          title="Location Amenities"
          subtitle="List available amenities like Wi-Fi, coffee, printers, and parking."
        />
        <FeatureBox
          icon={<LocationOnIcon sx={{ color: '#F44336', fontSize: 40 }} />}
          title="Map & Directions"
          subtitle="Add an address and map to help members find your space easily."
        />
        <FeatureBox
          icon={<GridViewIcon sx={{ color: '#3F51B5', fontSize: 40 }} />}
          title="Multi-Zone Support"
          subtitle="Create zones within a location for better desk and room segmentation."
        />
      </LeftSidePanel>
      <RightSidePanel
        title="Create Your Co-Working Location"
        description="Let's get your new location set up with the essential details. Add basic info so members can find, book, and enjoy your space with ease."
      >
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
              <Divider />

              <FormFieldLabel label="Feature image">
                <StackColumn>
                  {primaryFeatureImage?.thumbnail && primaryFeatureImage.original.height && primaryFeatureImage.original.width && (
                    <Image src={primaryFeatureImage.original.url} height={primaryFeatureImage.original.height} width={primaryFeatureImage.original.width} alt="" />
                  )}
                  <ImageFileUploaderWithCropper
                    defaultAspectRatio={locationFeatureImageWidth / locationFeatureImageHeight}
                    onUploadCompleted={handleFeatureImageUploadCompleted}
                    helperText="Upload a high-quality image that best represents your co-working space. This will appear in search results and marketing pages."
                  />
                </StackColumn>
              </FormFieldLabel>

              <FormFieldLabel label="Name" required={requiredFields.name}>
                <TextField
                  name="name"
                  required={requiredFields.name}
                  helperText={
                    <HelperText text="Enter the public name of your co-working location. This will be visible in the marketplace and should clearly represent your space." />
                  }
                />
              </FormFieldLabel>

              <FormFieldLabel label="About" required={requiredFields.about}>
                <TextField
                  name="about"
                  required={requiredFields.about}
                  multiline
                  rows={3}
                  helperText={
                    <HelperText text="Write a brief description of your co-working location. Highlight what makes it unique and the type of professionals or businesses it caters to." />
                  }
                />
              </FormFieldLabel>

              <FormFieldLabel label="Timezone" required={requiredFields.timezone}>
                <SingleChoinceTimezone
                  name="timezone"
                  required={requiredFields.timezone}
                  helperText="Select the local timezone of this location to ensure accurate scheduling and availability for bookings."
                />
              </FormFieldLabel>

              <FormFieldLabel label="Email" required={requiredFields.contactEmail}>
                <TextField
                  name="contactEmail"
                  required={requiredFields.contactEmail}
                  helperText={<HelperText text="Enter a public contact email for this location so visitors and potential members can get in touch easily." />}
                />
              </FormFieldLabel>

              <FormFieldLabel label="Phone Number" required={requiredFields.contactPhone}>
                <TextField
                  name="contactPhone"
                  required={requiredFields.contactPhone}
                  helperText={<HelperText text="Provide a phone number where your co-working space can be reached for inquiries or support." />}
                />
              </FormFieldLabel>

              {rootData.organization?.type.type === 'MARKETPLACE' && (
                <FormFieldLabel label="Location Tags" required={requiredFields.locationTagIds}>
                  <MultipleChoicesLocationTags
                    rootDataRelay={rootData}
                    name="locationTagIds"
                    required={requiredFields.locationTagIds}
                    organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                  />
                </FormFieldLabel>
              )}

              <StackRow>
                <Button variant="contained" sx={defaultButtonStyle} onClick={handleCloseClick}>
                  <BodyIconTypography label={cancelLabel ?? 'Cancel'} invertDefaultColor={paletteMode === 'dark'} />
                </Button>
                <PushToRight />

                <Button variant="contained" type="submit" sx={{ textTransform: 'none' }} color="primary">
                  <BodyIconTypography label={createLabel ?? 'Add'} invertDefaultColor={paletteMode === 'dark'} />
                </Button>
              </StackRow>
            </FormStackColumn>
          )}
        />{' '}
      </RightSidePanel>
    </TwoSideVerticalWizard>
  );
};

const MemoAddMarketplaceLocation = memo(AddMarketplaceLocation);

type RelayProps = {
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

const AddMarketplaceLocationWithRelay = ({ onReloadRequired, organizationUniqueAlphanumericName, onAdded, onCancel, cancelLabel, createLabel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addMarketplaceLocation_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
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
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

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
      <MemoAddMarketplaceLocation
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        onAdded={onAdded}
        onCancel={onCancel}
        cancelLabel={cancelLabel}
        createLabel={createLabel}
      />
    </ErrorBoundary>
  );
};

export default memo(AddMarketplaceLocationWithRelay);
