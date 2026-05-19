import { StackRow } from '@skedular/ui';
import { SlackButton } from '@/components/slackButtons';
import { memo } from 'react';

const RootPage = () => (
  <StackRow>
    <SlackButton />
  </StackRow>
);

export default memo(RootPage);
