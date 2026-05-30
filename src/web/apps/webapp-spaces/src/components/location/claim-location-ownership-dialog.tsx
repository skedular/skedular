import { PaletteModeContext, getRelayErrorMessage } from '@skedular/shared';
import { DefaultDialogTitle, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';

import type { claimLocationOwnershipDialog_claimLocationOwnershipMutation } from '@/queries/__generated__/claimLocationOwnershipDialog_claimLocationOwnershipMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  connectionIds: string[];
  isDialogOpen: boolean;
  onClaimClicked: () => void;
  onCancel: () => void;
  organizationCustomDomain: string;
};

type LocationClaimOwnershipDetails = {
  uniqueClaimCode: string;
};

const locationClaimOwnershipSchema = object({
  uniqueClaimCode: string().required('Location claim code is required'),
});

const ClaimLocationOwnershipDialog = ({ connectionIds, isDialogOpen, onClaimClicked, onCancel, organizationCustomDomain }: Props) => {
  const [commitClaimLocationOwnership] = useMutation<claimLocationOwnershipDialog_claimLocationOwnershipMutation>(graphql`
    mutation claimLocationOwnershipDialog_claimLocationOwnershipMutation($connectionIds: [ID!]!, $input: ClaimLocationOwnershipInput!) {
      claimLocationOwnership(input: $input) {
        location @appendNode(connections: $connectionIds, edgeTypeName: "LocationDetails") {
          id
          name
          customTags {
            id
            name
            color
          }
          zones {
            id
            name
            color
          }
          resources {
            totalCount
          }
          physicalAddress {
            formattedAddress
          }
          canModify
          canDelete
          organization {
            customDomain
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(locationClaimOwnershipSchema);
  const requiredFields = makeRequired(locationClaimOwnershipSchema);

  const handleAddClick = ({ uniqueClaimCode }: LocationClaimOwnershipDetails) => {
    const id = uuid();

    commitClaimLocationOwnership({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          uniqueClaimCode: uniqueClaimCode.toLocaleUpperCase(),
          organizationCustomDomain,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(
            <NotificationContent content={`We couldn't claim this location with claim code '${uniqueClaimCode}'. ${getRelayErrorMessage(errors)}`} />,
            errorNotificationOptions,
          );

          return;
        }

        onClaimClicked();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't claim this location with claim code '${uniqueClaimCode}'. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Claim location ownership" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            uniqueClaimCode: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="Claim Code">
                  <TextField name="uniqueClaimCode" required={requiredFields.uniqueClaimCode} helperText="Location unique claim code to claim ownership" />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Claim" secondaryLabel="Cancel" />
              </FormStackColumn>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(ClaimLocationOwnershipDialog);
