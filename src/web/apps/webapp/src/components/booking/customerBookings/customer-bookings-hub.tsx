import { LocationIcon, ResourceIcon, TeamIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink, getTeamsOrganizationBookingBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import type { customerBookingsHub_pastBookingsPaginationQuery } from '@/queries/__generated__/customerBookingsHub_pastBookingsPaginationQuery.graphql';
import type { customerBookingsHub_pastBookings_query$key } from '@/queries/__generated__/customerBookingsHub_pastBookings_query.graphql';
import type { customerBookingsHub_rootQuery } from '@/queries/__generated__/customerBookingsHub_rootQuery.graphql';
import type { customerBookingsHub_upcomingBookingsPaginationQuery } from '@/queries/__generated__/customerBookingsHub_upcomingBookingsPaginationQuery.graphql';
import type {
  customerBookingsHub_upcomingBookings_query$data,
  customerBookingsHub_upcomingBookings_query$key,
} from '@/queries/__generated__/customerBookingsHub_upcomingBookings_query.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import Link from '@mui/material/Link';
import Tab from '@mui/material/Tab';
import Tabs from '@mui/material/Tabs';
import { alpha } from '@mui/material/styles';
import Box from '@mui/system/Box';
import { RelayError, toRootError, toStoredBookingTimeRange, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import NextLink from 'next/link';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import useKnownParams from '@/hooks/use-known-params';

dayjs.extend(utc);

type Props = {
  queryReference: PreloadedQuery<customerBookingsHub_rootQuery, Record<string, unknown>>;
};
type BookingNode = NonNullable<customerBookingsHub_upcomingBookings_query$data['upcomingBookings']['edges'][number]['node']>;
type BookingTab = 'upcoming' | 'past';
type BookingDay = {
  key: string;
  date: string;
  bookings: ReadonlyArray<BookingNode>;
};

const RootQuery = graphql`
  query customerBookingsHub_rootQuery($today: DateTime!, $organizationCustomDomain: String!) {
    ...customerBookingsHub_upcomingBookings_query @arguments(today: $today, organizationCustomDomain: $organizationCustomDomain)
    ...customerBookingsHub_pastBookings_query @arguments(today: $today, organizationCustomDomain: $organizationCustomDomain)
  }
`;

const UpcomingBookingsFragment = graphql`
  fragment customerBookingsHub_upcomingBookings_query on Query
  @argumentDefinitions(today: { type: "DateTime!" }, organizationCustomDomain: { type: "String!" }, count: { type: "Int", defaultValue: 25 }, cursor: { type: "String" })
  @refetchable(queryName: "customerBookingsHub_upcomingBookingsPaginationQuery") {
    upcomingBookings: bookings(
      first: $count
      after: $cursor
      where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromGte: $today }
      orderBy: [{ field: FROM, direction: ASCENDING }]
    ) @connection(key: "customerBookingsHub_upcomingBookings") {
      totalCount
      pageInfo {
        hasNextPage
        endCursor
      }
      edges {
        node {
          id
          from
          until
          involvedOrganizations {
            name
            customDomain
          }
          involvedLocations {
            name
          }
          involvedTeams {
            name
          }
          bookingResources {
            resource {
              name
            }
          }
          marketplaceBooking {
            paymentStatus {
              type
              name
            }
          }
          recurringBooking {
            frequency {
              name
            }
          }
        }
      }
    }
  }
`;

const PastBookingsFragment = graphql`
  fragment customerBookingsHub_pastBookings_query on Query
  @argumentDefinitions(today: { type: "DateTime!" }, organizationCustomDomain: { type: "String!" }, count: { type: "Int", defaultValue: 25 }, cursor: { type: "String" })
  @refetchable(queryName: "customerBookingsHub_pastBookingsPaginationQuery") {
    recentBookings: bookings(
      first: $count
      after: $cursor
      where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, fromLt: $today }
      orderBy: [{ field: FROM, direction: DESCENDING }]
    ) @connection(key: "customerBookingsHub_pastBookings__recentBookings") {
      totalCount
      pageInfo {
        hasNextPage
        endCursor
      }
      edges {
        node {
          id
          from
          until
          involvedOrganizations {
            name
            customDomain
          }
          involvedLocations {
            name
          }
          involvedTeams {
            name
          }
          bookingResources {
            resource {
              name
            }
          }
          marketplaceBooking {
            paymentStatus {
              type
              name
            }
          }
          recurringBooking {
            frequency {
              name
            }
          }
        }
      }
    }
  }
`;

const CustomerBookingsHub = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<customerBookingsHub_rootQuery>(RootQuery, queryReference);
  const { integratedPlatform } = useIntegratedPlatform();
  const [selectedTab, setSelectedTab] = useState<BookingTab>('upcoming');

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: { xs: 5, md: 8 },
        bgcolor: 'background.default',
      }}
    >
      <Container maxWidth="lg" sx={{ pt: { xs: 3, md: 5 } }}>
        <CaptionIconTypography label="My bookings" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
        <LeadIconTypography label="Where you need to be" sx={{ mt: 0.5 }} />
        <BodyIconTypography label="Your schedule at this coworking space." sx={{ mt: 0.5, opacity: 0.72 }} />
        <Tabs
          value={selectedTab}
          onChange={(_, value: BookingTab) => setSelectedTab(value)}
          aria-label="Booking schedule"
          sx={{ mt: 3, minHeight: 42, borderBottom: 1, borderColor: 'divider' }}
        >
          <Tab value="upcoming" label="Upcoming" sx={tabSx} />
          <Tab value="past" label="Past" sx={tabSx} />
        </Tabs>
        {selectedTab === 'upcoming' ? (
          <UpcomingBookings rootDataRelay={rootData} integratedPlatform={integratedPlatform} />
        ) : (
          <PastBookings rootDataRelay={rootData} integratedPlatform={integratedPlatform} />
        )}
      </Container>
    </Box>
  );
};

const UpcomingBookings = ({ rootDataRelay, integratedPlatform }: { rootDataRelay: customerBookingsHub_upcomingBookings_query$key; integratedPlatform: string | undefined }) => {
  const { data, hasNext, isLoadingNext, loadNext } = usePaginationFragment<customerBookingsHub_upcomingBookingsPaginationQuery, customerBookingsHub_upcomingBookings_query$key>(
    UpcomingBookingsFragment,
    rootDataRelay,
  );
  const bookings = useMemo(() => toNodes(data.upcomingBookings.edges), [data.upcomingBookings.edges]);
  const days = useMemo(() => groupBookingsByDay(bookings), [bookings]);
  return (
    <>
      <NextBooking booking={bookings[0]} integratedPlatform={integratedPlatform} />
      <ScheduleSection
        days={days}
        emptyMessage="Nothing is scheduled yet."
        integratedPlatform={integratedPlatform}
        label="Upcoming"
        loadMoreLabel="Load 25 more upcoming bookings"
        loadedCount={bookings.length}
        onLoadMore={() => loadNext(25)}
        isLoadingMore={isLoadingNext}
        hasMore={hasNext}
        totalCount={data.upcomingBookings.totalCount}
      />
    </>
  );
};

const PastBookings = ({ rootDataRelay, integratedPlatform }: { rootDataRelay: customerBookingsHub_pastBookings_query$key; integratedPlatform: string | undefined }) => {
  const { data, hasNext, isLoadingNext, loadNext } = usePaginationFragment<customerBookingsHub_pastBookingsPaginationQuery, customerBookingsHub_pastBookings_query$key>(
    PastBookingsFragment,
    rootDataRelay,
  );
  const bookings = useMemo(() => toNodes(data.recentBookings.edges), [data.recentBookings.edges]);
  const days = useMemo(() => groupBookingsByDay(bookings), [bookings]);
  return (
    <ScheduleSection
      days={days}
      emptyMessage="No past bookings to show yet."
      integratedPlatform={integratedPlatform}
      label="Past bookings"
      loadMoreLabel="Load 25 earlier bookings"
      loadedCount={bookings.length}
      onLoadMore={() => loadNext(25)}
      isLoadingMore={isLoadingNext}
      hasMore={hasNext}
      totalCount={data.recentBookings.totalCount}
    />
  );
};

const NextBooking = ({ booking, integratedPlatform }: { booking: BookingNode | undefined; integratedPlatform: string | undefined }) => {
  if (!booking) return null;
  const bookingLink = getBookingLink(booking, integratedPlatform);
  return (
    <Box sx={{ mt: 3 }}>
      <CaptionIconTypography label="Next up" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
      <Link component={NextLink} href={bookingLink} underline="none" color="inherit" sx={{ display: 'block', mt: 1 }}>
        <Card
          sx={{
            borderRadius: 3,
            border: 1,
            borderColor: (theme) => alpha(theme.palette.success.main, 0.35),
            bgcolor: (theme) => alpha(theme.palette.success.light, theme.palette.mode === 'light' ? 0.1 : 0.16),
            boxShadow: 'none',
            '&:hover': { borderColor: 'success.main', boxShadow: 2 },
          }}
        >
          <CardContent
            sx={{
              display: 'grid',
              gridTemplateColumns: {
                xs: '60px minmax(0, 1fr)',
                sm: '76px minmax(0, 1fr) auto',
              },
              gap: { xs: 1.5, sm: 2 },
              p: { xs: 2, sm: 2.5 },
            }}
          >
            <DateBlock date={booking.from} />
            <Box sx={{ minWidth: 0 }}>
              <SubtitleIconTypography label={booking.involvedLocations[0]?.name ?? 'Location to be confirmed'} sx={{ overflowWrap: 'anywhere' }} />
              <BodyIconTypography label={toStoredBookingTimeRange(booking.from, booking.until) || 'Time to be confirmed'} sx={{ mt: 0.35, fontWeight: 700 }} />
              <ScheduleMeta booking={booking} resourcesLabel={toResourcesLabel(booking)} />
            </Box>
            <StackRow
              sx={{
                gridColumn: { xs: '1 / -1', sm: 'auto' },
                alignSelf: 'center',
                justifyContent: { xs: 'flex-end', sm: 'initial' },
                color: 'success.main',
              }}
            >
              <BodyIconTypography label="Open booking" sx={{ fontWeight: 700 }} />
              <ChevronRightIcon fontSize="small" />
            </StackRow>
          </CardContent>
        </Card>
      </Link>
    </Box>
  );
};

const ScheduleSection = ({
  days,
  emptyMessage,
  integratedPlatform,
  label,
  hasMore,
  isLoadingMore,
  loadMoreLabel,
  loadedCount,
  onLoadMore,
  totalCount,
}: {
  days: ReadonlyArray<BookingDay>;
  emptyMessage: string;
  integratedPlatform: string | undefined;
  label: string;
  hasMore: boolean;
  isLoadingMore: boolean;
  loadMoreLabel: string;
  loadedCount: number;
  onLoadMore: () => void;
  totalCount: number;
}) => (
  <Box sx={{ mt: 4 }}>
    <CaptionIconTypography label={label} sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
    {days.length > 0 ? (
      <StackColumn spacing={1.25} sx={{ mt: 1.25 }}>
        {days.map((day) => (
          <BookingDayGroup key={day.key} day={day} integratedPlatform={integratedPlatform} />
        ))}
      </StackColumn>
    ) : (
      <Card
        sx={{
          mt: 1.25,
          borderRadius: 3,
          border: 1,
          borderColor: 'divider',
          boxShadow: 'none',
        }}
      >
        <CardContent sx={{ p: 2.5 }}>
          <BodyIconTypography label={emptyMessage} sx={{ opacity: 0.75 }} />
        </CardContent>
      </Card>
    )}
    {hasMore ? (
      <Box
        sx={{
          mt: 2.5,
          mx: 'auto',
          width: '100%',
          maxWidth: 440,
          textAlign: 'center',
        }}
      >
        <Button
          variant="outlined"
          color="primary"
          size="large"
          fullWidth
          onClick={onLoadMore}
          disabled={isLoadingMore}
          sx={{ minHeight: 48, textTransform: 'none', fontWeight: 700 }}
        >
          {isLoadingMore ? 'Loading bookings…' : loadMoreLabel}
        </Button>
        <SmallIconTypography label={`${loadedCount} of ${totalCount} bookings shown`} sx={{ mt: 0.85, opacity: 0.68 }} />
      </Box>
    ) : null}
  </Box>
);

const BookingDayGroup = ({ day, integratedPlatform }: { day: BookingDay; integratedPlatform: string | undefined }) => (
  <Box
    sx={{
      display: 'grid',
      gridTemplateColumns: {
        xs: '54px minmax(0, 1fr)',
        sm: '84px minmax(0, 1fr)',
      },
      gap: { xs: 1, sm: 2 },
    }}
  >
    <Box sx={{ pt: 1.25, textAlign: { xs: 'center', sm: 'left' } }}>
      <SmallIconTypography label={dayjs.utc(day.date).format('ddd').toUpperCase()} sx={{ fontWeight: 800, letterSpacing: '0.08em', opacity: 0.7 }} />
      <LeadIconTypography label={dayjs.utc(day.date).format('D')} sx={{ mt: -0.2, fontSize: { xs: '1.6rem', sm: '2rem' }, lineHeight: 1 }} />
      <SmallIconTypography label={dayjs.utc(day.date).format('MMM')} sx={{ mt: 0.3, opacity: 0.68 }} />
    </Box>
    <Card
      sx={{
        overflow: 'hidden',
        borderRadius: 3,
        border: 1,
        borderColor: 'divider',
        boxShadow: 'none',
      }}
    >
      {day.bookings.map((booking, index) => (
        <Box key={booking.id}>
          {index > 0 ? <Divider /> : null}
          <BookingRow booking={booking} integratedPlatform={integratedPlatform} />
        </Box>
      ))}
    </Card>
  </Box>
);

const BookingRow = ({ booking, integratedPlatform }: { booking: BookingNode; integratedPlatform: string | undefined }) => {
  const paymentStatus = booking.marketplaceBooking?.paymentStatus;
  return (
    <Link
      component={NextLink}
      href={getBookingLink(booking, integratedPlatform)}
      underline="none"
      color="inherit"
      sx={{ display: 'block', '&:hover': { bgcolor: 'action.hover' } }}
    >
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '68px minmax(0, 1fr) 20px',
            sm: '108px minmax(0, 1fr) auto 20px',
          },
          gap: { xs: 1, sm: 2 },
          alignItems: 'center',
          p: { xs: 1.5, sm: 2 },
        }}
      >
        <SmallIconTypography label={toStoredBookingTimeRange(booking.from, booking.until) || 'Time TBC'} sx={{ fontWeight: 800, opacity: 0.88 }} />
        <Box sx={{ minWidth: 0 }}>
          <BodyIconTypography label={booking.involvedLocations[0]?.name ?? 'Location to be confirmed'} sx={{ fontWeight: 700, overflowWrap: 'anywhere' }} />
          <ScheduleMeta booking={booking} resourcesLabel={toResourcesLabel(booking)} compact />
        </Box>
        <StackRow
          sx={{
            display: { xs: 'none', sm: 'flex' },
            justifyContent: 'flex-end',
            gap: 0.75,
            flexWrap: 'wrap',
          }}
        >
          {paymentStatus ? <PaymentChip type={paymentStatus.type} name={paymentStatus.name} /> : null}
          {booking.recurringBooking ? <Chip label={booking.recurringBooking.frequency.name} size="small" variant="outlined" /> : null}
        </StackRow>
        <ChevronRightIcon fontSize="small" color="action" />
        {paymentStatus || booking.recurringBooking ? (
          <StackRow
            sx={{
              gridColumn: { xs: '2 / 4', sm: 'auto' },
              display: { xs: 'flex', sm: 'none' },
              gap: 0.75,
              flexWrap: 'wrap',
            }}
          >
            {paymentStatus ? <PaymentChip type={paymentStatus.type} name={paymentStatus.name} /> : null}
            {booking.recurringBooking ? <Chip label={booking.recurringBooking.frequency.name} size="small" variant="outlined" /> : null}
          </StackRow>
        ) : null}
      </Box>
    </Link>
  );
};

const ScheduleMeta = ({ booking, compact, resourcesLabel }: { booking: BookingNode; compact?: boolean; resourcesLabel: string }) => {
  const teamLabel = booking.involvedTeams.map((team) => team.name).join(', ');
  const organizationName = booking.involvedOrganizations[0]?.name;
  return (
    <StackRow
      spacing={compact ? 0.75 : 1.25}
      sx={{
        mt: compact ? 0.3 : 0.85,
        flexWrap: 'wrap',
        color: 'text.secondary',
      }}
    >
      {organizationName ? <SmallIconTypography startElement={<LocationIcon fontSize="inherit" />} label={organizationName} sx={{ opacity: 0.8 }} /> : null}
      <SmallIconTypography startElement={<ResourceIcon fontSize="inherit" />} label={resourcesLabel} sx={{ opacity: 0.8 }} />
      {teamLabel ? <SmallIconTypography startElement={<TeamIcon fontSize="inherit" />} label={teamLabel} sx={{ opacity: 0.8 }} /> : null}
    </StackRow>
  );
};
const DateBlock = ({ date }: { date: string }) => (
  <Box sx={{ textAlign: 'center', color: 'success.dark' }}>
    <SmallIconTypography label={dayjs.utc(date).format('ddd').toUpperCase()} sx={{ fontWeight: 800, letterSpacing: '0.08em' }} />
    <LeadIconTypography
      label={dayjs.utc(date).format('D')}
      sx={{
        mt: -0.2,
        fontSize: { xs: '1.8rem', sm: '2.25rem' },
        lineHeight: 1,
      }}
    />
    <SmallIconTypography label={dayjs.utc(date).format('MMM')} sx={{ mt: 0.2 }} />
  </Box>
);
const PaymentChip = ({ name, type }: { name: string; type: string }) => (
  <Chip
    label={name}
    color={type === 'CONFIRMED' ? 'success' : type === 'PENDING' ? 'warning' : 'default'}
    size="small"
    variant={type === 'CONFIRMED' || type === 'PENDING' ? 'filled' : 'outlined'}
  />
);
const groupBookingsByDay = (bookings: ReadonlyArray<BookingNode>): ReadonlyArray<BookingDay> => {
  const days = new Map<string, BookingNode[]>();
  bookings.forEach((booking) => {
    const key = dayjs.utc(booking.from).format('YYYY-MM-DD');
    const day = days.get(key) ?? [];
    day.push(booking);
    days.set(key, day);
  });
  return Array.from(days, ([key, items]) => ({
    key,
    date: items[0]?.from ?? key,
    bookings: items,
  }));
};
const getBookingLink = (booking: BookingNode, integratedPlatform: string | undefined) => {
  const organizationCustomDomain = booking.involvedOrganizations[0]?.customDomain ?? '';
  return booking.marketplaceBooking
    ? getMarketplaceBookingDetailsLink(integratedPlatform, false, organizationCustomDomain, booking.id)
    : getTeamsOrganizationBookingBaseLink(organizationCustomDomain, booking.id);
};
const toResourcesLabel = (booking: BookingNode) => booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
const toNodes = <TNode,>(edges: ReadonlyArray<{ readonly node: TNode | null | undefined }>) => edges.map((edge) => edge.node).filter((item): item is NonNullable<TNode> => !!item);
const tabSx = {
  minHeight: 42,
  px: 1.25,
  textTransform: 'none',
  fontWeight: 700,
};

const MemoCustomerBookingsHub = memo(CustomerBookingsHub);
const CustomerBookingsHubWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<customerBookingsHub_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();
  if (!organizationCustomDomain) throw new Error('organizationCustomDomain is required');
  useEffect(() => {
    loadQuery({ today: dayjs().startOf('day').toISOString(), organizationCustomDomain }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationCustomDomain]);
  if (!queryReference) return <Loading />;
  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoCustomerBookingsHub queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(CustomerBookingsHubWithRelay);
