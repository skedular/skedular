import { BodyIconTypography } from '@/components/commons';
import type { multipleChoicesLocationSpaceTypes_query$key } from '@/queries/__generated__/multipleChoicesLocationSpaceTypes_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesLocationSpaceTypes_query$key;
  name: string;
  required?: boolean;
};

type LocationSpaceTypeDetails = {
  readonly id: string;
  readonly name: string;
  readonly color: string | null | undefined;
};

const MultipleChoicesLocationSpaceTypes = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesLocationSpaceTypes_query$key>(
    graphql`
      fragment multipleChoicesLocationSpaceTypes_query on Query {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
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

  const locationSpaceTypes = useMemo<LocationSpaceTypeDetails[]>(
    () => (rootData.organization?.locationSpaceTypes ? rootData.organization.locationSpaceTypes.map((item) => item) : []),
    [rootData.organization],
  );
  const filter = createFilterOptions<LocationSpaceTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={locationSpaceTypes}
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
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as LocationSpaceTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesLocationSpaceTypes);
