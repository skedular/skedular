'use client';

import { MarketplaceUnsupportedPath } from '@/components/marketplaceUnsupportedPath';
import { memo } from 'react';

const RootPage = () => <MarketplaceUnsupportedPath pathCategory="unsupported-marketplace" ownerClassification="webapp" />;

export default memo(RootPage);
