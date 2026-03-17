import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceResourceType_query$key } from '@/queries/__generated__/singleChoiceResourceType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceResourceType_query$key;
  name: string;
  required?: boolean;
};

type ResourceTypeDetails = {
  readonly id: string;
  readonly name: string;
  readonly color: string | null | undefined;
};

const SingleChoiceResourceType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceResourceType_query$key>(
    graphql`
      fragment singleChoiceResourceType_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          resourceTypes {
            id
            name
            color
          }
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<ResourceTypeDetails[]>(
    () => (rootData.organization?.resourceTypes ? rootData.organization.resourceTypes.map((item) => item) : []),
    [rootData.organization],
  );
  const filter = createFilterOptions<ResourceTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as ResourceTypeDetails).id}
      getOptionLabel={(option: string | ResourceTypeDetails) => (option as ResourceTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as ResourceTypeDetails;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as ResourceTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceResourceType);
