import { DefaultDialogTitle, TwoButtonsDialogActions } from '@skedular/ui';
import { DialogTransition } from '@/components/transitions';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import { memo } from 'react';

type Props = {
  open: boolean;
  title: string;
  description: string;
  confirmLabel: string;
  onConfirm: () => void;
  onCancel: () => void;
};

const RecurringBookingDeleteConfirmationDialog = ({ open, title, description, confirmLabel, onConfirm, onCancel }: Props) => (
  <Dialog slots={{ transition: DialogTransition }} open={open} onClose={onCancel} fullWidth maxWidth="xs">
    <DefaultDialogTitle title={title} />
    <DialogContent sx={{ marginTop: 2 }}>
      <DialogContentText>{description}</DialogContentText>
      <TwoButtonsDialogActions onPrimaryClicked={onConfirm} onSecondaryClicked={onCancel} primaryLabel={confirmLabel} secondaryLabel="Cancel" />
    </DialogContent>
  </Dialog>
);

export default memo(RecurringBookingDeleteConfirmationDialog);
