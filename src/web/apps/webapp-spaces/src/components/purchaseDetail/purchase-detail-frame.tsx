'use client';

import Box from '@mui/system/Box';
import { PageHeaderPanel, StackColumn } from '@skedular/ui';
import type { ReactNode } from 'react';
import { PurchaseDetailNavigation } from './purchase-detail-navigation';

type Props = {
  children: ReactNode;
  hasLinkedBookings?: boolean;
  title?: string;
  description?: string;
};

export const PurchaseDetailFrame = ({
  children,
  hasLinkedBookings = true,
  title = 'Purchase details',
  description = 'Review payment, refunds, and the activity associated with this purchase.',
}: Props) => (
  <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: { xs: 1, sm: 2 } }}>
    <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pb: 4 }} spacing={2}>
      <PageHeaderPanel title={title} description={description} />
      <PurchaseDetailNavigation hasLinkedBookings={hasLinkedBookings} />
      {children}
    </StackColumn>
  </Box>
);
