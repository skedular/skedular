'use client';

import DashboardLayout from '@/components/dashboard-layout/DashboardLayout';
import { getOrganizationLocationOpeningHoursBaseLink, getOrganizationLocationPricingBaseLink, getOrganizationLocationSetupBaseLink } from '@/components/links';
import type { hostLocationDetailsQuery } from '@/queries/__generated__/hostLocationDetailsQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import Stack from '@mui/material/Stack';
import { useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, MediumHeadingIconTypography, SmallIconTypography } from '@skedular/ui';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { graphql, useLazyLoadQuery } from 'react-relay';

const LocationPage = () => {
  const { id: locationId } = useParams<{ id: string }>();
  const { integratedPlatform } = useIntegratedPlatform();
  const data = useLazyLoadQuery<hostLocationDetailsQuery>(
    graphql`
      query hostLocationDetailsQuery($locationId: String!) {
        location(id: $locationId) {
          id
          name
          timezone
          type {
            name
          }
          organization {
            customDomain
          }
          physicalAddress {
            multilinesFormattedAddress
            latitude
            longitude
          }
          extraMetadata {
            peopleCapacity {
              from
              to
            }
            contactDetails {
              contactEmails
              contactPhones
            }
          }
          products {
            id
            inactive
            listingMetadata {
              title
            }
            pricingOptions {
              price
              purchaseCadence
            }
            currency {
              name
            }
          }
        }
      }
    `,
    { locationId },
    { fetchPolicy: 'store-and-network' },
  );
  const location = data.location;
  const orgDomain = location?.organization?.customDomain ?? '';
  const firstProduct = location?.products?.[0];
  const hasLinkedProduct = Boolean(firstProduct);
  const hasPricing = Boolean(firstProduct?.pricingOptions?.length);
  const isPublished = Boolean(firstProduct && !firstProduct.inactive);

  return (
    <DashboardLayout>
      <Container maxWidth="xl">
        <Stack spacing={4} sx={{ py: 4 }}>
          <Button component={Link} href="/locations" sx={{ alignSelf: 'flex-start' }}>
            ← Locations
          </Button>
          {location ? (
            <>
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ justifyContent: 'space-between', alignItems: { sm: 'center' } }}>
                <Box>
                  <MediumHeadingIconTypography label={location.name} />
                  <BodyIconTypography label={location.physicalAddress?.multilinesFormattedAddress ?? 'No address provided'} />
                </Box>
                <Button component={Link} href={getOrganizationLocationSetupBaseLink(integratedPlatform, orgDomain, locationId)}>
                  Edit location
                </Button>
              </Stack>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 4 }}>
                  <Card variant="outlined">
                    <CardContent>
                      <SmallIconTypography label="Location setup" />
                      <BodyIconTypography label={hasLinkedProduct ? 'Ready to configure pricing and booking rules.' : 'Setup in progress.'} />
                      {!hasLinkedProduct ? <SmallIconTypography label="Pricing unlocks automatically once setup finishes." /> : null}
                    </CardContent>
                  </Card>
                </Grid>
                <Grid size={{ xs: 12, sm: 4 }}>
                  <Card variant="outlined">
                    <CardContent>
                      <SmallIconTypography label="Pricing setup" />
                      <BodyIconTypography label={hasPricing ? 'Configured' : 'Not configured'} />
                      <SmallIconTypography label={hasPricing ? 'Rates and cancellation rules are available to edit.' : 'Add pricing to continue.'} />
                    </CardContent>
                  </Card>
                </Grid>
                <Grid size={{ xs: 12, sm: 4 }}>
                  <Card variant="outlined">
                    <CardContent>
                      <SmallIconTypography label="Status" />
                      <BodyIconTypography label={isPublished ? 'Active' : 'Draft'} />
                      <SmallIconTypography label={isPublished ? 'Location is available for bookings.' : 'Complete setup to activate booking availability.'} />
                    </CardContent>
                  </Card>
                </Grid>
              </Grid>
              <Stack spacing={2}>
                <MediumHeadingIconTypography label="Manage this location" />
                <Grid container spacing={2}>
                  <Grid size={{ xs: 12, md: 6 }}>
                    <Card variant="outlined">
                      <CardContent>
                        <Stack spacing={2}>
                          <SmallIconTypography label="Managing the location" />
                          <BodyIconTypography label="Update address, timezone, and basic location details." />
                          <Button component={Link} href={getOrganizationLocationSetupBaseLink(integratedPlatform, orgDomain, locationId)} sx={{ alignSelf: 'flex-start' }}>
                            Edit details
                          </Button>
                        </Stack>
                      </CardContent>
                    </Card>
                  </Grid>
                  <Grid size={{ xs: 12, md: 6 }}>
                    <Card variant="outlined">
                      <CardContent>
                        <Stack spacing={2}>
                          <SmallIconTypography label="Opening hours" />
                          <BodyIconTypography label="Set when this location is open for bookings." />
                          <Button component={Link} href={getOrganizationLocationOpeningHoursBaseLink(integratedPlatform, orgDomain, locationId)} sx={{ alignSelf: 'flex-start' }}>
                            Edit opening hours
                          </Button>
                        </Stack>
                      </CardContent>
                    </Card>
                  </Grid>
                  <Grid size={{ xs: 12, md: 6 }}>
                    <Card variant="outlined">
                      <CardContent>
                        <Stack spacing={2}>
                          <SmallIconTypography label="Pricing and cancellation" />
                          <BodyIconTypography
                            label={hasLinkedProduct ? 'Set rates, cancellation policy, and booking rules.' : 'Pricing will unlock automatically after setup is ready.'}
                          />
                          <Button
                            component={Link}
                            href={getOrganizationLocationPricingBaseLink(integratedPlatform, orgDomain, locationId)}
                            sx={{ alignSelf: 'flex-start' }}
                            disabled={!hasLinkedProduct}
                          >
                            Edit pricing
                          </Button>
                        </Stack>
                      </CardContent>
                    </Card>
                  </Grid>
                </Grid>
              </Stack>
            </>
          ) : (
            <BodyIconTypography label="Location not found." />
          )}
        </Stack>
      </Container>
    </DashboardLayout>
  );
};

export default LocationPage;
