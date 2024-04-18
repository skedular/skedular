'use client';

import { PublicMainRootLayout } from '@/components/layouts';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

const AttendanceVisibility = () => {
  return (
    <PublicMainRootLayout>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          alignItems: 'center',
          p: '6rem',
        }}
      >
        <Typography variant="h1">You&apos;re all set!</Typography>
      </Box>
    </PublicMainRootLayout>
  );
};

export default memo(AttendanceVisibility);
