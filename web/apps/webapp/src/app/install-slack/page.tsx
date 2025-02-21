'use client';

import { StackRow } from '@/components/commons';
import { SlackButton } from '@/components/slackButtons';
import { memo } from 'react';

const InstallSlack = () => (
  <StackRow>
    <SlackButton />
  </StackRow>
);

export default memo(InstallSlack);
