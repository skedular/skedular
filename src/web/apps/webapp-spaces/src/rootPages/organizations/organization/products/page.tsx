import { OrganizationProducts } from '@/components/organization/organizationProducts';
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
      <OrganizationProducts organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(RootPage);
