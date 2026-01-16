import { SectionIconTypography } from '@/components/commons';
import { RelayError, toRootError } from '@/components/relayError';
import { startOfDay } from '@/libs/utils';
import type { organizationBookingInsightRoot_rootQuery } from '@/queries/__generated__/organizationBookingInsightRoot_rootQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationBookingInsight from './organization-booking-insight';

type Props = {
  queryReference: PreloadedQuery<organizationBookingInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootQuery = graphql`
  query organizationBookingInsightRoot_rootQuery($organizationUniqueAlphanumericName: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationBookingInsight_organizationAnalytics_query
  }
`;

const OrganizationBookingInsightRoot = ({ queryReference, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<organizationBookingInsightRoot_rootQuery>(RootQuery, queryReference);

  return <OrganizationBookingInsight rootDataOrganizationAnalyticsRelay={rootData} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />;
};

const MemoLocationBookingInsightRoot = memo(OrganizationBookingInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const OrganizationBookingInsightRootWithRelay = ({ onReloadRequired, organizationUniqueAlphanumericName }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookingInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(30, 'days');

    loadQuery(
      {
        organizationUniqueAlphanumericName,
        from: from.toISOString(),
        to: to.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader title={<SectionIconTypography label="Booking Insights" invertDefaultColor />} />
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoLocationBookingInsightRoot
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationBookingInsightRootWithRelay);
