'use client';

import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import { useMemo, useState } from 'react';

export function PartialRefundForm({
  remainingBalance,
  currency,
  onSubmit,
}: {
  remainingBalance: number;
  currency: string;
  onSubmit: (amount: number, reason: string, idempotencyKey: string) => Promise<void>;
}) {
  const [amount, setAmount] = useState('');
  const [reason, setReason] = useState('');
  const [busy, setBusy] = useState(false);
  const [confirming, setConfirming] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const parsedAmount = Number(amount);
  const validationError = useMemo(() => {
    if (!amount) return 'Enter a refund amount.';
    if (!Number.isFinite(parsedAmount) || parsedAmount <= 0) return 'Enter an amount greater than zero.';
    if (parsedAmount > remainingBalance) return `The amount cannot exceed the remaining balance of ${remainingBalance} ${currency}.`;
    if (!reason.trim()) return 'Enter a reason for the partial refund.';
    return null;
  }, [amount, currency, parsedAmount, reason, remainingBalance]);
  const submit = async () => {
    if (validationError) {
      setError(validationError);
      setConfirming(false);
      return;
    }
    if (!confirming) {
      setError(null);
      setConfirming(true);
      return;
    }
    setBusy(true);
    setError(null);
    try {
      await onSubmit(parsedAmount, reason.trim(), crypto.randomUUID());
    } catch (cause) {
      setError(cause instanceof Error ? cause.message : 'The refund could not be created.');
    } finally {
      setBusy(false);
    }
  };
  return (
    <Stack spacing={2}>
      {error && <Alert severity="error">{error}</Alert>}
      <TextField label={`Amount (${currency})`} value={amount} onChange={(event) => setAmount(event.target.value)} inputMode="decimal" required />
      <TextField label="Reason" value={reason} onChange={(event) => setReason(event.target.value)} multiline minRows={2} required />
      {confirming && (
        <Alert severity="warning">
          Confirm a partial refund of {parsedAmount.toFixed(2)} {currency} for “{reason.trim()}”. This action cannot be undone.
        </Alert>
      )}
      {confirming ? (
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" disabled={busy} onClick={() => setConfirming(false)}>
            Back
          </Button>
          <Button variant="contained" disabled={busy} onClick={submit}>
            Confirm partial refund
          </Button>
        </Stack>
      ) : (
        <Button variant="contained" disabled={busy} onClick={submit}>
          Create partial refund
        </Button>
      )}
    </Stack>
  );
}
