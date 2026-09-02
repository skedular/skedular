'use client';

import CommissionHistory, { type CommissionEntry } from '@/components/commission-history/CommissionHistory';
import DashboardLayout from '@/components/dashboard-layout/DashboardLayout';
import type { hostOperationsQuery } from '@/queries/__generated__/hostOperationsQuery.graphql';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Stack from '@mui/material/Stack';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import { BodyIconTypography, MediumHeadingIconTypography, SmallIconTypography } from '@skedular/ui';
import Link from 'next/link';
import { graphql, useLazyLoadQuery } from 'react-relay';

export type HostOperationsSection = 'products' | 'bookings' | 'commissions' | 'organization' | 'settings';

const currency = (value: number | null | undefined) => (value ?? 0).toLocaleString('en-US', { style: 'currency', currency: 'USD' });

const HostOperations = ({ section }: { section: HostOperationsSection }) => {
  const data = useLazyLoadQuery<hostOperationsQuery>(
    graphql`
      query hostOperationsQuery {
        myOrganizations(types: [HOST]) {
          uniqueId
          name
          customDomain
          logoUrl
          website
          contactEmail
          contactPhone
        }
      }
    `,
    {},
    { fetchPolicy: 'store-and-network' },
  );
  const organization = data.myOrganizations[0];

  if (!organization) {
    return (
      <DashboardLayout>
        <Alert severity="info" action={<Button href="/onboarding">Create organization</Button>}>
          Create a Host organization before managing listings.
        </Alert>
      </DashboardLayout>
    );
  }

  return (
    <DashboardLayout>
      {section === 'products' ? <Products organizationId={organization.uniqueId} /> : null}
      {section === 'bookings' ? <Bookings organizationId={organization.uniqueId} showCommission={false} /> : null}
      {section === 'commissions' ? <Bookings organizationId={organization.uniqueId} showCommission /> : null}
      {section === 'organization' || section === 'settings' ? (
        organization.customDomain ? (
          <Organization customDomain={organization.customDomain} />
        ) : (
          <Alert severity="warning">Configure an organization URL to manage public Host details.</Alert>
        )
      ) : null}
    </DashboardLayout>
  );
};

const Products = ({ organizationId }: { organizationId: string }) => {
  const data = useLazyLoadQuery(
    graphql`
      query hostOperationsProductsQuery($organizationId: String!) {
        myLocations(organizationId: $organizationId) {
          id
          name
          products {
            id
            inactive
            listingMetadata {
              title
              about
            }
            type {
              name
            }
            currency {
              name
            }
            pricingOptions {
              id
              price
              purchaseCadence
            }
          }
        }
      }
    `,
    { organizationId },
    { fetchPolicy: 'store-and-network' },
  ) as {
    myLocations?: ReadonlyArray<{
      id: string;
      name: string;
      products: ReadonlyArray<{
        id: string;
        inactive: boolean;
        listingMetadata: { title?: string | null; about?: string | null };
        type: { name: string };
        currency: { name: string };
        pricingOptions: ReadonlyArray<{ id: string; price: number; purchaseCadence: string }>;
      }>;
    }> | null;
  };
  const products = (data.myLocations ?? []).flatMap((location) => location.products.map((product) => ({ location, product })));

  return (
    <Stack spacing={3}>
      <Box>
        <MediumHeadingIconTypography label="Products" />
        <BodyIconTypography label="Offer the same location with different hourly, daily, monthly, or contract pricing." />
      </Box>
      {products.length === 0 ? (
        <Alert severity="info" action={<Button href="/locations">View locations</Button>}>
          Add a location before creating a product.
        </Alert>
      ) : (
        <Stack spacing={2}>
          {products.map(({ location, product }) => (
            <Card key={product.id} variant="outlined">
              <CardContent>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ justifyContent: 'space-between' }}>
                  <Box>
                    <MediumHeadingIconTypography label={product.listingMetadata.title || 'Hosted place'} />
                    <BodyIconTypography label={location.name} />
                    <SmallIconTypography label={product.listingMetadata.about || 'Full-place booking'} />
                  </Box>
                  <Stack spacing={1} sx={{ alignItems: { sm: 'flex-end' } }}>
                    <Chip label={product.inactive ? 'Draft' : 'Active'} color={product.inactive ? 'default' : 'success'} size="small" />
                    {product.pricingOptions.map((pricing) => (
                      <SmallIconTypography key={pricing.id} label={`${pricing.price} ${product.currency.name} · ${pricing.purchaseCadence}`} />
                    ))}
                    <Button component={Link} href={`/products/${product.id}/edit?locationId=${location.id}`}>
                      Edit product
                    </Button>
                  </Stack>
                </Stack>
              </CardContent>
            </Card>
          ))}
        </Stack>
      )}
    </Stack>
  );
};

const Bookings = ({ organizationId, showCommission }: { organizationId: string; showCommission: boolean }) => {
  const data = useLazyLoadQuery(
    graphql`
      query hostOperationsBookingsQuery($organizationId: String!) {
        bookings(first: 100, where: { organizationId: $organizationId }, orderBy: [{ field: FROM, direction: DESCENDING }]) {
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
  ) as {
    bookings: {
      edges: ReadonlyArray<{
        node: {
          id: string;
          from: string;
          until: string;
          involvedLocations: ReadonlyArray<{ name: string }>;
          marketplaceBooking?: {
            paymentStatus: { name: string };
            totalAmount?: number | null;
            hostCommissionRatePercentage?: number | null;
            hostCommissionAmount?: number | null;
            hostGrossProceedsAmount?: number | null;
          } | null;
        };
      }>;
    };
  };
  const bookings = data.bookings.edges.map(({ node }) => node);
  const commissions: CommissionEntry[] = bookings.flatMap((booking) =>
    booking.marketplaceBooking?.hostCommissionAmount == null
      ? []
      : [
          {
            bookingId: booking.id,
            bookingValue: booking.marketplaceBooking.totalAmount ?? 0,
            commission: booking.marketplaceBooking.hostCommissionAmount,
            rate: booking.marketplaceBooking.hostCommissionRatePercentage ?? 0,
            hostPayout: booking.marketplaceBooking.hostGrossProceedsAmount ?? 0,
            date: booking.from,
          },
        ],
  );

  if (showCommission) {
    return (
      <Stack spacing={3}>
        <Box>
          <MediumHeadingIconTypography label="Payments and commission" />
          <BodyIconTypography label="Card payments are processed through Stripe Connect. Skedular retains the offering commission and transfers the Host proceeds." />
        </Box>
        <CommissionHistory entries={commissions} />
      </Stack>
    );
  }

  return (
    <Stack spacing={3}>
      <Box>
        <MediumHeadingIconTypography label="Bookings" />
        <BodyIconTypography label="Bookings reserve the hidden entire-location resource through the standard booking engine." />
      </Box>
      <Table>
        <TableHead>
          <TableRow sx={{ '& th': { borderBottom: 1, borderColor: 'divider' } }}>
            <TableCell>Location</TableCell>
            <TableCell>From</TableCell>
            <TableCell>Until</TableCell>
            <TableCell>Status</TableCell>
            <TableCell align="right">Value</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {bookings.map((booking) => (
            <TableRow key={booking.id}>
              <TableCell>{booking.involvedLocations[0]?.name ?? 'Hosted place'}</TableCell>
              <TableCell>{new Date(booking.from).toLocaleDateString()}</TableCell>
              <TableCell>{new Date(booking.until).toLocaleDateString()}</TableCell>
              <TableCell>{booking.marketplaceBooking?.paymentStatus.name ?? 'Not set'}</TableCell>
              <TableCell align="right">{currency(booking.marketplaceBooking?.totalAmount)}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </Stack>
  );
};

type OrganizationData = {
  name: string;
  customDomain?: string | null;
  website?: string | null;
  contactEmail?: string | null;
  contactPhone?: string | null;
  isOwnershipVerified: boolean;
  marketplaceListingMetadata: { about?: string | null };
};

const Organization = ({ customDomain }: { customDomain: string }) => {
  const data = useLazyLoadQuery(
    graphql`
      query hostOperationsOrganizationQuery($customDomain: String!) {
        organization(customDomain: $customDomain) {
          name
          customDomain
          website
          contactEmail
          contactPhone
          isOwnershipVerified
          marketplaceListingMetadata {
            about
          }
        }
      }
    `,
    { customDomain },
    { fetchPolicy: 'store-and-network' },
  ) as { organization?: OrganizationData | null };
  const organization = data.organization;

  if (!organization) return <Alert severity="warning">The Host organization could not be loaded.</Alert>;

  return (
    <Stack spacing={3}>
      <Box>
        <MediumHeadingIconTypography label="Organization" />
        <BodyIconTypography label="Manage the business details used across your Host listings and customer communications." />
      </Box>
      {!organization.isOwnershipVerified ? (
        <Alert severity="warning">Ownership verification is in progress. You can prepare draft listings, but they cannot be published yet.</Alert>
      ) : (
        <Alert severity="success">Ownership verified. Active listings can appear in the marketplace.</Alert>
      )}
      <Card variant="outlined">
        <CardContent>
          <Stack spacing={1.5}>
            <MediumHeadingIconTypography label={organization.name} />
            <BodyIconTypography label={organization.marketplaceListingMetadata.about || 'No organization description has been added.'} />
            <SmallIconTypography label={`URL: ${organization.customDomain || 'Not configured'}`} />
            <SmallIconTypography label={`Website: ${organization.website || 'Not configured'}`} />
            <SmallIconTypography label={`Email: ${organization.contactEmail || 'Not configured'}`} />
            <SmallIconTypography label={`Phone: ${organization.contactPhone || 'Not configured'}`} />
          </Stack>
        </CardContent>
      </Card>
    </Stack>
  );
};

export default HostOperations;
