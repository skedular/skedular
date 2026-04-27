import { BodyIconTypography, CaptionIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import Box from '@mui/material/Box';
import dayjs from 'dayjs';
import { formatRefundAmount } from './refund-display';

type RefundTimelineEvent = {
  title: string;
  timestamp?: string | null | undefined;
  description: string;
};

type Props = {
  refund: {
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
    events?:
      | ReadonlyArray<{
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
        }>
      | null
      | undefined;
  };
};

const formatTimestamp = (value?: string | null | undefined) => {
  if (!value) {
    return null;
  }

  const parsed = dayjs(value);
  return parsed.isValid() ? parsed.format('D MMM YYYY, h:mm A') : null;
};

const toRefundStatusType = (value?: string | null | undefined) => value?.replace(/([a-z])([A-Z])/g, '$1_$2').toUpperCase() ?? '';

const MarketplaceRefundTimeline = ({ refund }: Props) => {
  const persistedEvents: RefundTimelineEvent[] =
    refund.events?.map((event) => {
      const eventAmountLabel = formatRefundAmount(event.refundAmount, refund.currency?.type, event.currencyToDisplay);
      const description = (() => {
        switch (event.eventType.type) {
          case 'REQUESTED':
            return eventAmountLabel
              ? `The refund entered review for ${eventAmountLabel}${refund.refundPercentage != null ? ` at ${refund.refundPercentage}% of the policy amount` : ''}.`
              : 'The refund entered review and is waiting for policy/accounting follow-up.';
          case 'PENDING_ACCOUNTING':
            return eventAmountLabel
              ? `The refund was approved locally for ${eventAmountLabel} and is waiting for accounting/provider completion.`
              : 'The refund was approved locally and is waiting for accounting/provider completion.';
          case 'SENT_TO_XERO':
            return 'The refund was sent to Xero for provider-side processing.';
          case 'MANUAL_REQUIRED':
            return event.lastError || event.reason || 'The refund has been moved to manual follow-up.';
          case 'MANUAL_COMPLETED':
            return eventAmountLabel ? `The refund was completed manually for ${eventAmountLabel}.` : 'The refund was completed manually.';
          case 'COMPLETED':
            return eventAmountLabel
              ? `Refund completed for ${eventAmountLabel}${event.externalRefundNumber ? ` under reference ${event.externalRefundNumber}` : ''}.`
              : 'Refund completed.';
          case 'FAILED':
            return event.lastError || event.reason || 'Refund processing failed and requires follow-up.';
          default:
            return event.reason || 'Refund timeline event recorded.';
        }
      })();

      return {
        title: event.actorName ? `${event.eventType.name} by ${event.actorName}` : event.eventType.name,
        timestamp: formatTimestamp(event.occurredAt),
        description,
      };
    }) ?? [];

  const amountLabel = formatRefundAmount(refund.refundAmount, refund.currency?.type, refund.currencyToDisplay);
  const timelineEvents =
    persistedEvents.length > 0
      ? persistedEvents
      : [
          {
            title: 'Refund requested',
            timestamp: formatTimestamp(refund.requestedAt),
            description: amountLabel
              ? `The refund entered review for ${amountLabel}${refund.refundPercentage != null ? ` at ${refund.refundPercentage}% of the policy amount` : ''}.`
              : 'The refund entered review and is waiting for policy/accounting follow-up.',
          },
          {
            title: refund.status.name,
            timestamp: formatTimestamp(refund.lastProcessedAt),
            description:
              toRefundStatusType(refund.status.type) === 'COMPLETED'
                ? amountLabel
                  ? `Refund completed for ${amountLabel}${refund.externalRefundNumber ? ` under reference ${refund.externalRefundNumber}` : ''}.`
                  : 'Refund completed.'
                : toRefundStatusType(refund.status.type) === 'MANUAL_COMPLETED'
                  ? amountLabel
                    ? `Refund completed manually for ${amountLabel}.`
                    : 'Refund completed manually.'
                  : toRefundStatusType(refund.status.type) === 'FAILED'
                    ? refund.lastError || 'Refund processing failed and requires follow-up.'
                    : toRefundStatusType(refund.status.type) === 'MANUAL_REQUIRED'
                      ? refund.lastError || 'Refund requires manual follow-up.'
                      : toRefundStatusType(refund.status.type) === 'PENDING_ACCOUNTING'
                        ? 'Refund has been approved locally and is waiting for accounting/provider completion.'
                        : 'Refund is waiting for admin review.',
          },
        ];

  return (
    <Box sx={{ mt: 2.5 }}>
      <CaptionIconTypography label="Refund timeline" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
      <StackColumn spacing={1.5} sx={{ mt: 1.25 }}>
        {timelineEvents.map((event, index) => (
          <StackRow key={`${event.title}-${index}`} sx={{ alignItems: 'flex-start', gap: 1.25 }}>
            <Box
              sx={{
                mt: 0.4,
                minWidth: 10,
                width: 10,
                height: 10,
                borderRadius: '50%',
                bgcolor: index === timelineEvents.length - 1 ? 'primary.main' : 'divider',
                flexShrink: 0,
              }}
            />
            <StackColumn spacing={0.25}>
              <StackRow sx={{ alignItems: 'baseline', gap: 1, flexWrap: 'wrap' }}>
                <SubtitleIconTypography label={event.title} />
                {event.timestamp ? <SmallIconTypography label={event.timestamp} sx={{ opacity: 0.68 }} /> : null}
              </StackRow>
              <BodyIconTypography label={event.description} sx={{ opacity: 0.82 }} />
            </StackColumn>
          </StackRow>
        ))}
        {refund.reason ? <SmallIconTypography label={`Admin note: ${refund.reason}`} sx={{ opacity: 0.72 }} /> : null}
      </StackColumn>
    </Box>
  );
};

export default MarketplaceRefundTimeline;
