import type { organizationBillingTab_query$key } from '@/queries/__generated__/organizationBillingTab_query.graphql';
import Box from '@mui/material/Box';
import Stack from '@mui/material/Stack';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import OrganizationBillingInfo from './organization-billing-info';
import OrganizationPaymentMethods from './organization-payment-methods';

type Props = {
  rootDataRelay: organizationBillingTab_query$key;
  onRefetchRequired: () => void;
};

const OrganizationBillingTab = ({ rootDataRelay, onRefetchRequired }: Props) => {
  const rootData = useFragment<organizationBillingTab_query$key>(
    graphql`
      fragment organizationBillingTab_query on Query {
        ...organizationPaymentMethods_query
        ...organizationBillingInfo_query
      }
    `,
    rootDataRelay,
  );

  return (
    <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
      <Stack direction="column">
        <OrganizationBillingInfo rootDataRelay={rootData} />
        <OrganizationPaymentMethods rootDataRelay={rootData} onRefetchRequired={onRefetchRequired} />
      </Stack>
    </Box>
  );
};

export default memo(OrganizationBillingTab);
