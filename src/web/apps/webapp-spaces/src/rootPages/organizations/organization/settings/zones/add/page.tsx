import { useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import { getOrganizationSettingsZonesBaseLink } from '@/components/links';
import { AddOrganizationZonePage } from '@/components/organization/addOrganizationZone';
import { RootShell } from '@/components/rootShell';

import { useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const searchParams = useSearchParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  const returnUrl = searchParams.get('redirectUrl') ?? getOrganizationSettingsZonesBaseLink(integratedPlatform, organizationCustomDomain);

  return (
    <RootShell>
      <AddOrganizationZonePage organizationCustomDomain={organizationCustomDomain} onAddClicked={() => router.push(returnUrl)} onCancel={() => router.push(returnUrl)} />
    </RootShell>
  );
};

export default memo(RootPage);
