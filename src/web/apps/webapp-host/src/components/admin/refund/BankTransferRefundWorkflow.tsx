'use client';

import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import { useState } from 'react';

type Props = {
  status: string;
  onApprove: () => Promise<void>;
  onRecordSent: (reference: string) => Promise<void>;
  onConfirmReceived: () => Promise<void>;
};

export function BankTransferRefundWorkflow({ status, onApprove, onRecordSent, onConfirmReceived }: Props) {
  const [reference, setReference] = useState('');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const run = async (action: () => Promise<void>) => {
    setBusy(true);
    setError(null);
    try {
      await action();
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'The refund action failed.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <Stack spacing={2}>
      {error && <Alert severity="error">{error}</Alert>}
      {status === 'UnderReview' && (
        <Button disabled={busy} variant="contained" onClick={() => run(onApprove)}>
          Approve refund
        </Button>
      )}
      {status === 'Approved' && (
        <Stack direction="row" spacing={1}>
          <TextField size="small" label="Bank transfer reference" value={reference} onChange={(event) => setReference(event.target.value)} required />
          <Button disabled={busy || !reference.trim()} variant="contained" onClick={() => run(() => onRecordSent(reference.trim()))}>
            Record transfer sent
          </Button>
        </Stack>
      )}
      {status === 'Processing' && (
        <Button disabled={busy} variant="contained" onClick={() => run(onConfirmReceived)}>
          Confirm transfer received
        </Button>
      )}
    </Stack>
  );
}
