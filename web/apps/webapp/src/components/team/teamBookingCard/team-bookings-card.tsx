import { TAG_TYPE_LOCATION_ZONE } from '@/components/zone';
import type { teamBookingsCard_addBookingMutation } from '@/queries/__generated__/teamBookingsCard_addBookingMutation.graphql';
import type { teamBookingsCard_deleteBookingMutation } from '@/queries/__generated__/teamBookingsCard_deleteBookingMutation.graphql';
import type { teamBookingsCard_rootQuery, teamBookingsCard_rootQuery$data } from '@/queries/__generated__/teamBookingsCard_rootQuery.graphql';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import type { GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGridPremium } from '@mui/x-data-grid-premium';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { WorkingFromHomeIcon, WorkingFromOfficeIcon } from '@repo/shared/components/icons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfWeek, joinErrors, startOfWeek, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

type Props = {
  queryReference: PreloadedQuery<teamBookingsCard_rootQuery, Record<string, unknown>>;
  onReloadRequire: (dateRangeType: DateRangeType) => void;
  organizationId?: string;
  teamId: string;
  teamName: string;
  startDate: Dayjs;
};

type CustomerDetails = {
  uniqueId: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
};

type BookingDetails = {
  customer: CustomerDetails;
  booking: teamBookingsCard_rootQuery$data['allBookings'][number] | undefined;
};

type RowType = {
  id: string;
  customer: CustomerDetails;
  mon: BookingDetails;
  tue: BookingDetails;
  wed: BookingDetails;
  thu: BookingDetails;
  fri: BookingDetails;
  sat: BookingDetails;
  sun: BookingDetails;
};

const dayIndex: { [key: string]: number } = { mon: 0, tue: 1, wed: 2, thu: 3, fri: 4, sat: 5, sun: 6 };
const getDateRangeSelector = (value: DateRangeType, onDateRangeTypeChange?: (event: React.MouseEvent<HTMLElement>, value: DateRangeType) => void) => (
  <ToggleButtonGroup color="primary" value={value} exclusive onChange={onDateRangeTypeChange} size="small">
    <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
    <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
  </ToggleButtonGroup>
);

const RootQuery = graphql`
  query teamBookingsCard_rootQuery($fetchBookingPermission: Boolean!, $organizationId: String!, $teamId: String!, $from: DateTime!, $to: DateTime!) {
    me {
      id
    }
    organizationBookingPermissions(organizationId: $organizationId) @include(if: $fetchBookingPermission) {
      canAddBookingOnBehalf
    }
    team(id: $teamId) {
      members {
        id
        customer {
          uniqueId
          name
          givenName
          middleName
          familyName
          photoUrl
        }
      }
    }
    allBookings(where: { teamIds: [$teamId], fromGTE: $from, toLT: $to }) {
      id
      from
      customer {
        uniqueId
      }
      location {
        name
      }
      desks {
        name
        locationTags {
          uniqueId
          name
          tagType
        }
      }
    }
  }
`;

const TeamBookingsCard = ({ queryReference, onReloadRequire, organizationId, teamId, teamName, startDate }: Props) => {
  const [commitAddBooking] = useMutation<teamBookingsCard_addBookingMutation>(graphql`
    mutation teamBookingsCard_addBookingMutation($input: AddBookingInput!) {
      addBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteBooking] = useMutation<teamBookingsCard_deleteBookingMutation>(graphql`
    mutation teamBookingsCard_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const rootData = usePreloadedQuery<teamBookingsCard_rootQuery>(RootQuery, queryReference);
  const { enqueueSnackbar } = useSnackbar();
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const handleDateRangeTypeChange = (event: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    setDateRangeType(value);
    onReloadRequire(value);
  };

  if (!rootData.me || !rootData.team) {
    return <></>;
  }

  const team = rootData.team;
  const meAsMember = team.members.find((member) => member.customer!.uniqueId === rootData.me!.id);
  const otherMembers = team.members.filter((member) => member.customer!.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList.map((member) => {
    const customer = member.customer!;
    const customerId = customer.uniqueId;

    return {
      id: customerId,
      customer,
      mon: {
        customer,
        booking: rootData.allBookings.find((booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.toISOString()),
      },
      tue: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(1, 'day').toISOString(),
        ),
      },
      wed: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(2, 'day').toISOString(),
        ),
      },
      thu: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(3, 'day').toISOString(),
        ),
      },
      fri: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(4, 'day').toISOString(),
        ),
      },
      sat: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(5, 'day').toISOString(),
        ),
      },
      sun: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(6, 'day').toISOString(),
        ),
      },
    };
  });

  const getBookingIcon = ({ booking }: BookingDetails) => {
    let tip = '';

    if (booking) {
      tip = `Working`;
      if (booking.location) {
        tip += ` from "${booking.location!.name}"`;
      }

      if (booking.desks.length > 0) {
        tip += ` at "${booking.desks.map(({ name }) => name).join(', ')}"`;

        const zones = booking.desks.flatMap(({ locationTags }) => locationTags).filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);
        if (zones.length > 0) {
          const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

          tip += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
        }
      }
    }

    return booking ? <WorkingFromOfficeIcon tip={tip} /> : <WorkingFromHomeIcon />;
  };

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'customer',
      headerName: '',
      renderCell: (params) => (
        // TODO: 20240919 - Morteza: I don't like below 80% custom height setup, get rid of it in future.
        <Box display="flex" justifyContent="center" alignItems="center" height="80%">
          <CustomerAvatar
            name={{
              name: params.value.name,
              givenName: params.value.givenName,
              middleName: params.value.middleName,
              familyName: params.value.familyName,
            }}
            photo={{
              url: params.value.photoUrl,
            }}
            size="small"
            showFullName={true}
          />
        </Box>
      ),
    },
    {
      field: 'mon',
      headerName: 'Mon',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'tue',
      headerName: 'Tue',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'wed',
      headerName: 'Wed',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'thu',
      headerName: 'Thu',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'fri',
      headerName: 'Fri',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'sat',
      headerName: 'Sat',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'sun',
      headerName: 'Sun',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
  ];

  const handleCellClick = (params: GridCellParams, event: MuiEvent, details: GridCallbackDetails) => {
    const { customer, booking } = params.value as BookingDetails;
    if (!rootData.organizationBookingPermissions?.canAddBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      return;
    }

    const id = booking ? booking.id : nanoid();
    const index = dayIndex[params.field]!;
    const startOfDay = startDate.add(index, 'day');
    const from = startOfDay.toISOString();
    const to = endOfDay(startOfDay).toISOString();
    const fromToPrint = toShortDate(startOfDay);

    if (booking) {
      commitDeleteBooking({
        variables: {
          input: {
            clientMutationId: nanoid(),
            id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }

          onReloadRequire(dateRangeType);
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
      });
    } else {
      commitAddBooking({
        variables: {
          input: {
            clientMutationId: nanoid(),
            id,
            customerId: customer.uniqueId,
            from,
            to,
            organizationId,
            teamId,
            deskIds: [],
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }

          onReloadRequire(dateRangeType);
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          addBooking: {
            booking: {
              id,
            },
          },
        },
      });
    }
  };

  return (
    <Card style={{ maxWidth: 500, maxHeight: 500, overflow: 'auto' }}>
      <CardHeader title={teamName} subheader={<>{getDateRangeSelector(dateRangeType, handleDateRangeTypeChange)}</>} />
      <CardActionArea>
        <CardContent>
          <DataGridPremium rows={rows} columns={columns} disableRowSelectionOnClick hideFooter density="compact" onCellClick={handleCellClick} />
        </CardContent>
      </CardActionArea>
    </Card>
  );
};

const MemoTeamBookingsCard = memo(TeamBookingsCard);

type RelayProps = {
  organizationId?: string;
  teamId: string;
  teamName: string;
};

const TeamBookingsWithRelay = ({ organizationId, teamId, teamName }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teamBookingsCard_rootQuery>(RootQuery);
  const [start, setStart] = useState(startOfWeek(null));
  const [end, setEnd] = useState(endOfWeek(start));

  useEffect(() => {
    loadQuery(
      {
        organizationId: organizationId ?? '',
        fetchBookingPermission: !!organizationId,
        teamId: teamId,
        from: start.toISOString(),
        to: end.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, teamId, start, end, organizationId]);

  const handleReloadRequire = (dateRangeType: DateRangeType) => {
    let start = startOfWeek(null);
    if (dateRangeType === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStart(start);
    setEnd(endOfWeek(start));
  };

  if (!queryReference) {
    return (
      <Card style={{ maxWidth: 500, maxHeight: 500, overflow: 'auto' }}>
        <CardHeader title={teamName} subheader={<>{getDateRangeSelector(DateRangeType.ThisWeek)}</>} />
        <CardActionArea>
          <CardContent>
            <Skeleton variant="rounded" width={470} height={350} />
          </CardContent>
        </CardActionArea>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamBookingsCard
        queryReference={queryReference}
        onReloadRequire={handleReloadRequire}
        organizationId={organizationId}
        teamId={teamId}
        teamName={teamName}
        startDate={start}
      />
    </ErrorBoundary>
  );
};

export default memo(TeamBookingsWithRelay);
