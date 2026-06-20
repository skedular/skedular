import { PaletteModeContext, getRelayErrorMessage } from '@skedular/shared';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, GridContainer, SectionIconTypography, StackColumn } from '@skedular/ui';
import { SingleChoiceCountry } from '@/components/forms';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import { defaultPadding } from '@skedular/ui';

import type { editBankAccount_query$key } from '@/queries/__generated__/editBankAccount_query.graphql';
import type { editBankAccount_updateOrganizationBankAccountMutation } from '@/queries/__generated__/editBankAccount_updateOrganizationBankAccountMutation.graphql';
import Box from '@mui/material/Box';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useCallback, useContext, useRef } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
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

type BankAccountPatchField = 'NAME' | 'BANK_NAME' | 'ACCOUNT_HOLDER_NAME' | 'ACCOUNT_NUMBER' | 'COUNTRY';

const inlinePatchDebounceTimeout = 1000;

const bankAccountSchema = object({
  name: string().min(3, 'Name must be at least three characters long.').required('Name is required'),
  bankName: string().required('Bank name is required'),
  accountHolderName: string().required('Account holder name is required'),
  accountNumber: string().required('Account number is required'),
  country: string().required('Country is required'),
});

const bankAccountPatchFields: Record<keyof BankAccountDetails, BankAccountPatchField> = {
  name: 'NAME',
  bankName: 'BANK_NAME',
  accountHolderName: 'ACCOUNT_HOLDER_NAME',
  accountNumber: 'ACCOUNT_NUMBER',
  country: 'COUNTRY',
};

const getValidBankAccountPatchFields = (fieldsToUpdate: BankAccountPatchField[], values: BankAccountDetails): BankAccountPatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    const formField = (Object.entries(bankAccountPatchFields) as [keyof BankAccountDetails, BankAccountPatchField][]).find(([, field]) => field === patchField)?.[0];
    if (!formField) {
      return false;
    }

    try {
      bankAccountSchema.validateSyncAt(formField, values);
      return true;
    } catch {
      return false;
    }
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

  const [commitUpdateOrganizationBankAccountPatch] = useMutation<editBankAccount_updateOrganizationBankAccountMutation>(graphql`
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
  const account = rootData.organizationBankAccount;
  const initialBankAccountValues = {
    name: account?.name ?? '',
    bankName: account?.bankName ?? '',
    accountHolderName: account?.accountHolderName ?? '',
    accountNumber: account?.accountNumber ?? '',
    country: account?.country ?? '',
  };
  const draftBankAccountValues = useRef(initialBankAccountValues);

  const commitBankAccountPatch = useCallback(
    (fieldsToUpdate: BankAccountPatchField[], values: BankAccountDetails) => {
      const { name, bankName, accountHolderName, accountNumber, country } = values;
      const account = rootData.organizationBankAccount;
      const validFieldsToUpdate = getValidBankAccountPatchFields(fieldsToUpdate, values);
      if (!account || validFieldsToUpdate.length === 0) {
        return;
      }

      commitUpdateOrganizationBankAccountPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: account.id,
            fieldsToUpdate: validFieldsToUpdate,
            name,
            bankName,
            accountHolderName,
            accountNumber,
            country,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            themedToast(<NotificationContent content={`Failed to update bank account '${account.name}'. Error: ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          }
        },
        onError: (error) => {
          themedToast(<NotificationContent content={`Failed to update bank account '${account.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
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
    },
    [commitUpdateOrganizationBankAccountPatch, rootData.organizationBankAccount, themedToast],
  );
  const debouncedCommitBankAccountPatch = useDebounceCallback(commitBankAccountPatch, inlinePatchDebounceTimeout);

  const handleCloseClick = () => {
    router.back();
  };

  if (!account) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Bank Account">
          <Form
            onSubmit={() => undefined}
            initialValues={initialBankAccountValues}
            validate={validateBankAccountDetails}
            render={({ handleSubmit, values }) => {
              const formValues = values as BankAccountDetails;
              const changedFields: BankAccountPatchField[] = [];
              if (draftBankAccountValues.current.name !== formValues.name) {
                changedFields.push('NAME');
              }
              if (draftBankAccountValues.current.bankName !== formValues.bankName) {
                changedFields.push('BANK_NAME');
              }
              if (draftBankAccountValues.current.accountHolderName !== formValues.accountHolderName) {
                changedFields.push('ACCOUNT_HOLDER_NAME');
              }
              if (draftBankAccountValues.current.accountNumber !== formValues.accountNumber) {
                changedFields.push('ACCOUNT_NUMBER');
              }
              if (draftBankAccountValues.current.country !== formValues.country) {
                changedFields.push('COUNTRY');
              }
              if (changedFields.length > 0) {
                draftBankAccountValues.current = formValues;
                debouncedCommitBankAccountPatch(changedFields, formValues);
              }

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
                    <GridContainer sx={{ justifyContent: 'space-between' }}>
                      <Grid>
                        <SectionIconTypography label="Bank Account Setup" />
                        <BodyIconTypography label="Edit your Bank account name and details" />
                      </Grid>
                    </GridContainer>
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding, paddingBottom: defaultPadding }}>
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
