'use client';

import { SmallHeadingIconTypography } from '@/components/commons';
import { InstallIcon } from '@/components/icons';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import { memo } from 'react';

const RootPage = () => (
  <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
    <SmallHeadingIconTypography label="Your administrator needs to install Skedular for you. This is a one-time setup. Please click the button below to start the installation." />
    <Button LinkComponent={Link} variant="contained" href="/start-install-msteams" startIcon={<InstallIcon />}>
      Install
    </Button>
  </Box>
);

export default memo(RootPage);
