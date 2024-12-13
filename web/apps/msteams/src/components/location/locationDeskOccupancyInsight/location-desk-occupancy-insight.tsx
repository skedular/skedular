import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { SectionIconTypography } from '@repo/shared/components/commons';
import { toDayAndMonthDate, toFixed } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationLink } from 'components/location';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { useFragment, useRefetchableFragment } from 'react-relay';
import type { locationDeskOccupancyInsight_locationAnalytics_query$key } from './__generated__/locationDeskOccupancyInsight_locationAnalytics_query.graphql';
import type { locationDeskOccupancyInsight_query$key } from './__generated__/locationDeskOccupancyInsight_query.graphql';

type Props = {
  rootDataRelay: locationDeskOccupancyInsight_query$key;
  rootDataLocationAnalyticsRelay: locationDeskOccupancyInsight_locationAnalytics_query$key;
  organizationId: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const LocationDeskOccupancyInsight = ({ rootDataRelay, rootDataLocationAnalyticsRelay, organizationId, locationId, hideLocationDetails }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationDeskOccupancyInsight_query on Query {
        location(id: $locationId) {
          name
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataLocationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment locationDeskOccupancyInsight_locationAnalytics_query on Query
      @refetchable(queryName: "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment") {
        locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {
          desksOccupancyPercentage {
            date
            percentage
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

  if (!rootDataLocationAnalytics.locationAnalytics) {
    return <></>;
  }

  const dataset =
    rootDataLocationAnalytics.locationAnalytics.desksOccupancyPercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataLocationAnalytics.locationAnalytics.desksOccupancyPercentage.map(({ date, percentage }) => ({
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
            <SectionIconTypography label="Desk Occupancy Insights" />
            {!hideLocationDetails && <LocationLink organizationId={organizationId} id={locationId} name={rootData.location?.name} analayticsLink />}
          </>
        }
        subheader={<AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />}
      />
      <CardContent>
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'percentage' }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(LocationDeskOccupancyInsight);
