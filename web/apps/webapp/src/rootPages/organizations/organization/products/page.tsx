import { OrganizationProducts } from '@/components/organization/organizationProducts';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';
import useKnownParams from '@/hooks/use-known-params';

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  return (
    <RootShell>
      <OrganizationProducts organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(RootPage);
