import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Image from 'next/image';
import Link from 'next/link';
import { memo } from 'react';

const Logo = () => {
  return (
    <Link href="/" style={{ textDecoration: 'none' }}>
      <Stack direction="row">
        <Image src="/images/logo.png" width={60} height={60} alt="UnityHub Logo" />
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
};

export default memo(Logo);
