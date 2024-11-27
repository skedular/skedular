import { BookingCard } from '@/components/booking';
import type { smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment } from '@/queries/__generated__/smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment.graphql';
import type { smallMonthlyViewCalendar_bookings_query$key } from '@/queries/__generated__/smallMonthlyViewCalendar_bookings_query.graphql';
import type { smallMonthlyViewCalendar_query$key } from '@/queries/__generated__/smallMonthlyViewCalendar_query.graphql';
import type { smallMonthlyViewCalendar_rootQuery } from '@/queries/__generated__/smallMonthlyViewCalendar_rootQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Autocomplete from '@mui/material/Autocomplete';
import Grid from '@mui/material/Grid2';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { StaticDatePicker } from '@mui/x-date-pickers/StaticDatePicker';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@repo/shared/components/generics';
import { OrganizationIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { GlobalReloadIdContext } from '@repo/shared/libs/providers';
import { endOfMonth, startOfDay, startOfMonth } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { memo, startTransition, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, useFragment, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import SmallMonthlyViewCalendarDay from './small-monthly-view-calendar-day';

type Props = {
  queryReference: PreloadedQuery<smallMonthlyViewCalendar_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query smallMonthlyViewCalendar_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $monthlyCalendarDateFrom: DateTime!
    $monthlyCalendarDateTo: DateTime!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingPeopleNameSearchText: String
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $smallMonthlyViewCalendarBookingsSortingValues: [BookingOrderInput!]
  ) {
    ...smallMonthlyViewCalendar_query
    ...smallMonthlyViewCalendar_bookings_query
  }
`;

type OrganizationDetails = {
  id: string;
  name: string;
};

const SmallMonthlyViewCalendar = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<smallMonthlyViewCalendar_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<smallMonthlyViewCalendar_query$key>(
    graphql`
      fragment smallMonthlyViewCalendar_query on Query {
        me {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
          defaultOrganization {
            uniqueId
          }
        }
        myOrganizations {
          id
          name
        }
        ...bookingCard_query
      }
    `,
    rootDataRelay,
  );
  const { data: rootDataBookings, refetch } = usePaginationFragment<
    smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment,
    smallMonthlyViewCalendar_bookings_query$key
  >(
    graphql`
      fragment smallMonthlyViewCalendar_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 1000 })
      @refetchable(queryName: "smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: { fromGTE: $monthlyCalendarDateFrom, toLT: $monthlyCalendarDateTo, includeMineOnly: true }
          orderBy: $smallMonthlyViewCalendarBookingsSortingValues
        ) @connection(key: "SmallMonthlyViewCalendar_bookings") {
          __id
          edges {
            node {
              id
              from
              to
              notes
              customer {
                uniqueId
                name
                givenName
                middleName
                familyName
                photoUrl
              }
              organization {
                uniqueId
                name
              }
              location {
                uniqueId
                name
              }
              team {
                uniqueId
                name
              }
              desks {
                uniqueId
                name
                deskTypes {
                  uniqueId
                  name
                }
                zones {
                  uniqueId
                  name
                }
              }
              ...bookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const globalReloadId = useContext(GlobalReloadIdContext);
  const [date, setDate] = useState(startOfMonth());

  useEffect(() => {
    startTransition(() => {
      refetch(
        {
          monthlyCalendarDateFrom: startOfMonth(date).toISOString(),
          monthlyCalendarDateTo: endOfMonth(date).toISOString(),
        },
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetch, globalReloadId, date]);

  const connectionIds = useMemo(() => (rootDataBookings.bookings ? [rootDataBookings.bookings.__id] : []), [rootDataBookings.bookings]);

  const nodes = useMemo(() => {
    if (!rootDataBookings.bookings) {
      return [];
    }

    return rootDataBookings.bookings.edges
      .map((edge) => edge.node)
      .sort((node1, node2) => {
        if (dayjs(node1.from).isBefore(dayjs(node2.from))) {
          return -1;
        }

        if (dayjs(node1.from).isAfter(dayjs(node2.from))) {
          return 1;
        }

        return 0;
      });
  }, [rootDataBookings.bookings]);

  const [pageContextOpen, setPageContextOpen] = useState(false);
  const organizations = useMemo<OrganizationDetails[]>(
    () => (rootData.myOrganizations ? rootData.myOrganizations.map((organization) => organization) : []),
    [rootData.myOrganizations],
  );

  const defaultOrganization = useMemo<OrganizationDetails | null>(() => {
    const matchingOrganization = organizations.find(
      (organization) => rootData.me?.defaultOrganization && organization.id === rootData.me.defaultOrganization.uniqueId,
    );

    return !!matchingOrganization ? matchingOrganization : null;
  }, [organizations, rootData.me?.defaultOrganization]);

  const [selectedOrganization, setSelectedOrganization] = useState<OrganizationDetails | null>(defaultOrganization);

  const filter = createFilterOptions<OrganizationDetails>();

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleMonthChange = (date: Dayjs) => {
    setDate(date.startOf('month').add(1, 'month'));
  };

  return (
    <Stack sx={{ alignItems: 'center' }} direction="column">
      <Paper elevation={24} sx={{ marginBottom: 1 }}>
        <StaticDatePicker
          slots={{
            toolbar: EmptyCalendarToolbar,
            day: SmallMonthlyViewCalendarDay({
              rootData,
              rootDataBookings,
              connectionIds,
              organizationId: selectedOrganization ? selectedOrganization.id : undefined,
            }),
          }}
          slotProps={SimpleCalendarSlotProps}
          onMonthChange={handleMonthChange}
          sx={{ marginBottom: 1 }}
        />
      </Paper>

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%', marginBottom: 1 }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          {!pageContextOpen && selectedOrganization && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <OrganizationIcon />
              <Typography>{selectedOrganization.name}</Typography>
            </Stack>
          )}
        </AccordionSummary>
        <AccordionDetails>
          <Autocomplete
            options={organizations}
            onChange={(event, option) => {
              const castedOption = option as OrganizationDetails;

              setSelectedOrganization(castedOption);
            }}
            defaultValue={selectedOrganization}
            getOptionLabel={(option: string | OrganizationDetails) => (option as OrganizationDetails).name}
            renderOption={(props, option) => {
              const castedOption = option as OrganizationDetails;

              return (
                <li {...props}>
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                    <Typography variant="body1">{castedOption.name}</Typography>
                  </Stack>
                </li>
              );
            }}
            renderInput={(params) => <TextField {...params} label="Organization" />}
            disableCloseOnSelect={false}
            freeSolo={true}
            filterOptions={(options, params) => filter(options as OrganizationDetails[], params)}
            selectOnFocus
            clearOnBlur
            handleHomeEndKeys
          />
        </AccordionDetails>
      </Accordion>

      <Grid container spacing={1}>
        {nodes.map((node) => (
          <Grid key={node.id}>
            <BookingCard
              rootDataRelay={rootData}
              bookingDetailsRelay={node}
              connectionIds={connectionIds}
              hideOrganizationControl={false}
              hideLocationControl={false}
              canJoinBooking={false}
            />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

const MemoSmallMonthlyViewCalendar = memo(SmallMonthlyViewCalendar);

const SmallMonthlyViewCalendarWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<smallMonthlyViewCalendar_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const date = startOfMonth();

    loadQuery(
      {
        monthlyCalendarDateFrom: startOfMonth(date).toISOString(),
        monthlyCalendarDateTo: endOfMonth(date).toISOString(),
        deskIdsToIncludeToGetAvailableDesks: [],
        organizationId: '',
        organizationExists: false,
        locationId: '',
        locationExists: false,
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        smallMonthlyViewCalendarBookingsSortingValues: [
          {
            direction: 'Ascending',
            field: 'From',
          },
        ],
        dateToGetAvailableDesks: startOfDay().toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoSmallMonthlyViewCalendar queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(SmallMonthlyViewCalendarWithRelay);
