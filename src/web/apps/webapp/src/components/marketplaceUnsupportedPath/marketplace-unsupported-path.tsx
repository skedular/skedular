'use client';

import logger from '@/libs/logging';
import { logUnsupportedWebappPathHandled } from '@/libs/logging/aggregate-marketplace-telemetry';
import Box from '@mui/material/Box';
import { BodyIconTypography, SmallHeadingIconTypography, SmallIconTypography, StackColumn } from '@skedular/ui';
import { useEffect } from 'react';

type Props = {
  pathCategory?: string;
  ownerClassification?: string;
};

const MarketplaceUnsupportedPath = ({ pathCategory = 'unsupported', ownerClassification }: Props) => {
  useEffect(() => {
    logUnsupportedWebappPathHandled({ logger, pathCategory, ownerClassification });
  }, [ownerClassification, pathCategory]);

  return (
    <Box component="section" role="status" aria-live="polite" sx={{ maxWidth: 720, mx: 'auto', px: 3, py: 8 }}>
      <StackColumn spacing={1.5}>
        <SmallHeadingIconTypography label="This page is not available here" />
        <BodyIconTypography label="The customer marketplace stays on this webapp, but this path is not part of the current customer experience." />
        <SmallIconTypography label="You can keep browsing marketplace locations from the current webapp without being redirected." />
      </StackColumn>
    </Box>
  );
};

export default MarketplaceUnsupportedPath;
