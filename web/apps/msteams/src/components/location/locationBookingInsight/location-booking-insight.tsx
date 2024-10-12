import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { toDayAndMonthDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationLink } from 'components/location';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { useFragment, useRefetchableFragment } from 'react-relay';
import type { locationBookingInsight_locationAnalytics_query$key } from './__generated__/locationBookingInsight_locationAnalytics_query.graphql';
import type { locationBookingInsight_query$key } from './__generated__/locationBookingInsight_query.graphql';

type Props = {
  rootDataRelay: locationBookingInsight_query$key;
  rootDataLocationAnalyticsRelay: locationBookingInsight_locationAnalytics_query$key;
  organizationId: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const LocationBookingInsight = ({ rootDataRelay, rootDataLocationAnalyticsRelay, organizationId, locationId, hideLocationDetails }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationBookingInsight_query on Query {
        location(id: $locationId) {
          name
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataLocationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment locationBookingInsight_locationAnalytics_query on Query
      @refetchable(queryName: "locationBookingInsight_locationAnalytics_refetchableFragment") {
        locationAnalytics(locationId: $locationId, from: $from, until: $to) @include(if: $locationExists) {
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
      <CardHeader
        title={
          <>
            <Typography variant="h5" color="primary">
              Booking Insights
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
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'total' }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(LocationBookingInsight);
