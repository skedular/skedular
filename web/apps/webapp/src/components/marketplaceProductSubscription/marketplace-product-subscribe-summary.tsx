import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, SubtitleIconTypography } from '@skedular/ui';
import { MarketplaceCancellationPolicyDetails } from '@/components/marketplaceProduct';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import { memo } from 'react';

type Props = {
  amountLabel: string;
  autoRenew: boolean;
  billingModeLabel: string;
  cadenceLabel: string;
  cancellationPolicyType: string | null | undefined;
  cancellationRefundRules: ReadonlyArray<{ minutesBefore: number; refundPercentage: number }> | null | undefined;
  productType: string | null | undefined;
  quantity: number;
  startsOnLabel: string;
  taxLabel: string;
  title: string;
};

const SummaryRow = ({ label, value }: { label: string; value: string }) => (
  <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
    <CaptionIconTypography label={label} sx={{ opacity: 0.7 }} />
    <BodyIconTypography label={value} sx={{ textAlign: 'right' }} />
  </Box>
);

const MarketplaceProductSubscribeSummary = ({
  amountLabel,
  autoRenew,
  billingModeLabel,
  cadenceLabel,
  cancellationPolicyType,
  cancellationRefundRules,
  productType,
  quantity,
  startsOnLabel,
  taxLabel,
  title,
}: Props) => {
  return (
    <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider, position: { md: 'sticky' }, top: { md: 96 } }}>
      <CardContent sx={{ p: 3 }}>
        <CaptionIconTypography label="Order summary" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
        <LeadIconTypography label={title} sx={{ mt: 1 }} />
        <SubtitleIconTypography label={amountLabel} sx={{ mt: 1.5, fontSize: '2rem', lineHeight: 1 }} />
        <CaptionIconTypography label={taxLabel} sx={{ mt: 0.5, opacity: 0.7 }} />

        <Divider sx={{ my: 2.25 }} />

        <StackColumn spacing={1.2}>
          <SummaryRow label="Plan" value={cadenceLabel} />
          <SummaryRow label="Quantity" value={`${quantity}`} />
          <SummaryRow label="Starts" value={startsOnLabel} />
          <SummaryRow label="Billing" value={billingModeLabel} />
          <SummaryRow label="Renewal" value={autoRenew ? 'Auto-renew on' : 'Ends after this period'} />
        </StackColumn>

        <Box sx={{ mt: 2.5, p: 2, borderRadius: 3, bgcolor: (theme) => theme.palette.action.hover }}>
          <StackColumn spacing={1}>
            <BodyIconTypography
              label={
                productType === 'EVENT'
                  ? autoRenew
                    ? 'Each cycle will reserve the full matching event resource set again, including across multiple locations. If one required resource is unavailable for a future cycle, that cycle cannot be materialized.'
                    : 'This purchase reserves the full matching event resource set for the current cadence window only.'
                  : autoRenew
                    ? 'Your next cycle will use the latest matching pricing option for the same cadence.'
                    : 'This purchase covers the current cadence window only. No additional renewal will be created.'
              }
              sx={{ opacity: 0.86 }}
            />
          </StackColumn>
        </Box>
        <Box sx={{ mt: 2.5 }}>
          <MarketplaceCancellationPolicyDetails cancellationPolicyType={cancellationPolicyType} cancellationRefundRules={cancellationRefundRules} eventLabel="the next renewal" />
        </Box>
      </CardContent>
    </Card>
  );
};

export default memo(MarketplaceProductSubscribeSummary);
