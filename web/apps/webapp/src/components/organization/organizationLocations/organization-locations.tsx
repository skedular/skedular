import { NewBookingButton } from '@/components/booking/addBooking';
import { getOrganizationBookingsBaseLink, getOrganizationLocationSetupBaseLink } from '@/components/links';
import { NewLocationButton } from '@/components/location/addLocation';
import { CustomTagSelector } from '@/components/organization/customTagSelector';
import { ZoneSelector } from '@/components/organization/zoneSelector';
import type { organizationLocations_addCustomerDefaultLocationMutation } from '@/queries/__generated__/organizationLocations_addCustomerDefaultLocationMutation.graphql';
import type { organizationLocations_deleteLocationMutation } from '@/queries/__generated__/organizationLocations_deleteLocationMutation.graphql';
import type { organizationLocations_locations_availableOrganizationDesks_query$key } from '@/queries/__generated__/organizationLocations_locations_availableOrganizationDesks_query.graphql';
import type { organizationLocations_locations_availableOrganizationDesks_refetchableFragment } from '@/queries/__generated__/organizationLocations_locations_availableOrganizationDesks_refetchableFragment.graphql';
import type { organizationLocations_removeCustomerDefaultLocationMutation } from '@/queries/__generated__/organizationLocations_removeCustomerDefaultLocationMutation.graphql';
import type { organizationLocations_rootQuery } from '@/queries/__generated__/organizationLocations_rootQuery.graphql';
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
import { DefaultDialogTitle, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
import { ListGridToggle } from '@repo/shared/components/listGridToggle';
import { Loading } from '@repo/shared/components/loading';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@repo/shared/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { DialogTransition } from '@repo/shared/components/transitions';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultGridStyle, defaultPadding, maxScreenWidth } from '@repo/shared/libs/theme';
import { joinErrors, startOfDay } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import LocationCard from './location-card';

type Props = {
  queryReference: PreloadedQuery<organizationLocations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationLocations_rootQuery(
    $organizationId: String!
    $locationsSortingValues: [LocationOrderInput!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $todayDate: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $zoneIds: [String!]
    $customTagIds: [String!]
  ) {
    me {
      id
      defaultLocations {
        uniqueId
      }
    }
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
    organization(id: $organizationId) {
      canModify
    }
    ...locationCard_query
    ...customTagSelector_allCustomTags_query
    ...zoneSelector_allZones_query
    ...organizationLocations_locations_availableOrganizationDesks_query
  }
`;

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
  color: string | null | undefined;
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
  preferred: boolean;
};

const OrganizationLocations = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationLocations_rootQuery>(RootQuery, queryReference);
  const [rootDataRefetchable, refetch] = useRefetchableFragment<
    organizationLocations_locations_availableOrganizationDesks_refetchableFragment,
    organizationLocations_locations_availableOrganizationDesks_query$key
  >(
    graphql`
      fragment organizationLocations_locations_availableOrganizationDesks_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocations_locations_availableOrganizationDesks_refetchableFragment") {
        locations(first: $count, after: $cursor, where: { organizationId: $organizationId, zoneIds: $zoneIds, customTagIds: $customTagIds }, orderBy: $locationsSortingValues)
          @connection(key: "organizationLocations_locations") {
          __id
          totalCount
          edges {
            node {
              id
              name
              customTags {
                uniqueId
                name
                color
              }
              zones {
                uniqueId
                name
                color
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
              ...locationCard_LocationDetails
            }
          }
        }
        availableDesks(
          where: { organizationId: $organizationId, date: $todayDate, deskIdsToInclude: [], zoneIds: $zoneIds, customTagIds: $customTagIds, combineCustomTagsZones: true }
        ) {
          location {
            uniqueId
          }
        }
      }
    `,
    rootData,
  );

  const [commitDeleteLocation] = useMutation<organizationLocations_deleteLocationMutation>(graphql`
    mutation organizationLocations_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocation] = useMutation<organizationLocations_addCustomerDefaultLocationMutation>(graphql`
    mutation organizationLocations_addCustomerDefaultLocationMutation($input: AddCustomerDefaultLocationInput!) {
      addCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultLocation] = useMutation<organizationLocations_removeCustomerDefaultLocationMutation>(graphql`
    mutation organizationLocations_removeCustomerDefaultLocationMutation($input: RemoveCustomerDefaultLocationInput!) {
      removeCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const [customTagIds, setCustomTagIds] = useState<string[]>([]);
  const [zoneIds, setZoneIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [defaultDate] = useState(startOfDay());
  const connectionIds = useMemo(() => (rootDataRefetchable.locations ? [rootDataRefetchable.locations.__id] : []), [rootDataRefetchable.locations]);
  const [selectedLocationId, setSelectedLocationId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const [preferredLocations, setPreferredLocations] = useState(rootData.me?.defaultLocations.map(({ uniqueId }) => uniqueId) ?? []);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocation],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocation],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ViewLocationBookings],
  ];

  const locations = useMemo(
    () => (rootDataRefetchable.locations ? rootDataRefetchable.locations.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name)) : []),
    [rootDataRefetchable.locations],
  );
  const locationDetails = useMemo(() => locations.find((item) => item.id === selectedLocationId), [selectedLocationId, locations]);

  const organizationMembers = useMemo(() => {
    if (!rootData.organizationMembers) {
      return [];
    }

    return rootData.organizationMembers.edges.map((edge) => edge.node);
  }, [rootData.organizationMembers]);

  const handleRefetch = useCallback(
    (customTagIds: string[], zoneIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            customTagIds,
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

  useEffect(() => handleRefetch(customTagIds, zoneIds), [handleRefetch, customTagIds, zoneIds]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditLocation:
        if (!locationDetails) {
          return;
        }

        router.push(getOrganizationLocationSetupBaseLink(locationDetails.organization?.uniqueId!, locationDetails.id));
        break;

      case MoreActionsMenuOptionType.DeleteLocation:
        handleRemoveLocationClicked();
        break;

      case MoreActionsMenuOptionType.ViewLocationBookings:
        if (!locationDetails) {
          return;
        }

        router.push(getOrganizationBookingsBaseLink(locationDetails.organization?.uniqueId!, { locationId: locationDetails.id }));
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

  const handleSetAsPreferredLocationClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const locationDetails = locations.find((item) => item.id === id);
    if (!locationDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting location '${locationDetails.name}' as your preferred location...`} />, infoNotificationOptions);

    commitAddCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been set as the preferred location.`} />,
        });

        setPreferredLocations(preferredLocations.concat([locationDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredLocationClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const locationDetails = locations.find((item) => item.id === id);
    if (!locationDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}' as your preferred location...`} />, infoNotificationOptions);

    commitRemoveCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationDetails.name}' has been removed as your preferred location.`} />,
        });

        setPreferredLocations(preferredLocations.filter((item) => item !== locationDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCustomTagChanged = (id?: string) => {
    setCustomTagIds(id ? [id] : []);
  };

  const handleZoneTypeChanged = (id?: string) => {
    setZoneIds(id ? [id] : []);
  };

  const handlViewModeChanged = (newViewMode: 'list' | 'grid') => {
    setViewMode(newViewMode);
  };

  const rows: RowType[] = locations.map((location) => {
    const desksCount = location.desks.length;
    const availableDesksCount = rootDataRefetchable.availableDesks ? rootDataRefetchable.availableDesks.filter((desk) => desk.location?.uniqueId === location.id).length : 0;
    const availablePercentage = (availableDesksCount / desksCount) * 100;
    const zones = location.zones.map(({ uniqueId, name, color }) => ({ id: uniqueId, name, color }));

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
      preferred: preferredLocations.includes(location.id),
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
      headerName: 'Desks Count',
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
          onReloadRequired={onReloadRequired}
          defaultDate={defaultDate}
          organizationId={organizationId}
          defaultLocationId={params.id as string}
          label="Book Now"
          hideIcon
          variant="contained"
          size="small"
          sx={{ textTransform: 'none' }}
          invertDefaultColor={paletteMode === 'dark'}
        />
      ),
      display: 'flex',
      minWidth: 140,
    },
    {
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredLocationClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredLocationClicked(id)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedLocationId(params.id as string);
              setMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  if (!rootDataRefetchable.locations || !rootDataRefetchable.availableDesks || !rootData.organizationMembers) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
          <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
          <CustomTagSelector rootDataRelay={rootData} onChange={handleCustomTagChanged} />
          <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
          <PushToRight />
          {rootData.organization?.canModify && <NewLocationButton organizationId={organizationId} />}
        </GridContainer>
        <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
          <SectionIconTypography label="Locations" />
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
                    <LocationCard
                      rootDataRelay={rootData}
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
              localeText={{ noRowsLabel: 'No location found' }}
            />
          )}
        </StackColumn>
      </StackColumn>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      {locationDetails && (
        <Dialog slots={{ transition: DialogTransition }} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
          <DefaultDialogTitle title="Remove Location" />
          <DialogContent sx={{ marginTop: 2 }}>
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

const MemoOrganizationLocations = memo(OrganizationLocations);

type RelayProps = {
  organizationId: string;
};

const OrganizationLocationsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationLocations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationId,
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        zonesSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        customTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        todayDate: today.toISOString(),
        organizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoOrganizationLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationLocationsWithRelay);
