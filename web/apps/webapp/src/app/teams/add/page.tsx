'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { HomeIcon } from '@repo/shared/components/icons';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddTeamPage = () => {
  const router = useRouter();

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

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddTeam onAdded={handleAdded} onCancelled={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddTeamPage);
