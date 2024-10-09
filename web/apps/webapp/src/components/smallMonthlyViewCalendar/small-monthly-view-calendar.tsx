import { BookingCard } from '@/components/booking';
import type { smallMonthlyViewCalendarPaginationQuery } from '@/queries/__generated__/smallMonthlyViewCalendarPaginationQuery.graphql';
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
import { endOfMonth, startOfDay, startOfMonth } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { memo, startTransition, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
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
  }
`;

type OrganizationDetails = {
  id: string;
  name: string;
};

const SmallMonthlyViewCalendar = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<smallMonthlyViewCalendar_rootQuery>(RootQuery, queryReference);
  const { data: rootData, refetch } = usePaginationFragment<smallMonthlyViewCalendarPaginationQuery, smallMonthlyViewCalendar_query$key>(
    graphql`
      fragment smallMonthlyViewCalendar_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 1000 })
      @refetchable(queryName: "smallMonthlyViewCalendarPaginationQuery") {
        monthlyBookings: bookings(
          first: $count
          after: $cursor
          where: { fromGTE: $monthlyCalendarDateFrom, toLT: $monthlyCalendarDateTo, includeMineOnly: true }

          orderBy: $smallMonthlyViewCalendarBookingsSortingValues
        ) @connection(key: "SmallMonthlyViewCalendar_monthlyBookings") {
          __id
          edges {
            node {
              id
              from
              to
              notes
              customer {
                photoUrl
              }
              ...bookingCard_BookingDetails
            }
          }
        }
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

  const [date, setDate] = useState(startOfMonth());

  useEffect(() => {
    // TODO: 20230711 - Morteza: This will refetch in addition to the root query. The first refetch on initial render time must be prevented
    startTransition(() => {
      refetch(
        {
          monthlyCalendarDateFrom: startOfMonth(date).toISOString(),
          monthlyCalendarDateTo: endOfMonth(date).toISOString(),
        },
        {
          fetchPolicy: 'store-and-network',
          onComplete: () => {},
        },
      );
    });
  }, [refetch, date]);

  const connectionIds = useMemo(() => (rootData.monthlyBookings ? [rootData.monthlyBookings.__id] : []), [rootData.monthlyBookings]);

  const nodes = useMemo(() => {
    if (!rootData.monthlyBookings) {
      return [];
    }

    return rootData.monthlyBookings.edges
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
  }, [rootData.monthlyBookings]);

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
              rootData: rootData,
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
  const [triggerReload, setTriggerReload] = useState(0);
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
            field: 'name',
          },
        ],
        smallMonthlyViewCalendarBookingsSortingValues: [
          {
            direction: 'Ascending',
            field: 'from',
          },
        ],
        dateToGetAvailableDesks: startOfDay().toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
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
