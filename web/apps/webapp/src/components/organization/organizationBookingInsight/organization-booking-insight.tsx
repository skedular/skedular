import { AnalyticsDaterangeSelector } from '@/components/analytics';
import { SectionIconTypography } from '@/components/commons';
import { toDayAndMonthDate } from '@/libs/utils';
import type { organizationBookingInsight_organizationAnalytics_query$key } from '@/queries/__generated__/organizationBookingInsight_organizationAnalytics_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { BarChart } from '@mui/x-charts/BarChart';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataOrganizationAnalyticsRelay: organizationBookingInsight_organizationAnalytics_query$key;
  organizationUniqueAlphanumericName: string;
};

const OrganizationBookingInsight = ({ rootDataOrganizationAnalyticsRelay, organizationUniqueAlphanumericName }: Props) => {
  const [rootDataOrganizationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationBookingInsight_organizationAnalytics_query on Query @refetchable(queryName: "organizationBookingInsight_organizationAnalytics_refetchableFragment") {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
          analytics(from: $from, until: $to) {
            dailyBookingsTotals {
              date
              total
            }
          }
        }
      }
    `,
    rootDataOrganizationAnalyticsRelay,
  );

  const [, startTransition] = useTransition();

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs) => {
      startTransition(() => {
        refetch(
          {
            organizationUniqueAlphanumericName,
            from: from.toISOString(),
            to: to.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetch, organizationUniqueAlphanumericName],
  );

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  if (!rootDataOrganizationAnalytics.organization) {
    return <></>;
  }

  const dataset =
    rootDataOrganizationAnalytics.organization.analytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataOrganizationAnalytics.organization.analytics.dailyBookingsTotals.map(({ date, total }) => ({
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

export default memo(OrganizationBookingInsight);
