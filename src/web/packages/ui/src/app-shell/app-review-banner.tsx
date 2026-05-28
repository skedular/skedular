'use client';

import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import type { SxProps, Theme } from '@mui/system';
import type { ReactNode } from 'react';
import BodyIconTypography from '../typography/body-icon-typography';
import SmallIconTypography from '../typography/small-icon-typography';

type Props = {
  title: ReactNode;
  description?: ReactNode;
  sx?: SxProps<Theme>;
};

const AppReviewBanner = ({ title, description, sx }: Props) => (
  <Box
    role="status"
    sx={[
      {
        border: '1px solid',
        borderColor: 'warning.light',
        bgcolor: 'warning.50',
        borderRadius: 2,
        p: 2,
      },
      ...(sx != null ? (Array.isArray(sx) ? sx : [sx]) : []),
    ]}
  >
    <Stack spacing={0.5}>
      <BodyIconTypography label={title} fontWeight={600} />
      {description ? <SmallIconTypography label={description} sx={{ color: 'text.secondary' }} /> : null}
    </Stack>
  </Box>
);

export default AppReviewBanner;
