import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Autocomplete from '@mui/material/Autocomplete';
import Grid from '@mui/material/Grid';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { StaticDatePicker } from '@mui/x-date-pickers/StaticDatePicker';
import { EmptyCalendarToolbar, SimpleCalendarSlotProps } from '@repo/shared/components/generics';
import { OrganizationIcon } from '@repo/shared/components/icons';
import { endOfMonth, startOfMonth } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingCard } from 'components/booking';
import dayjs, { Dayjs } from 'dayjs';
import { useEffect, useMemo, useState } from 'react';
import { usePaginationFragment } from 'react-relay';
import type { smallMonthlyViewCalendarPaginationQuery } from './__generated__/smallMonthlyViewCalendarPaginationQuery.graphql';
import type { smallMonthlyViewCalendar_query$key } from './__generated__/smallMonthlyViewCalendar_query.graphql';
import SmallMonthlyViewCalendarDay from './small-monthly-view-calendar-day';

type Props = {
  rootDataRelay: smallMonthlyViewCalendar_query$key;
};

interface OrganizationDetails {
  id: string;
  name: string;
}

const SmallMonthlyViewCalendar = ({ rootDataRelay }: Props) => {
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

  const [date, setDate] = useState(startOfMonth(null));

  useEffect(() => {
    // TODO: 20230711 - Morteza: This will refetch in addition to the root query. The first refetch on initial render time must be prevented
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
  }, [refetch, date]);

  const connectionIds = useMemo(() => [rootData.monthlyBookings.__id], [rootData.monthlyBookings]);

  const nodes = useMemo(
    () =>
      rootData.monthlyBookings.edges
        .map((edge) => edge.node)
        .sort((node1, node2) => {
          if (dayjs(node1.from).isBefore(dayjs(node2.from))) {
            return -1;
          }

          if (dayjs(node1.from).isAfter(dayjs(node2.from))) {
            return 1;
          }

          return 0;
        }),
    [rootData.monthlyBookings.edges],
  );

  const [pageContextOpen, setPageContextOpen] = useState(false);
  const organizations = useMemo<OrganizationDetails[]>(
    () => rootData.myOrganizations.map((organization) => organization),
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
    <Grid
      container
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
      }}
    >
      <Stack direction="column">
        <Paper elevation={24} sx={{ marginBottom: 1 }}>
          <StaticDatePicker
            slots={{
              toolbar: EmptyCalendarToolbar,
              day: SmallMonthlyViewCalendarDay({
                rootData: rootData,
                connectionIds,
                organizationId: selectedOrganization ? selectedOrganization.id : null,
              }),
            }}
            slotProps={SimpleCalendarSlotProps}
            onMonthChange={handleMonthChange}
            sx={{ marginBottom: 1 }}
          />
        </Paper>
      </Stack>

      <Stack direction="column" sx={{ width: '100%' }}>
        <Grid item sx={{ marginBottom: 1 }}>
          <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              {!pageContextOpen && selectedOrganization && (
                <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
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
                      <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
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
        </Grid>
      </Stack>

      <Grid container spacing={{ xs: 2, md: 3 }}>
        {nodes.map((node) => (
          <Grid item key={node.id}>
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
    </Grid>
  );
};

export default SmallMonthlyViewCalendar;
