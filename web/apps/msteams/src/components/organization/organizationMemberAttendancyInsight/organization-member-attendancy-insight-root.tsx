import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import { SectionIconTypography } from '@repo/shared/components/commons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationMemberAttendancyInsightRoot_rootQuery } from './__generated__/organizationMemberAttendancyInsightRoot_rootQuery.graphql';
import OrganizationMemberAttendancyInsight from './organization-member-attendancy-insight';

type Props = {
  queryReference: PreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMemberAttendancyInsightRoot_rootQuery($organizationId: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationMemberAttendancyInsight_organizationAnalytics_query
  }
`;

const OrganizationMemberAttendancyInsightRoot = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return <OrganizationMemberAttendancyInsight rootDataOrganizationAnalyticsRelay={rootData} organizationId={organizationId} />;
};

const MemoOrganizationMemberAttendancysCard = memo(OrganizationMemberAttendancyInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationMemberAttendancyInsightRootWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(30, 'days');

    loadQuery(
      {
        organizationId,
        from: from.toISOString(),
        to: to.toISOString(),
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
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader title={<SectionIconTypography label="Member Attendancy Insights" invertDefaultColor />} />
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationMemberAttendancysCard
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMemberAttendancyInsightRootWithRelay);
