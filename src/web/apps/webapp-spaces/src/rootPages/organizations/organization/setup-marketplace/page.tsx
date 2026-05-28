import { getOrganizationMarketplaceSetupMarketplaceListingBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { useIntegratedPlatrform, useKnownParams } from '@skedular/shared';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect } from 'react';

const legacySections = new Set(['marketplace-listing', 'billing-cycle', 'xero-setup', 'stripe-connect-accounts-setup', 'bank-accounts-setup', 'product-tags-setup']);

const RootPage = () => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { organizationCustomDomain } = useKnownParams();
  const { integratedPlatrform } = useIntegratedPlatrform();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    const requestedSection = searchParams.get('section');
    const section = requestedSection && legacySections.has(requestedSection) ? requestedSection : 'marketplace-listing';
    const target = `${getOrganizationMarketplaceSetupMarketplaceListingBaseLink(integratedPlatrform, organizationCustomDomain).split('?')[0]}?section=${section}`;

    router.replace(target);
  }, [integratedPlatrform, organizationCustomDomain, router, searchParams]);

  return <Loading />;
};

export default memo(RootPage);
