import { BodyIconTypography } from '@skedular/ui';
import type { singleChoiceLocationSpaceType_query$key } from '@/queries/__generated__/singleChoiceLocationSpaceType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceLocationSpaceType_query$key;
  name: string;
  required?: boolean;
};

type LocationSpaceTypeDetails = {
  readonly id: string;
  readonly name: string;
  readonly color: string | null | undefined;
};

const SingleChoiceLocationSpaceType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceLocationSpaceType_query$key>(
    graphql`
      fragment singleChoiceLocationSpaceType_query on Query {
        organization(customDomain: $organizationCustomDomain) {
          locationSpaceTypes {
            id
            name
            color
          }
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<LocationSpaceTypeDetails[]>(
    () => (rootData.organization?.locationSpaceTypes ? rootData.organization.locationSpaceTypes.map((item) => item) : []),
    [rootData.organization],
  );
  const filter = createFilterOptions<LocationSpaceTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as LocationSpaceTypeDetails).id}
      getOptionLabel={(option: string | LocationSpaceTypeDetails) => (option as LocationSpaceTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as LocationSpaceTypeDetails;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as LocationSpaceTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceLocationSpaceType);
