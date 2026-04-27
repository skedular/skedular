import { BodyIconTypography, CaptionIconTypography, SubtitleIconTypography } from '@skedular/ui';
import Alert from '@mui/material/Alert';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import MarketplaceRefundTimeline from '../marketplaceRefund/marketplace-refund-timeline';
import { formatRefundAmount } from '../marketplaceRefund/refund-display';

type Props = {
  entityLabel: 'booking' | 'subscription';
  isCancelled: boolean;
  isCancelAtPeriodEnd?: boolean;
  isPaymentRequired: boolean;
  paymentStatusType?: string | null | undefined;
  hasInvoice: boolean;
  refund?: {
    currency?: {
      type: string;
      name: string;
    } | null;
    status: {
      type: string;
      name: string;
    };
    requestedAt?: string | null | undefined;
    lastProcessedAt?: string | null | undefined;
    refundAmount?: number | null | undefined;
    refundPercentage?: number | null | undefined;
    currencyToDisplay: string;
    reason?: string | null | undefined;
    lastError?: string | null | undefined;
    externalRefundNumber?: string | null | undefined;
    requestedByCustomerName?: string | null | undefined;
    events?: ReadonlyArray<{
      id: string;
      eventType: {
        type: string;
        name: string;
      };
      occurredAt?: string | null | undefined;
      refundAmount?: number | null | undefined;
      currencyToDisplay: string;
      reason?: string | null | undefined;
      lastError?: string | null | undefined;
      externalRefundNumber?: string | null | undefined;
      actorName?: string | null | undefined;
    }> | null;
  } | null;
};

const toRefundStatusType = (value?: string | null | undefined) => value?.replace(/([a-z])([A-Z])/g, '$1_$2').toUpperCase() ?? '';

const MarketplaceRefundStatusCard = ({ entityLabel, isCancelled, isCancelAtPeriodEnd = false, isPaymentRequired, paymentStatusType, hasInvoice, refund }: Props) => {
  const isPaidOrRecorded = paymentStatusType === 'CONFIRMED' || hasInvoice;
  const amountLabel = formatRefundAmount(refund?.refundAmount, refund?.currency?.type, refund?.currencyToDisplay);
  const refundStatusType = toRefundStatusType(refund?.status.type);

  const content = (() => {
    if (refund) {
      switch (refundStatusType) {
        case 'COMPLETED':
          return {
            title: 'Refund completed',
            body: amountLabel
              ? `A refund of ${amountLabel} has been completed${refund.externalRefundNumber ? ` under reference ${refund.externalRefundNumber}` : ''}.`
              : 'This refund has been completed.',
            severity: 'success' as const,
          };

        case 'PENDING_ACCOUNTING':
          return {
            title: 'Refund in progress',
            body: amountLabel
              ? `We're submitting a refund of ${amountLabel}. It may take a little time for your provider to confirm it.`
              : 'We are submitting your refund now. It may take a little time for your provider to confirm it.',
            severity: 'info' as const,
          };

        case 'FAILED':
          return {
            title: 'Refund update needed',
            body: refund.lastError || 'We could not finish this refund automatically. Our team needs to review it before it can continue.',
            severity: 'warning' as const,
          };

        case 'MANUAL_REQUIRED':
          return {
            title: 'Refund under review',
            body: refund.lastError || 'This refund is being reviewed by our team before it can be completed.',
            severity: 'warning' as const,
          };

        case 'MANUAL_COMPLETED':
          return {
            title: 'Refund completed manually',
            body: amountLabel ? `A refund of ${amountLabel} has been completed manually by the team.` : 'This refund has been completed manually by the team.',
            severity: 'success' as const,
          };

        default:
          return {
            title: 'Refund requested',
            body: amountLabel ? `A refund of ${amountLabel} has been requested and is waiting for review.` : 'Your refund request has been received and is waiting for review.',
            severity: 'info' as const,
          };
      }
    }

    if (entityLabel === 'subscription' && isCancelAtPeriodEnd) {
      return {
        title: 'Subscription stays active for now',
        body: 'This subscription will end at the close of the current billing period. Because the current period stays active, a refund usually does not apply.',
        severity: 'info' as const,
      };
    }

    if (isCancelled && !isPaymentRequired) {
      return {
        title: 'No refund expected',
        body: `This ${entityLabel} was cancelled and no payment was required, so there is no refund to process.`,
        severity: 'success' as const,
      };
    }

    if (isCancelled && isPaidOrRecorded) {
      return {
        title: 'Refund review starts after cancellation',
        body: `This ${entityLabel} has been cancelled. If a refund is available, we'll review it separately based on the cancellation policy and payment records.`,
        severity: 'info' as const,
      };
    }

    if (entityLabel === 'subscription') {
      return {
        title: 'Billing changes and refunds are handled separately',
        body: 'Ending a subscription at period end usually keeps the current period active without a refund. Cancelling now may still require a separate refund review.',
        severity: 'info' as const,
      };
    }

    return {
      title: 'Refunds are reviewed after cancellation',
      body: 'If this booking is cancelled within the allowed window, any eligible refund will be reviewed after the cancellation is confirmed.',
      severity: 'info' as const,
    };
  })();

  return (
    <Card sx={{ mt: 3, borderRadius: 3, border: 1, borderColor: (theme) => theme.palette.divider, boxShadow: 'none' }}>
      <CardContent sx={{ p: 2.5 }}>
        <CaptionIconTypography label="Refund status" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
        <SubtitleIconTypography label={content.title} sx={{ mt: 1 }} />
        <BodyIconTypography label={content.body} sx={{ mt: 0.75, opacity: 0.82 }} />
        {refund?.requestedByCustomerName ? <BodyIconTypography label={`Requested by ${refund.requestedByCustomerName}`} sx={{ mt: 0.75, opacity: 0.68 }} /> : null}
        {refund?.reason ? <BodyIconTypography label={`Note: ${refund.reason}`} sx={{ mt: 0.75, opacity: 0.68 }} /> : null}
        <Alert severity={content.severity} sx={{ mt: 2, borderRadius: 2 }}>
          Refund and accounting updates can take a little longer to appear than the cancellation status shown on this page.
        </Alert>
        {refund ? <MarketplaceRefundTimeline refund={refund} /> : null}
      </CardContent>
    </Card>
  );
};

export default MarketplaceRefundStatusCard;
