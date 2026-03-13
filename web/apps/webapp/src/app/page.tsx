'use client';

import { postSignOutReturnToKey } from '@/components/links';
import { useKnownParams } from '@/libs/providers';
import OrganizationStoreFrontPage from '@/rootPages/marketplace/page';
import Page from '@/rootPages/page';
import { memo, useEffect } from 'react';

const RootPage = () => {
  const { isCustomDomain } = useKnownParams();

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

  if (isCustomDomain) {
    return <OrganizationStoreFrontPage />;
  }

  return <Page />;
};

export default memo(RootPage);
