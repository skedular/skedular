import { AnalyticsDaterangeSelector, AnalyticsInsightCard } from '@/components/analytics';
import type { locationResourceAvailabilityInsight_locationAnalytics_query$key } from '@/queries/__generated__/locationResourceAvailabilityInsight_locationAnalytics_query.graphql';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { toDayAndMonthDate } from '@skedular/shared';
import type { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataLocationAnalyticsRelay: locationResourceAvailabilityInsight_locationAnalytics_query$key;
  resourceType?: string;
};

const LocationResourceAvailabilityInsight = ({ rootDataLocationAnalyticsRelay, resourceType }: Props) => {
  const [rootDataLocationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment locationResourceAvailabilityInsight_locationAnalytics_query on Query
      @refetchable(queryName: "locationResourceAvailabilityInsight_locationAnalytics_refetchableFragment") {
        location(id: $locationId) {
          analytics(from: $from, until: $to) {
            resourceAvailabilitySnapshots {
              date
              resourceType
              availableCount
              unavailableCount
              bookedCount
            }
          }
        }
      }
    `,
    rootDataLocationAnalyticsRelay,
  );

  const [, startTransition] = useTransition();

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs) => {
      startTransition(() => {
        refetch(
          {
            from: from.toISOString(),
            to: to.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  if (!rootDataLocationAnalytics.location) {
    return null;
  }

  const allSnapshots = rootDataLocationAnalytics.location.analytics.resourceAvailabilitySnapshots;
  const snapshots = resourceType ? allSnapshots.filter((s) => s.resourceType === resourceType) : allSnapshots;

  const dataset =
    snapshots.length === 0
      ? [{ date: 'No data available', available: 0, unavailable: 0, booked: 0 }]
      : snapshots.map(({ date, availableCount, unavailableCount, bookedCount }) => ({
          date: toDayAndMonthDate(date),
          available: availableCount,
          unavailable: unavailableCount,
          booked: bookedCount,
        }));

  const chartSettings = {
    yAxis: [
      {
        label: 'Resource Count',
      },
    ],
    width: 440,
    height: 300,
    sx: {
      [`.${axisClasses.left} .${axisClasses.label}`]: {
        transform: 'translate(-8px, 0)',
      },
    },
  };

  return (
    <AnalyticsInsightCard title="Resource Availability Insights">
      <AnalyticsDaterangeSelector defaultPeriod="6months" onDateRangeChange={handleDateRangeChange} />
      <BarChart
        dataset={dataset}
        xAxis={[{ scaleType: 'band', dataKey: 'date' }]}
        series={[
          { dataKey: 'available', label: 'Available', stack: 'resource' },
          { dataKey: 'unavailable', label: 'Unavailable', stack: 'resource' },
          { dataKey: 'booked', label: 'Booked', stack: 'resource' },
        ]}
        {...chartSettings}
      />
    </AnalyticsInsightCard>
  );
};

export default memo(LocationResourceAvailabilityInsight);
