'use client';

import DashboardLayout from '@/components/dashboard-layout/DashboardLayout';
import type { locationsHostDataQuery } from '@/queries/__generated__/locationsHostDataQuery.graphql';
import type { locationsHostOrganizationQuery } from '@/queries/__generated__/locationsHostOrganizationQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Stack from '@mui/material/Stack';
import { BodyIconTypography, MediumHeadingIconTypography, SmallIconTypography } from '@skedular/ui';
import Link from 'next/link';
import { graphql, useLazyLoadQuery } from 'react-relay';

const HostLocations = ({ organizationId }: { organizationId: string }) => {
  const data = useLazyLoadQuery<locationsHostDataQuery>(
    graphql`
      query locationsHostDataQuery($organizationId: String!) {
        myLocations(organizationId: $organizationId) {
          id
          name
          timezone
          physicalAddress {
            multilinesFormattedAddress
          }
          products {
            id
            pricingOptions {
              billingMode
              purchaseCadence
              cancellationPolicyType
            }
          }
        }
      }
    `,
    { organizationId },
    { fetchPolicy: 'store-and-network' },
  );
  const locations = data.myLocations ?? [];

  if (locations.length === 0) {
    return (
      <Stack spacing={2} sx={{ alignItems: 'center', border: '1px dashed', borderColor: 'divider', borderRadius: 3, p: 6 }}>
        <MediumHeadingIconTypography label="No locations yet" />
        <BodyIconTypography label="Add your first place, then complete pricing and publish setup." />
        <Button component={Link} href="/locations/create" variant="contained">
          Add location
        </Button>
      </Stack>
    );
  }

  return (
    <Grid container spacing={3}>
      {locations.map((location) => {
        // Listing setup state is derived from the linked hidden listing configuration.
        const firstProduct = location.products?.[0];
        const billingMode = firstProduct?.pricingOptions?.[0]?.billingMode;
        const cadenceLabel = firstProduct?.pricingOptions?.[0]?.purchaseCadence;
        const isReady = Boolean(firstProduct);

        return (
          <Grid key={location.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <Card variant="outlined" sx={{ height: '100%' }}>
              <CardContent>
                <Stack spacing={1}>
                  <MediumHeadingIconTypography label={location.name} />
                  <BodyIconTypography label={location.physicalAddress?.multilinesFormattedAddress ?? 'No address provided'} />
                  <SmallIconTypography label={isReady ? 'Listing setup ready' : 'Setup required'} />
                  {!isReady ? <SmallIconTypography label="Add pricing to publish" /> : null}
                  {billingMode && cadenceLabel ? <SmallIconTypography color="primary" label={`${billingMode} - ${cadenceLabel}`} /> : null}
                  <SmallIconTypography label={location.timezone ?? 'Timezone not set'} />
                </Stack>
              </CardContent>
            </Card>
          </Grid>
        );
      })}
    </Grid>
  );
};

const LocationsPage = () => {
  const data = useLazyLoadQuery<locationsHostOrganizationQuery>(
    graphql`
      query locationsHostOrganizationQuery {
        myOrganizations(types: [HOST]) {
          uniqueId
        }
      }
    `,
    {},
  );
  const organization = data.myOrganizations[0];

  return (
    <DashboardLayout>
      <Container maxWidth="xl">
        <Stack spacing={4} sx={{ py: 4 }}>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ justifyContent: 'space-between', alignItems: { sm: 'center' } }}>
            <Box>
              <MediumHeadingIconTypography label="My locations" />
              <BodyIconTypography label="Manage the places you list on Skedular Host." />
            </Box>
            <Button component={Link} href="/locations/create" variant="contained">
              Add location
            </Button>
          </Stack>
          {organization ? <HostLocations organizationId={organization.uniqueId} /> : <BodyIconTypography label="Create a Host organization to add locations." />}
        </Stack>
      </Container>
    </DashboardLayout>
  );
};

export default LocationsPage;
