import { StackRow } from '@/components/commons';
import { SlackButton } from '@/components/slackButtons';
import { memo } from 'react';

const RootPage = () => (
  <StackRow>
    <SlackButton />
  </StackRow>
);

export default memo(RootPage);
