import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { useParams, useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const router = useRouter();
  const { organizationUniqueAlphanumericName } = useParams();
  let finalOrganizationUniqueAlphanumericName = '';

  if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] === 'undefined') {
      throw new Error('organizationUniqueAlphanumericName is required');
    }

    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
  } else {
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
        organizationUniqueAlphanumericName={finalOrganizationUniqueAlphanumericName}
        showDismiss={false}
        onAdded={handleAdded}
        onCancel={handleCancelled}
        onReloadRequired={handleReloadRequired}
      />
    </RootShell>
  );
};

export default memo(RootPage);
