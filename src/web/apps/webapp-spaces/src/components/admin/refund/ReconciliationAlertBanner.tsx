'use client';

import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';

export function ReconciliationAlertBanner({ count, onOpenQueue }: { count: number; onOpenQueue: () => void }) {
  if (count <= 0) return null;
  return (
    <Alert
      severity="warning"
      action={
        <Button color="inherit" size="small" onClick={onOpenQueue}>
          Open queue
        </Button>
      }
    >
      <Stack component="span">
        {count} refund{count === 1 ? '' : 's'} require reconciliation.
      </Stack>
    </Alert>
  );
}
