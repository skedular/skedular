import Grid from '@mui/material/Grid2';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { toDayAndMonthDate, toFixed } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { memo, startTransition, useCallback } from 'react';
import { useRefetchableFragment } from 'react-relay';
import type { locationAnalyticsPaginationQuery } from './__generated__/locationAnalyticsPaginationQuery.graphql';
import type { locationAnalyticsTab_query$key } from './__generated__/locationAnalyticsTab_query.graphql';

type Props = {
  rootDataRelay: locationAnalyticsTab_query$key;
  locationId: string;
};

const LocationAnalyticsTab = ({ rootDataRelay, locationId }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<locationAnalyticsPaginationQuery, locationAnalyticsTab_query$key>(
    graphql`
      fragment locationAnalyticsTab_query on Query @refetchable(queryName: "locationAnalyticsPaginationQuery") {
        locationAnalytics(locationId: $locationId, from: $locationAnalyticsFrom, until: $locationAnalyticsUntil) {
          desksOccupancyPercentage {
            date
            percentage
          }
          dailyBookingsTotals {
            date
            total
          }
        }
      }
    `,
    rootDataRelay,
  );

  const handleRefetch = useCallback(
    (locationAnalyticsFrom: Dayjs, locationAnalyticsUntil: Dayjs) => {
      startTransition(() => {
        refetch(
          {
            locationId,
            locationAnalyticsFrom: locationAnalyticsFrom.toISOString(),
            locationAnalyticsUntil: locationAnalyticsUntil.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {},
          },
        );
      });
    },
    [refetch, locationId],
  );

  if (!rootData.locationAnalytics) {
    return null;
  }

  const deskOccupancyPercentageChartSetting = {
    yAxis: [
      {
        label: 'Desk Occupancy Percentage',
        min: 0,
        max: 100,
        valueFormatter: (value: number) => `${value} %`,
      },
    ],
    width: 500,
    height: 300,
    sx: {
      [`.${axisClasses.left} .${axisClasses.label}`]: {
        transform: 'translate(-12px, 0)',
      },
    },
  };

  const dailyBookingsTotalsChartSetting = {
    yAxis: [
      {
        label: 'Total Bookings',
      },
    ],
    width: 500,
    height: 300,
    sx: {
      [`.${axisClasses.left} .${axisClasses.label}`]: {
        transform: 'translate(-12px, 0)',
      },
    },
  };

  const deskOccupancyPercentageDataset =
    rootData.locationAnalytics.desksOccupancyPercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.locationAnalytics.desksOccupancyPercentage.map(({ date, percentage }) => ({
          date: toDayAndMonthDate(date),
          percentage: toFixed(percentage, 2),
        }));

  const dailyBookingsTotalsDataset =
    rootData.locationAnalytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.locationAnalytics.dailyBookingsTotals.map(({ date, total }) => ({
          date: toDayAndMonthDate(date),
          total,
        }));

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  return (
    <>
      <Grid container sx={{ justifyContent: 'flex-end' }}>
        <Grid>
          <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
        </Grid>
      </Grid>
      <BarChart
        dataset={deskOccupancyPercentageDataset}
        xAxis={[{ scaleType: 'band', dataKey: 'date' }]}
        series={[{ dataKey: 'percentage' }]}
        {...deskOccupancyPercentageChartSetting}
      />
      <BarChart
        dataset={dailyBookingsTotalsDataset}
        xAxis={[{ scaleType: 'band', dataKey: 'date' }]}
        series={[{ dataKey: 'total' }]}
        {...dailyBookingsTotalsChartSetting}
      />
    </>
  );
};

export default memo(LocationAnalyticsTab);
