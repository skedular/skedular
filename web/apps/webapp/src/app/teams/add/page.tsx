'use client';

import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddTeamPage = () => {
  const router = useRouter();

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <AddTeam onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddTeamPage);
