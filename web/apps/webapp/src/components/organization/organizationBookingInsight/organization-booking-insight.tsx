import { AnalyticsDaterangeSelector, AnalyticsInsightCard } from '@/components/analytics';
import { toDayAndMonthDate } from '@skedular/shared';
import type { organizationBookingInsight_organizationAnalytics_query$key } from '@/queries/__generated__/organizationBookingInsight_organizationAnalytics_query.graphql';
import { BarChart } from '@mui/x-charts/BarChart';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataOrganizationAnalyticsRelay: organizationBookingInsight_organizationAnalytics_query$key;
  organizationCustomDomain: string;
};

const OrganizationBookingInsight = ({ rootDataOrganizationAnalyticsRelay, organizationCustomDomain }: Props) => {
  const [rootDataOrganizationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationBookingInsight_organizationAnalytics_query on Query @refetchable(queryName: "organizationBookingInsight_organizationAnalytics_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
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
            organizationCustomDomain,
            from: from.toISOString(),
            to: to.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [startTransition, refetch, organizationCustomDomain],
  );

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  if (!rootDataOrganizationAnalytics.organization) {
    return null;
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
    width: 440,
    height: 300,
  };

  return (
    <AnalyticsInsightCard title="Booking Insights">
      <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
      <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'total' }]} {...chartSettings} />
    </AnalyticsInsightCard>
  );
};

export default memo(OrganizationBookingInsight);
