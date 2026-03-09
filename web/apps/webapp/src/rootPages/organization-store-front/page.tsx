import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { OrganizationStoreFrontRootShell } from '@/components/rootShell';
import UnauthenticatedRootShell from '@/components/rootShell/unauthenticated-root-shell';
import type { pageOrganizationStoreFront_rootQuery } from '@/queries/__generated__/pageOrganizationStoreFront_rootQuery.graphql';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationStoreFront_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageOrganizationStoreFront_rootQuery {
    bookingVersion {
      major
    }
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationStoreFront_rootQuery>(RootQuery, queryReference);
  const { user, loading } = useAuth();

  if (loading) {
    return null;
  }

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <>User is signed in</>
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedRootShell>
      <>User is not signed in</>
    </UnauthenticatedRootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationStoreFront_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { user, loading } = useAuth();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, loading, user]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
