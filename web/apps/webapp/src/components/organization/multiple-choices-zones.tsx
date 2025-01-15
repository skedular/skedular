import type { multipleChoicesZones_query$key } from '@/queries/__generated__/multipleChoicesZones_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesZones_query$key;
  name: string;
  required?: boolean;
};

type ZoneDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesZones = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesZones_query$key>(
    graphql`
      fragment multipleChoicesZones_query on Query {
        zones(where: { organizationId: $organizationId }, orderBy: $multipleChoicesZonesSortingValues) {
          __id
          totalCount
          edges {
            node {
              id
              name
              color
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const zones = useMemo<ZoneDetails[]>(() => {
    if (!rootData.zones) {
      return [];
    }

    return rootData.zones.edges.map(({ node }) => node);
  }, [rootData.zones]);

  if (!rootData.zones) {
    return <></>;
  }

  const filter = createFilterOptions<ZoneDetails>();

  return (
    <Autocomplete
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
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as ZoneDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesZones);
