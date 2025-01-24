import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import Typography from '@mui/material/Typography';
import { BodyIconTypography, DefaultDialogTitle, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerShortName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { newFeedbackDialog_query$key } from './__generated__/newFeedbackDialog_query.graphql';
import type { newFeedbackDialog_submitCustomerFeedbackMutation } from './__generated__/newFeedbackDialog_submitCustomerFeedbackMutation.graphql';

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
  feedback: string().required('Feedback is required'),
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
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Send us feedback" />
      <DialogContent sx={{ marginTop: 2 }}>
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

              <TextField label="Feedback" name="feedback" required={requiredFields.feedback} multiline rows={10} />
              <BodyIconTypography label="A note from the team:" sx={{ fontStyle: 'italic' }} />
              <BodyIconTypography label="We value your feedback, whether it's big or small. Sometimes, it's the smallest details that distinguish a great product from a mediocre one. If you notice something missing or something that bothers you, please let us know, and we'll address it promptly!" />

              <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Send" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewFeedbackDialog);
