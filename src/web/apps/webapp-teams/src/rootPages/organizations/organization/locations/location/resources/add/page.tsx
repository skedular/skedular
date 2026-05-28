import { RootShell } from '@/components/rootShell';
import { AddResourcePage } from '@/components/resource/addResource/add-resource-dialog';
import { useKnownParams } from '@skedular/shared';
import { memo } from 'react';

const RootPage = () => {
  const { organizationCustomDomain, locationId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!locationId) {
    throw new Error('locationId is required');
  }

  return (
    <RootShell>
      <AddResourcePage organizationCustomDomain={organizationCustomDomain} locationId={locationId} />
    </RootShell>
  );
};

export default memo(RootPage);
