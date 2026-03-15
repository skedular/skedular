import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, SubtitleIconTypography } from '@/components/commons';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import { memo } from 'react';

type Props = {
  amountLabel: string;
  dateLabel: string;
  durationLabel: string;
  paymentLabel: string;
  quantity: number;
  taxLabel: string;
  title: string;
};

const SummaryRow = ({ label, value }: { label: string; value: string }) => (
  <Box sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
    <CaptionIconTypography label={label} sx={{ opacity: 0.7 }} />
    <BodyIconTypography label={value} sx={{ textAlign: 'right' }} />
  </Box>
);

const MarketplaceProductBookingSummary = ({ amountLabel, dateLabel, durationLabel, paymentLabel, quantity, taxLabel, title }: Props) => (
  <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider, position: { md: 'sticky' }, top: { md: 96 } }}>
    <CardContent sx={{ p: 3 }}>
      <CaptionIconTypography label="Booking summary" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
      <LeadIconTypography label={title} sx={{ mt: 1 }} />
      <SubtitleIconTypography label={amountLabel} sx={{ mt: 1.5, fontSize: '2rem', lineHeight: 1 }} />
      <CaptionIconTypography label={taxLabel} sx={{ mt: 0.5, opacity: 0.7 }} />

      <Divider sx={{ my: 2.25 }} />

      <StackColumn spacing={1.2}>
        <SummaryRow label="Date" value={dateLabel} />
        <SummaryRow label="Duration" value={durationLabel} />
        <SummaryRow label="Quantity" value={`${quantity}`} />
        <SummaryRow label="Payment" value={paymentLabel} />
      </StackColumn>

      <Box sx={{ mt: 2.5, p: 2, borderRadius: 3, bgcolor: (theme) => theme.palette.action.hover }}>
        <BodyIconTypography
          label="Resource allocation is handled after checkout for now. A dedicated floor-plan resource picker can be added later without changing this purchase flow."
          sx={{ opacity: 0.86 }}
        />
      </Box>
    </CardContent>
  </Card>
);

export default memo(MarketplaceProductBookingSummary);
