import { BodyIconTypography } from '@/components/commons';
import type { singleChoiceMarketplaceBookingType_query$key } from '@/queries/__generated__/singleChoiceMarketplaceBookingType_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: singleChoiceMarketplaceBookingType_query$key;
  name: string;
  required?: boolean;
};

type MarketplaceBookingTypeDetails = {
  type: string;
  name: string;
};

const SingleChoiceMarketplaceBookingType = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<singleChoiceMarketplaceBookingType_query$key>(
    graphql`
      fragment singleChoiceMarketplaceBookingType_query on Query {
        marketplaceBookingTypes {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const marketplaceBookingTypes = useMemo<MarketplaceBookingTypeDetails[]>(() => rootData.marketplaceBookingTypes.map((item) => item), [rootData.marketplaceBookingTypes]);
  const filter = createFilterOptions<MarketplaceBookingTypeDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={false}
      required={required}
      options={marketplaceBookingTypes}
      getOptionValue={(option) => (option as MarketplaceBookingTypeDetails).type}
      getOptionLabel={(option: string | MarketplaceBookingTypeDetails) => (option as MarketplaceBookingTypeDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as MarketplaceBookingTypeDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      filterOptions={(options, params) => filter(options as MarketplaceBookingTypeDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(SingleChoiceMarketplaceBookingType);
