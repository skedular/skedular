import { BodyIconTypography } from '@/components/commons';
import type { singleChoicePriceUnit_query$key } from '@/queries/__generated__/singleChoicePriceUnit_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoicePriceUnit_query$key;
  name: string;
  required?: boolean;
};

type PriceUnitDetails = {
  readonly type: string;
  readonly name: string;
};

const SingleChoicesPriceUnit = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoicePriceUnit_query$key>(
    graphql`
      fragment singleChoicePriceUnit_query on Query {
        priceUnits {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const priceUnits = useMemo<PriceUnitDetails[]>(() => rootData.priceUnits.map((item) => item), [rootData.priceUnits]);
  const filter = createFilterOptions<PriceUnitDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={priceUnits}
      getOptionValue={(option) => (option as PriceUnitDetails).type}
      getOptionLabel={(option: string | PriceUnitDetails) => (option as PriceUnitDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as PriceUnitDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as PriceUnitDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoicesPriceUnit);
