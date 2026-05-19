import type { multipleChoicesMarketplaceBookingSubscriptionStatuses_query$key } from '@/queries/__generated__/multipleChoicesMarketplaceBookingSubscriptionStatuses_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import { BodyIconTypography } from '@skedular/ui';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesMarketplaceBookingSubscriptionStatuses_query$key;
  name: string;
  required?: boolean;
};

type MarketplaceBookingSubscriptionStatusDetails = {
  type: string;
  name: string;
};

const MultipleChoicesMarketplaceBookingSubscriptionStatuses = ({ rootDataRelay, name, required }: Props) => {
  const rootData = useFragment<multipleChoicesMarketplaceBookingSubscriptionStatuses_query$key>(
    graphql`
      fragment multipleChoicesMarketplaceBookingSubscriptionStatuses_query on Query {
        marketplaceBookingSubscriptionStatuses {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const items = useMemo<MarketplaceBookingSubscriptionStatusDetails[]>(
    () => rootData.marketplaceBookingSubscriptionStatuses.map((item) => item),
    [rootData.marketplaceBookingSubscriptionStatuses],
  );
  const filter = createFilterOptions<MarketplaceBookingSubscriptionStatusDetails>();

  return (
    <Autocomplete
      name={name}
      multiple={true}
      required={required}
      options={items}
      getOptionValue={(option) => (option as MarketplaceBookingSubscriptionStatusDetails).type}
      getOptionLabel={(option: string | MarketplaceBookingSubscriptionStatusDetails) => (option as MarketplaceBookingSubscriptionStatusDetails).name}
      renderOption={(props, option) => {
        const castedOption = option as MarketplaceBookingSubscriptionStatusDetails;

        return (
          <li {...props} key={castedOption.type}>
            <BodyIconTypography label={castedOption.name} />
          </li>
        );
      }}
      disableCloseOnSelect
      filterOptions={(options, params) => filter(options as MarketplaceBookingSubscriptionStatusDetails[], params)}
      selectOnFocus
      clearOnBlur
      handleHomeEndKeys
    />
  );
};

export default memo(MultipleChoicesMarketplaceBookingSubscriptionStatuses);
