import { OrganizationUsers } from '@/components/organization/organizationUsers';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import { memo } from 'react';

const OrganizationsPage = () => {
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  return (
    <RootShell>
      <OrganizationUsers organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
