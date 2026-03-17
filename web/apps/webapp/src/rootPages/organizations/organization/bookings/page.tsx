import { OrganizationBookings } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import { memo } from 'react';

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  return (
    <RootShell>
      <OrganizationBookings organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(RootPage);
