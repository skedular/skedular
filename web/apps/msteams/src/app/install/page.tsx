import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import Typography from '@mui/material/Typography';
import { InstallIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

const Install = () => (
  <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
    <Typography variant="h2">
      Your administrator needs to install UnityHub for you. This is a one-time setup. Please click the button below to start the installation.
    </Typography>
    <Button LinkComponent={Link} variant="contained" href="/start-install" startIcon={<InstallIcon />}>
      Install
    </Button>
  </Box>
);

export default memo(Install);
