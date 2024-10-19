'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
import { memo } from 'react';

const TeamsPage = () => {
  const breadcrumps: appBarBreadcrumbs = {
    items: [
      {
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
