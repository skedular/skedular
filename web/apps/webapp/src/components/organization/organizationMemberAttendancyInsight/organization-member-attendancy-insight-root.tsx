import { AnalyticsInsightCard } from '@/components/analytics';
import { RelayError, toRootError } from '@/components/relayError';
import { startOfDay } from '@/libs/utils';
import type { organizationMemberAttendancyInsightRoot_rootQuery } from '@/queries/__generated__/organizationMemberAttendancyInsightRoot_rootQuery.graphql';
import Skeleton from '@mui/material/Skeleton';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationMemberAttendancyInsight from './organization-member-attendancy-insight';

type Props = {
  queryReference: PreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationMemberAttendancyInsightRoot_rootQuery($organizationCustomDomain: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationMemberAttendancyInsight_organizationAnalytics_query
  }
`;

const OrganizationMemberAttendancyInsightRoot = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return <OrganizationMemberAttendancyInsight rootDataOrganizationAnalyticsRelay={rootData} organizationCustomDomain={organizationCustomDomain} />;
};

const MemoOrganizationMemberAttendancysCard = memo(OrganizationMemberAttendancyInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  organizationName?: string;
};

const OrganizationMemberAttendancyInsightRootWithRelay = ({ organizationCustomDomain, onReloadRequired }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery);
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
      <AnalyticsInsightCard title="Member Attendancy Insights">
        <Skeleton variant="rounded" width="100%" height={350} />
      </AnalyticsInsightCard>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationMemberAttendancysCard queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMemberAttendancyInsightRootWithRelay);
