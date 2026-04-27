import { AnalyticsInsightCard } from '@/components/analytics';
import { RelayError, toRootError } from '@/components/relayError';
import { startOfDay } from '@skedular/shared';
import type { organizationBookingInsightRoot_rootQuery } from '@/queries/__generated__/organizationBookingInsightRoot_rootQuery.graphql';
import Skeleton from '@mui/material/Skeleton';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationBookingInsight from './organization-booking-insight';

type Props = {
  queryReference: PreloadedQuery<organizationBookingInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationBookingInsightRoot_rootQuery($organizationCustomDomain: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationBookingInsight_organizationAnalytics_query
  }
`;

const OrganizationBookingInsightRoot = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationBookingInsightRoot_rootQuery>(RootQuery, queryReference);

  return <OrganizationBookingInsight rootDataOrganizationAnalyticsRelay={rootData} organizationCustomDomain={organizationCustomDomain} />;
};

const MemoLocationBookingInsightRoot = memo(OrganizationBookingInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const OrganizationBookingInsightRootWithRelay = ({ onReloadRequired, organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookingInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(30, 'days');

    loadQuery(
      {
        organizationCustomDomain,
        from: from.toISOString(),
        to: to.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return (
      <AnalyticsInsightCard title="Booking Insights">
        <Skeleton variant="rounded" width="100%" height={350} />
      </AnalyticsInsightCard>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoLocationBookingInsightRoot queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationBookingInsightRootWithRelay);
