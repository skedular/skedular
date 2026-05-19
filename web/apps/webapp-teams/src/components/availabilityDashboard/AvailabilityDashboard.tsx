'use client';

import type { AvailabilityDashboard_data$key } from '@/queries/__generated__/AvailabilityDashboard_data.graphql';
import type {
  AvailabilityDashboard_OnResourceAvailabilityChangedSubscription,
  ResourceAvailabilityClassification,
} from '@/queries/__generated__/AvailabilityDashboard_OnResourceAvailabilityChangedSubscription.graphql';
import type { AvailabilityFilterBar_locations$key } from '@/queries/__generated__/AvailabilityFilterBar_locations.graphql';
import type { AvailabilityFilterBar_statuses$key } from '@/queries/__generated__/AvailabilityFilterBar_statuses.graphql';
import Alert from '@mui/material/Alert';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { PageHeaderPanel, StackColumn, defaultPadding } from '@skedular/ui';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useSubscription } from 'react-relay';
import type { AvailabilityFilters } from './AvailabilityFilterBar';
import AvailabilityFilterBar from './AvailabilityFilterBar';
import ResourceDayViewList from './ResourceDayViewList';

type Props = {
  dataRef: AvailabilityDashboard_data$key;
  locationsRef: AvailabilityFilterBar_locations$key;
  statusesRef: AvailabilityFilterBar_statuses$key;
  filters: AvailabilityFilters;
  onFiltersChange: (filters: AvailabilityFilters) => void;
  onRefresh?: () => void;
  isPending?: boolean;
  organizationCustomDomain: string;
};

const AvailabilityDashboard = ({ dataRef, locationsRef, statusesRef, filters, onFiltersChange, onRefresh, isPending, organizationCustomDomain }: Props) => {
  const data = useFragment<AvailabilityDashboard_data$key>(
    graphql`
      fragment AvailabilityDashboard_data on ResourceDayViewConnection {
        subscriptionKey
        ...ResourceDayViewList_result
      }
    `,
    dataRef,
  );

  const [subscriptionError, setSubscriptionError] = useState(false);

  useSubscription<AvailabilityDashboard_OnResourceAvailabilityChangedSubscription>(
    useMemo(
      () => ({
        variables: {
          subscriptionKey: data.subscriptionKey,
          filter: {
            date: filters.date,
            organizationCustomDomain,
            locationIds: filters.locationIds ?? [],
            statuses: (filters.statuses ?? []) as ResourceAvailabilityClassification[],
          },
        },
        subscription: graphql`
          subscription AvailabilityDashboard_OnResourceAvailabilityChangedSubscription($subscriptionKey: String!, $filter: ResourceAvailabilityFilterInput!) {
            resourceAvailability(subscriptionKey: $subscriptionKey, filter: $filter) {
              ...ResourceDayViewList_result
            }
          }
        `,
        onError: () => setSubscriptionError(true),
        onCompleted: () => {
          setSubscriptionError(false);
          onRefresh?.();
        },
      }),
      [data.subscriptionKey, filters, onRefresh, organizationCustomDomain],
    ),
  );

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', pb: defaultPadding }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, sm: 1, md: 2 } }} spacing={2}>
        <PageHeaderPanel title="Availability Dashboard" description="View resource availability across the organization for the selected day." />

        {subscriptionError && (
          <Alert
            severity="warning"
            aria-live="polite"
            action={
              onRefresh ? (
                <Button size="small" color="inherit" onClick={onRefresh}>
                  Retry
                </Button>
              ) : undefined
            }
          >
            Live updates paused — reconnecting…
          </Alert>
        )}

        <AvailabilityFilterBar filters={filters} locationsRef={locationsRef} statusesRef={statusesRef} onChange={onFiltersChange} isPending={isPending} />

        <ResourceDayViewList resultRef={data} />
      </StackColumn>
    </Box>
  );
};

export default memo(AvailabilityDashboard);
