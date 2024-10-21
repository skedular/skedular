'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
import { HomeIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

const TeamsPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        icon: <HomeIcon />,
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Teams',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Teams />
    </RootShell>
  );
};

export default memo(TeamsPage);
