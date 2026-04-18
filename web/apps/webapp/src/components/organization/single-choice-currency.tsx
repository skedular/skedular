import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceCurrency_query$key } from '@/queries/__generated__/singleChoiceCurrency_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceCurrency_query$key;
  name: string;
  required?: boolean;
};

type CurrencyDetails = {
  type: string;
  name: string;
};

const SingleChoiceCurrency = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceCurrency_query$key>(
    graphql`
      fragment singleChoiceCurrency_query on Query {
        currencies {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<CurrencyDetails[]>(() => rootData.currencies.map((item) => item), [rootData.currencies]);
  const filter = createFilterOptions<CurrencyDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={items}
      getOptionValue={(option) => (option as CurrencyDetails).type}
      getOptionLabel={(option: string | CurrencyDetails) => (option as CurrencyDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as CurrencyDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as CurrencyDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceCurrency);
