'use client';

import TryIcon from '@mui/icons-material/Try';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import { StackRow } from '@repo/shared/components/commons';
import { SlackButton } from '@repo/shared/components/slackButtons';
import NextLink from 'next/link';
import { memo } from 'react';

const Home = () => {
  return (
    <StackRow>
      <SlackButton />
      <Link component={NextLink} href="https://app.unityhub.io">
        <Button variant="contained" sx={{ marginLeft: 2, borderRadius: '50px' }} size="large" startIcon={<TryIcon />}>
          Try for free
        </Button>
      </Link>
    </StackRow>
  );
};

export default memo(Home);
