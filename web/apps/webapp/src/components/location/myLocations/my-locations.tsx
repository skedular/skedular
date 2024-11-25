import type { myLocations_locations_query$key } from '@/queries/__generated__/myLocations_locations_query.graphql';
import type { myLocations_locations_refetchableFragment } from '@/queries/__generated__/myLocations_locations_refetchableFragment.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import LinearProgress from '@mui/material/LinearProgress';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { DeskIcon, LocationIcon, ZoneIcon } from '@repo/shared/components/icons';
import { LOCATION_TAG_TYPE_LOCATION_ZONE, Zones } from '@repo/shared/components/zone';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: myLocations_locations_query$key;
  onReloadRequired: () => void;
  viewMode: 'list' | 'grid';
};

const MyLocations = ({ rootDataRelay, onReloadRequired, viewMode }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<myLocations_locations_refetchableFragment, myLocations_locations_query$key>(
    graphql`
      fragment myLocations_locations_query on Query @refetchable(queryName: "myLocations_locations_refetchableFragment") {
        locations(where: { organizationId: $organizationId }, orderBy: $locationsSortingValues) {
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
    rootDataRelay,
  );

  const locations = useMemo(() => {
    if (!rootData.locations) {
      return [];
    }

    return rootData.locations.edges.map((edge) => edge.node);
  }, [rootData.locations]);

  const handleRefetchAllBookings = useCallback(() => {
    startTransition(() => {
      refetch(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetch]);

  useEffect(() => handleRefetchAllBookings(), [handleRefetchAllBookings]);

  if (!rootData.locations || !rootData.availableOrganizationDesks) {
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
            const allDesksCount = location.desks.length;
            const availableDesksCount = rootData.availableOrganizationDesks
              ? rootData.availableOrganizationDesks.filter((desk) => desk.location?.uniqueId === location.id).length
              : 0;
            const availablePercentage = (availableDesksCount / allDesksCount) * 100;
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
                      <Typography variant="body1" sx={{ flexGrow: 0, flexShrink: 0 }}>{`${allDesksCount} Desks`}</Typography>

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
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      )}

      {viewMode === 'list' && <></>}
    </Stack>
  );
};

export default memo(MyLocations);
