import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { SingleChoinceTimezone } from '@/components/forms';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags } from '@/components/organization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addLocation_addLocationMutation } from '@/queries/__generated__/addLocation_addLocationMutation.graphql';
import type { addLocation_completeLocationOnboardingMutation } from '@/queries/__generated__/addLocation_completeLocationOnboardingMutation.graphql';
import type { addLocation_rootQuery } from '@/queries/__generated__/addLocation_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
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
  physicalAddress: string | null;
  locationTagIds: string[];
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  physicalAddress: string().nullable(),
  locationTagIds: array().nullable(),
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
          physicalAddress {
            formattedAddress
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

  const [commitCompleteLocationOnboarding] = useMutation<addLocation_completeLocationOnboardingMutation>(graphql`
    mutation addLocation_completeLocationOnboardingMutation($input: CompleteLocationOnboardingInput!) {
      completeLocationOnboarding(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateLocationDetails = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const handleCloseClick = () => {
    commitCompleteLocationOnboarding({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: () => {
        onCancel();
        onReloadRequired();
      },
      onError: (_) => {
        onCancel();
        onReloadRequired();
      },
    });
  };

  const handleLocationAddClick = ({ name, about, timezone, physicalAddress, locationTagIds }: LocationDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding location '${name}'...`} />, infoNotificationOptions);

    commitAddLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          about,
          organizationId,
          timezone,
          physicalAddress: {
            formattedAddress: physicalAddress,
          },
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

        commitCompleteLocationOnboarding({
          variables: {
            input: {
              clientMutationId: nanoid(),
            },
          },
          onCompleted: (_, errors) => {
            if (errors && errors.length > 0) {
              toast.update(toastId, {
                ...errorNotificationOptions,
                render: <NotificationContent content={`Failed to complete location onboarding. Error: ${joinErrors(errors)}.`} />,
              });
            } else {
              toast.update(toastId, {
                ...successNotificationOptions,
                render: <NotificationContent content={`Location ${name} added.`} />,
              });

              onAdded(id);
              onReloadRequired();
            }
          },
          onError: (error) => {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to complete location onboarding. Error: ${error.message}.`} />,
            });
          },
        });
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
            physicalAddress: {
              formattedAddress: physicalAddress,
            },
            locationTags: [],
          },
        },
      },
    });
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
              physicalAddress: '',
              locationTagIds: [],
            }}
            validate={validateLocationDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Location Setup" />
                  <BodyIconTypography label="Edit your location name and details" />
                  <Divider />
                </StackColumn>

                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <FormFieldLabel label="Name">
                    <TextField name="name" required={requiredFields.name} />
                  </FormFieldLabel>

                  <FormFieldLabel label="About">
                    <TextField name="about" required={requiredFields.about} multiline rows={3} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Timezone">
                    <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Physical Address">
                    <TextField name="physicalAddress" required={requiredFields.physicalAddress} multiline rows={5} />
                  </FormFieldLabel>

                  {rootData.organization?.type.type === 'Marketplace' && (
                    <FormFieldLabel label="Location Tags">
                      <MultipleChoicesLocationTags rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} organizationId={organizationId} />
                    </FormFieldLabel>
                  )}
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
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        multipleChoicesLocationTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
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
      setTriggerReloadId(nanoid());
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
