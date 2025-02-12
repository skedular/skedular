'use client';

import { LargeHeadingIconTypography, StackRow } from '@/components/commons';
import { memo } from 'react';

const Home = () => (
  <StackRow>
    <LargeHeadingIconTypography label="You are all set!" />
  </StackRow>
);

export default memo(Home);
