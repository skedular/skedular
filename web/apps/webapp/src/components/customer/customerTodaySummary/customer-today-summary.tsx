import type { customerTodaySummary_rootQuery } from '@/queries/__generated__/customerTodaySummary_rootQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Skeleton from '@mui/material/Skeleton';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<customerTodaySummary_rootQuery, Record<string, unknown>>;
};

const RootQuery = graphql`
  query customerTodaySummary_rootQuery {
    me {
      id
    }
  }
`;

const CustomerTodaySummary = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<customerTodaySummary_rootQuery>(RootQuery, queryReference);

  return (
    <Card sx={{ maxWidth: 500, height: '100%' }}>
      <CardContent>
        <Skeleton variant="rounded" width={470} height={350} />
      </CardContent>
    </Card>
  );
};

const MemoCustomerTodaySummary = memo(CustomerTodaySummary);

type RelayProps = {};

const CustomerTodaySummaryWithRelay = ({}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<customerTodaySummary_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoCustomerTodaySummary queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(CustomerTodaySummaryWithRelay);
