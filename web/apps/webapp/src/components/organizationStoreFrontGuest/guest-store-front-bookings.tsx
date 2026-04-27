import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { ArrowLeftIcon, LocationIcon, PaymentStatusIcon, QuantityIcon, ResourceIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@skedular/shared';
import { convertCalendarDayToStartOfDay, toStoredBookingTimeRange } from '@skedular/shared';
import type { guestStoreFrontBookings_rootQuery } from '@/queries/__generated__/guestStoreFrontBookings_rootQuery.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<guestStoreFrontBookings_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query guestStoreFrontBookings_rootQuery($organizationCustomDomain: String!, $today: DateTime!) {
    organizationPublic(customDomain: $organizationCustomDomain) {
      name
      marketplaceListingMetadata {
        title
        subTitle
      }
    }
    upcomingBookings: bookings(
      first: 24
      where: { organizationCustomDomain: $organizationCustomDomain, includeMineOnly: true, channel: MARKETPLACE, fromGte: $today }
      orderBy: [{ field: FROM, direction: ASCENDING }]
    ) {
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
    recentBookings: bookings(
      first: 24
      where: { organizationCustomDomain: $organizationCustomDomain, includeMineOnly: true, channel: MARKETPLACE, fromLt: $today }
      orderBy: [{ field: FROM, direction: DESCENDING }]
    ) {
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
`;

const GuestStoreFrontBookings = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<guestStoreFrontBookings_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const upcomingBookings = useMemo(
    () => rootData.upcomingBookings.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item),
    [rootData.upcomingBookings.edges],
  );
  const recentBookings = useMemo(
    () => rootData.recentBookings.edges.map((edge) => edge.node).filter((item): item is NonNullable<typeof item> => !!item),
    [rootData.recentBookings.edges],
  );

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 24%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 20%)',
      }}
    >
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        <Card
          sx={{
            borderRadius: 4,
            overflow: 'hidden',
            border: 1,
            borderColor: (theme) => alpha(theme.palette.primary.main, 0.18),
            background: (theme) =>
              `linear-gradient(135deg, ${alpha(theme.palette.primary.light, 0.12)} 0%, ${alpha(theme.palette.background.paper, 1)} 42%, ${alpha(theme.palette.warning.light, 0.1)} 100%)`,
          }}
        >
          <CardContent sx={{ p: { xs: 2.5, md: 3.5 } }}>
            <CaptionIconTypography label="Marketplace bookings" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
            <LeadIconTypography label={`Your bookings at ${rootData.organizationPublic?.name ?? 'this store'}`} sx={{ mt: 0.75 }} />
            <BodyIconTypography
              label="Track upcoming bookings, reopen payment details when needed, and review past visits without switching back to the admin-style booking screens."
              sx={{ mt: 0.9, opacity: 0.82, maxWidth: 760 }}
            />

            <StackRow sx={{ mt: 2 }}>
              <Chip label={`${rootData.upcomingBookings.totalCount} upcoming`} color="primary" variant="outlined" />
              <Chip label={`${rootData.recentBookings.totalCount} recent`} variant="outlined" />
              {rootData.organizationPublic?.marketplaceListingMetadata.title ? (
                <Chip label={rootData.organizationPublic.marketplaceListingMetadata.title} variant="filled" />
              ) : null}
            </StackRow>
          </CardContent>
        </Card>

        <BookingsSection
          bookings={upcomingBookings}
          integratedPlatrform={integratedPlatrform}
          isCustomDomain={isCustomDomain}
          label="Coming up"
          organizationCustomDomain={organizationCustomDomain}
          title="Upcoming bookings"
        />

        <BookingsSection
          bookings={recentBookings}
          integratedPlatrform={integratedPlatrform}
          isCustomDomain={isCustomDomain}
          label="Already happened"
          organizationCustomDomain={organizationCustomDomain}
          title="Recent bookings"
        />
      </Container>
    </Box>
  );
};

const BookingsSection = ({
  bookings,
  integratedPlatrform,
  isCustomDomain,
  label,
  organizationCustomDomain,
  title,
}: {
  bookings: ReadonlyArray<NonNullable<guestStoreFrontBookings_rootQuery['response']['upcomingBookings']['edges'][number]['node']>>;
  integratedPlatrform: string | undefined;
  isCustomDomain: boolean;
  label: string;
  organizationCustomDomain: string;
  title: string;
}) => (
  <Box sx={{ mt: 4 }}>
    <CaptionIconTypography label={label} sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
    <LeadIconTypography label={title} sx={{ mt: 0.5 }} />

    {bookings.length > 0 ? (
      <Box
        sx={{
          mt: 2,
          display: 'grid',
          gap: 1.5,
          gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(3, minmax(0, 1fr))' },
        }}
      >
        {bookings.map((booking) => {
          const bookingLink = getMarketplaceBookingDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, booking.id);
          const locationLabel = booking.involvedLocations[0]?.name ?? 'Location to be confirmed';
          const resourcesLabel = booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
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
                  boxShadow: (theme) => theme.shadows[4],
                  borderColor: (theme) => theme.palette.primary.main,
                },
              }}
            >
              <Box sx={{ p: 2.25 }}>
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
    ) : (
      <Card sx={{ mt: 2, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
        <CardContent sx={{ p: 2.5 }}>
          <BodyIconTypography label={title === 'Upcoming bookings' ? 'Nothing is scheduled yet.' : 'No recent bookings to show.'} sx={{ opacity: 0.8 }} />
        </CardContent>
      </Card>
    )}
  </Box>
);

const toStoredBookingDate = (date?: string | null) => (date ? dayjs.utc(date).format('dddd, Do MMM YYYY') : '');

const MemoGuestStoreFrontBookings = memo(GuestStoreFrontBookings);

const GuestStoreFrontBookingsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<guestStoreFrontBookings_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    const today = convertCalendarDayToStartOfDay(dayjs());

    loadQuery(
      {
        organizationCustomDomain,
        today: today.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoGuestStoreFrontBookings queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(GuestStoreFrontBookingsWithRelay);
