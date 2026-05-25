import { getOrganizationMarketplaceSetupProductTagsBaseLink } from '@/components/links';
import { EditOrganizationProductTagPage } from '@/components/organization/editOrganizationProductTag';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import { useIntegratedPlatrform } from '@skedular/shared';
import { useParams, useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const getParamValue = (value: string | string[] | undefined): string => (Array.isArray(value) ? (value[0] ?? '') : (value ?? ''));

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const searchParams = useSearchParams();
  const productTagId = getParamValue(useParams().productTagId);

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }
  if (!productTagId) {
    throw new Error('productTagId is required');
  }

  const returnUrl = searchParams.get('redirectUrl') ?? getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatrform, organizationCustomDomain);

  return (
    <RootShell>
      <EditOrganizationProductTagPage productTagId={productTagId} onSaved={() => router.push(returnUrl)} onCancel={() => router.push(returnUrl)} />
    </RootShell>
  );
};

export default memo(RootPage);
