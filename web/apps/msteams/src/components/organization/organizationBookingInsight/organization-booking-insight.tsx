import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { BarChart } from '@mui/x-charts/BarChart';
import { AnalyticsDaterangeSelector } from '@repo/shared/components/analytics';
import { SectionIconTypography } from '@repo/shared/components/commons';
import { toDayAndMonthDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationLink } from 'components/organization';
import { Dayjs } from 'dayjs';
import { memo, useCallback, useTransition } from 'react';
import { useFragment, useRefetchableFragment } from 'react-relay';
import type { organizationBookingInsight_organizationAnalytics_query$key } from './__generated__/organizationBookingInsight_organizationAnalytics_query.graphql';
import type { organizationBookingInsight_query$key } from './__generated__/organizationBookingInsight_query.graphql';

type Props = {
  rootDataRelay: organizationBookingInsight_query$key;
  rootDataOrganizationAnalyticsRelay: organizationBookingInsight_organizationAnalytics_query$key;
  organizationId: string;
  hideOrganizationDetails?: boolean;
};

const OrganizationBookingInsight = ({ rootDataRelay, rootDataOrganizationAnalyticsRelay, organizationId, hideOrganizationDetails }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationBookingInsight_query on Query {
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
      fragment organizationBookingInsight_organizationAnalytics_query on Query
      @refetchable(queryName: "organizationBookingInsight_organizationAnalytics_refetchableFragment") {
        organizationAnalytics(organizationId: $organizationId, from: $from, until: $to) {
          dailyBookingsTotals {
            date
            total
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
    return <></>;
  }

  const dataset =
    rootDataOrganizationAnalytics.organizationAnalytics.dailyBookingsTotals.length === 0
      ? [{ date: 'No data available', percentage: 0 }]
      : rootDataOrganizationAnalytics.organizationAnalytics.dailyBookingsTotals.map(({ date, total }) => ({
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
            <SectionIconTypography label="Booking Insights" invertDefaultColor />
            {!hideOrganizationDetails && <OrganizationLink id={organizationId} name={rootData.organization?.name} analayticsLink />}
          </>
        }
      />
      <CardContent>
        <AnalyticsDaterangeSelector defaultPeriod="month" onDateRangeChange={handleDateRangeChange} />
        <BarChart dataset={dataset} xAxis={[{ scaleType: 'band', dataKey: 'date' }]} series={[{ dataKey: 'total' }]} {...chartSettings} />
      </CardContent>
    </Card>
  );
};

export default memo(OrganizationBookingInsight);
