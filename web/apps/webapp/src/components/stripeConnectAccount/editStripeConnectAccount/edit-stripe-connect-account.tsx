import {
  AppBarWithStackColumn,
  BodyIconTypography,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { CompleteOnboardStripeConnectAccountButton } from '@/components/stripeConnectAccount';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { editStripeConnectAccount_query$key } from '@/queries/__generated__/editStripeConnectAccount_query.graphql';
import type { editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation } from '@/queries/__generated__/editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import type { TCountryCode } from 'countries-list';
import { getCountryData } from 'countries-list';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: editStripeConnectAccount_query$key;
  onReloadRequired: () => void;
};

type StripeConnectAccountDetails = {
  name: string;
};

const stripeConnectAccountSchema = object({
  name: string().min(3, 'Stripe Connect account nickname must be at least three characters long.').required('Stripe Connect account nickname is required'),
});

const EditStripeConnectAccount = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<editStripeConnectAccount_query$key>(
    graphql`
      fragment editStripeConnectAccount_query on Query {
        organizationStripeConnectAccount(id: $organizationStripeConnectAccountId) {
          id
          name
          country
          defaultCurrency
          businessType
          companyName
          url
          supportUrl
          contactEmail
          contactPhone
          onboardingUrl
          chargesEnabled
          payoutsEnabled
          detailsSubmitted
          isAuthorized
          isOnboardingCompleted
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateOrganizationStripeConnectAccount] = useMutation<editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation>(graphql`
    mutation editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation($input: UpdateOrganizationStripeConnectAccountInput!) @raw_response_type {
      updateOrganizationStripeConnectAccount(input: $input) {
        organizationStripeConnectAccount {
          id
          name
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateStripeConnectAccountDetails = makeValidate(stripeConnectAccountSchema);
  const requiredFields = makeRequired(stripeConnectAccountSchema);
  const [name, setName] = useState(rootData.organizationStripeConnectAccount?.name);

  const handleStripeConnectAccountDetailUpdateClick = ({ name }: StripeConnectAccountDetails) => {
    const account = rootData.organizationStripeConnectAccount;
    if (!account) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating Stripe Connect account '${account.name}'...`} />, infoNotificationOptions);

    commitUpdateOrganizationStripeConnectAccount({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: account.id,
          name,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Stripe Connect account '${account.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Stripe Connect account ${name} updated.`} />,
        });

        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Stripe Connect account '${account.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationStripeConnectAccount: {
          organizationStripeConnectAccount: {
            id: account.id,
            name,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.back();
  };

  const account = rootData.organizationStripeConnectAccount;
  if (!account) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Stripe Connect Account">
          <Form
            onSubmit={handleStripeConnectAccountDetailUpdateClick}
            initialValues={{
              name,
            }}
            validate={validateStripeConnectAccountDetails}
            render={({ handleSubmit, values }) => {
              setName(values!.name);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <GridContainer sx={{ justifyContent: 'space-between' }}>
                      <Grid>
                        <SectionIconTypography label="Stripe Connect Account Setup" />
                        <BodyIconTypography label="Edit your Stripe Connect account name and details" />
                      </Grid>

                      <Grid>
                        {!account.isOnboardingCompleted && <CompleteOnboardStripeConnectAccountButton onboardingUrl={account.onboardingUrl} variant="contained" size="medium" />}
                      </Grid>
                    </GridContainer>
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Nickname">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SmallIconTypography label={getCountryData(account.country as TCountryCode).name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Default Currency">
                      <SmallIconTypography label={account.defaultCurrency} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Business Type">
                      <SmallIconTypography label={account.businessType} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Company Name">
                      <SmallIconTypography label={account.companyName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Website">
                      <SmallIconTypography label={account.url} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Support Link">
                      <SmallIconTypography label={account.supportUrl} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Contact Email">
                      <SmallIconTypography label={account.contactEmail} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Contact Phone">
                      <SmallIconTypography label={account.contactPhone} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Charges">
                      <SmallIconTypography label={account.chargesEnabled ? 'Enabled' : 'Disabled'} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Payouts">
                      <SmallIconTypography label={account.payoutsEnabled ? 'Enabled' : 'Disabled'} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Details">
                      <SmallIconTypography label={account.detailsSubmitted ? 'Submitted' : 'N/A'} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Authorized">
                      <SmallIconTypography label={account.isAuthorized ? 'Yes' : 'No'} />
                    </FormFieldLabel>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
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

export default memo(EditStripeConnectAccount);
