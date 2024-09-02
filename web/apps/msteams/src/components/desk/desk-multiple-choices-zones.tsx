import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import graphql from 'babel-plugin-relay/macro';
import { Autocomplete } from 'mui-rff';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { usePaginationFragment } from 'react-relay';
import type { deskMultipleChoicesZonesPaginationQuery } from './__generated__/deskMultipleChoicesZonesPaginationQuery.graphql';
import type { deskMultipleChoicesZones_query$key } from './__generated__/deskMultipleChoicesZones_query.graphql';

type Props = {
  rootDataRelay: deskMultipleChoicesZones_query$key;
  name: string;
  required?: boolean;
};

interface ZoneDetails {
  id: string;
  name: string;
}

const DeskMultipleChoicesZones = ({ rootDataRelay, name, required }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<deskMultipleChoicesZonesPaginationQuery, deskMultipleChoicesZones_query$key>(
    graphql`
      fragment deskMultipleChoicesZones_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "deskMultipleChoicesZonesPaginationQuery") {
        paginatedLocationTags(
          first: $count
          after: $cursor

          where: { locationId: $locationId, tagType: $zoneTagType }
          orderBy: $deskMultipleChoicesZonesSortingValues
        ) @connection(key: "locationZonesTab_paginatedLocationTags") {
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

  const zones = useMemo<ZoneDetails[]>(() => {
    if (!rootData.paginatedLocationTags) {
      return [];
    }

    return rootData.paginatedLocationTags.edges.map(({ node }) => node);
  }, [rootData.paginatedLocationTags]);

  const handleRefetch = useCallback(() => {
    startTransition(() => {
      refetch(
        {},
        {
          fetchPolicy: 'store-and-network',
          onComplete: () => {},
        },
      );
    });
  }, [refetch]);

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
            <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
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
