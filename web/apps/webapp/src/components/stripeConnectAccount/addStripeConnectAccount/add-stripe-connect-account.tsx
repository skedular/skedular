import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle, defaultPadding } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { addStripeConnectAccount_addStripeConnectAccountMutation } from '@/queries/__generated__/addStripeConnectAccount_addStripeConnectAccountMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onAdded: (productId: string) => void;
  onCancel: () => void;
};

type StripeConnectAccountDetails = {
  name: string;
};

const stripeConnectAccountSchema = object({
  name: string().min(3, 'Stripe Connect account nickname must be at least three characters long.').required('Stripe Connect account nickname is required'),
});

const AddStripeConnectAccount = ({ onReloadRequired, organizationCustomDomain, onAdded, onCancel }: Props) => {
  const [commitAddStripeConnectAccount] = useMutation<addStripeConnectAccount_addStripeConnectAccountMutation>(graphql`
    mutation addStripeConnectAccount_addStripeConnectAccountMutation($input: AddOrganizationStripeConnectAccountInput!) @raw_response_type {
      addOrganizationStripeConnectAccount(input: $input) {
        organizationStripeConnectAccount {
          id
          name
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateStripeConnectAccountDetails = makeValidate(stripeConnectAccountSchema);
  const requiredFields = makeRequired(stripeConnectAccountSchema);
  const [name, setName] = useState('');

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleStripeConnectAccountAddClick = ({ name }: StripeConnectAccountDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding Stripe Connect account '${name}'...`} />, infoNotificationOptions);

    commitAddStripeConnectAccount({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          name,
          organizationCustomDomain,
          redirectUrl: new URL(`organizations/${organizationCustomDomain}/stripe-connect-accounts/${id}`, process.env.NEXT_PUBLIC_SITE_URL).toString(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new Stripe Connect account '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Stripe Connect account ${name} added.`} />,
        });

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new Stripe Connect account '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addOrganizationStripeConnectAccount: {
          organizationStripeConnectAccount: {
            id,
            name,
          },
        },
      },
    });
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Stripe Connect Account">
          <Form
            onSubmit={handleStripeConnectAccountAddClick}
            initialValues={{
              name,
            }}
            validate={validateStripeConnectAccountDetails}
            render={({ handleSubmit, values }) => {
              setName(values!.name);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Stripe Connect Account Setup" />
                    <BodyIconTypography label="Edit your Stripe Connect account name and details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Nickname">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        <BodyIconTypography label="Add" invertDefaultColor={paletteMode === 'dark'} />
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        </AppBarWithStackColumn>
      </Box>
    </Box>
  );
};

export default memo(AddStripeConnectAccount);
