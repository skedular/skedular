import type { organizationMemberAttendancyInsight_query$key } from '@/queries/__generated__/organizationMemberAttendancyInsight_query.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { axisClasses } from '@mui/x-charts';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { toDayAndMonthDate, toFixed } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import NextLink from 'next/link';
import { memo, useCallback, useTransition } from 'react';
import { graphql, useRefetchableFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizationMemberAttendancyInsight_query$key;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationMemberAttendancyInsight = ({ rootDataRelay, organizationId, hideOrganizationDetails }: Props) => {
  const [rootData, refetch] = useRefetchableFragment(
    graphql`
      fragment organizationMemberAttendancyInsight_query on Query
      @refetchable(queryName: "organizationMemberAttendancyInsight_organizationAnalytics") {
        organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {
          memberAttendancePercentage {
            date
            percentage
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
    rootData.organizationAnalytics.memberAttendancePercentage.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootData.organizationAnalytics.memberAttendancePercentage.map(({ date, percentage }) => ({
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
            <Typography variant="h5" color="primary">
              Member Attendancy Insights
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
