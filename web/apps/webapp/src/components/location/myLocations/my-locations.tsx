import type { myLocations_locations_availableOrganizationDesks_query$key } from '@/queries/__generated__/myLocations_locations_availableOrganizationDesks_query.graphql';
import type { myLocations_locations_availableOrganizationDesks_refetchableFragment } from '@/queries/__generated__/myLocations_locations_availableOrganizationDesks_refetchableFragment.graphql';
import type { myLocations_query$key } from '@/queries/__generated__/myLocations_query.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import LinearProgress from '@mui/material/LinearProgress';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { DeskIcon, LocationIcon, ZoneIcon } from '@repo/shared/components/icons';
import { LOCATION_TAG_TYPE_LOCATION_ZONE, Zones } from '@repo/shared/components/zone';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: myLocations_query$key;
  rootDataRefetchableRelay: myLocations_locations_availableOrganizationDesks_query$key;
  onReloadRequired: () => void;
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
  name: string;
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

const MyLocations = ({ rootDataRelay, rootDataRefetchableRelay, onReloadRequired, deskTypeIds, zoneIds, viewMode }: Props) => {
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
      @refetchable(queryName: "myLocations_locations_availableOrganizationDesks_refetchableFragment") {
        locations(where: { organizationId: $organizationId, zoneIds: $zoneIds, deskTypeIds: $deskTypeIds }, orderBy: $locationsSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
              name
              desks {
                id
              }
              locationTags {
                id
                name
                tagType
              }
              physicalAddress {
                formattedAddress
              }
            }
          }
        }
        availableOrganizationDesks(organizationId: $organizationId, date: $todayDate, deskIdsToInclude: []) {
          location {
            uniqueId
          }
        }
      }
    `,
    rootDataRefetchableRelay,
  );

  const locations = useMemo(() => {
    if (!rootDataRefetchable.locations) {
      return [];
    }

    return rootDataRefetchable.locations.edges.map((edge) => edge.node);
  }, [rootDataRefetchable.locations]);

  const organizationMembers = useMemo(() => {
    if (!rootData.organizationMembers) {
      return [];
    }

    return rootData.organizationMembers.edges.map((edge) => edge.node);
  }, [rootData.organizationMembers]);

  const handleRefetchAllBookings = useCallback(
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

  useEffect(() => handleRefetchAllBookings(deskTypeIds, zoneIds), [handleRefetchAllBookings, deskTypeIds, zoneIds]);

  const rows: RowType[] = locations.map((location) => {
    const desksCount = location.desks.length;
    const availableDesksCount = rootDataRefetchable.availableOrganizationDesks
      ? rootDataRefetchable.availableOrganizationDesks.filter((desk) => desk.location?.uniqueId === location.id).length
      : 0;
    const availablePercentage = (availableDesksCount / desksCount) * 100;
    const zones = location.locationTags.filter(({ tagType }) => tagType === LOCATION_TAG_TYPE_LOCATION_ZONE);

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
      renderCell: (params) => params.value.name,
      display: 'text',
      minWidth: 200,
    },
    {
      field: 'desksCount',
      headerName: 'Desks count',
      editable: false,
      renderCell: (params) => params.value.desksCount,
      display: 'text',
      minWidth: 150,
    },
    {
      field: 'desksAvailability',
      headerName: 'Availability',
      editable: false,
      renderCell: (params) => (
        <Stack direction="column" sx={{ alignItems: 'flex-end' }}>
          <Typography variant="body2">{`${params.value.desksCount} Available Today`}</Typography>
          <LinearProgress value={params.value.availablePercentage} variant="determinate" sx={{ width: '100%' }} />
        </Stack>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value} />,
      display: 'flex',
      minWidth: 300,
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
      renderCell: (params) => (
        <Typography variant="body1" sx={{ whiteSpace: 'pre-line' }}>
          {params.value ? params.value : 'N/A'}
        </Typography>
      ),
      display: 'flex',
      minWidth: 200,
    },
  ];

  if (!rootDataRefetchable.locations || !rootDataRefetchable.availableOrganizationDesks || !rootData.organizationMembers) {
    return <></>;
  }

  return (
    <Stack
      direction="column"
      spacing={1}
      sx={{
        paddingLeft: defaultPadding,
        paddingRight: defaultPadding,
        paddingTop: defaultPadding,
      }}
    >
      <Typography variant="h5">My Locations</Typography>

      <Divider />

      {viewMode === 'grid' && (
        <Grid container spacing={defaultSpacing} sx={{ alignItems: 'flex-start' }}>
          {locations.map((location) => {
            const desksCount = location.desks.length;
            const availableDesksCount = rootDataRefetchable.availableOrganizationDesks
              ? rootDataRefetchable.availableOrganizationDesks.filter((desk) => desk.location?.uniqueId === location.id).length
              : 0;
            const availablePercentage = (availableDesksCount / desksCount) * 100;
            const zones = location.locationTags.filter(({ tagType }) => tagType === LOCATION_TAG_TYPE_LOCATION_ZONE);

            return (
              <Grid key={location.id}>
                <Card sx={{ width: 600 }}>
                  <CardHeader
                    title={
                      <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                        <LocationIcon fontSize="medium" />
                        <Typography variant="h6">{location.name}</Typography>
                      </Stack>
                    }
                  />
                  <CardContent>
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1, width: '100%' }}>
                      <DeskIcon fontSize="medium" />
                      <Typography variant="body1" sx={{ flexGrow: 0, flexShrink: 0 }}>{`${desksCount} Desks`}</Typography>

                      <Stack direction="column" sx={{ paddingLeft: 20, alignItems: 'flex-end', width: '100%' }}>
                        <Typography variant="body2">{`${availableDesksCount} Available Today`}</Typography>
                        <LinearProgress value={availablePercentage} variant="determinate" sx={{ width: '100%' }} />
                      </Stack>
                    </Stack>

                    <Divider />

                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1 }}>
                      <ZoneIcon fontSize="medium" />
                      <Zones zones={zones} />
                    </Stack>

                    <Divider />

                    <Stack direction="row" spacing={1}>
                      <Stack direction="column" spacing={1}>
                        <Typography variant="body1">Shared with teammates</Typography>
                        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                          <AvatarGroup max={5}>
                            {organizationMembers.map(({ customer }) => (
                              <CustomerAvatar
                                key={customer?.uniqueId}
                                name={customer}
                                photo={{ url: customer?.photoUrl }}
                                size="medium"
                                showFullName
                              />
                            ))}
                          </AvatarGroup>
                        </Stack>
                      </Stack>

                      <Divider orientation="vertical" flexItem />

                      <Stack direction="column" spacing={1}>
                        <Typography variant="body1" sx={{ whiteSpace: 'pre-line' }}>
                          {location.physicalAddress?.formattedAddress ? location.physicalAddress?.formattedAddress : 'N/A'}
                        </Typography>
                      </Stack>
                    </Stack>
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
        </Grid>
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
          sx={{
            [`& .${gridClasses.cell}`]: {
              paddingTop: 1,
              paddingBottom: 1,
            },
            [`& .${gridClasses.row}`]: {
              paddingLeft: 1,
              paddingTop: 1,
              paddingBottom: 1,
              borderRadius: 2,
              backgroundColor: (theme) => theme.palette.background.paper,
            },
          }}
        />
      )}
    </Stack>
  );
};

export default memo(MyLocations);
