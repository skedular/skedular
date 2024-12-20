import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import { SectionIconTypography } from '@repo/shared/components/commons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationLink } from 'components/location';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { locationBookingInsightRoot_rootQuery } from './__generated__/locationBookingInsightRoot_rootQuery.graphql';
import LocationBookingInsight from './location-booking-insight';

type Props = {
  queryReference: PreloadedQuery<locationBookingInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const RootQuery = graphql`
  query locationBookingInsightRoot_rootQuery($locationId: String!, $locationExists: Boolean!, $from: DateTime!, $to: DateTime!) {
    ...locationBookingInsight_query
    ...locationBookingInsight_locationAnalytics_query
  }
`;

const LocationBookingInsightRoot = ({ queryReference, onReloadRequired, organizationId, locationId, hideLocationDetails }: Props) => {
  const rootData = usePreloadedQuery<locationBookingInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <LocationBookingInsight
      rootDataRelay={rootData}
      rootDataLocationAnalyticsRelay={rootData}
      organizationId={organizationId}
      locationId={locationId}
      hideLocationDetails={hideLocationDetails}
    />
  );
};

const MemoLocationBookingInsightRoot = memo(LocationBookingInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  locationId: string;
  locationName?: string;
  hideLocationDetails?: boolean;
};

const LocationBookingInsightRootWithRelay = ({ onReloadRequired, organizationId, locationId, locationName, hideLocationDetails }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationBookingInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(30, 'days');

    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        from: from.toISOString(),
        to: to.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
              {!hideLocationDetails && <LocationLink organizationId={organizationId} id={locationId} name={locationName} analayticsLink />}
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
        locationId={locationId}
        hideLocationDetails={hideLocationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationBookingInsightRootWithRelay);
