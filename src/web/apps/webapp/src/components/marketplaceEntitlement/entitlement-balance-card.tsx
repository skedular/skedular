import { BodyIconTypography, CaptionIconTypography, StackColumn } from '@skedular/ui';
import Box from '@mui/material/Box';
import LinearProgress from '@mui/material/LinearProgress';
import { memo } from 'react';

export type EntitlementBalanceCardProps = {
  availableQuantity: number;
  grantedQuantity: number;
  expiresAt: string;
  currency?: string;
  refundAmount?: number | null;
  restrictions?: { availableDays: readonly string[]; minDurationMinutes?: number | null; maxDurationMinutes?: number | null; numberOfResourcesToBook: number } | null;
};

const EntitlementBalanceCard = ({ availableQuantity, grantedQuantity, expiresAt, currency, refundAmount, restrictions }: EntitlementBalanceCardProps) => {
  const progress = grantedQuantity > 0 ? Math.max(0, Math.min(100, (availableQuantity / grantedQuantity) * 100)) : 0;
  return (
    <Box sx={{ p: 2, borderRadius: 3, border: 1, borderColor: 'divider' }}>
      <StackColumn spacing={1}>
        <CaptionIconTypography label="Booking credits" sx={{ textTransform: 'uppercase', opacity: 0.72 }} />
        <BodyIconTypography label={`${availableQuantity} credits remaining`} sx={{ fontWeight: 700 }} />
        <LinearProgress variant="determinate" value={progress} aria-label="Remaining booking credits" />
        <BodyIconTypography label={`Expires ${new Date(expiresAt).toLocaleDateString()}`} />
        {refundAmount != null && <BodyIconTypography label={`Refund amount: ${refundAmount} ${currency ?? ''}`} />}
        {restrictions?.availableDays.length ? <BodyIconTypography label={`Available days: ${restrictions.availableDays.join(', ')}`} /> : null}
        {restrictions?.minDurationMinutes != null && <BodyIconTypography label={`Minimum booking duration: ${restrictions.minDurationMinutes} minutes`} />}
        {restrictions?.maxDurationMinutes != null && <BodyIconTypography label={`Maximum booking duration: ${restrictions.maxDurationMinutes} minutes`} />}
        {restrictions && <BodyIconTypography label={`Resources per booking: ${restrictions.numberOfResourcesToBook}`} />}
      </StackColumn>
    </Box>
  );
};

export default memo(EntitlementBalanceCard);
