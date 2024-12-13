import Button from '@mui/material/Button';
import { FormStackColumn, StackRow } from '@repo/shared/components/commons';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { customerSettingsPersonalTab_query$key } from './__generated__/customerSettingsPersonalTab_query.graphql';
import type { customerSettingsPersonalTab_updateMyCustomerDetailsMutation } from './__generated__/customerSettingsPersonalTab_updateMyCustomerDetailsMutation.graphql';

type Props = {
  rootDataRelay: customerSettingsPersonalTab_query$key;
};

type SettingsDetails = {
  designation: string | null;
  title: string | null;
  name: string | null;
  givenName: string | null;
  middleName: string | null;
  familyName: string | null;
  timezone: string;
  phoneNumber: string | null;
};

const settingsSchema = object({
  designation: string().nullable(),
  title: string().nullable(),
  name: string().nullable(),
  givenName: string().nullable(),
  middleName: string().nullable(),
  familyName: string().nullable(),
  timezone: string().required('Timezone is required'),
  phoneNumber: string().nullable(),
});

const CustomerSettingsPersonalTab = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<customerSettingsPersonalTab_query$key>(
    graphql`
      fragment customerSettingsPersonalTab_query on Query {
        me {
          id
          timezone
          designation
          title
          name
          givenName
          middleName
          familyName
          phoneNumber
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateMyCustomerDetails] = useMutation<customerSettingsPersonalTab_updateMyCustomerDetailsMutation>(graphql`
    mutation customerSettingsPersonalTab_updateMyCustomerDetailsMutation($input: UpdateMyCustomerDetailsInput!) @raw_response_type {
      updateMyCustomerDetails(input: $input) {
        customer {
          id
          timezone
          designation
          title
          name
          givenName
          middleName
          familyName
          phoneNumber
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(settingsSchema);
  const requiredFields = makeRequired(settingsSchema);

  const handleSettingsUpdateClick = ({ timezone, designation, title, name, givenName, middleName, familyName, phoneNumber }: SettingsDetails) => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating personal details'...`} />, infoNotificationOptions);

    commitUpdateMyCustomerDetails({
      variables: {
        input: {
          clientMutationId: nanoid(),
          timezone,
          designation,
          title,
          name,
          givenName,
          middleName,
          familyName,
          phoneNumber,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update personal details. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Personal settings updated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update personal details. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateMyCustomerDetails: {
          customer: {
            id: rootData.me.id,
            timezone,
            designation,
            title,
            name,
            givenName,
            middleName,
            familyName,
            phoneNumber,
          },
        },
      },
    });
  };

  if (!rootData.me) {
    return null;
  }

  return (
    <Form
      onSubmit={handleSettingsUpdateClick}
      initialValues={{
        timezone: rootData.me.timezone,
        designation: rootData.me.designation,
        title: rootData.me.title,
        name: rootData.me.name,
        givenName: rootData.me.givenName,
        middleName: rootData.me.middleName,
        familyName: rootData.me.familyName,
        phoneNumber: rootData.me.phoneNumber,
      }}
      validate={validate}
      render={({ handleSubmit }) => (
        <FormStackColumn onSubmit={handleSubmit}>
          <TextField label="Designation" name="designation" required={requiredFields.designation} />
          <TextField label="Title" name="title" required={requiredFields.title} />
          <TextField label="Name" name="name" required={requiredFields.name} />
          <TextField label="Given Name" name="givenName" required={requiredFields.givenName} />
          <TextField label="Middle Name" name="middleName" required={requiredFields.middleName} />
          <TextField label="Family Name" name="familyName" required={requiredFields.familyName} />
          <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />
          <TextField label="Phone Number" name="phoneNumber" required={requiredFields.phoneNumber} />

          <StackRow sx={{ justifyContent: 'flex-end' }}>
            <Button color="primary" variant="contained" type="submit">
              Update
            </Button>
          </StackRow>
        </FormStackColumn>
      )}
    />
  );
};

export default memo(CustomerSettingsPersonalTab);
