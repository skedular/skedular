import { SectionIconTypography } from '@/components/commons';
import { RelayError, toRootError } from '@/components/relayError';
import { startOfDay } from '@/libs/utils';
import type { organizationMemberAttendancyInsightRoot_rootQuery } from '@/queries/__generated__/organizationMemberAttendancyInsightRoot_rootQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationMemberAttendancyInsight from './organization-member-attendancy-insight';

type Props = {
  queryReference: PreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootQuery = graphql`
  query organizationMemberAttendancyInsightRoot_rootQuery($organizationUniqueAlphanumericName: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationMemberAttendancyInsight_organizationAnalytics_query
  }
`;

const OrganizationMemberAttendancyInsightRoot = ({ queryReference, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return <OrganizationMemberAttendancyInsight rootDataOrganizationAnalyticsRelay={rootData} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />;
};

const MemoOrganizationMemberAttendancysCard = memo(OrganizationMemberAttendancyInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  organizationName?: string;
};

const OrganizationMemberAttendancyInsightRootWithRelay = ({ organizationUniqueAlphanumericName, onReloadRequired }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery);
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
        <CardHeader
          title={
            <>
              <SectionIconTypography label="Member Attendancy Insights" invertDefaultColor />
            </>
          }
        />
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationMemberAttendancysCard
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMemberAttendancyInsightRootWithRelay);
