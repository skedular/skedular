'use client';

import { postSignOutReturnToKey } from '@/components/links';
import { getProductAppDefinition, useKnownParams } from '@skedular/shared';
import CoWorkingSubdomain from './customer-facing-subdomain/co-working-subdomain';
import PrivateOrganizationSubdomain from './customer-facing-subdomain/private-organization-subdomain';
import { resolveCustomerFacingEntryPoint } from './customer-facing-subdomain/customer-facing-subdomain-resolver';
import OrganizationStoreFrontPage from '@/rootPages/marketplace/page';
import Page from '@/rootPages/page';
import { memo, useEffect } from 'react';

const appDefinition = getProductAppDefinition('webapp');

const RootPage = () => {
  const { isCustomDomain } = useKnownParams();
  const customerFacingEntryPoint = resolveCustomerFacingEntryPoint({ isCustomDomain });

  useEffect(() => {
    const rawReturnTo = sessionStorage.getItem(postSignOutReturnToKey);
    if (!rawReturnTo || !rawReturnTo.startsWith('/')) {
      return;
    }

    const currentPath = `${window.location.pathname}${window.location.search}${window.location.hash}`;
    if (currentPath === rawReturnTo) {
      sessionStorage.removeItem(postSignOutReturnToKey);
      return;
    }

    sessionStorage.removeItem(postSignOutReturnToKey);
    window.location.replace(rawReturnTo);
  }, []);

  if (customerFacingEntryPoint === 'private-organisation-subdomain') {
    return (
      <div data-product-app={appDefinition.id} data-review-scope={customerFacingEntryPoint}>
        <PrivateOrganizationSubdomain />
      </div>
    );
  }

  if (customerFacingEntryPoint === 'co-working-subdomain') {
    return (
      <div data-product-app={appDefinition.id} data-review-scope={customerFacingEntryPoint}>
        <CoWorkingSubdomain>
          <OrganizationStoreFrontPage />
        </CoWorkingSubdomain>
      </div>
    );
  }

  return (
    <div data-product-app={appDefinition.id} data-review-scope={customerFacingEntryPoint}>
      <Page />
    </div>
  );
};

export default memo(RootPage);
