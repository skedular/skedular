'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { memo } from 'react';

const AddTeamPage = () => {
  const breadcrumps: appBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
      {
        label: 'Teams',
        href: '/teams',
      },
    ],
    lastItemLabel: 'Add new team',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddTeam />
    </RootShell>
  );
};

export default memo(AddTeamPage);
