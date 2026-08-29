import { BodyIconTypography, CaptionIconTypography, StackColumn } from '@skedular/ui';
import Box from '@mui/material/Box';

export type MarketplacePurchaseHistoryEvent = {
  readonly id: string;
  readonly type: string;
  readonly name: string;
  readonly occurredAt: string;
  readonly cancellationRequestedAt: string | null | undefined;
  readonly cancellationEffectiveAt: string | null | undefined;
  readonly paymentStatus: string | null | undefined;
  readonly refundStatus: string | null | undefined;
  readonly creditQuantity: number | null | undefined;
  readonly remainingCreditQuantity: number | null | undefined;
  readonly reason: string | null | undefined;
};

export const MarketplacePurchaseHistoryEventList = ({ events }: { events: ReadonlyArray<MarketplacePurchaseHistoryEvent> }) => (
  <StackColumn spacing={1.5}>
    <CaptionIconTypography label="Purchase history" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
    {events.length === 0 ? (
      <BodyIconTypography label="No purchase history is available yet." />
    ) : (
      events.map((event) => (
        <Box key={event.id} sx={{ p: 1.5, borderRadius: 2, border: 1, borderColor: 'divider' }}>
          <BodyIconTypography label={event.name} sx={{ fontWeight: 700 }} />
          <BodyIconTypography label={new Date(event.occurredAt).toLocaleString()} />
          {event.paymentStatus ? <BodyIconTypography label={`Payment: ${event.paymentStatus}`} /> : null}
          {event.refundStatus ? <BodyIconTypography label={`Refund: ${event.refundStatus}`} /> : null}
          {event.creditQuantity !== null && event.creditQuantity !== undefined ? <BodyIconTypography label={`Credits: ${event.creditQuantity}`} /> : null}
          {event.remainingCreditQuantity !== null && event.remainingCreditQuantity !== undefined ? (
            <BodyIconTypography label={`Remaining: ${event.remainingCreditQuantity}`} />
          ) : null}
          {event.cancellationRequestedAt ? (
            <BodyIconTypography
              label={`Cancellation requested: ${new Date(event.cancellationRequestedAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' })}`}
            />
          ) : null}
          {event.cancellationEffectiveAt ? (
            <BodyIconTypography
              label={`Cancellation effective: ${new Date(event.cancellationEffectiveAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' })}`}
            />
          ) : null}
          {event.reason ? <BodyIconTypography label={event.reason} /> : null}
        </Box>
      ))
    )}
  </StackColumn>
);
