import { BodyIconTypography } from '@/components/commons';
import { Autocomplete } from '@/components/forms';
import { AddOrganizationZoneButton } from '@/components/organization/addOrganizationZone';
import type { multipleChoicesZones_query$key } from '@/queries/__generated__/multipleChoicesZones_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesZones_query$key;
  name: string;
  required?: boolean;
  organizationCustomDomain: string;
};

type ZoneDetails = {
  id: string;
  name: string;
  color: string | null | undefined;
};

const MultipleChoicesZones = ({ rootDataRelay, name, required, organizationCustomDomain }: Props) => {
  const rootData = useFragment<multipleChoicesZones_query$key>(
    graphql`
      fragment multipleChoicesZones_query on Query @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null }) {
        organization(customDomain: $organizationCustomDomain) {
          zones(first: $count, after: $cursor, orderBy: $multipleChoicesZonesSortingValues) @connection(key: "multipleChoicesZones_zones") {
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
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<ZoneDetails[]>(() => (rootData.organization ? rootData.organization.zones.edges.map(({ node }) => node) : []), [rootData.organization]);
  const connectionIds = useMemo(() => (rootData.organization ? [rootData.organization.zones.__id] : []), [rootData.organization]);
  const filter = createFilterOptions<ZoneDetails>();

  if (items.length === 0) {
    return <AddOrganizationZoneButton organizationCustomDomain={organizationCustomDomain} connectionIds={connectionIds} size="medium" />;
  }

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as ZoneDetails).id}
      getOptionLabel={(option: string | ZoneDetails) => (option as ZoneDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as ZoneDetails;

        return (
          <li {...props} key={castedOption.id}>
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
