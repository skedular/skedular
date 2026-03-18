import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { LocationIcon, PaymentStatusIcon, QuantityIcon, ResourceIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink, getMarketplaceBookingsLink } from '@/components/links';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import type { guestStoreFrontUpcomingBookingsStrip_query$key } from '@/queries/__generated__/guestStoreFrontUpcomingBookingsStrip_query.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontUpcomingBookingsStrip_query$key;
};

const GuestStoreFrontUpcomingBookingsStrip = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment guestStoreFrontUpcomingBookingsStrip_query on Query
      @argumentDefinitions(
        bookingsSearchCriteriaFrom: { type: "DateTime!" }
        bookingsSearchCriteriaTo: { type: "DateTime!" }
        includeUpcomingBookings: { type: "Boolean!", defaultValue: false }
        organizationCustomDomain: { type: "String!" }
      ) {
        bookings(
          first: 6
          where: {
            organizationCustomDomains: [$organizationCustomDomain]
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
            includeMineOnly: true
            channel: MARKETPLACE
          }
          orderBy: [{ field: FROM, direction: ASCENDING }]
        ) @include(if: $includeUpcomingBookings) {
          totalCount
          edges {
            node {
              id
              from
              until
              involvedLocations {
                name
              }
              bookingResources {
                resource {
                  id
                  name
                }
              }
              marketplaceBooking {
                quantity
                paymentStatus {
                  type
                  name
                }
              }
            }
          }
        }
      }
    `,
    rootDataRelay,
  );
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const upcomingBookings = useMemo(
    () => rootData.bookings?.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item) ?? [],
    [rootData.bookings?.edges],
  );

  if (!rootData.bookings) {
    return null;
  }

  return (
    <Card
      sx={{
        borderRadius: 4,
        overflow: 'hidden',
        border: 1,
        borderColor: (theme) => alpha(theme.palette.primary.main, 0.18),
        background: (theme) =>
          `linear-gradient(135deg, ${alpha(theme.palette.primary.light, 0.12)} 0%, ${alpha(theme.palette.background.paper, 1)} 46%, ${alpha(theme.palette.warning.light, 0.1)} 100%)`,
      }}
    >
      <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} justifyContent="space-between" alignItems={{ xs: 'flex-start', md: 'center' }}>
          <Box sx={{ maxWidth: 720 }}>
            <CaptionIconTypography label="Your week here" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <LeadIconTypography label="Upcoming bookings" sx={{ mt: 0.75 }} />
            <BodyIconTypography
              label={
                upcomingBookings.length > 0
                  ? 'Keep track of bookings coming up this week. Open any booking to review payment progress, invoice access, and the latest assignment details.'
                  : 'No upcoming bookings are scheduled for this week. As soon as you book a space here, it will show up in this strip.'
              }
              sx={{ mt: 0.75, opacity: 0.82 }}
            />
          </Box>

          <Button
            component={NextLink}
            href={getMarketplaceBookingsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain)}
            variant="text"
            sx={{ textTransform: 'none', whiteSpace: 'nowrap' }}
          >
            View all bookings
          </Button>
        </Stack>

        {upcomingBookings.length > 0 ? (
          <Box
            sx={{
              mt: 2.5,
              display: 'grid',
              gap: 1.5,
              gridTemplateColumns: { xs: '1fr', md: 'repeat(3, minmax(0, 1fr))' },
            }}
          >
            {upcomingBookings.map((booking) => {
              const bookingLink = getMarketplaceBookingDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, booking.id);
              const resourcesLabel = booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
              const locationLabel = booking.involvedLocations[0]?.name ?? 'Location to be confirmed';
              const isConfirmed = booking.marketplaceBooking?.paymentStatus.type === 'CONFIRMED';

              return (
                <Link
                  key={booking.id}
                  component={NextLink}
                  href={bookingLink}
                  underline="none"
                  color="inherit"
                  sx={{
                    display: 'block',
                    borderRadius: 3,
                    border: 1,
                    borderColor: (theme) => alpha(theme.palette.divider, 0.9),
                    bgcolor: (theme) => alpha(theme.palette.background.paper, 0.82),
                    backdropFilter: 'blur(10px)',
                    transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                    '&:hover': {
                      transform: 'translateY(-2px)',
                      boxShadow: (theme) => theme.shadows[4],
                      borderColor: (theme) => theme.palette.primary.main,
                    },
                  }}
                >
                  <Box sx={{ p: 2 }}>
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                      <Box>
                        <SmallIconTypography label={toStoredBookingDate(booking.from)} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                        <SubtitleIconTypography label={toStoredBookingTimeRange(booking.from, booking.until)} sx={{ mt: 0.4 }} />
                      </Box>
                      <Chip
                        size="small"
                        icon={<PaymentStatusIcon />}
                        label={booking.marketplaceBooking?.paymentStatus.name ?? 'Pending'}
                        color={isConfirmed ? 'success' : 'default'}
                        variant={isConfirmed ? 'filled' : 'outlined'}
                      />
                    </StackRow>

                    <StackColumn spacing={1.1} sx={{ mt: 2 }}>
                      <StackRow sx={{ flexWrap: 'nowrap' }}>
                        <LocationIcon fontSize="small" />
                        <BodyIconTypography label={locationLabel} sx={{ opacity: 0.88 }} />
                      </StackRow>
                      <StackRow sx={{ flexWrap: 'nowrap' }}>
                        <QuantityIcon fontSize="small" />
                        <BodyIconTypography label={`Quantity ${booking.marketplaceBooking?.quantity ?? 1}`} sx={{ opacity: 0.88 }} />
                      </StackRow>
                      <StackRow sx={{ flexWrap: 'nowrap' }}>
                        <ResourceIcon fontSize="small" />
                        <BodyIconTypography label={resourcesLabel} sx={{ opacity: 0.88 }} />
                      </StackRow>
                    </StackColumn>

                    <StackRow sx={{ mt: 2, justifyContent: 'space-between', flexWrap: 'nowrap' }}>
                      <BodyIconTypography label="Open booking" sx={{ color: 'primary.main', fontWeight: 600 }} />
                      <ChevronRightIcon fontSize="small" />
                    </StackRow>
                  </Box>
                </Link>
              );
            })}
          </Box>
        ) : null}
      </CardContent>
    </Card>
  );
};

const toStoredBookingDate = (date?: string | null) => (date ? dayjs.utc(date).format('ddd, Do MMM') : '');
const toStoredBookingTime = (date?: string | null) => (date ? dayjs.utc(date).format('hh:mm a') : '');
const toStoredBookingTimeRange = (from?: string | null, until?: string | null) => `${toStoredBookingTime(from)} - ${toStoredBookingTime(until)}`;

export default memo(GuestStoreFrontUpcomingBookingsStrip);
