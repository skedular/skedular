import { SingleChoiceCountry } from '@/components/forms';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@skedular/ui';

import { defaultButtonStyle, defaultPadding } from '@skedular/ui';

import type { addBankAccount_addBankAccountMutation } from '@/queries/__generated__/addBankAccount_addBankAccountMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form, FormSpy } from 'react-final-form';
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

const AddBankAccount = ({ onReloadRequired, organizationCustomDomain, onAdded, onCancel }: Props) => {
  const [commitAddBankAccount] = useMutation<addBankAccount_addBankAccountMutation>(graphql`
    mutation addBankAccount_addBankAccountMutation($input: AddOrganizationBankAccountInput!) @raw_response_type {
      addOrganizationBankAccount(input: $input) {
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateBankAccountDetails = makeValidate(bankAccountSchema);
  const requiredFields = makeRequired(bankAccountSchema);
  const [name, setName] = useState('');
  const [bankName, setBankName] = useState('');
  const [accountHolderName, setAccountHolderName] = useState('');
  const [accountNumber, setAccountNumber] = useState('');
  const [country, setCountry] = useState('');

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleBankAccountAddClick = ({ name, bankName, accountHolderName, accountNumber, country }: BankAccountDetails) => {
    const id = uuid();

    commitAddBankAccount({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          name,
          bankName,
          accountHolderName,
          accountNumber,
          country,
          organizationCustomDomain,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new Bank account '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new Bank account '${name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addOrganizationBankAccount: {
          organizationBankAccount: {
            id,
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

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Bank Account">
          <Form
            onSubmit={handleBankAccountAddClick}
            initialValues={{
              name,
              bankName,
              accountHolderName,
              accountNumber,
              country,
            }}
            validate={validateBankAccountDetails}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormSpy
                  subscription={{ values: true }}
                  onChange={({ values }) => {
                    if (!values) return;
                    setName(values.name);
                    setBankName(values.bankName);
                    setAccountHolderName(values.accountHolderName);
                    setAccountNumber(values.accountNumber);
                    setCountry(values.country);
                  }}
                />
                <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                  <SectionIconTypography label="Bank Account Setup" />
                  <BodyIconTypography label="Edit your Bank account name and details" />
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
                      <BodyIconTypography label="Add" invertDefaultColor={paletteMode === 'dark'} />
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

export default memo(AddBankAccount);
