import type { ResourceDayViewCard_resourceDayView$key } from '@/queries/__generated__/ResourceDayViewCard_resourceDayView.graphql';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Divider from '@mui/material/Divider';
import { CaptionIconTypography, SectionIconTypography, SmallIconTypography, StackColumn, StackRow } from '@skedular/ui';
import Link from 'next/link';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import AvailabilityStatusBadge from './AvailabilityStatusBadge';
import BookingWindowList from './BookingWindowList';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  resourceDayViewRef: ResourceDayViewCard_resourceDayView$key;
};

const ResourceDayViewCard = ({ resourceDayViewRef }: Props) => {
  const { organizationCustomDomain } = useKnownParams();
  const data = useFragment<ResourceDayViewCard_resourceDayView$key>(
    graphql`
      fragment ResourceDayViewCard_resourceDayView on ResourceDayViewDetails {
        resourceId
        resourceName
        resourceType
        locationId
        locationName
        floorId
        floorName
        zoneId
        zoneName
        date
        status
        openingFrom
        openingUntil
        totalOpeningMinutes
        bookedMinutes
        bookingWindows {
          bookingId
          from
          until
          isRecurring
          isCheckedIn
          bookedByName
          notes
        }
      }
    `,
    resourceDayViewRef,
  );

  const floorPlanUrl = data.locationId
    ? `/organizations/${organizationCustomDomain}/locations/${data.locationId}/floorPlans${data.floorId ? `/${data.floorId}` : ''}?date=${data.date}`
    : undefined;

  const locationLine = [data.locationName, data.floorName, data.zoneName].filter(Boolean).join(' › ');

  return (
    <Card
      aria-label={`Resource: ${data.resourceName}`}
      sx={{
        height: '100%',
        borderRadius: 4,
        border: 1,
        borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
        boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
        backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
        display: 'flex',
        flexDirection: 'column',
      }}
    >
      <Box
        sx={{
          px: 2.5,
          py: 1.5,
          borderBottom: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : theme.palette.action.hover),
        }}
      >
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
          <SectionIconTypography label={data.resourceName} />
          <AvailabilityStatusBadge status={data.status} />
        </StackRow>
      </Box>

      <CardContent sx={{ p: 2.5, flexGrow: 1, display: 'flex', flexDirection: 'column', gap: 1 }}>
        <StackColumn spacing={0.5}>
          <SmallIconTypography label={locationLine} />
          <CaptionIconTypography label={data.resourceType} />
        </StackColumn>

        <Divider sx={{ my: 0.5 }} />

        <CaptionIconTypography label={`${data.bookedMinutes} / ${data.totalOpeningMinutes} mins booked`} />

        {floorPlanUrl && (
          <Box>
            <Link href={floorPlanUrl} aria-label={`View floor plan for ${data.locationName}`} style={{ textDecoration: 'none' }}>
              <CaptionIconTypography label="View floor plan →" />
            </Link>
          </Box>
        )}

        {data.bookingWindows.length > 0 && (
          <>
            <Divider sx={{ my: 0.5 }} />
            <BookingWindowList bookingWindows={data.bookingWindows} />
          </>
        )}
      </CardContent>
    </Card>
  );
};

export default memo(ResourceDayViewCard);
