import { CustomerAvatar } from '@/components/avatars';
import { NewBookingButton } from '@/components/booking/addBooking';
import {
  BodyIconTypography,
  DefaultDialogTitle,
  GridContainer,
  PushToRight,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  TwoButtonsDialogActions,
} from '@/components/commons';
import StackRow from '@/components/commons/stack-row';
import { EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationLocationSetupBaseLink } from '@/components/links';
import { ListGridToggle } from '@/components/listGridToggle';
import { Loading } from '@/components/loading';
import { ClaimLocationOwnershipButton } from '@/components/location';
import { NewLocationButton } from '@/components/location/addLocation';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { CustomTagSelector } from '@/components/organization/customTagSelector';
import { ZoneSelector } from '@/components/organization/zoneSelector';
import { RelayError, toRootError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultGridStyle, defaultPadding, maxScreenWidth } from '@/libs/theme';
import { joinErrors, startOfDay } from '@/libs/utils';
import type { organizationLocations_addCustomerPreferredLocationMutation } from '@/queries/__generated__/organizationLocations_addCustomerPreferredLocationMutation.graphql';
import type { organizationLocations_deleteLocationMutation } from '@/queries/__generated__/organizationLocations_deleteLocationMutation.graphql';
import type { organizationLocations_locations_availableOrganizationResources_query$key } from '@/queries/__generated__/organizationLocations_locations_availableOrganizationResources_query.graphql';
import type { organizationLocations_locations_availableOrganizationResources_refetchableFragment } from '@/queries/__generated__/organizationLocations_locations_availableOrganizationResources_refetchableFragment.graphql';
import type { organizationLocations_removeCustomerPreferredLocationMutation } from '@/queries/__generated__/organizationLocations_removeCustomerPreferredLocationMutation.graphql';
import type { organizationLocations_rootQuery } from '@/queries/__generated__/organizationLocations_rootQuery.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import LinearProgress from '@mui/material/LinearProgress';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import LocationCard from './location-card';

type Props = {
  queryReference: PreloadedQuery<organizationLocations_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationLocations_rootQuery(
    $organizationCustomDomain: String!
    $locationsSortingValues: [LocationOrderInput!]
    $zonesSortingValues: [OrganizationTagOrderInput!]
    $customTagsSortingValues: [OrganizationTagOrderInput!]
    $fromTodayDate: DateTime!
    $untilTodayDate: DateTime!
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $zoneIds: [String!]
    $customTagIds: [String!]
    $locationNotContactedYet: Boolean!
  ) {
    me {
      id
      preferredLocations {
        id
      }
    }
    organization(customDomain: $organizationCustomDomain) {
      members(orderBy: $organizationMembersSortingValues) {
        __id
        totalCount
        edges {
          node {
            id
            customer {
              id
              name
              givenName
              middleName
              familyName
              photoUrl
            }
          }
        }
      }
      canModify
      customDomain
    }
    ...newLocationButton_query
    ...locationCard_query
    ...customTagSelector_allCustomTags_query
    ...zoneSelector_allZones_query
    ...organizationLocations_locations_availableOrganizationResources_query
  }
`;

type LocationDetails = {
  name: string;
};

type ResourcesAvailabilityDetails = {
  resourcesCount: number;
  availablePercentage: number;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color: string | null | undefined;
};

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

type RowType = {
  id: string;
  location: LocationDetails;
  resourcesCount: number;
  resourcesAvailability: ResourcesAvailabilityDetails;
  zones: ZoneDetails[];
  teammates: ReadonlyArray<CustomerDetails>;
  preferred: boolean;
};

const OrganizationLocations = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationLocations_rootQuery>(RootQuery, queryReference);
  const [rootDataRefetchable, refetch] = useRefetchableFragment<
    organizationLocations_locations_availableOrganizationResources_refetchableFragment,
    organizationLocations_locations_availableOrganizationResources_query$key
  >(
    graphql`
      fragment organizationLocations_locations_availableOrganizationResources_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationLocations_locations_availableOrganizationResources_refetchableFragment") {
        locations(
          first: $count
          after: $cursor
          where: { organizationCustomDomain: $organizationCustomDomain, zoneIds: $zoneIds, customTagIds: $customTagIds, notContactedYet: $locationNotContactedYet }
          orderBy: $locationsSortingValues
        ) @connection(key: "organizationLocations_locations") {
          __id
          totalCount
          edges {
            node {
              id
              name
              customTags {
                id
                name
                color
              }
              zones {
                id
                name
                color
              }
              resources {
                totalCount
              }
              physicalAddress {
                formattedAddress
                longitude
                latitude
              }
              hasFutureBooking
              canModify
              canDelete
              organization {
                customDomain
              }
              extraMetadata {
                contactDetails {
                  contactEmails
                  contactPhones
                }
              }
              ...locationCard_LocationDetails
            }
          }
        }
        availableResources(
          where: { organizationCustomDomain: $organizationCustomDomain, from: $fromTodayDate, until: $untilTodayDate, zoneIds: $zoneIds, customTagIds: $customTagIds }
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

  const [commitAddCustomerPreferredLocation] = useMutation<organizationLocations_addCustomerPreferredLocationMutation>(graphql`
    mutation organizationLocations_addCustomerPreferredLocationMutation($input: AddCustomerPreferredLocationInput!) {
      addCustomerPreferredLocation(input: $input) {
        customer {
          id
          preferredLocations {
            id
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredLocation] = useMutation<organizationLocations_removeCustomerPreferredLocationMutation>(graphql`
    mutation organizationLocations_removeCustomerPreferredLocationMutation($input: RemoveCustomerPreferredLocationInput!) {
      removeCustomerPreferredLocation(input: $input) {
        customer {
          id
          preferredLocations {
            id
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const [customTagIds, setCustomTagIds] = useState<string[]>([]);
  const [zoneIds, setZoneIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [defaultDate] = useState(startOfDay());
  const connectionIds = useMemo(() => [rootDataRefetchable.locations.__id], [rootDataRefetchable.locations]);
  const [selectedLocationId, setSelectedLocationId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const [preferredLocations, setPreferredLocations] = useState(rootData.me?.preferredLocations.map(({ id }) => id) ?? []);
  const [filterThoseWithoutCoordites, setFilterThoseWithoutCoordites] = useState(organizationCustomDomain === 'skedularpubliclocations');
  const [filterThoseWithCoordites, setFilterThoseWithCoordites] = useState(false);
  const [filterThoseWithEmails, setFilterThoseWithEmails] = useState(false);
  const [filterThoseWithPhones, setFilterThoseWithPhones] = useState(false);
  const [locationNotContactedYet, setLocationNotContactedYet] = useState(false);
  const [phoneStartWith, setPhoneStartWith] = useState('');

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditLocation],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteLocation],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ViewLocationBookings],
  ];

  const locations = useMemo(
    () =>
      rootDataRefetchable.locations.edges
        .map((edge) => edge.node)
        .filter((item) => !filterThoseWithoutCoordites || !item.physicalAddress?.latitude || !item.physicalAddress?.longitude)
        .filter((item) => !filterThoseWithCoordites || (item.physicalAddress?.latitude && item.physicalAddress?.longitude))
        .filter((item) => !filterThoseWithEmails || item.extraMetadata?.contactDetails?.contactEmails?.length !== 0)
        .filter((item) => !filterThoseWithPhones || item.extraMetadata?.contactDetails?.contactPhones?.length !== 0)
        .filter(
          (item) =>
            !phoneStartWith ||
            item.extraMetadata?.contactDetails?.contactPhones?.some((phone) => {
              const sanitizedFilter = phoneStartWith.replace(/[^\d+]/g, '');
              const sanitizedPhone = (phone ?? '').replace(/[^\d+]/g, '');

              return sanitizedPhone.startsWith(sanitizedFilter);
            }),
        ),
    [rootDataRefetchable.locations, filterThoseWithoutCoordites, filterThoseWithCoordites, filterThoseWithEmails, filterThoseWithPhones, phoneStartWith],
  );
  const locationDetails = useMemo(() => locations.find((item) => item.id === selectedLocationId), [selectedLocationId, locations]);
  const organizationMembers = useMemo(() => (rootData.organization ? rootData.organization.members.edges.map((edge) => edge.node) : []), [rootData.organization]);

  const handleRefetch = useCallback(
    (customTagIds: string[], zoneIds: string[], locationNotContactedYet: boolean) => {
      startTransition(() => {
        refetch(
          {
            customTagIds,
            zoneIds,
            locationNotContactedYet,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(customTagIds, zoneIds, locationNotContactedYet), [handleRefetch, customTagIds, zoneIds, locationNotContactedYet]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditLocation:
        if (!locationDetails) {
          return;
        }

        router.push(getOrganizationLocationSetupBaseLink(integratedPlatrform, locationDetails.organization!.customDomain!, locationDetails.id));
        break;

      case MoreActionsMenuOptionType.DeleteLocation:
        handleRemoveLocationClicked();
        break;

      case MoreActionsMenuOptionType.ViewLocationBookings:
        if (!locationDetails) {
          return;
        }

        router.push(getOrganizationBookingsBaseLink(integratedPlatrform, locationDetails.organization!.customDomain!, { locationId: locationDetails.id }));
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
          clientMutationId: uuid(),
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
    const locationDetails = locations.find((item) => item.id === id);
    if (!locationDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting location '${locationDetails.name}' as your preferred location...`} />, infoNotificationOptions);

    commitAddCustomerPreferredLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
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
    const locationDetails = locations.find((item) => item.id === id);
    if (!locationDetails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing location '${locationDetails.name}' as your preferred location...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredLocation({
      variables: {
        input: {
          clientMutationId: uuid(),
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
    const resourcesCount = location.resources.totalCount;
    const availableResourcesCount = rootDataRefetchable.availableResources
      ? rootDataRefetchable.availableResources.filter((resources) => resources.location?.uniqueId === location.id).length
      : 0;
    const availablePercentage = (availableResourcesCount / resourcesCount) * 100;
    const zones = location.zones.map(({ id, name, color }) => ({ id, name, color }));

    return {
      id: location.id,
      location,
      resourcesCount,
      resourcesAvailability: {
        resourcesCount,
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
      field: 'resourcesCount',
      headerName: 'Resources Count',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value.resourcesCount} />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'resourcesAvailability',
      headerName: 'Availability',
      editable: false,
      renderCell: (params) => (
        <StackColumn sx={{ alignItems: 'flex-end' }}>
          <SmallIconTypography label={`${params.value.resourcesCount} Available Today`} />
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
            <CustomerAvatar key={customer?.id} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
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
      renderCell: (params) => {
        if (rootData.organization?.customDomain === 'skedularpubliclocations') {
          return null;
        }

        return (
          <NewBookingButton
            onReloadRequired={onReloadRequired}
            defaultDate={defaultDate}
            organizationCustomDomain={organizationCustomDomain}
            defaultLocationId={params.id as string}
            label="Book Now"
            hideIcon
            variant="contained"
            size="small"
            sx={{ textTransform: 'none' }}
            invertDefaultColor={paletteMode === 'dark'}
          />
        );
      },
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
      field: 'More Actions',
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

  const handleFilterThoseWithoutCoorditesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilterThoseWithoutCoordites(event.target.checked);
  };

  const handleFilterThoseWithCoorditesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilterThoseWithCoordites(event.target.checked);
  };

  const handleFilterThoseWithEmailsChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilterThoseWithEmails(event.target.checked);
  };

  const handleFilterThoseWithPhonesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFilterThoseWithPhones(event.target.checked);
  };

  const handleLocationNotContactedYetChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setLocationNotContactedYet(event.target.checked);
  };

  const handlePhoneFilterChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setPhoneStartWith(event.target.value);
  };

  if (!rootDataRefetchable.locations || !rootDataRefetchable.availableResources || !rootData.organization) {
    return null;
  }

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
          <ZoneSelector rootDataRelay={rootData} onChange={handleZoneTypeChanged} />
          <CustomTagSelector rootDataRelay={rootData} onChange={handleCustomTagChanged} />
          <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
          <PushToRight />
          {rootData.organization?.canModify && <NewLocationButton rootDataRelay={rootData} organizationCustomDomain={organizationCustomDomain} />}
          {rootData.organization?.canModify && <ClaimLocationOwnershipButton organizationCustomDomain={organizationCustomDomain} connectionIds={connectionIds} />}
        </GridContainer>
        <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
          {organizationCustomDomain === 'skedularpubliclocations' && (
            <StackRow>
              <BodyIconTypography label="Filter those without address" />
              <Switch defaultChecked={filterThoseWithoutCoordites} onChange={handleFilterThoseWithoutCoorditesChange} />

              <BodyIconTypography label="Filter those with address" />
              <Switch defaultChecked={filterThoseWithCoordites} onChange={handleFilterThoseWithCoorditesChange} />

              <BodyIconTypography label="Filter those with emails" />
              <Switch defaultChecked={filterThoseWithEmails} onChange={handleFilterThoseWithEmailsChange} />

              <BodyIconTypography label="Filter those with phones" />
              <Switch defaultChecked={filterThoseWithPhones} onChange={handleFilterThoseWithPhonesChange} />

              <BodyIconTypography label="Filter those not contacted yet" />
              <Switch defaultChecked={locationNotContactedYet} onChange={handleLocationNotContactedYetChange} />

              <BodyIconTypography label="Phone starts with" />
              <TextField defaultValue={phoneStartWith} onChange={handlePhoneFilterChange} />
            </StackRow>
          )}
          <SectionIconTypography label="Locations" />
          <Divider />
          <Box sx={{ paddingBottom: defaultPadding }} />

          {viewMode === 'grid' && (
            <GridContainer sx={{ alignItems: 'stretch' }}>
              {locations.map((location) => {
                const resourcesCount = location.resources.totalCount;
                const availableResourcesCount = rootDataRefetchable.availableResources
                  ? rootDataRefetchable.availableResources.filter((resources) => resources.location?.uniqueId === location.id).length
                  : 0;
                const availablePercentage = (availableResourcesCount / resourcesCount) * 100;

                return (
                  <Grid key={location.id}>
                    <LocationCard
                      rootDataRelay={rootData}
                      locationDetailsRelay={location}
                      onReloadRequired={onReloadRequired}
                      organizationCustomDomain={organizationCustomDomain}
                      defaultDate={defaultDate}
                      connectionIds={connectionIds}
                      availableResourcesCount={availableResourcesCount}
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
  organizationCustomDomain: string;
};

const OrganizationLocationsWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationLocations_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationCustomDomain,
        locationsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        zonesSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        customTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        fromTodayDate: today.toISOString(),
        untilTodayDate: today.add(1, 'day').toISOString(),
        organizationMembersSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        locationNotContactedYet: false,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationLocations queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationLocationsWithRelay);
