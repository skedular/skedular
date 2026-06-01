'use client';

import useKnownParams from '@/hooks/use-known-params';
import OrganizationStoreFrontPage from '@/rootPages/marketplace/page';
import Page from '@/rootPages/page';
import { getProductAppDefinition } from '@skedular/shared';
import { memo } from 'react';
import CoWorkingSubdomain from './customer-facing-subdomain/co-working-subdomain';
import { resolveCustomerFacingEntryPoint } from './customer-facing-subdomain/customer-facing-subdomain-resolver';
import PrivateOrganizationSubdomain from './customer-facing-subdomain/private-organization-subdomain';

const appDefinition = getProductAppDefinition('webapp');

const RootPage = () => {
  const { isCustomDomain } = useKnownParams();
  const customerFacingEntryPoint = resolveCustomerFacingEntryPoint({ isCustomDomain });

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
