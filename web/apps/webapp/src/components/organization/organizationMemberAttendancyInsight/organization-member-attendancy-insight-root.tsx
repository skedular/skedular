import { OrganizationLink } from '@/components/organization';
import type { organizationMemberAttendancyInsightRoot_rootQuery } from '@/queries/__generated__/organizationMemberAttendancyInsightRoot_rootQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import Typography from '@mui/material/Typography';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import OrganizationMemberAttendancyInsight from './organization-member-attendancy-insight';

type Props = {
  queryReference: PreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const RootQuery = graphql`
  query organizationMemberAttendancyInsightRoot_rootQuery($organizationId: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationMemberAttendancyInsight_query
    ...organizationMemberAttendancyInsight_organizationAnalytics_query
  }
`;

const OrganizationMemberAttendancyInsightRoot = ({ queryReference, onReloadRequired, organizationId, hideOrganizationDetails }: Props) => {
  const rootData = usePreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <OrganizationMemberAttendancyInsight
      rootDataRelay={rootData}
      rootDataOrganizationAnalyticsRelay={rootData}
      organizationId={organizationId}
      hideOrganizationDetails={hideOrganizationDetails}
    />
  );
};

const MemoOrganizationMemberAttendancysCard = memo(OrganizationMemberAttendancyInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  organizationName?: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationMemberAttendancyInsightRootWithRelay = ({
  organizationId,
  onReloadRequired,
  organizationName,
  hideOrganizationDetails,
}: RelayProps) => {
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
        <CardHeader
          title={
            <>
              <Typography variant="h5" color="primary">
                Member Attendancy Insights
              </Typography>
              {!hideOrganizationDetails && <OrganizationLink id={organizationId} name={organizationName} analayticsLink />}
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
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationMemberAttendancysCard
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        hideOrganizationDetails={hideOrganizationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMemberAttendancyInsightRootWithRelay);
