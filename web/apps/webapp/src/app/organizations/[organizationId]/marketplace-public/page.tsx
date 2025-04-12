'use client';

import { OrganizationProducts } from '@/components/organization/organizationProducts';
import { RootShell } from '@/components/rootShell';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const OrganizationMarketplacePublicPage = () => {
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  return (
    <RootShell>
      <OrganizationProducts organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationMarketplacePublicPage);
