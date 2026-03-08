import { OrganizationBookings } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import { memo } from 'react';

const RootPage = () => {
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  return (
    <RootShell>
      <OrganizationBookings organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
    </RootShell>
  );
};

export default memo(RootPage);
