import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Typography from '@mui/material/Typography';
import type { GetApplyQuickFilterFn, GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid, GridToolbarQuickFilter } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BookingIcon as BookingIconComponent } from '@repo/shared/components/booking';
import {
  BookingIcon,
  DangerIcon,
  DeleteIcon,
  EllipseMenuIcon,
  NotPreferredIcon,
  OrganizationIcon,
  PreferredIcon,
  SettingsIcon,
} from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfWeek, getCustomerFullName, joinErrors, startOfWeek, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { TeamLink, getTeamBookingsLink, getTeamSettingsLink } from 'components/team';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useState, useTransition } from 'react';
import { useMutation, useRefetchableFragment } from 'react-relay';
import type { teamPeopleBookings_addBookingMutation } from './__generated__/teamPeopleBookings_addBookingMutation.graphql';
import type { teamPeopleBookings_addCustomerDefaultTeamMutation } from './__generated__/teamPeopleBookings_addCustomerDefaultTeamMutation.graphql';
import type { teamPeopleBookings_deleteBookingMutation } from './__generated__/teamPeopleBookings_deleteBookingMutation.graphql';
import type { teamPeopleBookings_deleteTeamMutation } from './__generated__/teamPeopleBookings_deleteTeamMutation.graphql';
import type { teamPeopleBookings_query$key } from './__generated__/teamPeopleBookings_query.graphql';
import type { teamPeopleBookings_removeCustomerDefaultTeamMutation } from './__generated__/teamPeopleBookings_removeCustomerDefaultTeamMutation.graphql';
import type { TeamMemberOrderInput } from './__generated__/teamPeopleBookingsTeamMembers_PaginationQuery.graphql';

type Props = {
  rootDataRelay: teamPeopleBookings_query$key;
  organizationId?: string;
  teamId: string;
  teamName?: string;
  teamsConnectionIds: string[];
  hideRemoveTeamOption?: boolean;
};

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

enum MoreActionsMenuOptionType {
  SetAsPreferredTeam,
  RemoveAsPreferredTeam,
  RemoveTeam,
}

interface MoreActionsMenuItemType {
  id: MoreActionsMenuOptionType;
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
}

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.SetAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.SetAsPreferredTeam,
    label: 'Set as preferred team',
    icon: <NotPreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredTeam,
    label: 'Remove as preferred team',
    icon: <PreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveTeam]: {
    id: MoreActionsMenuOptionType.RemoveTeam,
    label: 'Remove team',
    icon: <DeleteIcon />,
    color: 'warning',
  },
};

type CustomerDetails = {
  readonly uniqueId: string;
  readonly givenName?: string | null | undefined;
  readonly middleName?: string | null | undefined;
  readonly familyName?: string | null | undefined;
  readonly name?: string | null | undefined;
  readonly photoUrl?: string | null | undefined;
};

type LocationDetails = {
  readonly name?: string | null | undefined;
};

type LocationTagDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
  readonly tagType?: string | null | undefined;
};

type DeskDetails = {
  readonly name?: string | null | undefined;
  readonly locationTags: ReadonlyArray<LocationTagDetails>;
};

type TeamDetails = {
  readonly name?: string | null | undefined;
};

type BookingDetails = {
  readonly id: string;
  readonly customer: CustomerDetails;
  readonly location?: LocationDetails | null | undefined;
  readonly team?: TeamDetails | null | undefined;
  readonly desks: ReadonlyArray<DeskDetails>;
  readonly from: any;
  readonly to: any;
};

type BookingAndCustomerDetails = {
  customer: CustomerDetails;
  booking: BookingDetails | null | undefined;
};

type RowType = {
  id: string;
  person: CustomerDetails;
  mon: BookingAndCustomerDetails;
  tue: BookingAndCustomerDetails;
  wed: BookingAndCustomerDetails;
  thu: BookingAndCustomerDetails;
  fri: BookingAndCustomerDetails;
  sat: BookingAndCustomerDetails;
  sun: BookingAndCustomerDetails;
};

const dayIndex: { [key: string]: number } = { mon: 0, tue: 1, wed: 2, thu: 3, fri: 4, sat: 5, sun: 6 };

const QuickSearchToolbar = () => <GridToolbarQuickFilter placeholder="Find a person..." />;

const TeamPeopleBookings = ({ rootDataRelay, organizationId, teamId, teamName, teamsConnectionIds, hideRemoveTeamOption }: Props) => {
  const [rootData, refetch] = useRefetchableFragment(
    graphql`
      fragment teamPeopleBookings_query on Query @refetchable(queryName: "teamPeopleBookingsTeamMembers_PaginationQuery") {
        teamMembers(where: { teamId: $teamId, nameContains: $peopleNameSearchText }, orderBy: $peopleSortingValues) @include(if: $teamExists) {
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
        me {
          id
          defaultTeams {
            uniqueId
          }
        }
        team(id: $teamId) {
          name
          hasFutureBooking
          canModify
          canDelete
          organization {
            uniqueId
            name
          }
        }
        teamBookingPermissions(teamId: $teamId) @include(if: $teamExists) {
          canAddBookingOnBehalf
          canDeleteBookingOnBehalf
        }
        allBookings(where: { teamIds: [$teamId], fromGTE: $from, toLT: $to }) {
          id
          from
          to
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
          team {
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

  const [commitAddBooking] = useMutation<teamPeopleBookings_addBookingMutation>(graphql`
    mutation teamPeopleBookings_addBookingMutation($input: AddBookingInput!) {
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

  const [commitDeleteBooking] = useMutation<teamPeopleBookings_deleteBookingMutation>(graphql`
    mutation teamPeopleBookings_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteTeam] = useMutation<teamPeopleBookings_deleteTeamMutation>(graphql`
    mutation teamPeopleBookings_deleteTeamMutation($connectionIds: [ID!]!, $input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultTeam] = useMutation<teamPeopleBookings_addCustomerDefaultTeamMutation>(graphql`
    mutation teamPeopleBookings_addCustomerDefaultTeamMutation($input: AddCustomerDefaultTeamInput!) {
      addCustomerDefaultTeam(input: $input) {
        customer {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultTeam] = useMutation<teamPeopleBookings_removeCustomerDefaultTeamMutation>(graphql`
    mutation teamPeopleBookings_removeCustomerDefaultTeamMutation($input: RemoveCustomerDefaultTeamInput!) {
      removeCustomerDefaultTeam(input: $input) {
        customer {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const [, startTransition] = useTransition();
  const [sortingTeamMemberOrder] = useState<TeamMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfWeek());
  const [peopleNameSearchText] = useState<string>('');

  const handleRefetch = useCallback(
    (startDate: Dayjs) => {
      startTransition(() => {
        const endDate = endOfWeek(startDate);

        refetch(
          {
            peopleSortingValues: [sortingTeamMemberOrder],
            peopleNameSearchText,
            organizationId: organizationId ?? '',
            fetchBookingPermission: !!organizationId,
            teamId,
            teamExists: !!teamId,
            from: startDate.toISOString(),
            to: endDate.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch, sortingTeamMemberOrder, peopleNameSearchText, organizationId, teamId],
  );

  if (!rootData.me || !rootData.team || !rootData.teamMembers) {
    return <></>;
  }

  const allMembers = rootData.teamMembers.map((member) => member.customer);
  const meAsMember = allMembers.find((customer) => customer.uniqueId === rootData.me!.id);
  const otherMembers = allMembers.filter((customer) => customer.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList
    .map((customer) => {
      if (!rootData.allBookings) {
        return null;
      }

      const customerId = customer.uniqueId;

      return {
        id: customerId,
        person: customer,
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
    })
    .filter((row) => !!row);

  const getApplyQuickFilterNameSearch: GetApplyQuickFilterFn<any, unknown> = (value) => {
    return (cellValue) => {
      const lowercaseValue = value.toLowerCase();
      const customer = cellValue as CustomerDetails;

      return Object.entries(customer).some(
        ([key, value]) => key !== 'uniqueId' && key !== 'photoUrl' && typeof value === 'string' && value.toLowerCase().includes(lowercaseValue),
      );
    };
  };

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'person',
      headerName: '',
      renderCell: (params) => (
        <Box display="flex" justifyContent="center" alignItems="center" height="100%">
          <CustomerAvatar name={params.value} photo={{ url: params.value.photoUrl }} size="small" showFullName={true} />
        </Box>
      ),
      getApplyQuickFilterFn: getApplyQuickFilterNameSearch,
    },
    {
      field: 'mon',
      headerName: 'Mon',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'tue',
      headerName: 'Tue',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'wed',
      headerName: 'Wed',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'thu',
      headerName: 'Thu',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'fri',
      headerName: 'Fri',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'sat',
      headerName: 'Sat',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'sun',
      headerName: 'Sun',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
  ];

  const handleCellClick = (params: GridCellParams, event: MuiEvent, details: GridCallbackDetails) => {
    const { customer, booking } = params.value as BookingAndCustomerDetails;
    if (!booking && !rootData.teamBookingPermissions?.canAddBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      enqueueSnackbar(`You are not authorized to make a booking on behalf of someone else`, {
        variant: 'error',
        anchorOrigin,
      });

      return;
    }

    if (booking && !rootData.teamBookingPermissions?.canDeleteBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      enqueueSnackbar(`You are not authorized to remove this booking on behalf of someone else`, {
        variant: 'error',
        anchorOrigin,
      });

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

          handleRefetch(startDate);
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
            enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
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

          handleRefetch(startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
      });
    }
  };

  const handleDateRangeTypeChange = (event: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    let start = startOfWeek();
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);

    handleRefetch(start);
  };

  const rowCount = rootData.teamMembers.length;

  let moreActionsOption: MoreActionsMenuItemType[] = [];
  if (rootData.me.defaultTeams.some((team) => team.uniqueId === teamId)) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredTeam]);
  } else {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredTeam]);
  }

  if (rootData.team.canDelete && !hideRemoveTeamOption) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveTeam]);
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.SetAsPreferredTeam:
        handleSetAsPreferredTeamClicked();
        break;

      case MoreActionsMenuOptionType.RemoveAsPreferredTeam:
        handleRemoveAsPreferredTeamClicked();
        break;

      case MoreActionsMenuOptionType.RemoveTeam:
        handleRemoveTeamClicked();
        break;
    }
  };

  const handleSetAsPreferredTeamClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitAddCustomerDefaultTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: teamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to set team '${teamName}' as your preferred team. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }

        enqueueSnackbar(`Team '${teamName}' has been set as the preferred team.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to set team '${teamName}' as your preferred team. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addCustomerDefaultTeam: {
          customer: {
            id: rootData.me.id,
            defaultTeams: rootData.me.defaultTeams.concat([
              {
                uniqueId: teamId,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredTeamClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitRemoveCustomerDefaultTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: teamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove the team '${teamName}' as your preferred team. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }

        enqueueSnackbar(`Team '${teamName}' has been removed as your preferred team.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove the team '${teamName}' as your preferred team. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addCustomerDefaultTeam: {
          customer: {
            id: rootData.me.id,
            defaultTeams: rootData.me.defaultTeams.filter(({ uniqueId }) => uniqueId === teamId),
          },
        },
      },
    });
  };

  const handleRemoveTeamClicked = () => {
    setTeamRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingTeamClick = () => {
    setTeamRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingTeamClick = () => {
    if (!rootData.me) {
      return;
    }

    commitDeleteTeam({
      variables: {
        connectionIds: teamsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          id: teamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove team '${teamName}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }

        enqueueSnackbar(`Team '${teamName}' has been successfully removed.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove team '${teamName}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="column">
              <TeamLink organizationId={organizationId} id={teamId} name={teamName} />
              {rootData.team.organization && <OrganizationLink id={rootData.team.organization.uniqueId} name={rootData.team.organization.name} />}
            </Stack>
          }
          subheader={
            <Stack direction="row" sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <Link href={getTeamBookingsLink(teamId, organizationId)}>
                  <BookingIcon />
                </Link>

                {rootData.team.canModify && (
                  <Link href={getTeamSettingsLink(teamId, organizationId)}>
                    <SettingsIcon color="secondary" />
                  </Link>
                )}
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
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            {rootData.team.organization && (
              <Link href={`/organization/${rootData.team.organization.uniqueId}`}>
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <OrganizationIcon />
                  <Typography variant="body1">{rootData.team.organization.name}</Typography>
                </Stack>
              </Link>
            )}
          </Stack>

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
            ignoreDiacritics
            disableRowSelectionOnClick
            density="compact"
            onCellClick={handleCellClick}
            slots={{ toolbar: QuickSearchToolbar }}
          />
        </CardContent>
      </Card>
      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <IconButton color={option.color}>{option.icon}</IconButton>
              <Typography variant="body1">{option.label}</Typography>
            </Stack>
          </MenuItem>
        ))}
      </Menu>

      <Dialog TransitionComponent={DialogTransition} open={teamRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
        <DialogTitle>Remove team</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.team.hasFutureBooking
              ? `Bookings are scheduled for the team "${teamName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the team "${teamName}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingTeamClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingTeamClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamPeopleBookings);
