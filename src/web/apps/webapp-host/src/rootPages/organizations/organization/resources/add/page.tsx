import { RootShell } from '@/components/rootShell';
import { AddResourcePage } from '@/components/resource/addResource/add-resource-dialog';
import { useKnownParams } from '@skedular/shared';
import { memo } from 'react';

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  return (
    <RootShell>
      <AddResourcePage organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default memo(RootPage);
