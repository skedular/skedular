import { BodyIconTypography, CaptionIconTypography, SubtitleIconTypography } from '@/components/commons';
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
            title: 'Refund is being processed',
            body: amountLabel
              ? `A refund of ${amountLabel} is pending accounting processing and provider confirmation.`
              : 'This refund is pending accounting processing and provider confirmation.',
            severity: 'info' as const,
          };

        case 'FAILED':
          return {
            title: 'Refund needs attention',
            body: refund.lastError || 'Refund processing failed and needs follow-up from the team before it can complete.',
            severity: 'warning' as const,
          };

        case 'MANUAL_REQUIRED':
          return {
            title: 'Refund requires manual follow-up',
            body: refund.lastError || 'This refund needs manual follow-up from the team before it can complete.',
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
            body: amountLabel ? `A refund of ${amountLabel} has been requested and is waiting for review.` : 'A refund has been requested and is waiting for review.',
            severity: 'info' as const,
          };
      }
    }

    if (entityLabel === 'subscription' && isCancelAtPeriodEnd) {
      return {
        title: 'Current period stays active',
        body: 'This subscription is set to stop at the end of the current period. That change usually does not create a refund because the current period remains active.',
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
        title: 'Refund review follows cancellation',
        body: `This ${entityLabel} has been cancelled. Any refund is reviewed separately against the cancellation policy and the related invoice/accounting records before it is completed.`,
        severity: 'info' as const,
      };
    }

    if (entityLabel === 'subscription') {
      return {
        title: 'Refunds are separate from renewal changes',
        body: 'Ending at period end usually keeps the current period active without a refund. Immediate cancellation may still require refund review for already billed value.',
        severity: 'info' as const,
      };
    }

    return {
      title: 'Eligible refunds are reviewed separately',
      body: 'If this booking is cancelled inside the allowed window, any eligible refund is processed after the cancellation is accepted rather than as part of the same update.',
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
          Refund processing and accounting updates may complete after the cancellation state appears on this page.
        </Alert>
        {refund ? <MarketplaceRefundTimeline refund={refund} /> : null}
      </CardContent>
    </Card>
  );
};

export default MarketplaceRefundStatusCard;
