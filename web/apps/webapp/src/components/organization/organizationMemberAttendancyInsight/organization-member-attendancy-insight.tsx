import { AnalyticsDaterangeSelector } from '@/components/analytics';
import { SectionIconTypography } from '@/components/commons';
import { toDayAndMonthDate, toFixed } from '@/libs/utils';
import type { organizationMemberAttendancyInsight_organizationAnalytics_query$key } from '@/queries/__generated__/organizationMemberAttendancyInsight_organizationAnalytics_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataOrganizationAnalyticsRelay: organizationMemberAttendancyInsight_organizationAnalytics_query$key;
  organizationUniqueAlphanumericName: string;
};

const OrganizationMemberAttendancyInsight = ({ rootDataOrganizationAnalyticsRelay, organizationUniqueAlphanumericName }: Props) => {
  const [rootDataOrganizationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query
      @refetchable(queryName: "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment") {
        organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {
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
    [refetch, organizationUniqueAlphanumericName],
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
    width: 500,
    height: 300,
    sx: {
      [`.${axisClasses.left} .${axisClasses.label}`]: {
        transform: 'translate(-8px, 0)',
      },
    },
  };

  const valueFormatter = (value: number | null) => `${value}%`;

  return (
    <Card sx={{ maxWidth: 500, height: '100%' }}>
      <CardHeader title={<SectionIconTypography label="Member Attendancy Insights" invertDefaultColor />} />
      <CardContent>
        <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'percentage', valueFormatter }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(OrganizationMemberAttendancyInsight);
