'use client';

import { postSignOutReturnToKey } from '@/components/links';
import { getProductAppDefinition } from '@skedular/shared';
import Page from '@/rootPages/page';
import { memo, useEffect } from 'react';

const appDefinition = getProductAppDefinition('webapp-teams');

const RootPage = () => {
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

  return (
    <div data-product-app={appDefinition.id} data-review-scope="private-organisation-entry">
      <Page />
    </div>
  );
};

export default memo(RootPage);
