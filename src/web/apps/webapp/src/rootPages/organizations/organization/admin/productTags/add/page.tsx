import { getOrganizationMarketplaceSetupProductTagsBaseLink } from '@/components/links';
import { AddOrganizationProductTagPage } from '@/components/organization/addOrganizationProductTag';
import { RootShell } from '@/components/rootShell';
import useKnownParams from '@/hooks/use-known-params';
import { useIntegratedPlatform } from '@skedular/shared';
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

  const returnUrl = searchParams.get('redirectUrl') ?? getOrganizationMarketplaceSetupProductTagsBaseLink(integratedPlatform, organizationCustomDomain);

  return (
    <RootShell>
      <AddOrganizationProductTagPage organizationCustomDomain={organizationCustomDomain} onAddClicked={() => router.push(returnUrl)} onCancel={() => router.push(returnUrl)} />
    </RootShell>
  );
};

export default memo(RootPage);
