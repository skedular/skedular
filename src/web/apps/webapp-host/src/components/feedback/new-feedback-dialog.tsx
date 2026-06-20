import { InMsTeamsContext, PaletteModeContext, getCustomerShortName, getRelayErrorMessage } from '@skedular/shared';
import { BodyIconTypography, DefaultDialogTitle, FormStackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';

import type { newFeedbackDialog_query$key } from '@/queries/__generated__/newFeedbackDialog_query.graphql';
import type { newFeedbackDialog_submitCustomerFeedbackMutation } from '@/queries/__generated__/newFeedbackDialog_submitCustomerFeedbackMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: newFeedbackDialog_query$key;
  isDialogOpen: boolean;
  onSendClicked: () => void;
  onCancel: () => void;
};

type FeedbackDetails = {
  feedback: string;
};

const zoneSchema = object({
  feedback: string().required('Please enter your feedback.'),
});

const NewFeedbackDialog = ({ rootDataRelay, isDialogOpen, onSendClicked, onCancel }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment newFeedbackDialog_query on Query {
        me {
          name
          givenName
          middleName
          familyName
        }
      }
    `,
    rootDataRelay,
  );

  const [commitSubmitCustomerFeedback] = useMutation<newFeedbackDialog_submitCustomerFeedbackMutation>(graphql`
    mutation newFeedbackDialog_submitCustomerFeedbackMutation($input: SubmitCustomerFeedbackInput!) {
      submitCustomerFeedback(input: $input) {
        id
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const inMsTeams = useContext(InMsTeamsContext);

  const handleSubmitFeedbackClick = ({ feedback: feedbackContent }: FeedbackDetails) => {
    const id = uuid();

    commitSubmitCustomerFeedback({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          feedbackContent,
          channel: inMsTeams ? 'MS_TEAMS' : 'WEB',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't send your feedback. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onSendClicked();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't send your feedback. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Send us feedback" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleSubmitFeedbackClick}
          initialValues={{
            feedback: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <BodyIconTypography
                label={
                  <span>
                    Hi
                    <span style={{ fontWeight: 'bold' }}>{' ' + getCustomerShortName(rootData.me)}</span>
                    {`, what would you like to share with us?`}
                  </span>
                }
              />

              <TextField label="Feedback" name="feedback" required={requiredFields.feedback} multiline rows={10} />
              <BodyIconTypography label="A note from us:" sx={{ fontStyle: 'italic' }} />
              <BodyIconTypography label="We value every bit of feedback, whether it is about a major issue or a small detail. If something feels unclear, missing, or frustrating, please tell us so we can improve it." />
              <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Send" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewFeedbackDialog);
