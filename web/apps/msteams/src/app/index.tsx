import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { appHome_rootQuery } from './__generated__/appHome_rootQuery.graphql';

const RootQuery = graphql`
  query appHome_rootQuery {
    msTeamsVersion {
      major
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<appHome_rootQuery, Record<string, unknown>>;
};

const Home = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<appHome_rootQuery>(RootQuery, queryReference);
  
  return (
    <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
      <Typography variant="h4">Testing home page</Typography>
      {rootData.msTeamsVersion.major}
      </Box>
  );
};

const MemoHome = memo(Home);

const HomeWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<appHome_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoHome queryReference={queryReference}  />
    </ErrorBoundary>
  );
};

export default memo(HomeWithRelay);
