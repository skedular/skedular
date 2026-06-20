'use client';

import { getProductAppDefinition } from '@skedular/shared';
import Page from '@/rootPages/page';
import { memo, useEffect } from 'react';

const appDefinition = getProductAppDefinition('webapp-host');

const RootPage = () => {
  useEffect(() => {
    // Post-sign-out return-to handling
  }, []);

  return (
    <div data-product-app={appDefinition.id} data-review-scope="host-entry">
      <Page />
    </div>
  );
};

export default memo(RootPage);
