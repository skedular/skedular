import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { SectionIconTypography } from '@repo/shared/components/commons';
import { toDayAndMonthDate, toFixed } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { useFragment, useRefetchableFragment } from 'react-relay';
import type { organizationMemberAttendancyInsight_organizationAnalytics_query$key } from './__generated__/organizationMemberAttendancyInsight_organizationAnalytics_query.graphql';
import type { organizationMemberAttendancyInsight_query$key } from './__generated__/organizationMemberAttendancyInsight_query.graphql';

type Props = {
  rootDataRelay: organizationMemberAttendancyInsight_query$key;
  rootDataOrganizationAnalyticsRelay: organizationMemberAttendancyInsight_organizationAnalytics_query$key;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationMemberAttendancyInsight = ({
  rootDataRelay,
  rootDataOrganizationAnalyticsRelay,
  organizationId,
  hideOrganizationDetails,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationMemberAttendancyInsight_query on Query {
        organization(id: $organizationId) {
          name
          logoUrl
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataOrganizationAnalytics, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationMemberAttendancyInsight_organizationAnalytics_query on Query
      @refetchable(queryName: "organizationMemberAttendancyInsight_organizationAnalytics_refetchableFragment") {
        organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {
          memberAttendancePercentage {
            date
            percentage
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

  if (!rootDataOrganizationAnalytics.organizationAnalytics) {
    return <> </>;
  }

  const dataset =
    rootDataOrganizationAnalytics.organizationAnalytics.memberAttendancePercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataOrganizationAnalytics.organizationAnalytics.memberAttendancePercentage.map(({ date, percentage }) => ({
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
      <CardHeader
        title={
          <>
            <SectionIconTypography label="Member Attendancy Insights" />
            {!hideOrganizationDetails && <OrganizationLink id={organizationId} name={rootData.organization?.name} analayticsLink />}
          </>
        }
        subheader={<AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />}
      />
      <CardContent>
        <BarChart
          dataset={dataset}
          xAxis={[{ scaleType: 'band', dataKey: 'date' }]}
          series={[{ dataKey: 'percentage', valueFormatter }]}
          {...chartSettings}
        />
      </CardContent>
    </Card>
  );
};

export default memo(OrganizationMemberAttendancyInsight);
