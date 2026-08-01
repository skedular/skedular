import type { multipleChoicesMarketplaceBookingPaymentStatuses_query$key } from '@/queries/__generated__/multipleChoicesMarketplaceBookingPaymentStatuses_query.graphql';
import { DefaultSelect } from '@/components/styled';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@skedular/ui';
import { Field } from 'react-final-form';
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
  return (
    <Field name={name}>
      {({ input }) => {
        const selected = ((input.value as string[] | undefined) ?? [])[0] ?? '';
        const selectedName = items.find((item) => item.type === selected)?.name;
        return (
          <DefaultSelect
            {...input}
            value={selected}
            displayEmpty
            size="small"
            required={required}
            renderValue={() => (
              <StackRow sx={{ whiteSpace: 'nowrap', flexWrap: 'nowrap' }}>
                <LeadIconTypography label={label ?? ''} />
                <Divider orientation="vertical" flexItem />
                <PushToRight />
                <SmallIconTypography label={selectedName ?? 'All payments'} sx={{ ml: 1 }} />
              </StackRow>
            )}
            sx={{ width: '100%', minWidth: 0, '& .MuiSelect-select': { pr: '48px !important', minWidth: 0 } }}
            onChange={(event) => input.onChange(event.target.value ? [event.target.value as string] : [])}
          >
            <MenuItem value="">All payments</MenuItem>
            {items.map((item) => (
              <MenuItem key={item.type} value={item.type}>
                <BodyIconTypography label={item.name} />
              </MenuItem>
            ))}
          </DefaultSelect>
        );
      }}
    </Field>
  );
};

export default memo(MultipleChoicesMarketplaceBookingPaymentStatuses);
