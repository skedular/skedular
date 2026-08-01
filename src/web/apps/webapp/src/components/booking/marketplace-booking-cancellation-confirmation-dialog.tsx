import { RefundPreviewPanel } from '@/components/refund/RefundPreviewPanel';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { graphql, useLazyLoadQuery } from 'react-relay';
import type { MarketplaceBookingCancellationConfirmationDialog_query } from '@/queries/__generated__/MarketplaceBookingCancellationConfirmationDialog_query.graphql';

type Props = {
  open: boolean;
  bookingId: string;
  onConfirm: () => void;
  onCancel: () => void;
};

const query = graphql`
  query MarketplaceBookingCancellationConfirmationDialog_query($bookingId: String!) {
    ...RefundPreviewPanel_query @arguments(bookingId: $bookingId)
  }
`;

const MarketplaceBookingCancellationConfirmationDialog = ({ open, bookingId, onConfirm, onCancel }: Props) => {
  const data = useLazyLoadQuery<MarketplaceBookingCancellationConfirmationDialog_query>(query, { bookingId }, { fetchPolicy: 'network-only' });

  return (
    <Dialog open={open} onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>Cancel booking</DialogTitle>
      <DialogContent>
        <RefundPreviewPanel query={data} />
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Keep booking</Button>
        <Button onClick={onConfirm} variant="contained" color="primary">
          Confirm cancellation
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default MarketplaceBookingCancellationConfirmationDialog;
