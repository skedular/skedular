import type { organizationAnalyticsPaginationQuery } from '@/queries/__generated__/organizationAnalyticsPaginationQuery.graphql';
import type { organizationAnalyticsTab_query$key } from '@/queries/__generated__/organizationAnalyticsTab_query.graphql';
import Grid from '@mui/material/Grid2';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { toDayAndMonthDate, toFixed } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationAnalyticsTab_query$key;
  organizationId: string;
};

const OrganizationAnalyticsTab = ({ rootDataRelay, organizationId }: Props) => {
  const [rootData, refetch] = useRefetchableFragment<organizationAnalyticsPaginationQuery, organizationAnalyticsTab_query$key>(
    graphql`
      fragment organizationAnalyticsTab_query on Query @refetchable(queryName: "organizationAnalyticsPaginationQuery") {
        organizationAnalytics(organizationId: $organizationId, from: $organizationAnalyticsFrom, until: $organizationAnalyticsUntil) {
          memberAttendancePercentage {
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

  const [isPending, startTransition] = useTransition();
  const handleRefetch = useCallback(
    (organizationAnalyticsFrom: Dayjs, organizationAnalyticsUntil: Dayjs) => {
      startTransition(() => {
        refetch(
          {
            organizationId,
            organizationAnalyticsFrom: organizationAnalyticsFrom.toISOString(),
            organizationAnalyticsUntil: organizationAnalyticsUntil.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {},
          },
        );
      });
    },
    [refetch, organizationId],
  );

  if (!rootData.organizationAnalytics) {
    return null;
  }

  const deskOccupancyPercentageChartSetting = {
    yAxis: [
      {
        label: 'Members Attendance Percentage',
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
    rootData.organizationAnalytics.memberAttendancePercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.organizationAnalytics.memberAttendancePercentage.map(({ date, percentage }) => ({
          date: toDayAndMonthDate(date),
          percentage: toFixed(percentage, 2),
        }));

  const dailyBookingsTotalsDataset =
    rootData.organizationAnalytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.organizationAnalytics.dailyBookingsTotals.map(({ date, total }) => ({
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

export default memo(OrganizationAnalyticsTab);
