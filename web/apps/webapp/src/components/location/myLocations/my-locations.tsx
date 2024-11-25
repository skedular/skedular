import type { myLocations_locations_query$key } from '@/queries/__generated__/myLocations_locations_query.graphql';
import type { myLocations_locations_refetchableFragment } from '@/queries/__generated__/myLocations_locations_refetchableFragment.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { LocationIcon } from '@repo/shared/components/icons';
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

  if (!rootData.locations) {
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
                  <CardContent></CardContent>
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
