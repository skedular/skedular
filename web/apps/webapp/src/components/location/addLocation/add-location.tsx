import type { addLocation_addLocationMutation } from '@/queries/__generated__/addLocation_addLocationMutation.graphql';
import type { addLocation_completeLocationOnboardingMutation } from '@/queries/__generated__/addLocation_completeLocationOnboardingMutation.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import {
  BodyIconTypography,
  FormFieldLabel,
  SectionIconTypography,
  StackColumn,
  StackColumnWithSaveExitCancelAppBar,
} from '@repo/shared/components/commons';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  onReloadRequired: () => void;
  organizationId?: string;
  onAdded: (id: string) => void;
  onCancel: () => void;
  cancelButtonText?: string;
};

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
  physicalAddress: string | null;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three characters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  physicalAddress: string().nullable(),
});

const AddLocation = ({ onReloadRequired, organizationId, onAdded, onCancel, cancelButtonText }: Props) => {
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
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const handleCancelClick = () => {
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

  const handleLocationCreateClick = ({ name, about, timezone, physicalAddress }: LocationDetails) => {
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
          },
        },
      },
    });
  };

  return (
    <>
      <Box sx={{ display: 'flex' }}>
        <Box sx={{ flexGrow: 1 }}>
          <Form
            onSubmit={handleLocationCreateClick}
            initialValues={{
              name: '',
              about: null,
              organizationId,
              physicalAddress: null,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <StackColumnWithSaveExitCancelAppBar onSubmit={handleSubmit} onCancel={handleCancelClick} label="Add Location">
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
                </StackColumn>
              </StackColumnWithSaveExitCancelAppBar>
            )}
          />
        </Box>
      </Box>
    </>
  );
};

export default memo(AddLocation);
