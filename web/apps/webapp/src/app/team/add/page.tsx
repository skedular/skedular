'use client';

import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { memo } from 'react';

const AddTeamPage = () => (
  <RootShell>
    <AddTeam />
  </RootShell>
);

export default memo(AddTeamPage);
