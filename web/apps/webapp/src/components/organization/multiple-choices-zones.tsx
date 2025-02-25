import { BodyIconTypography } from '@/components/commons';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import type { multipleChoicesZones_query$key } from '@/queries/__generated__/multipleChoicesZones_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesZones_query$key;
  name: string;
  required?: boolean;
  organizationId: string;
};

type ZoneDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesZones = ({ rootDataRelay, name, required, organizationId }: Props) => {
  const rootData = useFragment<multipleChoicesZones_query$key>(
    graphql`
      fragment multipleChoicesZones_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        zones(first: $count, after: $cursor, where: { organizationId: $organizationId }, orderBy: $multipleChoicesZonesSortingValues)
          @connection(key: "multipleChoicesZones_zones") {
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

  const connectionIds = useMemo(() => (rootData.zones ? [rootData.zones.__id] : []), [rootData.zones]);
  const zones = useMemo<ZoneDetails[]>(() => (rootData.zones ? rootData.zones.edges.map(({ node }) => node) : []), [rootData.zones]);
  const filter = createFilterOptions<ZoneDetails>();

  if (zones.length === 0) {
    return <AddOrganizationZoneButton organizationId={organizationId} connectionIds={connectionIds} size="medium" />;
  }

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
