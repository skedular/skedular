import type { organizationBillingTab_query$key } from '@/queries/__generated__/organizationBillingTab_query.graphql';
import Stack from '@mui/material/Stack';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import OrganizationBillingInfo from './organization-billing-info';
import OrganizationPaymentMethods from './organization-payment-methods';

type Props = {
  rootDataRelay: organizationBillingTab_query$key;
  onReloadRequired: () => void;
};

const OrganizationBillingTab = ({ rootDataRelay, onReloadRequired }: Props) => {
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
    <Stack direction="column" spacing={1}>
      <OrganizationBillingInfo rootDataRelay={rootData} />
      <OrganizationPaymentMethods rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
    </Stack>
  );
};

export default memo(OrganizationBillingTab);
