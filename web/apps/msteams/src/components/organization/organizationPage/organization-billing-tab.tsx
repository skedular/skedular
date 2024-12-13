import { StackColumn } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationBillingTab_rootQuery } from './__generated__/organizationBillingTab_rootQuery.graphql';
import OrganizationBillingInfo from './organization-billing-info';
import OrganizationPaymentMethods from './organization-payment-methods';

type Props = {
  queryReference: PreloadedQuery<organizationBillingTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationBillingTab_rootQuery($organizationId: String!) {
    ...organizationPaymentMethods_query
    ...organizationBillingInfo_query
  }
`;

const OrganizationBillingTab = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<organizationBillingTab_rootQuery>(RootQuery, queryReference);

  return (
    <StackColumn>
      <OrganizationBillingInfo rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      <OrganizationPaymentMethods rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
    </StackColumn>
  );
};
const MemoOrganizationBillingTab = memo(OrganizationBillingTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationBillingTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBillingTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationBillingTab queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationBillingTabWithRelay);
