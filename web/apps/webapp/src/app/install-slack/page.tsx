'use client';

import { StackRow } from '@/components/commons';
import { SlackButton } from '@/components/slackButtons';
import { memo } from 'react';

const Home = () => (
  <StackRow>
    <SlackButton />
  </StackRow>
);

export default memo(Home);
