import {
  BodyIconTypography,
  CaptionIconTypography,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
} from '@skedular/ui';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { CompleteOnboardStripeConnectAccountButton } from '@/components/stripeConnectAccount';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle, defaultPadding, PageHeaderPanel } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { editStripeConnectAccount_query$key } from '@/queries/__generated__/editStripeConnectAccount_query.graphql';
import type { editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation } from '@/queries/__generated__/editStripeConnectAccount_updateOrganizationStripeConnectAccountMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
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

const formColumnSx = {
  width: '100%',
  maxWidth: 760,
};

const detailValueSx = {
  opacity: 0.82,
  overflowWrap: 'anywhere',
};

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
            render: <NotificationContent content={`Failed to update Stripe Connect account '${account.name}'. Error: ${getRelayErrorMessage(errors)}`} />,
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

  const countryName = account.country ? getCountryData(account.country as TCountryCode).name : 'Not available';
  const onboardingStatusLabel = account.isOnboardingCompleted ? 'Onboarding complete' : 'Onboarding required';
  const onboardingStatusColor = account.isOnboardingCompleted ? 'success' : 'warning';

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', px: { xs: 0, sm: 1, md: 2 }, pt: { xs: 1, sm: 1, md: 2 }, pb: defaultPadding }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', backgroundColor: 'transparent', gap: 2 }}>
        <PageHeaderPanel
          eyebrow="Stripe Connect account"
          title={account.name}
          description="Review payout readiness, business details, and the public support information Stripe uses for marketplace payments."
        >
          <StackRow sx={{ alignItems: 'center', flexWrap: 'wrap', gap: 1 }}>
            <Chip label={onboardingStatusLabel} color={onboardingStatusColor} variant={account.isOnboardingCompleted ? 'filled' : 'outlined'} />
            <Chip label={account.chargesEnabled ? 'Charges enabled' : 'Charges disabled'} color={account.chargesEnabled ? 'success' : 'default'} variant="outlined" />
            <Chip label={account.payoutsEnabled ? 'Payouts enabled' : 'Payouts disabled'} color={account.payoutsEnabled ? 'success' : 'default'} variant="outlined" />
          </StackRow>
        </PageHeaderPanel>

        <Box
          sx={{
            borderRadius: 4,
            border: 1,
            borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : 'divider'),
            bgcolor: (theme) => (theme.palette.mode === 'light' ? 'common.white' : theme.palette.background.paper),
            boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 12px 32px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
            overflow: 'hidden',
          }}
        >
          <Form
            onSubmit={handleStripeConnectAccountDetailUpdateClick}
            initialValues={{
              name,
            }}
            validate={validateStripeConnectAccountDetails}
            render={({ handleSubmit, values }) => {
              setName(values!.name);

              return (
                <FormStackColumn onSubmit={handleSubmit} sx={{ p: defaultPadding, ...formColumnSx }}>
                  <StackColumn spacing={2}>
                    <StackColumn spacing={0.5}>
                      <LeadIconTypography label="Account details" />
                      <SmallIconTypography label="Only the local nickname can be edited here. Business and payout details come from Stripe onboarding." />
                    </StackColumn>

                    <Divider />

                    <FormFieldLabel label="Nickname">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>

                    {!account.isOnboardingCompleted && account.onboardingUrl ? (
                      <StackColumn spacing={1.25}>
                        <Divider />
                        <StackColumn spacing={0.5}>
                          <SectionIconTypography label="Onboarding" />
                          <BodyIconTypography label="Complete Stripe onboarding before this account can reliably accept charges and receive payouts." sx={detailValueSx} />
                        </StackColumn>
                        <StackRow>
                          <CompleteOnboardStripeConnectAccountButton onboardingUrl={account.onboardingUrl} variant="contained" size="medium" />
                        </StackRow>
                      </StackColumn>
                    ) : null}

                    <Divider />

                    <StackColumn spacing={1.5}>
                      <SectionIconTypography label="Business details" />
                      <DetailRow label="Country" value={countryName} />
                      <DetailRow label="Default currency" value={account.defaultCurrency} />
                      <DetailRow label="Business type" value={account.businessType} />
                      <DetailRow label="Company name" value={account.companyName} />
                      <DetailRow label="Website" value={account.url} />
                      <DetailRow label="Support link" value={account.supportUrl} />
                      <DetailRow label="Contact email" value={account.contactEmail} />
                      <DetailRow label="Contact phone" value={account.contactPhone} />
                    </StackColumn>

                    <Divider />

                    <StackColumn spacing={1.5}>
                      <SectionIconTypography label="Connection status" />
                      <DetailRow label="Charges" value={account.chargesEnabled ? 'Enabled' : 'Disabled'} />
                      <DetailRow label="Payouts" value={account.payoutsEnabled ? 'Enabled' : 'Disabled'} />
                      <DetailRow label="Details" value={account.detailsSubmitted ? 'Submitted' : 'Not submitted'} />
                      <DetailRow label="Authorised" value={account.isAuthorized ? 'Yes' : 'No'} />
                    </StackColumn>
                  </StackColumn>

                  <EditorActionBar
                    primaryAction={
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        Update
                      </Button>
                    }
                    secondaryActions={
                      <Button variant="text" onClick={handleCloseClick} sx={{ textTransform: 'none' }}>
                        Cancel
                      </Button>
                    }
                  />
                </FormStackColumn>
              );
            }}
          />
        </Box>
      </StackColumn>
    </Box>
  );
};

const DetailRow = ({ label, value }: { label: string; value?: string | null }) => (
  <StackColumn spacing={0.35}>
    <CaptionIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
    <BodyIconTypography label={value || 'Not available'} sx={detailValueSx} />
  </StackColumn>
);

export default memo(EditStripeConnectAccount);
