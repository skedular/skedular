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
import { Stack, Typography } from '@mui/material';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import type { GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BookingIcon, DeleteIcon, EllipseMenuIcon, SettingsIcon, WorkingFromHomeIcon, WorkingFromOfficeIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfWeek, getCustomerFullName, joinErrors, startOfWeek, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
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

enum MoreActionsMenuOptionType {
  Settings,
  RemoveTeam,
}

interface MoreActionsMenuItemType {
  id: MoreActionsMenuOptionType;
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
}

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.Settings]: {
    id: MoreActionsMenuOptionType.Settings,
    label: 'Settings',
    icon: <SettingsIcon />,
    color: 'secondary',
  },
  [MoreActionsMenuOptionType.RemoveTeam]: {
    id: MoreActionsMenuOptionType.RemoveTeam,
    label: 'Remove',
    icon: <DeleteIcon />,
    color: 'warning',
  },
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

const getBookingIcon = ({ booking }: BookingDetails) => {
  let tip = '';

  if (booking) {
    tip = `Working`;
    if (booking.location) {
      tip += ` from the "${booking.location!.name}"`;
    }

    if (booking.desks.length > 0) {
      tip += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

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
  const { data: rootData, refetch } = usePaginationFragment<teamPeopleBookingsMatrixTeamMembersPaginationQuery, teamPeopleBookingsMatrix_query$key>(
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
        team(id: $teamId) {
          canModify
          canDelete
        }
        organizationBookingPermissions(organizationId: $organizationId) @include(if: $fetchBookingPermission) {
          canAddBookingOnBehalf
        }
        allBookings(where: { teamIds: [$teamId], fromGTE: $from, toLT: $to }) {
          id
          from
          customer {
            uniqueId
            name
            givenName
            middleName
            familyName
            photoUrl
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
          from
          customer {
            name
            givenName
            middleName
            familyName
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
  const router = useRouter();
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
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

          let message = `Booking removed for ${getCustomerFullName(booking.customer)}`;

          if (booking.location) {
            message += ` at the "${booking.location!.name}"`;
          }

          message += ` on ${toShortDate(booking.from)}`;

          handleRefetch(pageSize, startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
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
        onCompleted: (response, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }

          const booking = response.addBooking?.booking!;
          let message = `Booking added for ${getCustomerFullName(booking.customer)} to work`;

          if (booking.location) {
            message += ` from the "${booking.location!.name}"`;
          }

          if (booking.desks.length > 0) {
            message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

            const zones = booking.desks.flatMap(({ locationTags }) => locationTags).filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);
            if (zones.length > 0) {
              const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

              message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
            }
          }

          message += ` on ${toShortDate(booking.from)}`;

          handleRefetch(pageSize, startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
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

  if (!rootData.me || !rootData.team) {
    return <></>;
  }

  const rowCount = rootData.paginatedTeamMembers?.totalCount ?? 0;

  let moreActionsOption: MoreActionsMenuItemType[] = [];
  if (rootData.team.canModify) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.Settings]);
  }

  if (rootData.team.canDelete) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveTeam]);
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.Settings:
        handleSettingsClicked();
        break;
    }
  };

  const handleBookingsClicked = () => {
    router.push(organizationId ? `/organization/${organizationId}/team/${teamId}?tab=bookings` : `/team/${teamId}?tab=bookings`);
  };

  const handleSettingsClicked = () => {
    router.push(organizationId ? `/organization/${organizationId}/team/${teamId}?tab=about` : `/team/${teamId}?tab=about`);
  };

  return (
    <>
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={teamName}
          subheader={
            <Stack direction="row" justifyContent="space-between" width="100%">
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <IconButton color="primary" onClick={handleBookingsClicked}>
                  <BookingIcon />
                </IconButton>
              </Stack>
            </Stack>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <IconButton onClick={handleMoreActionsMenuClick}>
                  <EllipseMenuIcon />
                </IconButton>
              )}
            </>
          }
        />
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
      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <IconButton color={option.color}>{option.icon}</IconButton>
              <Typography variant="body1">{option.label}</Typography>
            </Stack>
          </MenuItem>
        ))}
      </Menu>
    </>
  );
};

export default memo(TeamPeopleBookingsMatrix);
