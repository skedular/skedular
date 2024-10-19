'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
import { memo } from 'react';

const TeamsPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Teams',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <Teams />
    </RootShell>
  );
};

export default memo(TeamsPage);
