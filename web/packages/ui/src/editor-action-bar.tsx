'use client';

import Button from '@mui/material/Button';
import type { ReactNode } from 'react';
import { StackRow } from './index';

type Props = {
  primaryAction: ReactNode;
  secondaryActions?: ReactNode;
};

const EditorActionBar = ({ primaryAction, secondaryActions }: Props) => (
  <StackRow spacing={1} sx={{ gap: 1, justifyContent: 'space-between', flexWrap: 'wrap' }}>
    <StackRow spacing={1} sx={{ gap: 1, flexWrap: 'wrap' }}>
      {secondaryActions}
    </StackRow>
    {typeof primaryAction === 'string' ? (
      <Button variant="contained" type="submit" sx={{ textTransform: 'none' }}>
        {primaryAction}
      </Button>
    ) : (
      primaryAction
    )}
  </StackRow>
);

export default EditorActionBar;
