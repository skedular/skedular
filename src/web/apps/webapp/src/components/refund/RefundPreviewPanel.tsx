'use client';

import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Stack from '@mui/material/Stack';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography } from '@skedular/ui';
import { graphql, useFragment } from 'react-relay';

import type { RefundPreviewPanel_query$key } from '../../queries/__generated__/RefundPreviewPanel_query.graphql';

const query = graphql`
  fragment RefundPreviewPanel_query on Query @argumentDefinitions(bookingId: { type: "String!" }) {
    marketplaceBookingRefundPreview(bookingId: $bookingId) {
      refundAmount
      baseAmount
      refundPercentage
      currencyToDisplay
      isRefundable
    }
  }
`;

export type RefundPreviewPanelProps = {
  query: RefundPreviewPanel_query$key | null;
};

export function RefundPreviewPanel({ query: queryRef }: RefundPreviewPanelProps) {
  const data = useFragment(query, queryRef);
  const preview = data?.marketplaceBookingRefundPreview;

  if (!preview || !preview.isRefundable) {
    return null;
  }

  const finalRefundableAmount = preview.refundAmount ?? 0;
  const baseAmount = preview.baseAmount ?? 0;
  const nonRefundableAmount = baseAmount - finalRefundableAmount;
  const refundPercentage = preview.refundPercentage;
  const currency = preview.currencyToDisplay;

  const calculationReason =
    refundPercentage === 100
      ? 'Full refund within cancellation window'
      : refundPercentage === 0
        ? 'No refund - outside cancellation window'
        : `Partial refund - ${refundPercentage}% of original amount`;

  return (
    <Card variant="outlined">
      <CardContent>
        <Stack spacing={1.5}>
          <LeadIconTypography label="Refund preview" />
          <BodyIconTypography label={`Refundable: ${finalRefundableAmount} ${currency}`} />
          <CaptionIconTypography label={`Non-refundable: ${nonRefundableAmount} ${currency}`} />
          <CaptionIconTypography label={calculationReason} />
        </Stack>
      </CardContent>
    </Card>
  );
}
