import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { DialogTransition } from '@repo/shared/components/transitions';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerShortName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';
import type { newFeedbackDialog_query$key } from './__generated__/newFeedbackDialog_query.graphql';
import type { newFeedbackDialog_submitCustomerFeedbackMutation } from './__generated__/newFeedbackDialog_submitCustomerFeedbackMutation.graphql';

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

  const { enqueueSnackbar } = useSnackbar();
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [feedbackContent, setFeedbackContent] = useState<string>('');

  const handleSendClick = ({ feedback: feedbackContent }: FeedbackDetails) => {
    const id = nanoid();

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
          enqueueSnackbar(`Failed to submit feedback. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setFeedbackContent('');
        onSendClicked();
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to submit feedback. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
      <DialogTitle>Send us feedback</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleSendClick}
          initialValues={{
            feedbackContent,
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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

              <DialogActions>
                <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                  Cancel
                </Button>
                <Button color="primary" variant="contained" type="submit">
                  Send
                </Button>
              </DialogActions>
            </Stack>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewFeedbackDialog);
