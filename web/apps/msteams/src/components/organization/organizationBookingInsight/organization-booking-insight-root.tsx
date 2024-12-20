import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import { SectionIconTypography } from '@repo/shared/components/commons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { organizationBookingInsightRoot_rootQuery } from './__generated__/organizationBookingInsightRoot_rootQuery.graphql';
import OrganizationBookingInsight from './organization-booking-insight';

type Props = {
  queryReference: PreloadedQuery<organizationBookingInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const RootQuery = graphql`
  query organizationBookingInsightRoot_rootQuery($organizationId: String!, $from: DateTime!, $to: DateTime!) {
    ...organizationBookingInsight_query
    ...organizationBookingInsight_organizationAnalytics_query
  }
`;

const OrganizationBookingInsightRoot = ({ queryReference, onReloadRequired, organizationId, hideOrganizationDetails }: Props) => {
  const rootData = usePreloadedQuery<organizationBookingInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <OrganizationBookingInsight
      rootDataRelay={rootData}
      rootDataOrganizationAnalyticsRelay={rootData}
      organizationId={organizationId}
      hideOrganizationDetails={hideOrganizationDetails}
    />
  );
};

const MemoLocationBookingInsightRoot = memo(OrganizationBookingInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  organizationName?: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationBookingInsightRootWithRelay = ({ onReloadRequired, organizationId, organizationName, hideOrganizationDetails }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationBookingInsightRoot_rootQuery>(RootQuery);
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
              <SectionIconTypography label="Booking Insights" invertDefaultColor />
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
      <MemoLocationBookingInsightRoot
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        hideOrganizationDetails={hideOrganizationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationBookingInsightRootWithRelay);
