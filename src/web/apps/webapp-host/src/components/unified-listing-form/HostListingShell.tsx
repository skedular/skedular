import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { BodyIconTypography } from '@skedular/ui';
import { ReactNode } from 'react';

export type HostListingShellProps = {
  locationContent: ReactNode;
  productContent: ReactNode;
  isProductReady: boolean;
  pendingMessage?: string;
};

const HostListingShell = ({ locationContent, productContent, isProductReady, pendingMessage }: HostListingShellProps) => {
  return (
    <Stack spacing={3}>
      <Box>{locationContent}</Box>
      <Box>
        {isProductReady ? (
          productContent
        ) : (
          <BodyIconTypography label={pendingMessage ?? 'Pricing and booking settings are still being prepared. You can continue editing location details now.'} />
        )}
      </Box>
    </Stack>
  );
};

export default HostListingShell;
