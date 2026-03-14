import { BodyIconTypography } from '@/components/commons';
import type { multipleChoicesAmenities_query$key } from '@/queries/__generated__/multipleChoicesAmenities_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesAmenities_query$key;
  name: string;
  required?: boolean;
};

type AmenityDetails = {
  readonly id: string;
  readonly name: string;
  readonly color: string | null | undefined;
};

const MultipleChoicesAmenities = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesAmenities_query$key>(
    graphql`
      fragment multipleChoicesAmenities_query on Query {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          amenities {
            id
            name
            color
          }
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<AmenityDetails[]>(() => (rootData.organization?.amenities ? rootData.organization.amenities.map((item) => item) : []), [rootData.organization]);
  const filter = createFilterOptions<AmenityDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as AmenityDetails).id}
      getOptionLabel={(option: string | AmenityDetails) => (option as AmenityDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as AmenityDetails;

        return (
          <li {...props} key={castedOption.id}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as AmenityDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesAmenities);
