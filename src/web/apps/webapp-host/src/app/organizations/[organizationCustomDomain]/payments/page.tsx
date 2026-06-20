'use client';

import CommissionHistory, { type CommissionEntry } from '@/components/commission-history/CommissionHistory';
import { RootShell } from '@/components/rootShell';
import type { pageHostOrganizationPaymentsQuery } from '@/queries/__generated__/pageHostOrganizationPaymentsQuery.graphql';
import Box from '@mui/material/Box';
import { BodyIconTypography, MediumHeadingIconTypography, StackColumn } from '@skedular/ui';
import { useKnownParams } from '@skedular/shared';
import { graphql, useLazyLoadQuery } from 'react-relay';

const Payments = ({ organizationCustomDomain }: { organizationCustomDomain: string }) => {
  const data = useLazyLoadQuery<pageHostOrganizationPaymentsQuery>(
    graphql`
      query pageHostOrganizationPaymentsQuery($organizationCustomDomain: String!) {
        bookings(first: 100, where: { organizationCustomDomain: $organizationCustomDomain }, orderBy: [{ field: FROM, direction: DESCENDING }]) {
          edges {
            node {
              id
              from
              marketplaceBooking {
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
    { organizationCustomDomain },
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

  return (
    <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center' }}>
      <StackColumn sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pt: { xs: 1, md: 2 } }} spacing={2}>
        <MediumHeadingIconTypography label="Payments and commission" />
        <BodyIconTypography label="Customer card payments are processed through Stripe Connect. Skedular retains the offering commission and transfers the remaining Host proceeds." />
        <CommissionHistory entries={entries} />
      </StackColumn>
    </Box>
  );
};

const Page = () => {
  const { organizationCustomDomain } = useKnownParams();
  if (!organizationCustomDomain) throw new Error('organizationCustomDomain is required');

  return (
    <RootShell>
      <Payments organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

export default Page;
