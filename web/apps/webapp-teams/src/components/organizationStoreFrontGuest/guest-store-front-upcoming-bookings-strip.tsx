import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { LocationIcon, PaymentStatusIcon, QuantityIcon, ResourceIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink, getMarketplaceBookingsLink } from '@/components/links';
import { useIntegratedPlatrform, useKnownParams } from '@skedular/shared';
import { toStoredBookingTimeRange } from '@skedular/shared';
import type { guestStoreFrontUpcomingBookingsStrip_query$key } from '@/queries/__generated__/guestStoreFrontUpcomingBookingsStrip_query.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Link from '@mui/material/Link';
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
            organizationCustomDomain: $organizationCustomDomain
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
        height: '100%',
        borderRadius: 4,
        overflow: 'hidden',
        border: 1,
        borderColor: (theme) => alpha(theme.palette.primary.main, 0.18),
        background: (theme) =>
          `linear-gradient(135deg, ${alpha(theme.palette.primary.light, 0.1)} 0%, ${alpha(theme.palette.background.paper, 1)} 58%, ${alpha(theme.palette.warning.light, 0.08)} 100%)`,
      }}
    >
      <CardContent sx={{ p: { xs: 2, md: 2.5 } }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', gap: 1.5 }}>
          <Box sx={{ minWidth: 0 }}>
            <CaptionIconTypography label="Your week here" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <StackRow sx={{ mt: 0.5, alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
              <LeadIconTypography label="Bookings" />
              <Chip
                size="small"
                color={upcomingBookings.length > 0 ? 'primary' : 'default'}
                variant={upcomingBookings.length > 0 ? 'filled' : 'outlined'}
                label={upcomingBookings.length > 0 ? `${upcomingBookings.length} this week` : 'Nothing booked'}
              />
            </StackRow>
            <BodyIconTypography
              label={upcomingBookings.length > 0 ? 'Open a booking to check time, payment, and assignment details.' : 'When you book here, it will appear in this summary.'}
              sx={{ mt: 0.75, opacity: 0.78 }}
            />
          </Box>

          <Button
            component={NextLink}
            href={getMarketplaceBookingsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain)}
            variant="text"
            endIcon={<ChevronRightIcon fontSize="small" />}
            sx={{ textTransform: 'none', whiteSpace: 'nowrap', px: 0, minWidth: 'auto', alignSelf: 'flex-start' }}
          >
            All bookings
          </Button>
        </StackRow>

        {upcomingBookings.length > 0 ? (
          <StackColumn
            sx={{
              mt: 1.75,
              gap: 1,
            }}
          >
            {upcomingBookings.map((booking) => {
              const bookingLink = getMarketplaceBookingDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, booking.id);
              const resourcesLabel = booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
              const locationLabel = booking.involvedLocations[0]?.name ?? 'Location to be confirmed';
              const paymentStatusType = booking.marketplaceBooking?.paymentStatus.type;
              const isConfirmed = paymentStatusType === 'CONFIRMED';
              const isPending = paymentStatusType === 'PENDING';

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
                    bgcolor: (theme) => alpha(theme.palette.background.paper, 0.86),
                    backdropFilter: 'blur(10px)',
                    transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                    '&:hover': {
                      transform: 'translateY(-2px)',
                      boxShadow: (theme) => theme.shadows[3],
                      borderColor: (theme) => theme.palette.primary.main,
                    },
                  }}
                >
                  <Box sx={{ p: 1.5 }}>
                    <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                      <Box>
                        <SmallIconTypography label={toStoredBookingDate(booking.from)} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
                        {toStoredBookingTimeRange(booking.from, booking.until) ? (
                          <SubtitleIconTypography label={toStoredBookingTimeRange(booking.from, booking.until)} sx={{ mt: 0.4 }} />
                        ) : null}
                      </Box>
                      <Chip
                        size="small"
                        icon={<PaymentStatusIcon />}
                        label={booking.marketplaceBooking?.paymentStatus.name ?? 'Pending'}
                        color={isConfirmed ? 'success' : isPending ? 'warning' : 'default'}
                        variant={isConfirmed || isPending ? 'filled' : 'outlined'}
                      />
                    </StackRow>

                    <StackColumn spacing={0.8} sx={{ mt: 1.25 }}>
                      <StackRow sx={{ flexWrap: 'nowrap', alignItems: 'center' }}>
                        <LocationIcon fontSize="small" />
                        <BodyIconTypography label={locationLabel} sx={{ opacity: 0.88 }} />
                      </StackRow>
                      <StackRow sx={{ flexWrap: 'nowrap', alignItems: 'center' }}>
                        <QuantityIcon fontSize="small" />
                        <BodyIconTypography label={`Quantity ${booking.marketplaceBooking?.quantity ?? 1}`} sx={{ opacity: 0.88 }} />
                      </StackRow>
                      <StackRow sx={{ flexWrap: 'nowrap', alignItems: 'center' }}>
                        <ResourceIcon fontSize="small" />
                        <BodyIconTypography label={resourcesLabel} sx={{ opacity: 0.88 }} />
                      </StackRow>
                    </StackColumn>
                  </Box>
                </Link>
              );
            })}
          </StackColumn>
        ) : (
          <Box
            sx={{
              mt: 1.75,
              borderRadius: 3,
              border: 1,
              borderStyle: 'dashed',
              borderColor: (theme) => alpha(theme.palette.primary.main, 0.24),
              bgcolor: (theme) => alpha(theme.palette.background.paper, 0.64),
              px: 1.5,
              py: 1.25,
            }}
          >
            <SmallIconTypography label="No bookings for this week yet." sx={{ opacity: 0.72 }} />
          </Box>
        )}
      </CardContent>
    </Card>
  );
};

const toStoredBookingDate = (date?: string | null) => (date ? dayjs.utc(date).format('ddd, Do MMM') : '');

export default memo(GuestStoreFrontUpcomingBookingsStrip);
