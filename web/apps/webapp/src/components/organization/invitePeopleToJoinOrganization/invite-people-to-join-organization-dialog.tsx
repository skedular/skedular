import type { invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation } from '@/queries/__generated__/invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { DefaultDialogTitle, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  isDialogOpen: boolean;
  onInviteClicked: () => void;
  onCancelClicked: () => void;
  organizationId: string;
};

type PeopleToJoin = {
  emails: (string | undefined)[];
};

const peopleToInviteSchema = object({
  emails: array()
    .transform(function (value, originalValue) {
      if (this.isType(value) && value !== null) {
        return value;
      }

      return originalValue ? originalValue.split(/[\s,]+/) : [];
    })
    .of(string().email(({ value }) => `${value} is not a valid email`))
    .required('List of emails separated by comma is required'),
});

const InvitePeopleToJoinOrganizationDialog = ({ isDialogOpen, onInviteClicked, onCancelClicked, organizationId }: Props) => {
  const [commitInviteCustomersToJoinOrganization] = useMutation<invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation>(
    graphql`
      mutation invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation($input: InviteCustomersToJoinOrganizationInput!) {
        inviteCustomersToJoinOrganization(input: $input) {
          clientMutationId
        }
      }
    `,
  );

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(peopleToInviteSchema);
  const requiredFields = makeRequired(peopleToInviteSchema);

  const handleInvitePeopleClick = ({ emails: originalEmailsStr }: PeopleToJoin) => {
    if (!originalEmailsStr) {
      return;
    }

    const emails = originalEmailsStr as unknown as string;
    if (!emails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Inviting people to join organization...`} />, infoNotificationOptions);

    commitInviteCustomersToJoinOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId,
          emails: emails
            .split(/[\s,]+/)
            .map((email) => email.trim())
            .filter((email) => email),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to invite people to join organization. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation sent to people to join organization.`} />,
        });

        onInviteClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to invite people to join organization. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DefaultDialogTitle title="Invite people to join your organization" />
      <DialogContent>
        <Form
          onSubmit={handleInvitePeopleClick}
          initialValues={{
            emails: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <FormFieldLabel label="Emails" useWiderSpace>
                <TextField name="emails" required={requiredFields.emails} multiline={true} helperText="member1@example.com,member2@example.com" />
              </FormFieldLabel>

              <TwoButtonsDialogActions onSecondaryClicked={onCancelClicked} primaryLabel="Invite" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(InvitePeopleToJoinOrganizationDialog);
