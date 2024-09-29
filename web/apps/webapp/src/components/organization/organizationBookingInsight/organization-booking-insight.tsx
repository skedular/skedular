import type { organizationBookingInsight_query$key } from '@/queries/__generated__/organizationBookingInsight_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { toDayAndMonthDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationBookingInsight_query$key;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationBookingInsight = ({ rootDataRelay, organizationId, hideOrganizationDetails }: Props) => {
  const [rootData, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationBookingInsight_query on Query @refetchable(queryName: "organizationBookingInsight_organizationAnalytics") {
        organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {
          dailyBookingsTotals {
            date
            total
          }
        }
        organization(id: $organizationId) {
          name
          logoUrl
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
            organizationId,
            from: from.toISOString(),
            to: to.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch, organizationId],
  );

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  const dataset =
    rootData.organizationAnalytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.organizationAnalytics.dailyBookingsTotals.map(({ date, total }) => ({
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
            {!hideOrganizationDetails && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <OrganizationAvatar name={{ name: rootData.organization?.name }} photo={{ url: rootData.organization?.logoUrl }} size="small" />
                <Link component={NextLink} href={`/organization/${organizationId}?tab=analytics`}>
                  {rootData.organization?.name && <Typography variant="h6">{rootData.organization?.name}</Typography>}
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

export default memo(OrganizationBookingInsight);
