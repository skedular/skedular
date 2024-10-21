'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { HomeIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

const AddTeamPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        icon: <HomeIcon />,
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
