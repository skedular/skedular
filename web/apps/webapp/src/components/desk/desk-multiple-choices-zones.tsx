import type { deskMultipleChoicesZones_PaginationQuery } from '@/queries/__generated__/deskMultipleChoicesZones_PaginationQuery.graphql';
import type { deskMultipleChoicesZones_query$key } from '@/queries/__generated__/deskMultipleChoicesZones_query.graphql';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useCallback, useEffect, useMemo, useTransition } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: deskMultipleChoicesZones_query$key;
  locationId: string;
  name: string;
  required?: boolean;
};

interface ZoneDetails {
  id: string;
  name: string;
}

const DeskMultipleChoicesZones = ({ rootDataRelay, locationId, name, required }: Props) => {
  const { data: rootData, refetch } = usePaginationFragment<deskMultipleChoicesZones_PaginationQuery, deskMultipleChoicesZones_query$key>(
    graphql`
      fragment deskMultipleChoicesZones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "deskMultipleChoicesZones_PaginationQuery") {
        paginatedLocationTags(
          first: $count
          after: $cursor
          where: { locationId: $locationId, tagType: $zoneTagType }
          orderBy: $deskMultipleChoicesZonesSortingValues
        ) @connection(key: "locationZonesTab_paginatedLocationTags") @include(if: $locationExists) {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const zones = useMemo<ZoneDetails[]>(() => {
    if (!rootData.paginatedLocationTags) {
      return [];
    }

    return rootData.paginatedLocationTags.edges.map(({ node }) => node);
  }, [rootData.paginatedLocationTags]);

  const handleRefetch = useCallback(() => {
    startTransition(() => {
      refetch(
        {
          locationExists: !!locationId,
        },
        {
          fetchPolicy: 'store-and-network',
          onComplete: () => {},
        },
      );
    });
  }, [refetch, locationId]);

  // Workaround to ensure we have all the zones if new zones added using zone dialog
  useEffect(() => {
    handleRefetch();
  }, [handleRefetch]);

  if (!rootData.paginatedLocationTags) {
    return <></>;
  }

  const filter = createFilterOptions<ZoneDetails>();

  return (
    <Autocomplete
      label="Zones"
      name={name}
      multiple={true}
      required={required}
      options={zones}
      getOptionValue={(option) => (option as ZoneDetails).id}
      getOptionLabel={(option: string | ZoneDetails) => (option as ZoneDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as ZoneDetails;

        return (
          <li {...props}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{castedOption.name}</Typography>
            </Stack>
          </li>
        );
      }}
      disableCloseOnSelect={true}
      freeSolo={true}
      filterOptions={(options, params) => filter(options as ZoneDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(DeskMultipleChoicesZones);
