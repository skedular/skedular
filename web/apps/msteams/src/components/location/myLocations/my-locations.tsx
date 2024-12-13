import AvatarGroup from '@mui/material/AvatarGroup';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import LinearProgress from '@mui/material/LinearProgress';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@repo/shared/components/commons';
import { Zones } from '@repo/shared/components/zone';
import { defaultPadding } from '@repo/shared/libs/theme';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { memo, startTransition, useCallback, useEffect, useMemo, useState } from 'react';
import { useFragment, useRefetchableFragment } from 'react-relay';
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

  const [today] = useState(startOfDay());
  const connectionIds = useMemo(() => (rootDataRefetchable.locations ? [rootDataRefetchable.locations.__id] : []), [rootDataRefetchable.locations]);
  const locations = useMemo(() => {
    if (!rootDataRefetchable.locations) {
      return [];
    }

    return rootDataRefetchable.locations.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name));
  }, [rootDataRefetchable.locations]);

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
      renderCell: (params) => <BodyIconTypography label={params.value ? params.value : 'N/A'} sx={{ whiteSpace: 'pre-line' }} />,
      display: 'flex',
      minWidth: 200,
    },
  ];

  if (!rootDataRefetchable.locations || !rootDataRefetchable.availableDesks || !rootData.organizationMembers) {
    return <></>;
  }

  return (
    <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
      <SectionIconTypography label="My Locations" />

      <Divider />

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
                  defaultDate={today}
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
    </StackColumn>
  );
};

export default memo(MyLocations);
