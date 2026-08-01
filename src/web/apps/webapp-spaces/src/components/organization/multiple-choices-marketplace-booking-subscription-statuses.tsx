import type { multipleChoicesMarketplaceBookingSubscriptionStatuses_query$key } from '@/queries/__generated__/multipleChoicesMarketplaceBookingSubscriptionStatuses_query.graphql';
import { DefaultSelect } from '@/components/styled';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import { BodyIconTypography, LeadIconTypography, PushToRight, SmallIconTypography, StackRow } from '@skedular/ui';
import { Field } from 'react-final-form';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: multipleChoicesMarketplaceBookingSubscriptionStatuses_query$key;
  name: string;
  label?: string;
  required?: boolean;
};

type MarketplaceBookingSubscriptionStatusDetails = {
  type: string;
  name: string;
};

const MultipleChoicesMarketplaceBookingSubscriptionStatuses = ({ rootDataRelay, name, label, required }: Props) => {
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
                <SmallIconTypography label={selectedName ?? 'All statuses'} sx={{ ml: 1 }} />
              </StackRow>
            )}
            sx={{ width: '100%', minWidth: 0, '& .MuiSelect-select': { pr: '48px !important', minWidth: 0 } }}
            onChange={(event) => input.onChange(event.target.value ? [event.target.value as string] : [])}
          >
            <MenuItem value="">All statuses</MenuItem>
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

export default memo(MultipleChoicesMarketplaceBookingSubscriptionStatuses);
