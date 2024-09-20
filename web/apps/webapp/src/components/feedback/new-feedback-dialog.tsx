import type { newFeedbackDialog_query$key } from '@/queries/__generated__/newFeedbackDialog_query.graphql';
import type { newFeedbackDialog_submitCustomerFeedbackMutation } from '@/queries/__generated__/newFeedbackDialog_submitCustomerFeedbackMutation.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerShortName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: newFeedbackDialog_query$key;
  isDialogOpen: boolean;
  onSendClicked: () => void;
  onCancelClicked: () => void;
};

interface FeedbackDetails {
  feedback: string;
}

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
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to submit feedback. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setFeedbackContent('');

          onSendClicked();
        }
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
    <Dialog fullWidth={true} open={isDialogOpen}>
      <DialogTitle>Send us feedback</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleSendClick}
          initialValues={{
            feedbackContent,
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
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
                <Button color="primary" variant="contained" type="submit">
                  Send
                </Button>
                <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                  Cancel
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
