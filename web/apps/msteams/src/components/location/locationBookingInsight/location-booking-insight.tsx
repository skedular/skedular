import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { SectionIconTypography } from '@repo/shared/components/commons';
import { toDayAndMonthDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { useFragment, useRefetchableFragment } from 'react-relay';
import type { locationBookingInsight_locationAnalytics_query$key } from './__generated__/locationBookingInsight_locationAnalytics_query.graphql';
import type { locationBookingInsight_query$key } from './__generated__/locationBookingInsight_query.graphql';

type Props = {
  rootDataRelay: locationBookingInsight_query$key;
  rootDataLocationAnalyticsRelay: locationBookingInsight_locationAnalytics_query$key;
};

const LocationBookingInsight = ({ rootDataRelay, rootDataLocationAnalyticsRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationBookingInsight_query on Query {
        location(id: $locationId) {
          id
          name
          organization {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataLocationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment locationBookingInsight_locationAnalytics_query on Query
      @refetchable(queryName: "locationBookingInsight_locationAnalytics_refetchableFragment") {
        locationAnalytics(locationId: $locationId, from: $from, until: $to) {
          dailyBookingsTotals {
            date
            total
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

  if (!rootDataLocationAnalytics.locationAnalytics || !rootData.location || !rootData.location.organization) {
    return <></>;
  }

  const dataset =
    rootDataLocationAnalytics.locationAnalytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataLocationAnalytics.locationAnalytics.dailyBookingsTotals.map(({ date, total }) => ({
          date: toDayAndMonthDate(date),
          total,
        }));

  const chartSettings = {
    yAxis: [
      {
        label: 'Total Bookings',
      },
    ],
    width: 500,
    height: 300,
  };

  return (
    <Card sx={{ maxWidth: 500, height: '100%' }}>
      <CardHeader title={<SectionIconTypography label="Booking Insights" invertDefaultColor />} />
      <CardContent>
        <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'total' }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(LocationBookingInsight);
