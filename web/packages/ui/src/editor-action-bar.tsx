import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import type { ReactNode } from 'react';

type Props = {
  primaryAction: ReactNode;
  secondaryActions?: ReactNode;
};

const EditorActionBar = ({ primaryAction, secondaryActions }: Props) => (
  <Stack direction="row" spacing={1} sx={{ gap: 1, justifyContent: 'space-between', flexWrap: 'wrap' }}>
    <Stack direction="row" spacing={1} sx={{ gap: 1, flexWrap: 'wrap' }}>
      {secondaryActions}
    </Stack>
    {typeof primaryAction === 'string' ? (
      <Button variant="contained" type="submit" sx={{ textTransform: 'none' }}>
        {primaryAction}
      </Button>
    ) : (
      primaryAction
    )}
  </Stack>
);

export default EditorActionBar;
