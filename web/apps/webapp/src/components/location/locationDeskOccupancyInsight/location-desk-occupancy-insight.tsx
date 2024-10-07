import { LocationLink } from '@/components/location';
import type { locationDeskOccupancyInsight_query$key } from '@/queries/__generated__/locationDeskOccupancyInsight_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { toDayAndMonthDate, toFixed } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: locationDeskOccupancyInsight_query$key;
  organizationId?: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const LocationDeskOccupancyInsight = ({ rootDataRelay, organizationId, locationId, hideLocationDetails }: Props) => {
  const [rootData, refetch] = useRefetchableFragment(
    graphql`
      fragment locationDeskOccupancyInsight_query on Query @refetchable(queryName: "locationDeskOccupancyInsight_organizationAnalytics") {
        locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {
          desksOccupancyPercentage {
            date
            percentage
          }
        }
        location(id: $locationId) {
          name
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs) => {
      startTransition(() => {
        refetch(
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
      });
    },
    [refetch, locationId],
  );

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  if (!rootData.locationAnalytics) {
    return <></>;
  }

  const dataset =
    rootData.locationAnalytics.desksOccupancyPercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.locationAnalytics.desksOccupancyPercentage.map(({ date, percentage }) => ({
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
      <CardHeader
        title={
          <>
            <Typography variant="h5" color="primary">
              DeskOccupancy Insights
            </Typography>
            {!hideLocationDetails && <LocationLink organizationId={organizationId} id={locationId} name={rootData.location?.name} analayticsLink />}
          </>
        }
        subheader={
          <Stack direction="row" sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
            <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
          </Stack>
        }
      />
      <CardContent>
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'percentage' }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(LocationDeskOccupancyInsight);
