import { BodyIconTypography, CaptionIconTypography, StackColumn } from '@skedular/ui';
import Box from '@mui/material/Box';
import { memo } from 'react';

export type EntitlementHistoryItem = {
  id: string;
  status: string;
  availableQuantity: number;
  expiresAt: string;
  refundStatus?: string | null;
  paymentStatus?: string | null;
  renewalStatus?: string | null;
  nextRenewalAt?: string | null;
  paymentAction?: string | null;
  bookingIds?: string[];
};

export type EntitlementHistoryListProps = { items: EntitlementHistoryItem[] };

const EntitlementHistoryList = ({ items }: EntitlementHistoryListProps) => (
  <StackColumn spacing={1.5}>
    <CaptionIconTypography label="Credit history" sx={{ textTransform: 'uppercase', opacity: 0.72 }} />
    {items.length === 0 ? (
      <BodyIconTypography label="No credit entitlements found." />
    ) : (
      items.map((item) => (
        <Box key={item.id} sx={{ p: 1.5, borderRadius: 2, border: 1, borderColor: 'divider' }}>
          <BodyIconTypography label={`${item.availableQuantity} credits available · ${item.status}`} sx={{ fontWeight: 700 }} />
          <BodyIconTypography label={`Expires ${new Date(item.expiresAt).toLocaleDateString()}${item.refundStatus ? ` · Refund: ${item.refundStatus}` : ''}`} />
          {item.paymentStatus ? <BodyIconTypography label={`Payment: ${item.paymentStatus}`} /> : null}
          {item.renewalStatus ? (
            <BodyIconTypography label={`Renewal: ${item.renewalStatus}${item.nextRenewalAt ? ` · Next: ${new Date(item.nextRenewalAt).toLocaleDateString()}` : ''}`} />
          ) : null}
          {item.paymentAction ? <BodyIconTypography label={`Payment action: ${item.paymentAction}`} /> : null}
          {item.bookingIds?.length ? <BodyIconTypography label={`Linked bookings: ${item.bookingIds.join(', ')}`} /> : null}
        </Box>
      ))
    )}
  </StackColumn>
);

export default memo(EntitlementHistoryList);
