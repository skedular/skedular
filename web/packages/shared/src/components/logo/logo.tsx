import Box from '@mui/material/Box';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { memo } from 'react';

const Logo = () => (
  <Link href="/" style={{ textDecoration: 'none' }}>
    <Stack direction="row">
      <Box component="img" src="/images/logo.png" sx={{ width: 60, height: 60 }} alt="UnityHub Logo" />
      <Typography
        variant="h4"
        sx={{
          fontWeight: 700,
          color: 'primary.main',
          py: 1.2,
        }}
      >
        unityhub.io
      </Typography>
    </Stack>
  </Link>
);

export default memo(Logo);
