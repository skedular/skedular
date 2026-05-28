'use client';

import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import type { SxProps, Theme } from '@mui/system';
import type { PropsWithChildren, ReactNode } from 'react';
import { StackColumn } from './index';

type Props = {
  title: ReactNode;
  description?: ReactNode;
  top?: number;
  sx?: SxProps<Theme>;
};

const StickyReviewRail = ({ title, description, top = 24, sx, children }: PropsWithChildren<Props>) => (
  <StackColumn
    spacing={2}
    sx={[
      {
        pl: { xs: 2, xl: 0 },
        pr: 2,
        pt: 2,
        position: { xl: 'sticky' },
        top: { xl: top },
        alignSelf: 'start',
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <Box>
      <Typography variant="subtitle1">{title}</Typography>
      {description ? (
        <Typography variant="body2" sx={{ opacity: 0.75, mt: 0.5 }}>
          {description}
        </Typography>
      ) : null}
    </Box>
    {children}
  </StackColumn>
);

export default StickyReviewRail;
