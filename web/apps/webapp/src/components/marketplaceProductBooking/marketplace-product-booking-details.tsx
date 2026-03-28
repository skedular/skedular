import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { ArrowLeftIcon } from '@/components/icons';
import { getMarketplaceLocationLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { getCustomerFullName } from '@/libs/utils';
import type { marketplaceProductBookingDetails_rootQuery } from '@/queries/__generated__/marketplaceProductBookingDetails_rootQuery.graphql';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Link from '@mui/material/Link';
import dayjs from 'dayjs';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, ReactNode, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader, useSubscription } from 'react-relay';
import MarketplaceProductBookingDetailsHero from './marketplace-product-booking-details-hero';
import MarketplaceProductBookingPaymentPanel from './marketplace-product-booking-payment-panel';

const RootQuery = graphql`
  query marketplaceProductBookingDetails_rootQuery($bookingId: String!) {
    booking(id: $bookingId) {
      id
      from
      until
      involvedCustomers {
        id
        name
        givenName
        middleName
        familyName
      }
      involvedLocations {
        uniqueId
        name
      }
      bookingResources {
        resource {
          id
          name
        }
      }
      marketplaceBooking {
        id
        quantity
        invoiceUrl
        invoiceNumber
        isPaymentRequired
        paymentExpiry
        productVersion {
          type {
            type
            name
          }
          listingMetadata {
            title
            subTitle
            about
            includedFeatures
          }
          featureImages {
            original {
              url
            }
          }
        }
        bookingCheckoutSession {
          checkoutUrl
        }
        paymentMethod {
          type
          name
        }
        paymentStatus {
          type
          name
        }
      }
      arrearsInvoices {
        invoiceNumber
        invoiceUrl
        billingPeriodStartInclusive
        billingPeriodEndExclusive
      }
    }
  }
`;

const BookingSubscription = graphql`
  subscription marketplaceProductBookingDetails_booking_Subscription($bookingId: String!) {
    booking(id: $bookingId) {
      marketplaceBooking {
        id
        invoiceUrl
        invoiceNumber
        isPaymentRequired
        paymentExpiry
        bookingCheckoutSession {
          checkoutUrl
        }
        paymentStatus {
          type
          name
        }
      }
      arrearsInvoices {
        invoiceNumber
        invoiceUrl
        billingPeriodStartInclusive
        billingPeriodEndExclusive
      }
    }
  }
`;

const MarketplaceProductBookingDetails = ({ queryReference }: { queryReference: PreloadedQuery<marketplaceProductBookingDetails_rootQuery, Record<string, unknown>> }) => {
  const rootData = usePreloadedQuery<marketplaceProductBookingDetails_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const booking = rootData.booking;
  const marketplaceBooking = booking?.marketplaceBooking;

  useSubscription({
    variables: { bookingId: booking?.id ?? '' },
    subscription: BookingSubscription,
  });

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
      }}
    >
      <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        {marketplaceBooking?.productVersion ? (
          <MarketplaceProductBookingDetailsHero
            about={marketplaceBooking.productVersion.listingMetadata.about}
            imageUrl={marketplaceBooking.productVersion.featureImages[0]?.original?.url}
            includedFeatures={marketplaceBooking.productVersion.listingMetadata.includedFeatures}
            subTitle={marketplaceBooking.productVersion.listingMetadata.subTitle}
            title={marketplaceBooking.productVersion.listingMetadata.title}
          />
        ) : null}

        {!booking || !marketplaceBooking ? (
          <Alert severity="info" sx={{ mt: 2, borderRadius: 3 }}>
            We couldn&apos;t find this booking anymore.
          </Alert>
        ) : (
          <Box
            sx={{
              mt: 1,
              display: 'grid',
              gap: { xs: 3, lg: 4 },
              gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.15fr) 380px' },
              alignItems: 'start',
            }}
          >
            <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
              <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
                <CaptionIconTypography label="Booking details" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                <LeadIconTypography label="Review your booking and payment status" sx={{ mt: 1 }} />
                <BodyIconTypography
                  label="This page stays in sync while checkout is being prepared, and it remains the place to return to after payment."
                  sx={{ mt: 1, opacity: 0.82 }}
                />

                <StackRow sx={{ mt: 2, rowGap: 1 }}>
                  <Chip label={marketplaceBooking.paymentStatus.name} color={marketplaceBooking.paymentStatus.type === 'CONFIRMED' ? 'success' : 'default'} />
                  <Chip label={marketplaceBooking.paymentMethod.name} variant="outlined" />
                </StackRow>

                <StackColumn spacing={2} sx={{ mt: 3 }}>
                  <DetailsRow label="Booking date" value={toStoredBookingDate(booking.from)} />
                  <DetailsRow label="Booking time" value={`${toStoredBookingTime(booking.from)} - ${toStoredBookingTime(booking.until)}`} />
                  {marketplaceBooking.productVersion?.type.type !== 'EVENT' ? <DetailsRow label="Quantity" value={`${marketplaceBooking.quantity}`} /> : null}
                  <DetailsRow label="Booked for" value={booking.involvedCustomers.map((item) => getCustomerFullName(item)).join(', ') || 'Not available'} />
                  <DetailsRow
                    label="Location"
                    value={
                      booking.involvedLocations.length > 0 ? (
                        <StackRow sx={{ rowGap: 1 }}>
                          {booking.involvedLocations.map((location) => (
                            <Link
                              key={location.uniqueId}
                              component={NextLink}
                              href={getMarketplaceLocationLink(integratedPlatrform, location.uniqueId)}
                              underline="none"
                              color="inherit"
                              sx={{
                                display: 'inline-flex',
                                width: 'fit-content',
                                px: 1.25,
                                py: 0.75,
                                borderRadius: 2,
                                bgcolor: (theme) => theme.palette.action.hover,
                                border: 1,
                                borderColor: (theme) => theme.palette.divider,
                                transition: 'background-color 120ms ease, border-color 120ms ease, transform 120ms ease',
                                '&:hover': {
                                  bgcolor: (theme) => theme.palette.action.selected,
                                  borderColor: (theme) => theme.palette.primary.main,
                                  transform: 'translateY(-1px)',
                                },
                              }}
                            >
                              <SubtitleIconTypography label={location.name} />
                            </Link>
                          ))}
                        </StackRow>
                      ) : (
                        'Not available'
                      )
                    }
                  />
                  <DetailsRow label="Resources" value={booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later'} />
                </StackColumn>
              </CardContent>
            </Card>

            <MarketplaceProductBookingPaymentPanel
              checkoutUrl={marketplaceBooking.bookingCheckoutSession?.checkoutUrl ?? null}
              entityLabel="booking"
              invoices={booking.arrearsInvoices ?? []}
              invoiceUrl={marketplaceBooking.invoiceUrl ?? null}
              isPaymentRequired={marketplaceBooking.isPaymentRequired}
              paymentExpiry={marketplaceBooking.paymentExpiry}
              paymentMethodType={marketplaceBooking.paymentMethod.type}
              paymentStatusLabel={marketplaceBooking.paymentStatus.name}
              paymentStatusType={marketplaceBooking.paymentStatus.type}
            />
          </Box>
        )}
      </Container>
    </Box>
  );
};

const DetailsRow = ({ label, value }: { label: string; value: ReactNode }) => (
  <StackColumn spacing={0.35}>
    <SmallIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }} />
    {typeof value === 'string' ? <SubtitleIconTypography label={value} /> : value}
  </StackColumn>
);

const toStoredBookingDate = (date?: string | null) => {
  // Marketplace scheduler timestamps are stored as timezone-free wall-clock values in UTC.
  return date ? dayjs.utc(date).format('dddd, Do MMM YYYY') : '';
};

const toStoredBookingTime = (date?: string | null) => {
  // Keep the exact stored time instead of converting it to the browser timezone.
  return date ? dayjs.utc(date).format('hh:mm a') : '';
};

const MemoMarketplaceProductBookingDetails = memo(MarketplaceProductBookingDetails);

const MarketplaceProductBookingDetailsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<marketplaceProductBookingDetails_rootQuery>(RootQuery);
  const { bookingId } = useKnownParams();

  if (!bookingId) {
    throw new Error('bookingId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        bookingId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [bookingId, loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoMarketplaceProductBookingDetails queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(MarketplaceProductBookingDetailsWithRelay);
