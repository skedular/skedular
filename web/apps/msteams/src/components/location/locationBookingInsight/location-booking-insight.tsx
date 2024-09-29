import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { LocationAvatar } from '@repo/shared/components/avatars';
import { toDayAndMonthDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { useRefetchableFragment } from 'react-relay';
import type { locationBookingInsight_query$key } from './__generated__/locationBookingInsight_query.graphql';

type Props = {
  rootDataRelay: locationBookingInsight_query$key;
  organizationId?: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const LocationBookingInsight = ({ rootDataRelay, organizationId, locationId, hideLocationDetails }: Props) => {
  const [rootData, refetch] = useRefetchableFragment(
    graphql`
      fragment locationBookingInsight_query on Query @refetchable(queryName: "locationBookingInsight_organizationAnalytics") {
        locationAnalytics(locationId: $locationId, from: $from, until: $to) {
          dailyBookingsTotals {
            date
            total
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

  const dataset =
    rootData.locationAnalytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.locationAnalytics.dailyBookingsTotals.map(({ date, total }) => ({
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
            {!hideLocationDetails && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <LocationAvatar name={{ name: rootData.location?.name }} photo={{ url: null }} size="small" />
                <Link
                  href={
                    organizationId ? `/organization/${organizationId}/location/${locationId}?tab=analytics` : `/location/${locationId}?tab=analytics`
                  }
                >
                  <Typography variant="h6">{rootData.location?.name}</Typography>
                </Link>
              </Stack>
            )}
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
