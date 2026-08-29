'use client';

import { LocationIcon, PaymentStatusIcon, QuantityIcon, ResourceIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink, getMarketplaceEntitlementBookingLink } from '@/components/links';
import { Loading } from '@/components/loading';
import MarketplaceProductBookingPaymentPanel from '@/components/marketplaceProductBooking/marketplace-product-booking-payment-panel';
import MarketplaceProductBookingDetailsHero from '@/components/marketplaceProductBooking/marketplace-product-booking-details-hero';
import SubscriptionCancellationSection from '@/components/marketplaceProductSubscription/subscription-cancellation-section';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { OrganizationStoreFrontRootShell } from '@/components/rootShell';
import useKnownParams from '@/hooks/use-known-params';
import type { entitlementPurchaseDetails_rootQuery } from '@/queries/__generated__/entitlementPurchaseDetails_rootQuery.graphql';
import type { entitlementPurchaseDetails_setRenewalPolicyMutation } from '@/queries/__generated__/entitlementPurchaseDetails_setRenewalPolicyMutation.graphql';
import type { entitlementPurchaseDetails_cancelEntitlementMutation } from '@/queries/__generated__/entitlementPurchaseDetails_cancelEntitlementMutation.graphql';
import Card from '@mui/material/Card';
import Button from '@mui/material/Button';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import Box from '@mui/material/Box';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import {
  BodyIconTypography,
  CaptionIconTypography,
  DefaultDialogTitle,
  LeadIconTypography,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
  TwoButtonsDialogActions,
} from '@skedular/ui';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { useState } from 'react';
import { graphql, useLazyLoadQuery, useMutation, useSubscription } from 'react-relay';
import { useIntegratedPlatform } from '@skedular/shared';
import { MarketplacePurchaseHistoryEventList } from '@/components/marketplacePurchaseHistory/marketplace-purchase-history-event-list';
import { toast } from 'react-toastify';

dayjs.extend(utc);

// Booking timestamps are returned with a UTC marker but represent the organization's wall-clock time.
// Preserve their displayed date/time fields and compare them in the browser's local timezone.
const toLocalBookingWallClock = (timestamp: string) => {
  const timestampParts = dayjs.utc(timestamp);

  return dayjs(
    new Date(
      timestampParts.year(),
      timestampParts.month(),
      timestampParts.date(),
      timestampParts.hour(),
      timestampParts.minute(),
      timestampParts.second(),
      timestampParts.millisecond(),
    ),
  );
};

const RootQuery = graphql`
  query entitlementPurchaseDetails_rootQuery($purchaseId: String!) {
    entitlementPurchase(purchaseId: $purchaseId) {
      id
      history(first: 100) {
        edges {
          node {
            id
            type
            name
            occurredAt
            cancellationRequestedAt
            cancellationEffectiveAt
            paymentStatus
            refundStatus
            creditQuantity
            remainingCreditQuantity
            reason
          }
        }
      }
      paymentStatus
      lifecycleState
      paymentMethod
      paymentExpiry
      serviceStartAt
      pricingId
      productVersion {
        listingMetadata {
          about
          includedFeatures
          subTitle
          title
        }
        featureImages {
          original {
            url
          }
        }
        pricingOptions {
          id
          listingMetadata {
            title
          }
        }
      }
      amount
      currency
      creditQuantity
      validityDays
      invoiceNumber
      invoiceUrl
      paymentAction
      entitlement {
        id
        availableQuantity
        autoRenew
        cancelAtPeriodEnd
        status
        nextRenewalAt
        renewalFailureReason
      }
      linkedBookings(first: 10) {
        totalCount
        edges {
          node {
            id
            from
            until
            involvedLocations {
              name
            }
            bookingResources {
              resource {
                name
              }
            }
            marketplaceBooking {
              quantity
              paymentStatus {
                name
                type
              }
            }
          }
        }
      }
    }
  }
`;

const EntitlementPurchaseUpdates = graphql`
  subscription entitlementPurchaseDetails_subscription_EntitlementPurchase($purchaseId: String!) {
    entitlementPurchase(purchaseId: $purchaseId) {
      id
      paymentStatus
      lifecycleState
      paymentMethod
      paymentExpiry
      serviceStartAt
      amount
      currency
      pricingId
      creditQuantity
      validityDays
      invoiceNumber
      invoiceUrl
      paymentAction
      entitlement {
        id
        availableQuantity
        autoRenew
        cancelAtPeriodEnd
        status
        nextRenewalAt
        renewalFailureReason
      }
    }
  }
`;

const RenewalPolicyMutation = graphql`
  mutation entitlementPurchaseDetails_setRenewalPolicyMutation($input: SetEntitlementRenewalPolicyInput!) {
    setEntitlementRenewalPolicy(input: $input) {
      entitlement {
        id
        autoRenew
        cancelAtPeriodEnd
        status
        nextRenewalAt
        renewalFailureReason
      }
      error
    }
  }
`;

const CancelEntitlementMutation = graphql`
  mutation entitlementPurchaseDetails_cancelEntitlementMutation($input: CancelEntitlementInput!) {
    cancelEntitlement(input: $input) {
      entitlement {
        id
        status
        autoRenew
        cancelAtPeriodEnd
        nextRenewalAt
        renewalFailureReason
      }
      error
    }
  }
`;

const EntitlementPurchaseDetails = () => {
  const { purchaseId } = useParams<{ purchaseId: string }>();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const [linkedBookingsRefreshKey, setLinkedBookingsRefreshKey] = useState(0);
  const data = useLazyLoadQuery<entitlementPurchaseDetails_rootQuery>(RootQuery, { purchaseId }, { fetchKey: linkedBookingsRefreshKey, fetchPolicy: 'network-only' });
  const purchase = data.entitlementPurchase;
  useSubscription({
    variables: { purchaseId: purchase?.id ?? purchaseId },
    subscription: EntitlementPurchaseUpdates,
  });
  const [commitRenewalPolicy, isRenewalPolicyInFlight] = useMutation<entitlementPurchaseDetails_setRenewalPolicyMutation>(RenewalPolicyMutation);
  const [commitCancellation, isCancellationInFlight] = useMutation<entitlementPurchaseDetails_cancelEntitlementMutation>(CancelEntitlementMutation);
  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [showPeriodEndDialog, setShowPeriodEndDialog] = useState(false);

  if (!purchase) return <Loading />;

  const isPending = purchase.paymentStatus === 'PENDING';
  const checkoutUrl = purchase.paymentMethod === 'CARD' && purchase.paymentAction?.startsWith('http') ? purchase.paymentAction : null;
  const serviceStart = dayjs.utc(purchase.serviceStartAt);
  const serviceEnd = serviceStart.add(purchase.validityDays, 'day');
  const productVersion = purchase.productVersion;
  const pricingOption = productVersion.pricingOptions.find((item) => item.id === purchase.pricingId);
  const productTitle = productVersion.listingMetadata.title ?? 'Credit entitlement';
  const pricingOptionTitle = pricingOption?.listingMetadata.title ?? 'Credit package';
  const activeLinkedBookingCount = purchase.linkedBookings.edges.filter(({ node }) => node && !toLocalBookingWallClock(node.until).isBefore(dayjs())).length;
  const hasActiveLinkedBookings = activeLinkedBookingCount > 0;
  const cancelEntitlement = () => {
    if (!purchase.entitlement?.id) return;
    commitCancellation({
      variables: { input: { clientMutationId: purchase.entitlement.id, entitlementId: purchase.entitlement.id, reason: 'Customer cancelled entitlement.' } },
      onCompleted: (response) => {
        if (response.cancelEntitlement.error) toast(<NotificationContent content={response.cancelEntitlement.error} />, errorNotificationOptions);
        else {
          setShowCancelDialog(false);
          setLinkedBookingsRefreshKey((value) => value + 1);
        }
      },
    });
  };
  const cancelAtPeriodEnd = () => {
    if (!purchase.entitlement?.id) return;
    commitRenewalPolicy({
      variables: { input: { clientMutationId: purchase.entitlement.id, entitlementId: purchase.entitlement.id, autoRenew: false, cancelAtPeriodEnd: true } },
      onCompleted: (response) => {
        if (response.setEntitlementRenewalPolicy.error) toast(<NotificationContent content={response.setEntitlementRenewalPolicy.error} />, errorNotificationOptions);
        else {
          setShowPeriodEndDialog(false);
        }
      },
    });
  };

  return (
    <OrganizationStoreFrontRootShell>
      <Box
        sx={{
          minHeight: '100vh',
          pb: 8,
          background:
            'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
        }}
      >
        <Container maxWidth="xl" sx={{ pt: { xs: 3, md: 4 } }}>
          <MarketplaceProductBookingDetailsHero
            about={productVersion.listingMetadata.about}
            imageUrl={productVersion.featureImages[0]?.original?.url}
            includedFeatures={productVersion.listingMetadata.includedFeatures}
            subTitle={productVersion.listingMetadata.subTitle ?? pricingOptionTitle}
            title={productTitle}
          />

          <StackColumn spacing={3} sx={{ mt: 1 }}>
            <Box sx={{ display: 'grid', gap: { xs: 3, lg: 4 }, gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.15fr) 380px' }, alignItems: 'start' }}>
              <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
                <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
                  <CaptionIconTypography label="Entitlement details" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                  <LeadIconTypography label="Review your credits and purchase status" sx={{ mt: 1 }} />
                  <BodyIconTypography label="This is your record of the credit package, its validity period, and every booking that uses it." sx={{ mt: 1, opacity: 0.82 }} />

                  <StackRow sx={{ mt: 2, rowGap: 1 }}>
                    <Chip
                      label={purchase.paymentStatus === 'CONFIRMED' ? 'Payment confirmed' : purchase.paymentStatus}
                      color={purchase.paymentStatus === 'CONFIRMED' ? 'success' : 'default'}
                    />
                    <Chip label={purchase.paymentMethod} variant="outlined" />
                  </StackRow>

                  {purchase.entitlement?.status === 'ACTIVE' && purchase.entitlement.availableQuantity > 0 && purchase.paymentStatus === 'CONFIRMED' ? (
                    <Button
                      component={Link}
                      href={getMarketplaceEntitlementBookingLink(integratedPlatform, purchase.entitlement.id)}
                      variant="contained"
                      sx={{ mt: 2, textTransform: 'none' }}
                    >
                      Book with credits
                    </Button>
                  ) : null}

                  <Divider sx={{ my: 2.5 }} />
                  {purchase.entitlement ? (
                    <StackColumn spacing={2}>
                      <SubscriptionCancellationSection
                        entityLabel="entitlement"
                        isCancelled={purchase.entitlement.status === 'CANCELLED'}
                        cancelAtPeriodEnd={purchase.entitlement.cancelAtPeriodEnd || !purchase.entitlement.autoRenew}
                        hasConfirmedPayment={purchase.paymentStatus === 'CONFIRMED'}
                        isInFlight={isRenewalPolicyInFlight || isCancellationInFlight}
                        immediateCancellationMode={{ type: 'IMMEDIATE', name: 'Cancel now' }}
                        atPeriodEndCancellationMode={{ type: 'AT_PERIOD_END', name: 'Cancel at period end' }}
                        cancellationBlockedMessage={
                          hasActiveLinkedBookings
                            ? `There ${activeLinkedBookingCount === 1 ? 'is' : 'are'} ${activeLinkedBookingCount} active or upcoming booking${activeLinkedBookingCount === 1 ? '' : 's'} linked to this entitlement. Cancel ${activeLinkedBookingCount === 1 ? 'it' : 'them'} from Related bookings below, then return here to cancel the entitlement.`
                            : undefined
                        }
                        onImmediateCancellationClick={() => setShowCancelDialog(true)}
                        onAtPeriodEndCancellationClick={() => setShowPeriodEndDialog(true)}
                      />
                      <Divider />
                    </StackColumn>
                  ) : null}
                  <StackColumn spacing={2}>
                    <DetailRow label="Product" value={productTitle} />
                    <DetailRow label="Pricing option" value={pricingOptionTitle} />
                    <DetailRow label="Credits included" value={`${purchase.creditQuantity} credits`} />
                    <DetailRow label="Valid from" value={serviceStart.format('D MMM YYYY')} />
                    <DetailRow label="Valid until" value={serviceEnd.format('D MMM YYYY')} />
                    <DetailRow label="Purchase total" value={`${purchase.amount} ${purchase.currency}`} />
                  </StackColumn>

                  <Divider sx={{ my: 3 }} />
                  <MarketplacePurchaseHistoryEventList events={purchase.history.edges.map(({ node }) => node)} />
                  <Divider sx={{ my: 3 }} />
                  <CaptionIconTypography label="Related bookings" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
                  <LeadIconTypography label="Bookings made with these credits" sx={{ mt: 0.75 }} />
                  <BodyIconTypography label={`${purchase.linkedBookings.totalCount} booking(s) use this entitlement.`} sx={{ mt: 1, opacity: 0.82 }} />

                  {purchase.linkedBookings.edges.length > 0 ? (
                    <Box sx={{ mt: 1, display: 'grid', gap: 1.25, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' } }}>
                      {purchase.linkedBookings.edges.map(({ node: booking }) => {
                        const locationLabel = booking.involvedLocations[0]?.name ?? 'Location to be confirmed';
                        const resourcesLabel = booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
                        const paymentStatus = booking.marketplaceBooking?.paymentStatus;

                        return (
                          <Box
                            key={booking.id}
                            component={Link}
                            href={getMarketplaceBookingDetailsLink(undefined, isCustomDomain, organizationCustomDomain, booking.id)}
                            sx={{
                              display: 'block',
                              p: 2,
                              border: 1,
                              borderColor: 'divider',
                              borderRadius: 3,
                              color: 'inherit',
                              textDecoration: 'none',
                              transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
                              '&:hover': { transform: 'translateY(-2px)', boxShadow: (theme) => theme.shadows[3], borderColor: 'primary.main' },
                            }}
                          >
                            <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                              <Box>
                                <SmallIconTypography
                                  label={dayjs.utc(booking.from).format('D MMM YYYY')}
                                  sx={{ opacity: 0.62, textTransform: 'uppercase', letterSpacing: '0.06em' }}
                                />
                                <SubtitleIconTypography label={`${dayjs.utc(booking.from).format('HH:mm')}–${dayjs.utc(booking.until).format('HH:mm')}`} sx={{ mt: 0.35 }} />
                              </Box>
                              <Chip
                                size="small"
                                icon={<PaymentStatusIcon />}
                                label={paymentStatus?.name ?? 'Payment confirmed'}
                                color={paymentStatus?.type === 'CONFIRMED' ? 'success' : 'default'}
                                variant={paymentStatus?.type === 'CONFIRMED' ? 'filled' : 'outlined'}
                              />
                            </StackRow>

                            <StackColumn spacing={1} sx={{ mt: 2 }}>
                              <StackRow sx={{ flexWrap: 'nowrap' }}>
                                <LocationIcon fontSize="small" />
                                <BodyIconTypography label={locationLabel} sx={{ opacity: 0.88 }} />
                              </StackRow>
                              <StackRow sx={{ flexWrap: 'nowrap' }}>
                                <QuantityIcon fontSize="small" />
                                <BodyIconTypography label={`Quantity ${booking.marketplaceBooking?.quantity ?? 1}`} sx={{ opacity: 0.88 }} />
                              </StackRow>
                              <StackRow sx={{ flexWrap: 'nowrap' }}>
                                <ResourceIcon fontSize="small" />
                                <BodyIconTypography label={resourcesLabel} sx={{ opacity: 0.88 }} />
                              </StackRow>
                            </StackColumn>
                          </Box>
                        );
                      })}
                    </Box>
                  ) : (
                    <BodyIconTypography label="No bookings have used these credits yet." sx={{ mt: 2, opacity: 0.7 }} />
                  )}
                </CardContent>
              </Card>

              <MarketplaceProductBookingPaymentPanel
                checkoutUrl={checkoutUrl}
                ctaLabel="Pay now"
                entityLabel="credit purchase"
                invoiceNumber={purchase.invoiceNumber}
                invoiceUrl={purchase.invoiceUrl ?? null}
                isPaymentRequired={isPending}
                paymentExpiry={purchase.paymentExpiry}
                paymentMethodType={purchase.paymentMethod}
                paymentStatusLabel={purchase.paymentStatus}
                paymentStatusType={purchase.paymentStatus}
              />
            </Box>
            <Dialog open={showCancelDialog} onClose={() => setShowCancelDialog(false)}>
              <DefaultDialogTitle title="Cancel entitlement" />
              <DialogContent>
                <DialogContentText>Cancel this entitlement? Unused credits will be forfeited or refunded according to the product policy.</DialogContentText>
                <TwoButtonsDialogActions
                  onPrimaryClicked={cancelEntitlement}
                  onSecondaryClicked={() => setShowCancelDialog(false)}
                  primaryLabel="Confirm cancellation"
                  secondaryLabel="Go back"
                />
              </DialogContent>
            </Dialog>
            <Dialog open={showPeriodEndDialog} onClose={() => setShowPeriodEndDialog(false)}>
              <DefaultDialogTitle title="Cancel at period end" />
              <DialogContent>
                <DialogContentText>
                  This entitlement will remain active for the current period, and no new period will be created. Confirm cancellation at the end of the current period?
                </DialogContentText>
                <TwoButtonsDialogActions
                  onPrimaryClicked={cancelAtPeriodEnd}
                  onSecondaryClicked={() => setShowPeriodEndDialog(false)}
                  primaryLabel="Confirm cancellation"
                  secondaryLabel="Go back"
                />
              </DialogContent>
            </Dialog>
          </StackColumn>
        </Container>
      </Box>
    </OrganizationStoreFrontRootShell>
  );
};

const DetailRow = ({ label, value }: { label: string; value: string }) => (
  <StackColumn spacing={0.25}>
    <CaptionIconTypography label={label} sx={{ opacity: 0.66 }} />
    <BodyIconTypography label={value} />
  </StackColumn>
);

export default EntitlementPurchaseDetails;
