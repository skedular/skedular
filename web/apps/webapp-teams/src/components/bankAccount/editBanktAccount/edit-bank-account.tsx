import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, GridContainer, SectionIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { SingleChoiceCountry } from '@/components/forms';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { PaletteModeContext } from '@skedular/shared';
import { defaultButtonStyle, defaultPadding } from '@skedular/ui';
import { getRelayErrorMessage } from '@skedular/shared';
import type { editBankAccount_query$key } from '@/queries/__generated__/editBankAccount_query.graphql';
import type { editBankAccount_updateOrganizationBankAccountMutation } from '@/queries/__generated__/editBankAccount_updateOrganizationBankAccountMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: editBankAccount_query$key;
  onReloadRequired: () => void;
};

type BankAccountDetails = {
  name: string;
  bankName: string;
  accountHolderName: string;
  accountNumber: string;
  country: string;
};

const bankAccountSchema = object({
  name: string().min(3, 'Name must be at least three characters long.').required('Name is required'),
  bankName: string().required('Bank name is required'),
  accountHolderName: string().required('Account holder name is required'),
  accountNumber: string().required('Account number is required'),
  country: string().required('Country is required'),
});

const EditBankAccount = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<editBankAccount_query$key>(
    graphql`
      fragment editBankAccount_query on Query {
        organizationBankAccount(id: $organizationBankAccountId) {
          id
          name
          bankName
          accountHolderName
          accountNumber
          country
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateOrganizationBankAccount] = useMutation<editBankAccount_updateOrganizationBankAccountMutation>(graphql`
    mutation editBankAccount_updateOrganizationBankAccountMutation($input: UpdateOrganizationBankAccountInput!) @raw_response_type {
      updateOrganizationBankAccount(input: $input) {
        organizationBankAccount {
          id
          name
          bankName
          accountHolderName
          accountNumber
          country
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateBankAccountDetails = makeValidate(bankAccountSchema);
  const requiredFields = makeRequired(bankAccountSchema);
  const [name, setName] = useState(rootData.organizationBankAccount?.name);
  const [bankName, setBankName] = useState(rootData.organizationBankAccount?.bankName);
  const [accountHolderName, setAccountHolderName] = useState(rootData.organizationBankAccount?.accountHolderName);
  const [accountNumber, setAccountNumber] = useState(rootData.organizationBankAccount?.accountNumber);
  const [country, setCountry] = useState(rootData.organizationBankAccount?.country);

  const handleBankAccountDetailUpdateClick = ({ name, bankName, accountHolderName, accountNumber, country }: BankAccountDetails) => {
    const account = rootData.organizationBankAccount;
    if (!account) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating Bank account '${account.name}'...`} />, infoNotificationOptions);

    commitUpdateOrganizationBankAccount({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: account.id,
          name,
          bankName,
          accountHolderName,
          accountNumber,
          country,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Bank account '${account.name}'. Error: ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Bank account ${name} updated.`} />,
        });

        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Bank account '${account.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationBankAccount: {
          organizationBankAccount: {
            id: account.id,
            name,
            bankName,
            accountHolderName,
            accountNumber,
            country,
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.back();
  };

  const account = rootData.organizationBankAccount;
  if (!account) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Bank Account">
          <Form
            onSubmit={handleBankAccountDetailUpdateClick}
            initialValues={{
              name,
              bankName,
              accountHolderName,
              accountNumber,
              country,
            }}
            validate={validateBankAccountDetails}
            render={({ handleSubmit, values }) => {
              setName(values!.name);
              setBankName(values!.bankName);
              setAccountHolderName(values!.accountHolderName);
              setAccountNumber(values!.accountNumber);
              setCountry(values!.country);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <GridContainer sx={{ justifyContent: 'space-between' }}>
                      <Grid>
                        <SectionIconTypography label="Bank Account Setup" />
                        <BodyIconTypography label="Edit your Bank account name and details" />
                      </Grid>
                    </GridContainer>
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Bank Name">
                      <TextField name="bankName" required={requiredFields.bankName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Account Holder Name">
                      <TextField name="accountHolderName" required={requiredFields.accountHolderName} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Account Number">
                      <TextField name="accountNumber" required={requiredFields.accountNumber} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Country">
                      <SingleChoiceCountry name="country" required={requiredFields.country} />
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

export default memo(EditBankAccount);
