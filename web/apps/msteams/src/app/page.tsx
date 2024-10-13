import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useNavigate } from 'react-router-dom';
import type { pageHome_rootQuery } from './__generated__/pageHome_rootQuery.graphql';

const RootQuery = graphql`
  query pageHome_rootQuery {
    isAzureTenantInstalled
    azureTenantOrganization {
      id
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
};

const Home = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageHome_rootQuery>(RootQuery, queryReference);
  const navigate = useNavigate();

  useEffect(() => {
    if (!rootData.isAzureTenantInstalled || !rootData.azureTenantOrganization) {
      navigate('/install');
    } else {
      navigate(`/organizations/${rootData.azureTenantOrganization.id}`);
    }
  }, [navigate, rootData.isAzureTenantInstalled, rootData.azureTenantOrganization]);

  return <Loading />;
};

const MemoHome = memo(Home);

const HomeWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoHome queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(HomeWithRelay);
