import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Link from '@mui/material/Link';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationMemberAttendancyInsightRoot_rootQuery } from './__generated__/organizationMemberAttendancyInsightRoot_rootQuery.graphql';
import OrganizationMemberAttendancyInsight from './organization-member-attendancy-insight';

type Props = {
  queryReference: PreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery, Record<string, unknown>>;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const RootQuery = graphql`
  query organizationMemberAttendancyInsightRoot_rootQuery($organizationId: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationMemberAttendancyInsight_query
  }
`;

const OrganizationMemberAttendancyInsightRoot = ({ queryReference, organizationId, hideOrganizationDetails }: Props) => {
  const rootData = usePreloadedQuery<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <OrganizationMemberAttendancyInsight rootDataRelay={rootData} organizationId={organizationId} hideOrganizationDetails={hideOrganizationDetails} />
  );
};

const MemoOrganizationMemberAttendancysCard = memo(OrganizationMemberAttendancyInsightRoot);

type RelayProps = {
  organizationId: string;
  organizationName: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationMemberAttendancyInsightRootWithRelay = ({ organizationId, organizationName, hideOrganizationDetails }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMemberAttendancyInsightRoot_rootQuery>(RootQuery);

  useEffect(() => {
    const to = startOfDay(null);
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
  }, [loadQuery, organizationId]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <>
              <Typography variant="h5" color="primary">
                Member Attendancy Insights
              </Typography>
              {!hideOrganizationDetails && (
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <Link href={`/organization/${organizationId}?tab=analytics`}>
                    {organizationName && <Typography variant="h6">{organizationName}</Typography>}
                  </Link>
                </Stack>
              )}
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
        organizationId={organizationId}
        hideOrganizationDetails={hideOrganizationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMemberAttendancyInsightRootWithRelay);
