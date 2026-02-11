import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, HelperText, PushToRight, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { DeleteIcon } from '@/components/icons';
import { Loading } from '@/components/loading';
import { SingleChoiceLocationType } from '@/components/location';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags } from '@/components/organization';
import { RelayError, toRootError } from '@/components/relayError';
import { FeatureBox, LeftSidePanel, RightSidePanel, TwoSideVerticalWizard } from '@/components/wizard';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { addPrivateLocation_addLocationMutation, LocationType } from '@/queries/__generated__/addPrivateLocation_addLocationMutation.graphql';
import type { addPrivateLocation_rootQuery } from '@/queries/__generated__/addPrivateLocation_rootQuery.graphql';
import ApartmentIcon from '@mui/icons-material/Apartment';
import ChairAltIcon from '@mui/icons-material/ChairAlt';
import EventNoteIcon from '@mui/icons-material/EventNote';
import LockOpenIcon from '@mui/icons-material/LockOpen';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
import VisibilityIcon from '@mui/icons-material/Visibility';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, object, string } from 'yup';

const RootQuery = graphql`
  query addPrivateLocation_rootQuery($organizationUniqueAlphanumericName: String!, $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]) {
    emailsToShowLatestCapabilities
    me {
      emails
    }
    organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
      type {
        type
      }
    }
    ...multipleChoicesLocationTags_query
    ...singleChoiceLocationType_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<addPrivateLocation_rootQuery, Record<string, unknown>>;
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
  type: string;
  locationTagIds: string[];
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  category: string().required('Category is required'),
  locationTagIds: array().nullable(),
});

const AddPrivateLocation = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, onAdded, onCancel, cancelLabel, createLabel }: Props) => {
  const rootData = usePreloadedQuery<addPrivateLocation_rootQuery>(RootQuery, queryReference);

  const [commitAddLocation] = useMutation<addPrivateLocation_addLocationMutation>(graphql`
    mutation addPrivateLocation_addLocationMutation($input: AddLocationInput!) @raw_response_type {
      addLocation(input: $input) {
        location {
          id
          name
          about
          timezone
          type {
            type
            name
          }
          featureImages {
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
            id
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

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>([]);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(null);

  const [locationName, setLocationName] = useState<string>('');
  const debounceSetLocationName = useDebounceCallback(setLocationName, keyboardTextFieldDebounceTimeout);
  const [locationAbout, setLocationAbout] = useState<string | null>('');
  const debounceSetLocationAbout = useDebounceCallback(setLocationAbout, keyboardTextFieldDebounceTimeout);
  const [locationTimezone, setLocationTimezone] = useState<string>('');
  const debounceSetLocationTimezone = useDebounceCallback(setLocationTimezone, keyboardTextFieldDebounceTimeout);
  const [locationType, setLocationType] = useState<string>('PRIVATE');
  const debounceSetLocationType = useDebounceCallback(setLocationType, keyboardTextFieldDebounceTimeout);
  const [locationTagIds, setLocationTagIds] = useState<string[]>([]);
  const debounceSetLocationTagIds = useDebounceCallback(setLocationTagIds, keyboardTextFieldDebounceTimeout);

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleLocationAddClick = ({ name, about, timezone, type, locationTagIds }: LocationDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding location '${name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitAddLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          name,
          about,
          organizationUniqueAlphanumericName,
          timezone,
          type: type as LocationType,
          featureImages: finalFeatureImages,
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
            type: {
              type: type as LocationType,
              name: '',
            },
            featureImages: finalFeatureImages,
            locationTags: [],
          },
        },
      },
    });
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => [response, ...prev]);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  return (
    <TwoSideVerticalWizard>
      <LeftSidePanel title="Add a New Location" description="Set up your office location to manage workspace resources, bookable areas, and team access — all in one place.">
        <FeatureBox
          icon={<ApartmentIcon sx={{ color: '#3949AB', fontSize: 40 }} />}
          title="Multi-Floor Support"
          subtitle="Define buildings and floors to organize your workspace layout clearly."
        />
        <FeatureBox
          icon={<ChairAltIcon sx={{ color: '#00796B', fontSize: 40 }} />}
          title="Desk Management"
          subtitle="Add and manage desks to enable hot-desking or assigned seating."
        />
        <FeatureBox
          icon={<MeetingRoomIcon sx={{ color: '#6A1B9A', fontSize: 40 }} />}
          title="Meeting Rooms & Spaces"
          subtitle="Configure bookable rooms and common areas for collaboration."
        />
        <FeatureBox
          icon={<LockOpenIcon sx={{ color: '#D84315', fontSize: 40 }} />}
          title="Capacity & Access Control"
          subtitle="Set maximum occupancy and control who can access the space."
        />
        <FeatureBox
          icon={<EventNoteIcon sx={{ color: '#1565C0', fontSize: 40 }} />}
          title="Resource Scheduling"
          subtitle="Enable bookings for desks, rooms, and equipment based on availability."
        />
        <FeatureBox icon={<VisibilityIcon sx={{ color: '#2E7D32', fontSize: 40 }} />} title="Location Visibility" subtitle="Control who can see and interact with this location." />
      </LeftSidePanel>
      <RightSidePanel
        title="Set Up Your Location"
        description="Let's get started by adding your primary workplace location. This helps organize your office layout, manage bookings, and connect your team to physical spaces from day one."
      >
        <Form
          onSubmit={handleLocationAddClick}
          initialValues={{
            name: locationName,
            about: locationAbout,
            timezone: locationTimezone,
            type: locationType,
            locationTagIds,
          }}
          validate={validateLocationDetails}
          render={({ handleSubmit, values }) => {
            debounceSetLocationName(values!.name);
            debounceSetLocationAbout(values!.about);
            debounceSetLocationTimezone(values!.timezone);
            debounceSetLocationType(values!.type);
            debounceSetLocationTagIds(values!.locationTagIds);

            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <Divider />
                <FormFieldLabel label="Feature Images">
                  <StackColumn>
                    <Box
                      sx={{
                        display: 'grid',
                        gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' },
                        gap: 2,
                      }}
                    >
                      {featureImages.map((image, index) => (
                        <Box
                          key={index}
                          sx={{
                            position: 'relative',
                            borderRadius: 2,
                            overflow: 'hidden',
                            border: 1,
                            borderColor: 'divider',
                            backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                          }}
                        >
                          {/* eslint-disable-next-line @next/next/no-img-element */}
                          <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                          <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                            <IconButton size="small" aria-label="Remove feature image" onClick={() => handleRemoveFeatureImage(image)}>
                              <DeleteIcon fontSize="small" />
                            </IconButton>
                          </StackRow>
                          <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                            {primaryFeatureImage?.original?.url === image.original?.url ? (
                              <Chip size="small" color="success" label="Cover image" />
                            ) : (
                              <Button variant="contained" size="small" onClick={() => handleSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                                Make cover
                              </Button>
                            )}
                          </StackRow>
                        </Box>
                      ))}

                      {featureImages.length === 0 && (
                        <Box
                          sx={{
                            borderRadius: 2,
                            border: '1px dashed',
                            borderColor: 'divider',
                            backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                            padding: 2,
                            display: 'flex',
                            alignItems: 'center',
                          }}
                        >
                          <BodyIconTypography label="Add feature images to showcase this location. A 4:3 aspect ratio works best." />
                        </Box>
                      )}
                    </Box>

                    <ImageFileUploaderWithCropper
                      defaultAspectRatio={1}
                      onUploadCompleted={handleFeatureImageUploadCompleted}
                      helperText="Upload a high-quality image that represents this location. This image will be used in dashboards and reports to visually identify the workspace."
                    />
                  </StackColumn>
                </FormFieldLabel>

                <FormFieldLabel label="Name" required={requiredFields.name}>
                  <TextField
                    name="name"
                    required={requiredFields.name}
                    helperText={
                      <HelperText text="Enter a unique and descriptive name for this location. This will help team members quickly identify it when booking workspaces or managing resources." />
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
                      <HelperText text="Provide a brief description of this location. Include details like the purpose of the site, departments it hosts, or any distinguishing features to help users understand its context." />
                    }
                  />
                </FormFieldLabel>

                <FormFieldLabel label="Timezone" required={requiredFields.timezone}>
                  <SingleChoinceTimezone
                    name="timezone"
                    required={requiredFields.timezone}
                    helperText="Select the time zone for this location. It ensures that bookings, events, and notifications are displayed in the correct local time for everyone using this site."
                  />
                </FormFieldLabel>

                {rootData.me.emails.some((item) => !!rootData.emailsToShowLatestCapabilities.find((email) => email.toLocaleLowerCase() === item.toLocaleLowerCase())) && (
                  <FormFieldLabel label="Type" required={requiredFields.type}>
                    <SingleChoiceLocationType rootDataRelay={rootData} name="type" required={requiredFields.type} />
                  </FormFieldLabel>
                )}

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
            );
          }}
        />{' '}
      </RightSidePanel>
    </TwoSideVerticalWizard>
  );
};

const MemoAddPrivateLocation = memo(AddPrivateLocation);

type RelayProps = {
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelLabel?: string;
  createLabel?: string;
};

const AddPrivateLocationWithRelay = ({ onReloadRequired, organizationUniqueAlphanumericName, onAdded, onCancel, cancelLabel, createLabel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addPrivateLocation_rootQuery>(RootQuery);
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
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddPrivateLocation
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

export default memo(AddPrivateLocationWithRelay);
