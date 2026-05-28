import { Loading } from '@/components/loading';
import { OrganizationUser } from '@/components/organization/organizationUser';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@skedular/shared';
import type { pageOrganizationUser_rootQuery } from '@/queries/__generated__/pageOrganizationUser_rootQuery.graphql';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationUser_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  customerId: string;
};

const RootQuery = graphql`
  query pageOrganizationUser_rootQuery($organizationCustomDomain: String!, $customerId: String!) {
    customer(id: $customerId) {
      name
      givenName
      middleName
      familyName
    }
    ...organizationUser_query
  }
`;

const UserPage = ({ queryReference, onReloadRequired, organizationCustomDomain, customerId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationUser_rootQuery>(RootQuery, queryReference);

  return (
    <RootShell>
      <OrganizationUser rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} customerId={customerId} />
    </RootShell>
  );
};

const MemoUserPage = memo(UserPage);

const UserPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationUser_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, customerId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!customerId) {
    throw new Error('customerId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        customerId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain, customerId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoUserPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} customerId={customerId} />
    </ErrorBoundary>
  );
};

export default memo(UserPageWithRelay);
