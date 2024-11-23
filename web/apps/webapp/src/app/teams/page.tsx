'use client';

import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
import { memo } from 'react';

const TeamsPage = () => (
  <RootShell>
    <Teams />
  </RootShell>
);

export default memo(TeamsPage);
