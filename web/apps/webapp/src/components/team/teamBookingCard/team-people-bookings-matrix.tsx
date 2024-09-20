import { TAG_TYPE_LOCATION_ZONE } from '@/components/zone';
import type { teamPeopleBookingsMatrix_addBookingMutation } from '@/queries/__generated__/teamPeopleBookingsMatrix_addBookingMutation.graphql';
import type { teamPeopleBookingsMatrix_deleteBookingMutation } from '@/queries/__generated__/teamPeopleBookingsMatrix_deleteBookingMutation.graphql';
import type {
  teamPeopleBookingsMatrix_query$data,
  teamPeopleBookingsMatrix_query$key,
} from '@/queries/__generated__/teamPeopleBookingsMatrix_query.graphql';
import type {
  TeamMemberOrderInput,
  teamPeopleBookingsMatrixTeamMembersPaginationQuery,
} from '@/queries/__generated__/teamPeopleBookingsMatrixTeamMembersPaginationQuery.graphql';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import type { GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { WorkingFromHomeIcon, WorkingFromOfficeIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfWeek, joinErrors, startOfWeek, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { graphql, useMutation, usePaginationFragment } from 'react-relay';

const defaultPageSize = 10000;

type Props = {
  rootDataRelay: teamPeopleBookingsMatrix_query$key;
  organizationId?: string;
  teamId: string;
  teamName: string;
};

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

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
  booking: teamPeopleBookingsMatrix_query$data['allBookings'][number] | undefined;
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

const TeamPeopleBookingsMatrix = ({ rootDataRelay, organizationId, teamId, teamName }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<teamPeopleBookingsMatrixTeamMembersPaginationQuery, teamPeopleBookingsMatrix_query$key>(
    graphql`
      fragment teamPeopleBookingsMatrix_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 10000 })
      @refetchable(queryName: "teamPeopleBookingsMatrixTeamMembersPaginationQuery") {
        paginatedTeamMembers(
          first: $count
          after: $cursor
          where: { teamId: $teamId, nameContains: $peopleNameSearchText }
          orderBy: $peopleSortingValues
        ) @connection(key: "teamPeopleBookingsMatrix_paginatedTeamMembers") {
          __id
          totalCount
          pageInfo {
            hasNextPage
          }
          edges {
            node {
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
        }
        me {
          id
        }
        organizationBookingPermissions(organizationId: $organizationId) @include(if: $fetchBookingPermission) {
          canAddBookingOnBehalf
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
    `,
    rootDataRelay,
  );

  const [commitAddBooking] = useMutation<teamPeopleBookingsMatrix_addBookingMutation>(graphql`
    mutation teamPeopleBookingsMatrix_addBookingMutation($input: AddBookingInput!) {
      addBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteBooking] = useMutation<teamPeopleBookingsMatrix_deleteBookingMutation>(graphql`
    mutation teamPeopleBookingsMatrix_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const [, startTransition] = useTransition();
  const [sortingTeamMemberOrder] = useState<TeamMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });

  const [startDate, setStartDate] = useState<Dayjs>(startOfWeek(null));
  const [peopleNameSearchText] = useState<string>('');
  const [page, setPage] = useState(0);
  const [pageSize] = useState(defaultPageSize);

  const handleRefetch = useCallback(
    (pageSize: number, startDate: Dayjs) => {
      startTransition(() => {
        const endDate = endOfWeek(startDate);

        refetch(
          {
            count: pageSize,
            peopleSortingValues: [sortingTeamMemberOrder],
            peopleNameSearchText,
            organizationId: organizationId ?? '',
            fetchBookingPermission: !!organizationId,
            teamId: teamId,
            from: startDate.toISOString(),
            to: endDate.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {
              setPage(0);
            },
          },
        );
      });
    },
    [refetch, sortingTeamMemberOrder, peopleNameSearchText, organizationId, teamId],
  );

  const memebrs = useMemo(() => rootData.paginatedTeamMembers, [rootData.paginatedTeamMembers]);
  const slicedEdges = memebrs.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > memebrs.edges.length ? memebrs.edges.length : page * pageSize + pageSize,
  );
  const allMembers = slicedEdges.map((member) => member.node);
  const meAsMember = allMembers.find((member) => member.customer!.uniqueId === rootData.me!.id);
  const otherMembers = allMembers.filter((member) => member.customer!.uniqueId !== rootData.me!.id);
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

          handleRefetch(pageSize, startDate);
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

          handleRefetch(pageSize, startDate);
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

  const handleDateRangeTypeChange = (event: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    let start = startOfWeek(null);
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);

    handleRefetch(pageSize, start);
  };

  if (!rootData.me) {
    return <></>;
  }

  const rowCount = rootData.paginatedTeamMembers?.totalCount ?? 0;

  return (
    <Card style={{ maxWidth: 500, height: '100%', overflow: 'auto' }}>
      <CardHeader title={teamName} subheader={<>{getDateRangeSelector(dateRangeType, handleDateRangeTypeChange)}</>} />
      <CardContent>
        <DataGrid
          rows={rows}
          columns={columns}
          hideFooterPagination={rowCount <= 10}
          initialState={{
            pagination: {
              rowCount,
              paginationModel: {
                pageSize: 10,
              },
            },
          }}
          pageSizeOptions={[10]}
          disableRowSelectionOnClick
          density="compact"
          onCellClick={handleCellClick}
        />
      </CardContent>
    </Card>
  );
};

export default memo(TeamPeopleBookingsMatrix);
