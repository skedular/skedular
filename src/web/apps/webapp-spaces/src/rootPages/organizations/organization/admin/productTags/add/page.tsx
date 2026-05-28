import { getOrganizationMarketplaceSetupProductTagsBaseLink } from '@/components/links';
import { AddOrganizationProductTagPage } from '@/components/organization/addOrganizationProductTag';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import { useIntegratedPlatrform } from '@skedular/shared';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const { organizationCustomDomain } = useKnownParams();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const searchParams = useSearchParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  const returnUrl = searchParams.get('redirectUrl') ?? getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatrform, organizationCustomDomain);

  return (
    <RootShell>
      <AddOrganizationProductTagPage organizationCustomDomain={organizationCustomDomain} onAddClicked={() => router.push(returnUrl)} onCancel={() => router.push(returnUrl)} />
    </RootShell>
  );
};

export default memo(RootPage);
