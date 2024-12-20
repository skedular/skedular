import type { newFeedbackDialog_query$key } from '@/queries/__generated__/newFeedbackDialog_query.graphql';
import type { newFeedbackDialog_submitCustomerFeedbackMutation } from '@/queries/__generated__/newFeedbackDialog_submitCustomerFeedbackMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Typography from '@mui/material/Typography';
import { FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerShortName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: newFeedbackDialog_query$key;
  isDialogOpen: boolean;
  onSendClicked: () => void;
  onCancelClicked: () => void;
};

type FeedbackDetails = {
  feedback: string;
};

const zoneSchema = object({
  feedback: string().required('Feedback is required'),
});

const NewFeedbackDialog = ({ rootDataRelay, isDialogOpen, onSendClicked, onCancelClicked }: Props) => {
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

  const handleSubmitFeedbackClick = ({ feedback: feedbackContent }: FeedbackDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Submitting feedback...`} />, infoNotificationOptions);

    commitSubmitCustomerFeedback({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          feedbackContent,
          channel: 'MsTeams',
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to submit feedback. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Feedback submitted.`} />,
        });

        onSendClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to submit feedback. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
      <DialogTitle>Send us feedback</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleSubmitFeedbackClick}
          initialValues={{
            feedbackContent: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <Typography>
                Hi
                <span style={{ fontWeight: 'bold' }}>{' ' + getCustomerShortName(rootData.me)}</span>, what feedback would you like to share with us?
              </Typography>

              <TextField
                label="Feedback"
                name="feedback"
                required={requiredFields.feedback}
                multiline={true}
                sx={{
                  textAlign: 'center',
                }}
                rows={10}
              />

              <Typography sx={{ fontStyle: 'italic' }}>A note from the team:</Typography>
              <Typography>
                We value your feedback, whether it&apos;s big or small. Sometimes, it&apos;s the smallest details that distinguish a great product
                from a mediocre one. If you notice something missing or something that bothers you, please let us know, and we&apos;ll address it
                promptly!
              </Typography>
              <TwoButtonsDialogActions onSecondaryClicked={onCancelClicked} primaryLabel="Send" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewFeedbackDialog);
