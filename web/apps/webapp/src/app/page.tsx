'use client';

import { useKnownParams } from '@/libs/providers';
import OrganizationStoreFrontPage from '@/rootPages/organization-store-front/page';
import Page from '@/rootPages/page';

import { memo } from 'react';

const RootPage = () => {
  const { isCustomDomain } = useKnownParams();

  if (isCustomDomain) {
    return <OrganizationStoreFrontPage />;
  }

  return <Page />;
};

export default memo(RootPage);
