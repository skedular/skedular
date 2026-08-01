'use client';

import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';
import { BodyIconTypography, CaptionIconTypography } from '@skedular/ui';

export type RefundHistoryEvent = {
  id: string;
  eventType: string;
  occurredAt: string;
  actorName?: string | null;
  previousStatus?: string | null;
  newStatus?: string | null;
};

export function RefundHistoryTimeline({ events }: { events: readonly RefundHistoryEvent[] }) {
  return (
    <Stack divider={<Divider flexItem />} spacing={1.5}>
      {events.map((event) => (
        <Stack key={event.id} spacing={0.5}>
          <BodyIconTypography label={event.eventType} />
          <CaptionIconTypography label={`${new Date(event.occurredAt).toLocaleString()}${event.actorName ? ` · ${event.actorName}` : ''}`} />
          {(event.previousStatus || event.newStatus) && <CaptionIconTypography label={`${event.previousStatus ?? 'Initial'} → ${event.newStatus ?? 'Unknown'}`} />}
        </Stack>
      ))}
    </Stack>
  );
}
