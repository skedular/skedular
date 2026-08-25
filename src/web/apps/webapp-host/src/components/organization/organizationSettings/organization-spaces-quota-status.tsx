import { Loading } from '@/components/loading';
import { useSpacesSubscription } from '@/components/rootShell/spaces-subscription-context';
import type { organizationSpacesQuotaStatusQuery } from '@/queries/__generated__/organizationSpacesQuotaStatusQuery.graphql';
import LinearProgress from '@mui/material/LinearProgress';
import { BodyIconTypography, CaptionIconTypography, StackColumn } from '@skedular/ui';
import { memo, useEffect } from 'react';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  organizationId: string;
};

type InnerProps = {
  queryReference: PreloadedQuery<organizationSpacesQuotaStatusQuery>;
};

const RootQuery = graphql`
  query organizationSpacesQuotaStatusQuery($organizationId: String!) {
    bookingSpacesQuotaStatus(organizationId: $organizationId) {
      organizationId
      currentPeriodStartUtc
      currentPeriodEndUtc
      planCode
      quotaLimit
      currentUsage
      attemptedCurrentPeriodCount
      excludedOutOfPeriodCount
      totalAttemptedInstanceCount
      remainingQuota
      quotaExceeded
      reasonCode {
        type
        name
      }
      upgradePlans {
        planCode
        name
        availability
        priceDescription
      }
    }
  }
`;

const SpacesQuotaStatusInner = memo(({ queryReference }: InnerProps) => {
  const data = usePreloadedQuery<organizationSpacesQuotaStatusQuery>(RootQuery, queryReference);
  const quotaStatus = data.bookingSpacesQuotaStatus;

  if (!quotaStatus) {
    return null;
  }

  if (quotaStatus.planCode === 4 || quotaStatus.quotaLimit == null || quotaStatus.remainingQuota == null) {
    return null;
  }

  const usagePercent = quotaStatus.quotaLimit > 0 ? Math.min(100, Math.round((quotaStatus.currentUsage / quotaStatus.quotaLimit) * 100)) : 0;

  return (
    <StackColumn spacing={1}>
      <BodyIconTypography label={`Booking usage: ${quotaStatus.currentUsage} / ${quotaStatus.quotaLimit}`} />
      <LinearProgress variant="determinate" value={usagePercent} color={quotaStatus.quotaExceeded ? 'error' : 'primary'} />
      <CaptionIconTypography
        label={quotaStatus.remainingQuota > 0 ? `${quotaStatus.remainingQuota} booking instances remaining this period` : 'Booking quota exceeded for this period'}
        color={quotaStatus.quotaExceeded ? 'error' : 'textSecondary'}
      />
    </StackColumn>
  );
});

SpacesQuotaStatusInner.displayName = 'SpacesQuotaStatusInner';

const PaidSpacesQuotaStatus = memo(({ organizationId }: Props) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationSpacesQuotaStatusQuery>(RootQuery);

  useEffect(() => {
    loadQuery({ organizationId }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationId]);

  if (!queryReference) {
    return <Loading />;
  }

  return <SpacesQuotaStatusInner queryReference={queryReference} />;
});

PaidSpacesQuotaStatus.displayName = 'PaidSpacesQuotaStatus';

const SpacesQuotaStatus = memo(({ organizationId }: Props) => {
  const subscription = useSpacesSubscription();
  if (!subscription || subscription.subscriptionStatus === 'MISSING_STATE' || subscription.subscriptionStatus === 'PAID_INACTIVE') {
    return (
      <StackColumn spacing={2}>
        <BodyIconTypography label="Spaces subscription unavailable" />
        <CaptionIconTypography label="Review your subscription or contact support. Your data and configuration remain preserved." color="error" />
      </StackColumn>
    );
  }

  if (!subscription?.subscriptionStatus.startsWith('TRIAL_')) {
    return <PaidSpacesQuotaStatus organizationId={organizationId} />;
  }

  const expired = subscription.subscriptionStatus === 'TRIAL_EXPIRED';
  return (
    <StackColumn spacing={2}>
      <BodyIconTypography label={expired ? '14-day trial expired' : '14-day free trial'} />
      <CaptionIconTypography
        label={
          expired
            ? 'Upgrade to continue using Spaces and accepting bookings. Your data and configuration are preserved.'
            : `${subscription.remainingTrialDays} ${subscription.remainingTrialDays === 1 ? 'day' : 'days'} remaining. The existing Free plan limit of 100 booking instances per month still applies.`
        }
        color={expired ? 'error' : 'textSecondary'}
      />
      {expired ? null : <PaidSpacesQuotaStatus organizationId={organizationId} />}
    </StackColumn>
  );
});

SpacesQuotaStatus.displayName = 'SpacesQuotaStatus';

export { SpacesQuotaStatus, SpacesQuotaStatusInner };
export type { InnerProps as SpacesQuotaStatusProps };
