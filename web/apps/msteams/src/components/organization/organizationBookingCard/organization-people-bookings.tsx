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
import { BookingIcon, DangerIcon, DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon, SettingsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfIsoWeek, getCustomerFullName, joinErrors, startOfIsoWeek, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { Dayjs } from 'dayjs';
import { GlobalReloadIdContext, UpdateGlobalReloadIdContext } from 'libs/providers';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import type { organizationPeopleBookings_addBookingMutation } from './__generated__/organizationPeopleBookings_addBookingMutation.graphql';
import type { organizationPeopleBookings_allBookings_query$key } from './__generated__/organizationPeopleBookings_allBookings_query.graphql';
import type { organizationPeopleBookings_clearCustomerDefaultOrganizationMutation } from './__generated__/organizationPeopleBookings_clearCustomerDefaultOrganizationMutation.graphql';
import type { organizationPeopleBookings_deleteBookingMutation } from './__generated__/organizationPeopleBookings_deleteBookingMutation.graphql';
import type { organizationPeopleBookings_deleteOrganizationMutation } from './__generated__/organizationPeopleBookings_deleteOrganizationMutation.graphql';
import type { organizationPeopleBookings_query$key } from './__generated__/organizationPeopleBookings_query.graphql';
import type { organizationPeopleBookings_setCustomerDefaultOrganizationMutation } from './__generated__/organizationPeopleBookings_setCustomerDefaultOrganizationMutation.graphql';

type Props = {
  rootDataRelay: organizationPeopleBookings_query$key;
  rootDataAllBookingsRelay: organizationPeopleBookings_allBookings_query$key;
  organizationId: string;
  organizationName?: string;
  organizationsConnectionIds: string[];
  hideRemoveOrganizationOption?: boolean;
};

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

enum MoreActionsMenuOptionType {
  MarkAsDefaultOrganization,
  ClearAsPreferredOrganization,
  RemoveOrganization,
}

type MoreActionsMenuItemType = {
  id: MoreActionsMenuOptionType;
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
};

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.MarkAsDefaultOrganization]: {
    id: MoreActionsMenuOptionType.MarkAsDefaultOrganization,
    label: 'Mark as default organization',
    icon: <NotPreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.ClearAsPreferredOrganization]: {
    id: MoreActionsMenuOptionType.ClearAsPreferredOrganization,
    label: 'Clear as default organization',
    icon: <PreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveOrganization]: {
    id: MoreActionsMenuOptionType.RemoveOrganization,
    label: 'Remove organization',
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

const OrganizationPeopleBookings = ({
  rootDataRelay,
  rootDataAllBookingsRelay,
  organizationId,
  organizationName,
  organizationsConnectionIds,
  hideRemoveOrganizationOption,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationPeopleBookings_query on Query {
        organizationMembers(where: { organizationId: $organizationId }, orderBy: $peopleSortingValues) {
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
          defaultOrganization {
            uniqueId
          }
        }
        organization(id: $organizationId) {
          name
          logoUrl
          hasFutureBooking
          canModify
          canDelete
        }
        organizationBookingPermissions(organizationId: $organizationId) {
          canAddBookingOnBehalf
          canDeleteBookingOnBehalf
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataAllBookings, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationPeopleBookings_allBookings_query on Query
      @refetchable(queryName: "organizationPeopleBookings_allBookings_refetchableFragment") {
        allBookings(where: { organizationIds: [$organizationId], fromGTE: $from, toLT: $to }) {
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
    rootDataAllBookingsRelay,
  );

  const [commitAddBooking] = useMutation<organizationPeopleBookings_addBookingMutation>(graphql`
    mutation organizationPeopleBookings_addBookingMutation($input: AddBookingInput!) {
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

  const [commitDeleteBooking] = useMutation<organizationPeopleBookings_deleteBookingMutation>(graphql`
    mutation organizationPeopleBookings_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteOrganization] = useMutation<organizationPeopleBookings_deleteOrganizationMutation>(graphql`
    mutation organizationPeopleBookings_deleteOrganizationMutation($connectionIds: [ID!]!, $input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitSetCustomerDefaultOrganization] = useMutation<organizationPeopleBookings_setCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationPeopleBookings_setCustomerDefaultOrganizationMutation($input: SetCustomerDefaultOrganizationInput!) {
      setCustomerDefaultOrganization(input: $input) {
        customer {
          id
          defaultOrganization {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitClearCustomerDefaultOrganization] = useMutation<organizationPeopleBookings_clearCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationPeopleBookings_clearCustomerDefaultOrganizationMutation($input: ClearCustomerDefaultOrganizationInput!) {
      clearCustomerDefaultOrganization(input: $input) {
        customer {
          id
          defaultOrganization {
            uniqueId
          }
        }
      }
    }
  `);

  const globalReloadId = useContext(GlobalReloadIdContext);
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const { enqueueSnackbar } = useSnackbar();
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const [, startTransition] = useTransition();
  const [organizationRemoveConfirmationDialogOpen, setOrganizationRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfIsoWeek());
  const handleRefetch = useCallback(
    (startDate: Dayjs) => {
      startTransition(() => {
        const endDate = endOfIsoWeek(startDate);

        refetch(
          {
            from: startDate.toISOString(),
            to: endDate.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => {
    handleRefetch(startDate);
  }, [handleRefetch, globalReloadId, startDate]);

  if (!rootData.me || !rootData.organization || !rootData.organizationMembers) {
    return <></>;
  }

  const allMembers = rootData.organizationMembers.map((member) => member.customer);
  const meAsMember = allMembers.find((customer) => customer.uniqueId === rootData.me!.id);
  const otherMembers = allMembers.filter((customer) => customer.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList
    .map((customer) => {
      if (!rootDataAllBookings.allBookings) {
        return null;
      }

      const customerId = customer.uniqueId;

      return {
        id: customerId,
        person: customer,
        mon: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.toISOString(),
          ),
        },
        tue: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(1, 'day').toISOString(),
          ),
        },
        wed: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(2, 'day').toISOString(),
          ),
        },
        thu: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(3, 'day').toISOString(),
          ),
        },
        fri: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(4, 'day').toISOString(),
          ),
        },
        sat: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(5, 'day').toISOString(),
          ),
        },
        sun: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
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
    if (!booking && !rootData.organizationBookingPermissions?.canAddBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      enqueueSnackbar(`You are not authorized to make a booking on behalf of someone else`, {
        variant: 'error',
        anchorOrigin,
      });

      return;
    }

    if (booking && !rootData.organizationBookingPermissions?.canDeleteBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
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

            return;
          }

          let message = `Booking removed for ${getCustomerFullName(booking.customer)}`;

          if (booking.location) {
            message += ` at the "${booking.location!.name}"`;
          }

          message += ` on ${toShortDate(booking.from)}`;

          handleRefetch(startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
          UpdateGlobalReloadId();
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
            deskIds: [],
          },
        },
        onCompleted: (response, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });

            return;
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
          UpdateGlobalReloadId();
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
    let start = startOfIsoWeek();
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);

    handleRefetch(start);
  };

  const rowCount = rootData.organizationMembers.length;

  let moreActionsOption: MoreActionsMenuItemType[] = [];
  if (rootData.me.defaultOrganization?.uniqueId === organizationId) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.ClearAsPreferredOrganization]);
  } else {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.MarkAsDefaultOrganization]);
  }

  if (rootData.organization.canDelete && !hideRemoveOrganizationOption) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveOrganization]);
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.MarkAsDefaultOrganization:
        handleMarkAsDefaultOrganizationClicked();
        break;

      case MoreActionsMenuOptionType.ClearAsPreferredOrganization:
        handleClearAsDefaultOrganizationClicked();
        break;

      case MoreActionsMenuOptionType.RemoveOrganization:
        handleRemoveOrganizationClicked();
        break;
    }
  };

  const handleMarkAsDefaultOrganizationClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitSetCustomerDefaultOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to mark '${organizationName}' as your default organization. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`'${organizationName}' is now your default organization.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to mark '${organizationName}' as your default organization. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        setCustomerDefaultOrganization: {
          customer: {
            id: rootData.me.id,
            defaultOrganizations: { uniqueId: organizationId },
          },
        },
      },
    });
  };

  const handleClearAsDefaultOrganizationClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitClearCustomerDefaultOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to clear '${organizationName}' as your default organization. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`'${organizationName}' is no longer set as your default organization.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to clear '${organizationName}' as your default organization. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        setCustomerDefaultOrganization: {
          customer: {
            id: rootData.me.id,
            defaultOrganization: null,
          },
        },
      },
    });
  };

  const handleRemoveOrganizationClicked = () => {
    setOrganizationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingOrganizationClick = () => {
    setOrganizationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingOrganizationClick = () => {
    if (!rootData.me) {
      return;
    }

    commitDeleteOrganization({
      variables: {
        connectionIds: organizationsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          id: organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove organization '${organizationName}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`Organization '${organizationName}' has been successfully removed.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove organization '${organizationName}'. Error: ${error.message}`, {
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
          title={<OrganizationLink id={organizationId} name={rootData.organization?.name} />}
          subheader={
            <Stack direction="row" sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <Link href={`/organization/${organizationId}?tab=bookings`}>
                  <BookingIcon />
                </Link>

                {rootData.organization.canModify && (
                  <Link href={`/organization/${organizationId}?tab=about`}>
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

      <Dialog TransitionComponent={DialogTransition} open={organizationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingOrganizationClick}>
        <DialogTitle>Remove organization</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.organization.hasFutureBooking
              ? `Bookings are scheduled for the organization "${organizationName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the organization "${organizationName}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingOrganizationClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingOrganizationClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(OrganizationPeopleBookings);
