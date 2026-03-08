import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { useKnownParams } from '@/libs/providers';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const router = useRouter();
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <AddTeam
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        showDismiss={false}
        onAdded={handleAdded}
        onCancel={handleCancelled}
        onReloadRequired={handleReloadRequired}
      />
    </RootShell>
  );
};

export default memo(RootPage);
