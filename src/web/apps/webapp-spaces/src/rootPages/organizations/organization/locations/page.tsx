import { OrganizationLocations } from '@/components/organization/organizationLocations';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import { memo } from 'react';

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  return (
    <RootShell>
      <OrganizationLocations organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(RootPage);
