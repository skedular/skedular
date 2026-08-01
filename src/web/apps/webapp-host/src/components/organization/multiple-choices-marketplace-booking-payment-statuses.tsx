import type { multipleChoicesMarketplaceBookingPaymentStatuses_query$key } from '@/queries/__generated__/multipleChoicesMarketplaceBookingPaymentStatuses_query.graphql';
import { createFilterOptions } from '@mui/material/useAutocomplete';
import Divider from '@mui/material/Divider';
import { BodyIconTypography, LeadIconTypography, StackRow } from '@skedular/ui';
import { Autocomplete } from 'mui-rff';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesMarketplaceBookingPaymentStatuses_query$key;
  name: string;
  label?: string;
  required?: boolean;
};

type MarketplaceBookingPaymentStatusDetails = {
  type: string;
  name: string;
};

const MultipleChoicesMarketplaceBookingPaymentStatuses = ({ rootDataRelay, name, label, required }: Props) => {
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
    <StackRow sx={{ alignItems: 'center', gap: 1 }}>
      <LeadIconTypography label={label ?? ''} />
      <Divider orientation="vertical" flexItem />
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
    </StackRow>
  );
};

export default memo(MultipleChoicesMarketplaceBookingPaymentStatuses);
