import AvatarGroup from '@mui/material/AvatarGroup';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import LinearProgress from '@mui/material/LinearProgress';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  DefaultDialogTitle,
  GridContainer,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { EllipseMenuIcon } from '@repo/shared/components/icons';
import {
  MoreActionsMenu,
  moreActionsMenuAllOptions,
  MoreActionsMenuItemType,
  MoreActionsMenuOptionType,
} from '@repo/shared/components/moreActionsMenu';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors, startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { NewBookingButton } from 'components/booking/addBooking';
import { getModernOrganizationLocationSetupBaseLink } from 'components/organization';
import { nanoid } from 'nanoid';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import type { myLocations_deleteLocationMutation } from './__generated__/myLocations_deleteLocationMutation.graphql';
import type { myLocations_locations_availableOrganizationDesks_query$key } from './__generated__/myLocations_locations_availableOrganizationDesks_query.graphql';
import type { myLocations_locations_availableOrganizationDesks_refetchableFragment } from './__generated__/myLocations_locations_availableOrganizationDesks_refetchableFragment.graphql';
import type { myLocations_query$key } from './__generated__/myLocations_query.graphql';
import MyLocationCard from './my-location-card';

type Props = {
  rootDataRelay: myLocations_query$key;
  rootDataRefetchableRelay: myLocations_locations_availableOrganizationDesks_query$key;
  onReloadRequired: () => void;
  organizationId: string;
  deskTypeIds: string[];
  zoneIds: string[];
  viewMode: 'list' | 'grid';
};

type LocationDetails = {
  name: string;
};

type DesksAvailabilityDetails = {
  desksCount: number;
  availablePercentage: number;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

type RowType = {
  id: string;
  location: LocationDetails;
  desksCount: number;
  desksAvailability: DesksAvailabilityDetails;
  zones: ZoneDetails[];
  teammates: ReadonlyArray<CustomerDetails>;
};

const MyLocations = ({ rootDataRelay, rootDataRefetchableRelay, onReloadRequired, organizationId, deskTypeIds, zoneIds, viewMode }: Props) => {
  const rootData = useFragment<myLocations_query$key>(
    graphql`
      fragment myLocations_query on Query {
        organizationMembers(where: { organizationId: $organizationId }, orderBy: $organizationMembersSortingValues) {
          __id
          totalCount
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
      }
    `,
    rootDataRelay,
  );

  const [rootDataRefetchable, refetch] = useRefetchableFragment<
    myLocations_locations_availableOrganizationDesks_refetchableFragment,
    myLocations_locations_availableOrganizationDesks_query$key
  >(
    graphql`
      fragment myLocations_locations_availableOrganizationDesks_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "myLocations_locations_availableOrganizationDesks_refetchableFragment") {
        locations(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, zoneIds: $zoneIds, deskTypeIds: $deskTypeIds }
          orderBy: $locationsSortingValues
        ) @connection(key: "myLocations_locations") {
          __id
          totalCount
          edges {
            node {
              id
              name
              deskTypes {
                uniqueId
                name
              }
              zones {
                uniqueId
                name
              }
              desks {
                id
              }
              physicalAddress {
                formattedAddress
              }
              hasFutureBooking
              canModify
              canDelete
              organization {
                uniqueId
              }
              ...myLocationCard_LocationDetails
            }
          }
        }
        availableDesks(
          where: {
            organizationId: $organizationId
            date: $todayDate
            deskIdsToInclude: []
            zoneIds: $zoneIds
            deskTypeIds: $deskTypeIds
            combineDeskTypesZones: true
          }
        ) {
          location {
            uniqueId
          }
        }
      }
    `,
    rootDataRefetchableRelay,
  );

  const [commitDeleteLocation] = useMutation<myLocations_deleteLocationMutation>(graphql`
    mutation myLocations_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const navigate = useNavigate();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [defaultDate] = useState(startOfDay());
  const connectionIds = useMemo(() => (rootDataRefetchable.locations ? [rootDataRefetchable.locations.__id] : []), [rootDataRefetchable.locations]);
  const [selectedLocationId, setSelectedLocationId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocation],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocation],
  ];

  const locations = useMemo(() => {
    if (!rootDataRefetchable.locations) {
      return [];
    }

    return rootDataRefetchable.locations.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name));
  }, [rootDataRefetchable.locations]);

  const locationDetails = useMemo(() => locations.find((item) => item.id === selectedLocationId), [selectedLocationId, locations]);

  const organizationMembers = useMemo(() => {
    if (!rootData.organizationMembers) {
      return [];
    }

    return rootData.organizationMembers.edges.map((edge) => edge.node);
  }, [rootData.organizationMembers]);

  const handleRefetch = useCallback(
    (deskTypeIds: string[], zoneIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            deskTypeIds,
            zoneIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(deskTypeIds, zoneIds), [handleRefetch, deskTypeIds, zoneIds]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditLocation:
        if (locationDetails) {
          navigate(getModernOrganizationLocationSetupBaseLink(locationDetails.organization?.uniqueId!, locationDetails.id));
        }
        break;

      case MoreActionsMenuOptionType.DeleteLocation:
        handleRemoveLocationClicked();
        break;
    }
  };

  const handleRemoveLocationClicked = () => {
    setLocationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingLocationClick = () => {
    setLocationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingLocationClick = () => {
    if (!locationDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteLocation({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location '${locationDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been successfully removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location '${locationDetails.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const rows: RowType[] = locations.map((location) => {
    const desksCount = location.desks.length;
    const availableDesksCount = rootDataRefetchable.availableDesks
      ? rootDataRefetchable.availableDesks.filter((desk) => desk.location?.uniqueId === location.id).length
      : 0;
    const availablePercentage = (availableDesksCount / desksCount) * 100;
    const zones = location.zones.map(({ uniqueId, name }) => ({ id: uniqueId, name }));

    return {
      id: location.id,
      location,
      desksCount,
      desksAvailability: {
        desksCount,
        availablePercentage,
      },
      zones,
      teammates: organizationMembers.map(({ customer }) => customer),
      physicalAddress: location.physicalAddress?.formattedAddress,
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'location',
      headerName: 'Location',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value.name} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'desksCount',
      headerName: 'Desks count',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value.desksCount} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'desksAvailability',
      headerName: 'Availability',
      editable: false,
      renderCell: (params) => (
        <StackColumn sx={{ alignItems: 'flex-end' }}>
          <SmallIconTypography label={`${params.value.desksCount} Available Today`} />
          <LinearProgress value={params.value.availablePercentage} variant="determinate" sx={{ width: '100%' }} />
        </StackColumn>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value} hideIcon />,
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'teammates',
      headerName: 'Shared with teammates',
      editable: false,
      renderCell: (params) => (
        <AvatarGroup max={5}>
          {params.value.map((customer: CustomerDetails) => (
            <CustomerAvatar key={customer?.uniqueId} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
          ))}
        </AvatarGroup>
      ),
      display: 'flex',
      minWidth: 300,
    },
    {
      field: 'physicalAddress',
      headerName: 'Address',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value ? params.value : 'N/A'} sx={{ whiteSpace: 'pre-line' }} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'bookNow',
      headerName: '',
      editable: false,
      renderCell: (params) => (
        <NewBookingButton
          hideLocationControl={false}
          hideOrganizationControl={true}
          onReloadRequired={onReloadRequired}
          defaultDate={defaultDate}
          organizationId={organizationId}
          locationId={params.id as string}
          label="Book Now"
          hideIcon
          variant="contained"
          size="small"
        />
      ),
      display: 'flex',
      minWidth: 140,
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <IconButton
          onClick={(event: React.MouseEvent<HTMLElement>) => {
            setSelectedLocationId(params.id as string);
            setMoreActionsAnchorEl(event.currentTarget);
          }}
        >
          <EllipseMenuIcon />
        </IconButton>
      ),
    },
  ];

  if (!rootDataRefetchable.locations || !rootDataRefetchable.availableDesks || !rootData.organizationMembers) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
        <SectionIconTypography label="My Locations" />
        <Divider />
        <Box sx={{ paddingBottom: defaultPadding }} />

        {viewMode === 'grid' && (
          <GridContainer>
            {locations.map((location) => {
              const desksCount = location.desks.length;
              const availableDesksCount = rootDataRefetchable.availableDesks
                ? rootDataRefetchable.availableDesks.filter((desk) => desk.location?.uniqueId === location.id).length
                : 0;
              const availablePercentage = (availableDesksCount / desksCount) * 100;

              return (
                <Grid key={location.id}>
                  <MyLocationCard
                    locationDetailsRelay={location}
                    onReloadRequired={onReloadRequired}
                    organizationId={organizationId}
                    defaultDate={defaultDate}
                    connectionIds={connectionIds}
                    availableDesksCount={availableDesksCount}
                    availablePercentage={availablePercentage}
                    sharedWithTeammates={organizationMembers!.map(({ customer }) => customer)}
                  />
                </Grid>
              );
            })}
          </GridContainer>
        )}

        {viewMode === 'list' && (
          <DataGrid
            rows={rows}
            columns={columns}
            ignoreDiacritics
            disableRowSelectionOnClick
            hideFooter
            getRowHeight={() => 'auto'}
            rowSpacingType="margin"
            getRowSpacing={() => ({ top: 3, bottom: 3 })}
            sx={defaultGridStyle}
          />
        )}
      </StackColumn>

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />

      {locationDetails && (
        <Dialog TransitionComponent={DialogTransition} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
          <DefaultDialogTitle title="Remove Location" />
          <DialogContent>
            <DialogContentText>
              {locationDetails.hasFutureBooking
                ? `Bookings are scheduled for the location "${locationDetails.name}". Are you sure you want to remove it?`
                : `Are you sure you want to remove the location "${locationDetails.name}"?`}
            </DialogContentText>
            <TwoButtonsDialogActions
              onPrimaryClicked={handleConfirmRemovingLocationClick}
              onSecondaryClicked={handleCancelRemovingLocationClick}
              primaryLabel="Remove"
              secondaryLabel="Cancel"
            />
          </DialogContent>
        </Dialog>
      )}
    </>
  );
};

export default memo(MyLocations);
