'use client';

import CommissionHistory, { type CommissionEntry } from '@/components/commission-history/CommissionHistory';
import DashboardLayout from '@/components/dashboard-layout/DashboardLayout';
import type { dashboardHostDataQuery } from '@/queries/__generated__/dashboardHostDataQuery.graphql';
import type { dashboardHostOrganizationQuery } from '@/queries/__generated__/dashboardHostOrganizationQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Grid from '@mui/material/Grid';
import Stack from '@mui/material/Stack';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import { BodyIconTypography, LeadIconTypography, MediumHeadingIconTypography } from '@skedular/ui';
import { graphql, useLazyLoadQuery } from 'react-relay';

const HostDashboardData = ({ organizationId }: { organizationId: string }) => {
  const data = useLazyLoadQuery<dashboardHostDataQuery>(
    graphql`
      query dashboardHostDataQuery($organizationId: String!) {
        myLocations(organizationId: $organizationId) {
          id
        }
        products(first: 1, where: { organizationIds: [$organizationId], includeInactive: true }) {
          totalCount
        }
        bookings(first: 100, where: { organizationId: $organizationId }, orderBy: [{ field: FROM, direction: DESCENDING }]) {
          totalCount
          edges {
            node {
              id
              from
              until
              involvedLocations {
                name
              }
              marketplaceBooking {
                paymentStatus {
                  name
                }
                totalAmount
                hostCommissionRatePercentage
                hostCommissionAmount
                hostGrossProceedsAmount
              }
            }
          }
        }
      }
    `,
    { organizationId },
    { fetchPolicy: 'store-and-network' },
  );

  const entries: CommissionEntry[] = data.bookings.edges.flatMap(({ node }) =>
    node.marketplaceBooking?.hostCommissionAmount == null
      ? []
      : [
          {
            bookingId: node.id,
            bookingValue: node.marketplaceBooking.totalAmount ?? 0,
            commission: node.marketplaceBooking.hostCommissionAmount,
            rate: node.marketplaceBooking.hostCommissionRatePercentage ?? 0,
            hostPayout: node.marketplaceBooking.hostGrossProceedsAmount ?? 0,
            date: node.from,
          },
        ],
  );
  const totalCommission = entries.reduce((total, entry) => total + entry.commission, 0);

  return (
    <Stack spacing={4}>
      <Grid container spacing={2}>
        {[
          ['Locations', data.myLocations?.length ?? 0],
          ['Products', data.products.totalCount],
          ['Bookings', data.bookings.totalCount],
          ['Commission', totalCommission.toLocaleString('en-US', { style: 'currency', currency: 'USD' })],
        ].map(([label, value]) => (
          <Grid key={label} size={{ xs: 12, sm: 6, md: 3 }}>
            <Card variant="outlined">
              <CardContent>
                <BodyIconTypography label={String(label)} />
                <MediumHeadingIconTypography label={String(value)} />
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Stack spacing={2}>
        <LeadIconTypography label="Recent bookings" />
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Location</TableCell>
              <TableCell>From</TableCell>
              <TableCell>Until</TableCell>
              <TableCell>Status</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data.bookings.edges.map(({ node }) => (
              <TableRow key={node.id}>
                <TableCell>{node.involvedLocations[0]?.name ?? 'Hosted place'}</TableCell>
                <TableCell>{new Date(node.from).toLocaleDateString()}</TableCell>
                <TableCell>{new Date(node.until).toLocaleDateString()}</TableCell>
                <TableCell>{node.marketplaceBooking?.paymentStatus.name ?? 'Not set'}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Stack>

      <Stack spacing={2}>
        <LeadIconTypography label="Commission history" />
        <CommissionHistory entries={entries} />
      </Stack>
    </Stack>
  );
};

const DashboardPage = () => {
  const data = useLazyLoadQuery<dashboardHostOrganizationQuery>(
    graphql`
      query dashboardHostOrganizationQuery {
        myOrganizations(types: [HOST]) {
          uniqueId
          name
        }
      }
    `,
    {},
  );
  const organization = data.myOrganizations[0];

  return (
    <DashboardLayout>
      <Stack spacing={3}>
        <MediumHeadingIconTypography label={organization ? `${organization.name} dashboard` : 'Host dashboard'} />
        {organization ? <HostDashboardData organizationId={organization.uniqueId} /> : <BodyIconTypography label="Create a Host organization to get started." />}
      </Stack>
    </DashboardLayout>
  );
};

export default DashboardPage;
