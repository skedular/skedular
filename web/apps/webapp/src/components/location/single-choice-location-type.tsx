import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceLocationType_query$key } from '@/queries/__generated__/singleChoiceLocationType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceLocationType_query$key;
  name: string;
  required?: boolean;
};

type LocationTypeDetails = {
  type: string;
  name: string;
};

const SingleChoiceLocationType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceLocationType_query$key>(
    graphql`
      fragment singleChoiceLocationType_query on Query {
        locationTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const locationTypes = useMemo<LocationTypeDetails[]>(() => rootData.locationTypes.map((item) => item), [rootData.locationTypes]);
  const filter = createFilterOptions<LocationTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={locationTypes}
      getOptionValue={(option) => (option as LocationTypeDetails).type}
      getOptionLabel={(option: string | LocationTypeDetails) => (option as LocationTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as LocationTypeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as LocationTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceLocationType);
