'use client';

import DashboardLayout from '@/components/dashboard-layout/DashboardLayout';
import { useHostListingCoordinator } from '@/components/unified-listing-form';
import type { createUnifiedHostLocationMutation } from '@/queries/__generated__/createUnifiedHostLocationMutation.graphql';
import type { createUnifiedHostLocationOrganizationQuery } from '@/queries/__generated__/createUnifiedHostLocationOrganizationQuery.graphql';
import type { ProductPricingCadence, ProductPricingCancellationPolicyType } from '@/queries/__generated__/createUnifiedHostUpdateProductMutation.graphql';
import { hostListingProductReadinessQuery } from '@/queries/hostListingProductReadiness';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Container from '@mui/material/Container';
import Grid from '@mui/material/Grid';
import MenuItem from '@mui/material/MenuItem';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TextField from '@mui/material/TextField';
import { BodyIconTypography, MediumHeadingIconTypography, SmallIconTypography } from '@skedular/ui';
import { useRouter } from 'next/navigation';
import { FormEvent, useEffect, useMemo, useRef, useState } from 'react';
import { graphql, useLazyLoadQuery, useMutation, useRelayEnvironment } from 'react-relay';
import { toast } from 'react-toastify';
import { fetchQuery } from 'relay-runtime';

type FormValues = {
  locationName: string;
  addressLine1: string;
  city: string;
  country: string;
  timezone: string;
  listingTitle: string;
  listingAbout: string;
  price: string;
  currency: 'USD' | 'NZD';
  cadence: ProductPricingCadence;
  cancellationPolicyType: ProductPricingCancellationPolicyType;
};

const defaultValues: FormValues = {
  locationName: '',
  addressLine1: '',
  city: '',
  country: '',
  timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
  listingTitle: '',
  listingAbout: '',
  price: '',
  currency: 'USD',
  cadence: 'DAILY',
  cancellationPolicyType: 'NO_CANCELLATION',
};

const cadenceOptions = [
  ['PER_HOUR', 'Hourly'],
  ['DAILY', 'Daily'],
  ['WEEKLY', 'Weekly'],
  ['MONTHLY', 'Monthly'],
] as const;

export const validateCreateListing = (values: FormValues) => {
  const errors: string[] = [];
  if (!values.locationName.trim()) errors.push('Location name is required.');
  if (!values.addressLine1.trim()) errors.push('Street address is required.');
  if (!values.country.trim()) errors.push('Country is required.');
  if (!values.listingTitle.trim()) errors.push('Pricing title is required.');
  if (!values.price.trim() || Number.isNaN(Number(values.price)) || Number(values.price) <= 0) {
    errors.push('Price must be a positive number.');
  }

  return errors;
};

const CreateLocationPage = () => {
  const router = useRouter();
  const environment = useRelayEnvironment();
  const coordinator = useHostListingCoordinator();
  const pollingRef = useRef<number | null>(null);
  const [values, setValues] = useState<FormValues>(defaultValues);
  const [submitting, setSubmitting] = useState(false);
  const [errorSummary, setErrorSummary] = useState<string[]>([]);
  const [statusMessage, setStatusMessage] = useState<string | null>(null);

  const organizationData = useLazyLoadQuery<createUnifiedHostLocationOrganizationQuery>(
    graphql`
      query createUnifiedHostLocationOrganizationQuery {
        myOrganizations(types: [HOST]) {
          uniqueId
        }
      }
    `,
    {},
  );

  const [commitCreateLocation] = useMutation<createUnifiedHostLocationMutation>(graphql`
    mutation createUnifiedHostLocationMutation($input: AddLocationInput!) {
      addLocation(input: $input) {
        location {
          id
          name
        }
      }
    }
  `);

  const [commitUpdateProduct] = useMutation(graphql`
    mutation createUnifiedHostUpdateProductMutation($input: UpdateProductInput!) {
      updateProduct(input: $input) {
        product {
          id
        }
      }
    }
  `);

  const canEditListing = coordinator.canEditProduct;

  const onChange = (field: keyof FormValues) => (event: React.ChangeEvent<HTMLInputElement>) => {
    setValues((current) => ({ ...current, [field]: event.target.value }));
  };

  const stopPolling = () => {
    if (pollingRef.current !== null) {
      window.clearInterval(pollingRef.current);
      pollingRef.current = null;
    }
  };

  useEffect(() => {
    return () => stopPolling();
  }, []);

  const startProductReadinessPolling = (locationId: string) => {
    stopPolling();
    pollingRef.current = window.setInterval(() => {
      fetchQuery(environment, hostListingProductReadinessQuery, { locationId }).subscribe({
        next: (payload: unknown) => {
          const data = payload as { location?: { products?: Array<{ id: string }> } };
          const productId = data.location?.products?.[0]?.id;
          if (!productId) {
            return;
          }

          coordinator.setProductReady(productId);
          stopPolling();
          setStatusMessage('Pricing is now ready. Finishing listing setup...');

          const pendingDraft = coordinator.state.pendingProductDraft;
          if (!pendingDraft) {
            router.push(`/locations/${locationId}`);
            return;
          }

          const pricingId = crypto.randomUUID();
          commitUpdateProduct({
            variables: {
              input: {
                id: productId,
                type: 'EVENT',
                currency: values.currency,
                tagIds: [],
                fieldsToUpdate: ['LISTING_METADATA', 'PRICING_OPTIONS', 'CURRENCY'],
                listingMetadata: {
                  title: pendingDraft.title,
                  subTitle: '',
                  about: pendingDraft.about,
                  includedFeatures: [],
                },
                pricingOptions: [
                  {
                    id: pricingId,
                    index: 0,
                    listingMetadata: {
                      title: pendingDraft.title,
                      subTitle: '',
                      about: pendingDraft.about,
                      includedFeatures: [],
                    },
                    purchaseCadence: values.cadence,
                    bookingCadence: values.cadence,
                    price: pendingDraft.price,
                    isTaxInclusive: false,
                    acceptedPaymentMethods: ['CARD'],
                    minDurationMinutes: values.cadence === 'PER_HOUR' ? 60 : null,
                    maxDurationMinutes: null,
                    maxAllowedResourcesLockTimePaidViaCard: 15,
                    maxAllowedResourcesLockTimePaidViaBankTransfer: 0,
                    numberOfResourcesToBook: 1,
                    billingMode: 'UPFRONT',
                    supportsSubscriptionAutoRenewal: ['MONTHLY'].includes(values.cadence),
                    cancellationPolicyType: values.cancellationPolicyType,
                    cancellationRefundRules: [],
                  },
                ],
              },
            },
            onCompleted: () => {
              coordinator.clearPendingProductDraft();
              router.push(`/locations/${locationId}`);
            },
            onError: (cause) => {
              setStatusMessage('Pricing setup is pending. You can continue from the listing card.');
              toast.warning(`Pricing setup needs retry: ${cause.message}`);
              router.push('/locations');
            },
          });
        },
      });
    }, 2000);
  };

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault();
    const errors = validateCreateListing(values);
    setErrorSummary(errors);
    if (errors.length > 0) {
      return;
    }

    const organizationId = organizationData.myOrganizations[0]?.uniqueId;
    if (!organizationId) {
      setErrorSummary(['Create a Host organization before adding a listing.']);
      return;
    }

    setSubmitting(true);
    setStatusMessage('Creating location...');

    commitCreateLocation({
      variables: {
        input: {
          name: values.locationName.trim(),
          organizationId,
          type: 'MARKETPLACE',
          tagIds: [],
          timezone: values.timezone,
          physicalAddress: {
            addressLine1: values.addressLine1.trim(),
            city: values.city.trim() || null,
            province: null,
            country: values.country.trim(),
            zipcode: '',
            latitude: null,
            longitude: null,
          },
        },
      },
      onCompleted: (response, gqlErrors) => {
        if (gqlErrors?.length) {
          setErrorSummary([gqlErrors[0].message]);
          setSubmitting(false);
          return;
        }

        const locationId = response.addLocation.location.id;
        coordinator.setLocationCreated(locationId);
        coordinator.setPendingProductDraft({
          title: values.listingTitle.trim(),
          about: values.listingAbout.trim(),
          price: Number(values.price),
          cadence: values.cadence,
          cancellationPolicyType: values.cancellationPolicyType,
        });

        setStatusMessage('Location created. Waiting for pricing setup to unlock...');
        startProductReadinessPolling(locationId);
        setSubmitting(false);
      },
      onError: (cause) => {
        setErrorSummary([cause.message]);
        setSubmitting(false);
        setStatusMessage(null);
      },
    });
  };

  const groupedCards = useMemo(
    () => [
      {
        title: 'Location details',
        description: 'Name and address of your place.',
        ready: Boolean(coordinator.state.locationId),
      },
      {
        title: 'Pricing and booking',
        description: canEditListing ? 'Ready to configure.' : 'Unlocks automatically after setup is ready.',
        ready: canEditListing,
      },
      {
        title: 'Media',
        description: canEditListing ? 'Ready to configure.' : 'Unlocks automatically after setup is ready.',
        ready: canEditListing,
      },
    ],
    [canEditListing, coordinator.state.locationId],
  );

  return (
    <DashboardLayout>
      <Container maxWidth="lg">
        <Stack spacing={3} sx={{ py: 4 }}>
          <MediumHeadingIconTypography label="Create listing" />
          <BodyIconTypography label="Start with a few details. Pricing setup unlocks automatically in the background." />

          {errorSummary.length > 0 ? (
            <Alert severity="error">
              <Stack>
                {errorSummary.map((item) => (
                  <SmallIconTypography key={item} label={item} />
                ))}
              </Stack>
            </Alert>
          ) : null}

          {statusMessage ? <Alert severity="info">{statusMessage}</Alert> : null}

          <Grid container spacing={2}>
            {groupedCards.map((item) => (
              <Grid key={item.title} size={{ xs: 12, md: 4 }}>
                <Paper variant="outlined" sx={{ p: 2 }}>
                  <Stack spacing={1}>
                    <SmallIconTypography label={item.title} />
                    <BodyIconTypography label={item.description} />
                    <SmallIconTypography label={item.ready ? 'Ready' : 'Pending'} />
                  </Stack>
                </Paper>
              </Grid>
            ))}
          </Grid>

          <Paper component="form" onSubmit={handleSubmit} variant="outlined" sx={{ p: 3 }}>
            <Stack spacing={3}>
              <Box>
                <SmallIconTypography label="Location" />
              </Box>
              <TextField label="Location name" required value={values.locationName} onChange={onChange('locationName')} />
              <TextField label="Street address" required value={values.addressLine1} onChange={onChange('addressLine1')} />
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 6 }}>
                  <TextField fullWidth label="City" value={values.city} onChange={onChange('city')} />
                </Grid>
                <Grid size={{ xs: 12, md: 6 }}>
                  <TextField fullWidth label="Country" required value={values.country} onChange={onChange('country')} />
                </Grid>
              </Grid>
              <TextField label="Timezone" value={values.timezone} onChange={onChange('timezone')} />

              <Box sx={{ pt: 2 }}>
                <SmallIconTypography label="Pricing and booking" />
              </Box>
              <TextField label="Pricing title" required value={values.listingTitle} onChange={onChange('listingTitle')} />
              <TextField label="Description" multiline minRows={3} value={values.listingAbout} onChange={onChange('listingAbout')} />
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 3 }}>
                  <TextField fullWidth label="Price" required value={values.price} onChange={onChange('price')} />
                </Grid>
                <Grid size={{ xs: 12, md: 3 }}>
                  <TextField select fullWidth label="Currency" value={values.currency} onChange={onChange('currency')}>
                    <MenuItem value="USD">USD</MenuItem>
                    <MenuItem value="NZD">NZD</MenuItem>
                  </TextField>
                </Grid>
                <Grid size={{ xs: 12, md: 3 }}>
                  <TextField select fullWidth label="Cadence" value={values.cadence} onChange={onChange('cadence')}>
                    {cadenceOptions.map(([value, label]) => (
                      <MenuItem key={value} value={value}>
                        {label}
                      </MenuItem>
                    ))}
                  </TextField>
                </Grid>
                <Grid size={{ xs: 12, md: 3 }}>
                  <TextField select fullWidth label="Cancellation" value={values.cancellationPolicyType} onChange={onChange('cancellationPolicyType')}>
                    <MenuItem value="NO_CANCELLATION">No cancellation</MenuItem>
                    <MenuItem value="FULL_REFUND_BEFORE_CUTOFF">Full refund before cutoff</MenuItem>
                    <MenuItem value="TIERED_REFUND">Tiered refund</MenuItem>
                  </TextField>
                </Grid>
              </Grid>

              <Stack direction="row" spacing={2}>
                <Button type="submit" variant="contained" disabled={submitting}>
                  {submitting ? 'Creating...' : 'Create listing'}
                </Button>
                <Button onClick={() => router.push('/locations')}>Cancel</Button>
              </Stack>
            </Stack>
          </Paper>
        </Stack>
      </Container>
    </DashboardLayout>
  );
};

export default CreateLocationPage;
