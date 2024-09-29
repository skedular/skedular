import type { organizationBookingInsightRoot_rootQuery } from '@/queries/__generated__/organizationBookingInsightRoot_rootQuery.graphql';
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
import NextLink from 'next/link';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import OrganizationBookingInsight from './organization-booking-insight';

type Props = {
  queryReference: PreloadedQuery<organizationBookingInsightRoot_rootQuery, Record<string, unknown>>;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const RootQuery = graphql`
  query organizationBookingInsightRoot_rootQuery($organizationId: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationBookingInsight_query
  }
`;

const OrganizationBookingInsightRoot = ({ queryReference, organizationId, hideOrganizationDetails }: Props) => {
  const rootData = usePreloadedQuery<organizationBookingInsightRoot_rootQuery>(RootQuery, queryReference);

  return <OrganizationBookingInsight rootDataRelay={rootData} organizationId={organizationId} hideOrganizationDetails={hideOrganizationDetails} />;
};

const MemoLocationBookingInsightRoot = memo(OrganizationBookingInsightRoot);

type RelayProps = {
  organizationId: string;
  organizationName: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationBookingInsightRootWithRelay = ({ organizationId, organizationName, hideOrganizationDetails }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookingInsightRoot_rootQuery>(RootQuery);

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
                Booking Insights
              </Typography>
              {!hideOrganizationDetails && (
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <Link component={NextLink} href={`/organization/${organizationId}?tab=analytics`}>
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
      <MemoLocationBookingInsightRoot
        queryReference={queryReference}
        organizationId={organizationId}
        hideOrganizationDetails={hideOrganizationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationBookingInsightRootWithRelay);
