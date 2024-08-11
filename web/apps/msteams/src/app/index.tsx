import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { appHome_rootQuery } from './__generated__/appHome_rootQuery.graphql';

const RootQuery = graphql`
  query appHome_rootQuery {
    msTeamsCustomerRecordSynced
    bookingCustomerRecordSynced
    ...rootShell_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<appHome_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const Home = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<appHome_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.msTeamsCustomerRecordSynced && rootData?.bookingCustomerRecordSynced,
    [rootData?.msTeamsCustomerRecordSynced, rootData?.bookingCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.msTeamsCustomerRecordSynced, rootData?.bookingCustomerRecordSynced]}
    >
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography variant="h4">Testing home page</Typography>
      </Box>
    </RootShell>
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

  const handleReloadRequire = useCallback(() => {
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
      <MemoHome queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(HomeWithRelay);
