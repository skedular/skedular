import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

const Logo = () => (
  <Link href="/" style={{ textDecoration: 'none' }}>
    <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
      <Box component="img" src="/images/logo.png" sx={{ width: 60, height: 60 }} alt="UnityHub Logo" />
      <Typography variant="h3" sx={{ fontWeight: 700 }}>
        UnityHub
      </Typography>
    </Stack>
  </Link>
);

export default memo(Logo);
