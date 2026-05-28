import { OrganizationUsers } from '@/components/organization/organizationUsers';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';
import useKnownParams from '@/hooks/use-known-params';

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
