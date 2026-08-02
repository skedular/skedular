import { BodyIconTypography, DefaultDialogTitle, StackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import { useState } from 'react';
import { graphql, useLazyLoadQuery } from 'react-relay';
import type { operatorCancelBookingButton_query } from '@/queries/__generated__/operatorCancelBookingButton_query.graphql';

type Props = {
  bookingId: string;
  label: string;
  onConfirm: (reason: string) => void;
};

const query = graphql`
  query operatorCancelBookingButton_query($bookingId: String!) {
    marketplaceBookingRefundPreview(bookingId: $bookingId) {
      refundAmount
      baseAmount
      currencyToDisplay
    }
  }
`;

const OperatorCancelBookingButton = ({ bookingId, label, onConfirm }: Props) => {
  const [open, setOpen] = useState(false);
  const [reason, setReason] = useState('');
  const data = useLazyLoadQuery<operatorCancelBookingButton_query>(query, { bookingId }, { fetchPolicy: 'store-and-network' });
  const preview = data.marketplaceBookingRefundPreview;

  const close = () => setOpen(false);
  const confirm = () => {
    const trimmedReason = reason.trim();
    if (!trimmedReason) {
      return;
    }

    close();
    onConfirm(trimmedReason);
  };

  return (
    <>
      <Button
        variant="outlined"
        color="error"
        size="small"
        onClick={() => {
          setReason('');
          setOpen(true);
        }}
        sx={{ textTransform: 'none' }}
      >
        {label}
      </Button>
      <Dialog open={open} onClose={close} fullWidth maxWidth="sm">
        <DefaultDialogTitle title="Cancel booking" />
        <DialogContent>
          <StackColumn spacing={1.5}>
            <BodyIconTypography label="Review the refund before confirming this operator cancellation." />
            <TextField
              label="Cancellation reason"
              value={reason}
              onChange={(event) => setReason(event.target.value)}
              required
              multiline
              minRows={2}
              helperText="Required when the cancellation policy would block this cancellation."
              fullWidth
            />
            <BodyIconTypography label={`Refund amount: ${preview.currencyToDisplay} ${preview.refundAmount ?? 0}`} />
            {preview.baseAmount && preview.refundAmount !== null && preview.baseAmount > preview.refundAmount ? (
              <BodyIconTypography label={`Non-refundable amount: ${preview.currencyToDisplay} ${preview.baseAmount - (preview.refundAmount ?? 0)}`} />
            ) : null}
          </StackColumn>
          <TwoButtonsDialogActions
            primaryLabel="Cancel booking"
            primaryDisabled={!reason.trim()}
            secondaryLabel="Keep booking"
            onPrimaryClicked={confirm}
            onSecondaryClicked={close}
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default OperatorCancelBookingButton;
