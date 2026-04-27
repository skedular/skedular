import { AnalyticsDaterangeSelector, AnalyticsInsightCard } from '@/components/analytics';
import { toDayAndMonthDate, toFixed } from '@skedular/shared';
import type { organizationMemberAttendancyInsight_organizationAnalytics_query$key } from '@/queries/__generated__/organizationMemberAttendancyInsight_organizationAnalytics_query.graphql';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataOrganizationAnalyticsRelay: organizationMemberAttendancyInsight_organizationAnalytics_query$key;
  organizationCustomDomain: string;
};

const OrganizationMemberAttendancyInsight = ({ rootDataOrganizationAnalyticsRelay, organizationCustomDomain }: Props) => {
  const [rootDataOrganizationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query
      @refetchable(queryName: "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment") {
        organization(customDomain: $organizationCustomDomain) {
          analytics(from: $from, until: $to) {
            memberAttendancePercentage {
              date
              percentage
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
    [refetch, organizationCustomDomain],
  );

  const handleDateRangeChange = (from: Dayjs, until: Dayjs) => {
    handleRefetch(from, until);
  };

  if (!rootDataOrganizationAnalytics.organization) {
    return <> </>;
  }

  const dataset =
    rootDataOrganizationAnalytics.organization.analytics.memberAttendancePercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataOrganizationAnalytics.organization.analytics.memberAttendancePercentage.map(({ date, percentage }) => ({
          date: toDayAndMonthDate(date),
          percentage: toFixed(percentage, 2),
        }));

  const chartSettings = {
    yAxis: [
      {
        label: 'Members Attendance Percentage',
        valueFormatter: (value: number) => `${value} %`,
      },
    ],
    width: 440,
    height: 300,
    sx: {
      [`.${axisClasses.left} .${axisClasses.label}`]: {
        transform: 'translate(-8px, 0)',
      },
    },
  };

  const valueFormatter = (value: number | null) => `${value}%`;

  return (
    <AnalyticsInsightCard title="Member Attendancy Insights">
      <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
      <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'percentage', valueFormatter }]} {...chartSettings} />
    </AnalyticsInsightCard>
  );
};

export default memo(OrganizationMemberAttendancyInsight);
