import type { multipleChoicesMarketplaceBookingPaymentStatuses_query$key } from '@/queries/__generated__/multipleChoicesMarketplaceBookingPaymentStatuses_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@skedular/ui';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesMarketplaceBookingPaymentStatuses_query$key;
  name: string;
  required?: boolean;
};

type MarketplaceBookingPaymentStatusDetails = {
  type: string;
  name: string;
};

const MultipleChoicesMarketplaceBookingPaymentStatuses = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesMarketplaceBookingPaymentStatuses_query$key>(
    graphql`
      fragment multipleChoicesMarketplaceBookingPaymentStatuses_query on Query {
        marketplaceBookingPaymentStatuses {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<MarketplaceBookingPaymentStatusDetails[]>(
    () => rootData.marketplaceBookingPaymentStatuses.map((item) => item),
    [rootData.marketplaceBookingPaymentStatuses],
  );
  const filter = createFilterOptions<MarketplaceBookingPaymentStatusDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as MarketplaceBookingPaymentStatusDetails).type}
      getOptionLabel={(option: string | MarketplaceBookingPaymentStatusDetails) => (option as MarketplaceBookingPaymentStatusDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as MarketplaceBookingPaymentStatusDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as MarketplaceBookingPaymentStatusDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesMarketplaceBookingPaymentStatuses);
