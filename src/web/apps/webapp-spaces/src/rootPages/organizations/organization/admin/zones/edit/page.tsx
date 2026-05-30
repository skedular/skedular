import { useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import { getOrganizationAdminZonesBaseLink } from '@/components/links';
import { EditOrganizationZonePage } from '@/components/organization/editOrganizationZone';
import { RootShell } from '@/components/rootShell';

import { useParams, useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const getParamValue = (value: string | string[] | undefined): string => (Array.isArray(value) ? (value[0] ?? '') : (value ?? ''));

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const searchParams = useSearchParams();
  const zoneId = getParamValue(useParams().zoneId);

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }
  if (!zoneId) {
    throw new Error('zoneId is required');
  }

  const returnUrl = searchParams.get('redirectUrl') ?? getOrganizationAdminZonesBaseLink(integratedPlatform, organizationCustomDomain);

  return (
    <RootShell>
      <EditOrganizationZonePage zoneId={zoneId} onSaved={() => router.push(returnUrl)} onCancel={() => router.push(returnUrl)} />
    </RootShell>
  );
};

export default memo(RootPage);
