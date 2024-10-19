'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { memo } from 'react';

const AddTeamPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
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
    <RootShell appBarBreadcrumb={breadcrumps}>
      <AddTeam />
    </RootShell>
  );
};

export default memo(AddTeamPage);
