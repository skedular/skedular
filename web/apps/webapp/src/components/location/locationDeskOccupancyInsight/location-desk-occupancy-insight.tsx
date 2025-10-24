import { AnalyticsDaterangeSelector } from '@/components/analytics';
import { SectionIconTypography } from '@/components/commons';
import { toDayAndMonthDate, toFixed } from '@/libs/utils';
import type { locationDeskOccupancyInsight_locationAnalytics_query$key } from '@/queries/__generated__/locationDeskOccupancyInsight_locationAnalytics_query.graphql';
import type { locationDeskOccupancyInsight_query$key } from '@/queries/__generated__/locationDeskOccupancyInsight_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: locationDeskOccupancyInsight_query$key;
  rootDataLocationAnalyticsRelay: locationDeskOccupancyInsight_locationAnalytics_query$key;
};

const LocationDeskOccupancyInsight = ({ rootDataRelay, rootDataLocationAnalyticsRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationDeskOccupancyInsight_query on Query {
        location(id: $locationId) {
          id
          name
          organization {
            id
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataLocationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment locationDeskOccupancyInsight_locationAnalytics_query on Query @refetchable(queryName: "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment") {
        location(id: $locationId) {
          analytics(from: $from, until: $to) {
            desksOccupancyPercentage {
              date
              percentage
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

  if (!rootDataLocationAnalytics.location || !rootData.location) {
    return null;
  }

  const dataset =
    rootDataLocationAnalytics.location.analytics.desksOccupancyPercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataLocationAnalytics.location.analytics.desksOccupancyPercentage.map(({ date, percentage }) => ({
          date: toDayAndMonthDate(date),
          percentage: toFixed(percentage, 2),
        }));

  const chartSettings = {
    yAxis: [
      {
        label: 'Desk Occupancy Percentage',
        valueFormatter: (value: number) => `${value}%`,
      },
    ],
    width: 500,
    height: 300,
    sx: {
      [`.${axisClasses.left} .${axisClasses.label}`]: {
        transform: 'translate(-8px, 0)',
      },
    },
  };

  return (
    <Card sx={{ maxWidth: 500, height: '100%' }}>
      <CardHeader title={<SectionIconTypography label="Desk Occupancy Insights" invertDefaultColor />} />
      <CardContent>
        <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'percentage' }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(LocationDeskOccupancyInsight);
