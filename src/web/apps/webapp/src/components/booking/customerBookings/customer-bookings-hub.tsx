import { ArrowLeftIcon, CalendarIcon, LocationIcon, PaymentStatusIcon, QuantityIcon, ResourceIcon, TeamIcon } from '@/components/icons';
import { getMarketplaceBookingDetailsLink, getMarketplaceSubscriptionDetailsLink, getTeamsOrganizationBookingBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import {
  SupportedMarketplaceBookingSubscriptionCancellationMode,
  SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  toSupportedMarketplaceBookingSubscriptionCancellationModeDetails,
} from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-cancellation-mode';
import { toMarketplaceBookingSubscriptionLifecycleDisplay } from '@/components/marketplaceProductSubscription/marketplace-booking-subscription-lifecycle';
import SubscriptionCancellationSection from '@/components/marketplaceProductSubscription/subscription-cancellation-section';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import logger from '@/libs/logging';
import { logCustomerPurchaseHubLoaded } from '@/libs/logging/aggregate-marketplace-telemetry';
import type { customerBookingsHub_deleteMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/customerBookingsHub_deleteMarketplaceBookingSubscriptionMutation.graphql';
import type { customerBookingsHub_rootQuery } from '@/queries/__generated__/customerBookingsHub_rootQuery.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Link from '@mui/material/Link';
import { alpha, type Theme } from '@mui/material/styles';
import type { SxProps } from '@mui/system';
import Box from '@mui/system/Box';
import { getRelayErrorMessage, RelayError, toRootError, toStoredBookingTimeRange, useIntegratedPlatform } from '@skedular/shared';
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
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<customerBookingsHub_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

type BookingNode = NonNullable<customerBookingsHub_rootQuery['response']['upcomingBookings']['edges'][number]['node']>;
type SubscriptionNode = NonNullable<customerBookingsHub_rootQuery['response']['marketplaceBookingSubscriptions']['edges'][number]['node']>;

type PendingCancellationConfirmation = {
  subscriptionId: string;
  productTitle: string;
  mode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails;
} | null;

const RootQuery = graphql`
  query customerBookingsHub_rootQuery($today: DateTime!) {
    marketplaceBookingFailures {
      id
      category {
        type
        name
      }
      scope {
        type
        name
      }
      finalizedAt
      requestedFrom
      requestedUntil
      customerAction {
        type
        name
      }
    }
    marketplaceBookingSubscriptionCancellationModes {
      type
      name
    }
    upcomingBookings: bookings(first: 48, where: { includeMineOnly: true, fromGte: $today }, orderBy: [{ field: FROM, direction: ASCENDING }]) {
      totalCount
      edges {
        node {
          id
          from
          until
          channel {
            channel
            name
          }
          involvedOrganizations {
            id
            name
            customDomain
          }
          involvedLocations {
            name
          }
          involvedTeams {
            id
            name
          }
          bookingResources {
            resource {
              id
              name
            }
          }
          marketplaceBooking {
            quantity
            paymentStatus {
              type
              name
            }
          }
          recurringBooking {
            id
            frequency {
              name
            }
            marketplaceBooking {
              id
            }
          }
        }
      }
    }
    recentBookings: bookings(first: 24, where: { includeMineOnly: true, fromLt: $today }, orderBy: [{ field: FROM, direction: DESCENDING }]) {
      totalCount
      edges {
        node {
          id
          from
          until
          channel {
            channel
            name
          }
          involvedOrganizations {
            id
            name
            customDomain
          }
          involvedLocations {
            name
          }
          involvedTeams {
            id
            name
          }
          bookingResources {
            resource {
              id
              name
            }
          }
          marketplaceBooking {
            quantity
            paymentStatus {
              type
              name
            }
          }
          recurringBooking {
            id
            frequency {
              name
            }
            marketplaceBooking {
              id
            }
          }
        }
      }
    }
    marketplaceBookingSubscriptions(first: 48, where: { includeMineOnly: true }, orderBy: [{ field: NEXT_RENEWAL_AT, direction: ASCENDING }]) {
      totalCount
      edges {
        node {
          id
          startedAt
          nextRenewalAt
          autoRenew
          cancelAtPeriodEnd
          status {
            type
            name
          }
          involvedOrganizations {
            id
            name
            customDomain
          }
          involvedTeams {
            id
            name
          }
          marketplaceBooking {
            quantity
            paymentStatus {
              type
              name
            }
            paymentMethod {
              name
            }
            productVersion {
              listingMetadata {
                title
                subTitle
              }
            }
          }
          recurringBookings {
            id
            startDate
            endDate
          }
        }
      }
    }
  }
`;

const CustomerBookingsHub = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<customerBookingsHub_rootQuery>(RootQuery, queryReference);
  const router = useRouter();
  const { integratedPlatform } = useIntegratedPlatform();
  const [pendingCancellationConfirmation, setPendingCancellationConfirmation] = useState<PendingCancellationConfirmation>(null);
  const [commitDeleteMarketplaceBookingSubscription, isDeleteMarketplaceBookingSubscriptionInFlight] =
    useMutation<customerBookingsHub_deleteMarketplaceBookingSubscriptionMutation>(graphql`
      mutation customerBookingsHub_deleteMarketplaceBookingSubscriptionMutation($input: DeleteMarketplaceBookingSubscriptionInput!) {
        deleteMarketplaceBookingSubscription(input: $input) {
          marketplaceBookingSubscription {
            id
            cancelAtPeriodEnd
            nextRenewalAt
            status {
              type
              name
            }
          }
        }
      }
    `);
  const upcomingBookings = useMemo(() => toNodes(rootData.upcomingBookings.edges), [rootData.upcomingBookings.edges]);
  const recentBookings = useMemo(() => toNodes(rootData.recentBookings.edges), [rootData.recentBookings.edges]);
  const subscriptions = useMemo(() => toNodes(rootData.marketplaceBookingSubscriptions.edges), [rootData.marketplaceBookingSubscriptions.edges]);
  const totalCount = rootData.upcomingBookings.totalCount + rootData.recentBookings.totalCount + rootData.marketplaceBookingSubscriptions.totalCount;
  const immediateCancellationMode = useMemo((): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => {
    const mode = rootData.marketplaceBookingSubscriptionCancellationModes.find((item) => item.type === 'IMMEDIATE');

    return mode ? toSupportedMarketplaceBookingSubscriptionCancellationModeDetails(mode.type, mode.name) : null;
  }, [rootData.marketplaceBookingSubscriptionCancellationModes]);
  const atPeriodEndCancellationMode = useMemo((): SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null => {
    const mode = rootData.marketplaceBookingSubscriptionCancellationModes.find((item) => item.type === 'AT_PERIOD_END');

    return mode ? toSupportedMarketplaceBookingSubscriptionCancellationModeDetails(mode.type, mode.name) : null;
  }, [rootData.marketplaceBookingSubscriptionCancellationModes]);

  useEffect(() => {
    logCustomerPurchaseHubLoaded({
      logger,
      customerIdHash: 'current-customer',
      bookingCount: rootData.upcomingBookings.totalCount + rootData.recentBookings.totalCount,
      subscriptionCount: rootData.marketplaceBookingSubscriptions.totalCount,
    });
  }, [rootData.marketplaceBookingSubscriptions.totalCount, rootData.recentBookings.totalCount, rootData.upcomingBookings.totalCount]);

  const handleDeleteMarketplaceBookingSubscriptionClick = (
    subscriptionId: string,
    productTitle: string,
    cancellationModeType: SupportedMarketplaceBookingSubscriptionCancellationMode,
  ) => {
    commitDeleteMarketplaceBookingSubscription({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: subscriptionId,
          cancellationMode: cancellationModeType,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast(<NotificationContent content={`Failed to update ${productTitle}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onReloadRequired();
      },
      onError: (error) => {
        toast(<NotificationContent content={`Failed to update ${productTitle}. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };
  const handleRequestImmediateCancellationClick = (
    subscriptionId: string,
    productTitle: string,
    cancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails,
  ) => {
    setPendingCancellationConfirmation({
      subscriptionId,
      productTitle,
      mode: cancellationMode,
    });
  };
  const handleCancelImmediateCancellationClick = () => {
    setPendingCancellationConfirmation(null);
  };
  const handleConfirmImmediateCancellationClick = () => {
    if (!pendingCancellationConfirmation) {
      return;
    }

    handleDeleteMarketplaceBookingSubscriptionClick(
      pendingCancellationConfirmation.subscriptionId,
      pendingCancellationConfirmation.productTitle,
      pendingCancellationConfirmation.mode.type,
    );
    setPendingCancellationConfirmation(null);
  };

  return (
    <Box
      sx={{
        minHeight: '100vh',
        width: '100%',
        maxWidth: '100vw',
        overflowX: 'hidden',
        pb: 8,
        px: { xs: 2, sm: 3, md: 0 },
        boxSizing: 'border-box',
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 24%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 20%)',
      }}
    >
      <Container disableGutters maxWidth="xl" sx={{ pt: { xs: 3, md: 4 }, width: '100%', minWidth: 0, maxWidth: '100%', overflowX: 'hidden' }}>
        <Button variant="text" onClick={() => router.back()} sx={{ textTransform: 'none', px: 0, mb: 2 }}>
          <StackRow spacing={0.5} sx={{ flexWrap: 'nowrap' }}>
            <ArrowLeftIcon fontSize="small" />
            <BodyIconTypography label="Back" />
          </StackRow>
        </Button>

        <Card
          sx={{
            borderRadius: 4,
            overflow: 'hidden',
            border: 1,
            borderColor: (theme) => alpha(theme.palette.primary.main, 0.18),
            background: (theme) =>
              `linear-gradient(135deg, ${alpha(theme.palette.primary.light, 0.12)} 0%, ${alpha(theme.palette.background.paper, 1)} 42%, ${alpha(theme.palette.warning.light, 0.1)} 100%)`,
          }}
        >
          <CardContent sx={{ p: { xs: 2, sm: 2.5, md: 3.5 } }}>
            <CaptionIconTypography label="My bookings" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
            <LeadIconTypography label="Your bookings across Skedular" sx={{ mt: 0.75 }} />
            <BodyIconTypography
              label="Review private bookings, marketplace bookings, and marketplace subscriptions from one place. Marketplace bookings stay here for payment and cancellation; private bookings open in the Teams scheduler."
              sx={{ mt: 0.9, opacity: 0.82, maxWidth: 820 }}
            />

            <StackRow sx={{ mt: 2, rowGap: 1 }}>
              <Chip label={`${rootData.upcomingBookings.totalCount} upcoming`} color="primary" variant="outlined" />
              <Chip label={`${rootData.recentBookings.totalCount} recent`} variant="outlined" />
              <Chip label={`${rootData.marketplaceBookingSubscriptions.totalCount} subscriptions`} color="success" variant="outlined" />
            </StackRow>
          </CardContent>
        </Card>

        {rootData.marketplaceBookingFailures.length > 0 ? (
          <Card sx={{ mt: 3, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
            <CardContent sx={{ p: { xs: 2, sm: 2.5 } }}>
              <CaptionIconTypography label="Booking notifications" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
              <SubtitleIconTypography label="Recent booking outcomes" sx={{ mt: 0.75 }} />
              {rootData.marketplaceBookingFailures.map((failure) => (
                <Box key={failure.id} sx={{ borderTop: 1, borderColor: 'divider', mt: 1.5, pt: 1.5 }}>
                  <BodyIconTypography label={failure.category.name} />
                  <SmallIconTypography label={`${failure.scope.name} · ${dayjs(failure.finalizedAt).format('MMM D, YYYY h:mm A')}`} sx={{ mt: 0.25, opacity: 0.72 }} />
                  <BodyIconTypography
                    label={failure.customerAction.type === 'Rebook' ? 'Please start a new booking from current availability.' : failure.customerAction.name}
                    sx={{ mt: 0.5, opacity: 0.82 }}
                  />
                </Box>
              ))}
            </CardContent>
          </Card>
        ) : null}

        {totalCount === 0 ? (
          <Card sx={{ mt: 4, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
            <CardContent sx={{ p: 2.5 }}>
              <SubtitleIconTypography label="No bookings found." />
              <BodyIconTypography
                label="When you book a private space, reserve a marketplace product, or start a subscription, it will appear here."
                sx={{ mt: 0.75, opacity: 0.8 }}
              />
            </CardContent>
          </Card>
        ) : null}

        <BookingsSection bookings={upcomingBookings} integratedPlatform={integratedPlatform} label="Coming up" title="Upcoming bookings" />
        <SubscriptionsSection
          atPeriodEndCancellationMode={atPeriodEndCancellationMode}
          immediateCancellationMode={immediateCancellationMode}
          integratedPlatform={integratedPlatform}
          isDeleteMarketplaceBookingSubscriptionInFlight={isDeleteMarketplaceBookingSubscriptionInFlight}
          onAtPeriodEndCancellationClick={handleDeleteMarketplaceBookingSubscriptionClick}
          onImmediateCancellationClick={handleRequestImmediateCancellationClick}
          subscriptions={subscriptions}
        />
        <BookingsSection bookings={recentBookings} integratedPlatform={integratedPlatform} label="Already happened" title="Recent bookings" />
      </Container>

      <Dialog open={!!pendingCancellationConfirmation} onClose={handleCancelImmediateCancellationClick}>
        <DefaultDialogTitle title="Cancel subscription now" />
        <DialogContent sx={{ mt: 2 }}>
          <DialogContentText>
            {`Cancel ${pendingCancellationConfirmation?.productTitle ?? 'this subscription'} now? Future billing will stop right away. Previous invoices will still stay on record.`}
          </DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmImmediateCancellationClick}
            onSecondaryClicked={handleCancelImmediateCancellationClick}
            primaryLabel="Cancel now"
            secondaryLabel="Keep subscription"
          />
        </DialogContent>
      </Dialog>
    </Box>
  );
};

const BookingsSection = ({
  bookings,
  integratedPlatform,
  label,
  title,
}: {
  bookings: ReadonlyArray<BookingNode>;
  integratedPlatform: string | undefined;
  label: string;
  title: string;
}) => (
  <Box sx={{ mt: 4, minWidth: 0 }}>
    <CaptionIconTypography label={label} sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
    <LeadIconTypography label={title} sx={{ mt: 0.5 }} />

    {bookings.length > 0 ? (
      <Box sx={{ mt: 2, display: 'grid', gap: 1.5, minWidth: 0, gridTemplateColumns: { xs: 'minmax(0, 1fr)', md: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(3, minmax(0, 1fr))' } }}>
        {bookings.map((booking) => (
          <BookingCard key={booking.id} booking={booking} integratedPlatform={integratedPlatform} />
        ))}
      </Box>
    ) : (
      <Card sx={{ mt: 2, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
        <CardContent sx={{ p: 2.5 }}>
          <BodyIconTypography label={title === 'Upcoming bookings' ? 'Nothing is scheduled yet.' : 'No recent bookings to show.'} sx={{ opacity: 0.8 }} />
        </CardContent>
      </Card>
    )}
  </Box>
);

const BookingCard = ({ booking, integratedPlatform }: { booking: BookingNode; integratedPlatform: string | undefined }) => {
  const organization = booking.involvedOrganizations[0];
  const organizationCustomDomain = organization?.customDomain ?? '';
  const organizationName = organization?.name ?? 'Organization pending';
  const isMarketplaceBooking = !!booking.marketplaceBooking;
  const bookingLink = isMarketplaceBooking
    ? getMarketplaceBookingDetailsLink(integratedPlatform, false, organizationCustomDomain, booking.id)
    : getTeamsOrganizationBookingBaseLink(organizationCustomDomain, booking.id);
  const locationLabel = booking.involvedLocations[0]?.name ?? 'Location to be confirmed';
  const resourcesLabel = booking.bookingResources.map((item) => item.resource.name).join(', ') || 'Assigned later';
  const teamLabel = booking.involvedTeams.map((team) => team.name).join(', ');
  const paymentStatusType = booking.marketplaceBooking?.paymentStatus.type;
  const isConfirmed = paymentStatusType === 'CONFIRMED';
  const isPending = paymentStatusType === 'PENDING';
  const callToActionLabel = isMarketplaceBooking ? 'Open marketplace booking' : 'Open in Teams';

  return (
    <Link component={NextLink} href={bookingLink} underline="none" color="inherit" sx={cardLinkSx('primary')}>
      <Box sx={{ p: { xs: 2, sm: 2.25 }, minWidth: 0 }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 1, minWidth: 0 }}>
          <Box sx={{ minWidth: 0 }}>
            <SmallIconTypography label={organizationName} sx={{ opacity: 0.62, overflowWrap: 'anywhere', textTransform: 'uppercase' }} />
            <SubtitleIconTypography label={locationLabel} sx={{ mt: 0.4, overflowWrap: 'anywhere' }} />
          </Box>
          <Chip
            size="small"
            label={isMarketplaceBooking ? 'Marketplace' : 'Private'}
            color={isMarketplaceBooking ? 'primary' : 'default'}
            variant={isMarketplaceBooking ? 'filled' : 'outlined'}
          />
        </StackRow>

        <StackColumn spacing={1.1} sx={{ mt: 2 }}>
          <StackRow sx={{ flexWrap: 'nowrap', minWidth: 0 }}>
            <CalendarIcon fontSize="small" />
            <BodyIconTypography label={toBookingDateTimeLabel(booking.from, booking.until)} sx={{ minWidth: 0, overflowWrap: 'anywhere', opacity: 0.88 }} />
          </StackRow>
          <StackRow sx={{ flexWrap: 'nowrap', minWidth: 0 }}>
            <LocationIcon fontSize="small" />
            <BodyIconTypography label={locationLabel} sx={{ minWidth: 0, overflowWrap: 'anywhere', opacity: 0.88 }} />
          </StackRow>
          {teamLabel ? (
            <StackRow sx={{ flexWrap: 'nowrap', minWidth: 0 }}>
              <TeamIcon fontSize="small" />
              <BodyIconTypography label={teamLabel} sx={{ minWidth: 0, overflowWrap: 'anywhere', opacity: 0.88 }} />
            </StackRow>
          ) : null}
          <StackRow sx={{ flexWrap: 'nowrap', minWidth: 0 }}>
            <ResourceIcon fontSize="small" />
            <BodyIconTypography label={resourcesLabel} sx={{ minWidth: 0, overflowWrap: 'anywhere', opacity: 0.88 }} />
          </StackRow>
          {booking.marketplaceBooking ? (
            <StackRow sx={{ flexWrap: 'nowrap', minWidth: 0 }}>
              <QuantityIcon fontSize="small" />
              <BodyIconTypography label={`Quantity ${booking.marketplaceBooking.quantity}`} sx={{ minWidth: 0, overflowWrap: 'anywhere', opacity: 0.88 }} />
            </StackRow>
          ) : null}
        </StackColumn>

        <StackRow sx={{ mt: 2, gap: 1, flexWrap: 'wrap' }}>
          {booking.marketplaceBooking ? (
            <Chip
              size="small"
              icon={<PaymentStatusIcon />}
              label={booking.marketplaceBooking.paymentStatus.name}
              color={isConfirmed ? 'success' : isPending ? 'warning' : 'default'}
              variant={isConfirmed || isPending ? 'filled' : 'outlined'}
            />
          ) : null}
          {booking.recurringBooking ? <Chip label={`${booking.recurringBooking.frequency.name} recurring`} size="small" variant="outlined" /> : null}
        </StackRow>

        <Divider sx={{ mt: 2 }} />

        <StackRow sx={{ mt: 2, justifyContent: 'space-between', flexWrap: 'wrap', gap: 1, minWidth: 0 }}>
          <BodyIconTypography label={callToActionLabel} sx={{ color: 'primary.main', fontWeight: 600, minWidth: 0, overflowWrap: 'anywhere' }} />
          <ChevronRightIcon fontSize="small" />
        </StackRow>
      </Box>
    </Link>
  );
};

const SubscriptionsSection = ({
  atPeriodEndCancellationMode,
  immediateCancellationMode,
  integratedPlatform,
  isDeleteMarketplaceBookingSubscriptionInFlight,
  onAtPeriodEndCancellationClick,
  onImmediateCancellationClick,
  subscriptions,
}: {
  atPeriodEndCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  immediateCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  integratedPlatform: string | undefined;
  isDeleteMarketplaceBookingSubscriptionInFlight: boolean;
  onAtPeriodEndCancellationClick: (subscriptionId: string, productTitle: string, cancellationModeType: SupportedMarketplaceBookingSubscriptionCancellationMode) => void;
  onImmediateCancellationClick: (subscriptionId: string, productTitle: string, cancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails) => void;
  subscriptions: ReadonlyArray<SubscriptionNode>;
}) => (
  <Box sx={{ mt: 4, minWidth: 0 }}>
    <CaptionIconTypography label="Subscriptions" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
    <LeadIconTypography label="Marketplace subscriptions" sx={{ mt: 0.5 }} />

    {subscriptions.length > 0 ? (
      <Box sx={{ mt: 2, display: 'grid', gap: 1.5, minWidth: 0, gridTemplateColumns: { xs: 'minmax(0, 1fr)', md: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(3, minmax(0, 1fr))' } }}>
        {subscriptions.map((subscription) => (
          <SubscriptionCard
            key={subscription.id}
            atPeriodEndCancellationMode={atPeriodEndCancellationMode}
            immediateCancellationMode={immediateCancellationMode}
            integratedPlatform={integratedPlatform}
            isDeleteMarketplaceBookingSubscriptionInFlight={isDeleteMarketplaceBookingSubscriptionInFlight}
            onAtPeriodEndCancellationClick={onAtPeriodEndCancellationClick}
            onImmediateCancellationClick={onImmediateCancellationClick}
            subscription={subscription}
          />
        ))}
      </Box>
    ) : (
      <Card sx={{ mt: 2, borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
        <CardContent sx={{ p: 2.5 }}>
          <BodyIconTypography label="You don't have any subscriptions yet." sx={{ opacity: 0.8 }} />
        </CardContent>
      </Card>
    )}
  </Box>
);

const SubscriptionCard = ({
  atPeriodEndCancellationMode,
  immediateCancellationMode,
  integratedPlatform,
  isDeleteMarketplaceBookingSubscriptionInFlight,
  onAtPeriodEndCancellationClick,
  onImmediateCancellationClick,
  subscription,
}: {
  atPeriodEndCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  immediateCancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails | null;
  integratedPlatform: string | undefined;
  isDeleteMarketplaceBookingSubscriptionInFlight: boolean;
  onAtPeriodEndCancellationClick: (subscriptionId: string, productTitle: string, cancellationModeType: SupportedMarketplaceBookingSubscriptionCancellationMode) => void;
  onImmediateCancellationClick: (subscriptionId: string, productTitle: string, cancellationMode: SupportedMarketplaceBookingSubscriptionCancellationModeDetails) => void;
  subscription: SubscriptionNode;
}) => {
  const organization = subscription.involvedOrganizations[0];
  const organizationCustomDomain = organization?.customDomain ?? '';
  const subscriptionLink = getMarketplaceSubscriptionDetailsLink(integratedPlatform, false, organizationCustomDomain, subscription.id);
  const productTitle = subscription.marketplaceBooking.productVersion.listingMetadata.title ?? 'Subscription';
  const latestRecurringBooking = [...subscription.recurringBookings].sort((left, right) => new Date(right.startDate).getTime() - new Date(left.startDate).getTime())[0];
  const paymentStatusType = subscription.marketplaceBooking.paymentStatus.type;
  const isConfirmed = paymentStatusType === 'CONFIRMED';
  const isPending = paymentStatusType === 'PENDING';
  const lifecycleDisplay = toMarketplaceBookingSubscriptionLifecycleDisplay({
    autoRenew: subscription.autoRenew,
    cancelAtPeriodEnd: subscription.cancelAtPeriodEnd,
    isCancelled: subscription.status.type === 'CANCELLED',
    fallbackActiveLabel: subscription.status.name,
  });
  const paymentStatusLabel = subscription.status.type === 'CANCELLED' ? lifecycleDisplay.statusLabel : subscription.marketplaceBooking.paymentStatus.name;

  return (
    <Link component={NextLink} href={subscriptionLink} underline="none" color="inherit" sx={cardLinkSx('success')}>
      <Box sx={{ p: { xs: 2, sm: 2.25 }, minWidth: 0 }}>
        <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 1, minWidth: 0 }}>
          <Box sx={{ minWidth: 0 }}>
            <SmallIconTypography label={organization?.name ?? 'Organization pending'} sx={{ opacity: 0.62, overflowWrap: 'anywhere', textTransform: 'uppercase' }} />
            <SubtitleIconTypography label={productTitle} sx={{ mt: 0.4, overflowWrap: 'anywhere' }} />
          </Box>
          <Chip
            size="small"
            icon={<PaymentStatusIcon />}
            label={paymentStatusLabel}
            color={isConfirmed ? 'success' : isPending ? 'warning' : 'default'}
            variant={isConfirmed || isPending ? 'filled' : 'outlined'}
          />
        </StackRow>

        <StackColumn spacing={1.1} sx={{ mt: 2 }}>
          <DetailsRow label="Quantity" value={`${subscription.marketplaceBooking.quantity}`} />
          <DetailsRow
            label="Current period"
            value={latestRecurringBooking ? `${toStoredDate(latestRecurringBooking.startDate)} - ${toStoredDate(latestRecurringBooking.endDate)}` : 'Preparing cycle'}
          />
          <DetailsRow label="Next renewal" value={subscription.nextRenewalAt ? toStoredDate(subscription.nextRenewalAt) : lifecycleDisplay.nextRenewalFallbackLabel} />
          <DetailsRow label="Cancellation" value={lifecycleDisplay.renewalLabel} />
          <DetailsRow label="Payment method" value={subscription.marketplaceBooking.paymentMethod.name} />
        </StackColumn>

        {subscription.status.type === 'ACTIVE' ? (
          <Box sx={{ mt: 2 }} onClick={(event) => event.preventDefault()}>
            <Divider sx={{ mb: 2 }} />
            <SubscriptionCancellationSection
              cancelAtPeriodEnd={subscription.cancelAtPeriodEnd}
              hasConfirmedPayment={subscription.marketplaceBooking.paymentStatus.type === 'CONFIRMED'}
              isInFlight={isDeleteMarketplaceBookingSubscriptionInFlight}
              immediateCancellationMode={immediateCancellationMode}
              atPeriodEndCancellationMode={subscription.autoRenew ? atPeriodEndCancellationMode : null}
              onImmediateCancellationClick={() => (immediateCancellationMode ? onImmediateCancellationClick(subscription.id, productTitle, immediateCancellationMode) : undefined)}
              onAtPeriodEndCancellationClick={() =>
                atPeriodEndCancellationMode ? onAtPeriodEndCancellationClick(subscription.id, productTitle, atPeriodEndCancellationMode.type) : undefined
              }
            />
          </Box>
        ) : null}

        <Divider sx={{ mt: 2 }} />

        <StackRow sx={{ mt: 2, justifyContent: 'space-between', flexWrap: 'wrap', gap: 1, minWidth: 0 }}>
          <BodyIconTypography label="Open subscription" sx={{ color: 'success.main', fontWeight: 600, minWidth: 0, overflowWrap: 'anywhere' }} />
          <ChevronRightIcon fontSize="small" />
        </StackRow>
      </Box>
    </Link>
  );
};

const DetailsRow = ({ label, value }: { label: string; value: string }) => (
  <StackColumn spacing={0.35} sx={{ minWidth: 0 }}>
    <SmallIconTypography label={label} sx={{ opacity: 0.62, textTransform: 'uppercase' }} />
    <BodyIconTypography label={value} sx={{ minWidth: 0, overflowWrap: 'anywhere', opacity: 0.88 }} />
  </StackColumn>
);

const cardLinkSx = (accent: 'primary' | 'success'): SxProps<Theme> => ({
  display: 'block',
  width: '100%',
  minWidth: 0,
  maxWidth: '100%',
  boxSizing: 'border-box',
  borderRadius: 3,
  border: 1,
  borderColor: (theme: Theme) => alpha(theme.palette.divider, 0.9),
  bgcolor: (theme: Theme) => alpha(theme.palette.background.paper, 0.86),
  backdropFilter: 'blur(10px)',
  transition: 'transform 120ms ease, box-shadow 120ms ease, border-color 120ms ease',
  '&:hover': {
    transform: 'translateY(-2px)',
    boxShadow: (theme: Theme) => theme.shadows[4],
    borderColor: (theme: Theme) => (accent === 'success' ? theme.palette.success.main : theme.palette.primary.main),
  },
});

const toNodes = <TNode,>(edges: ReadonlyArray<{ readonly node: TNode | null | undefined }>) => edges.map((edge) => edge.node).filter((item): item is NonNullable<TNode> => !!item);

const toStoredDate = (date?: string | null) => (date ? dayjs.utc(date).format('dddd, Do MMM YYYY') : '');
const toBookingDateTimeLabel = (from?: string | null, until?: string | null) => {
  const bookingDate = toStoredDate(from);
  const bookingTime = toStoredBookingTimeRange(from, until);

  return bookingTime ? `${bookingDate}, ${bookingTime}` : bookingDate;
};

const MemoCustomerBookingsHub = memo(CustomerBookingsHub);

const CustomerBookingsHubWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<customerBookingsHub_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        today: dayjs().startOf('day').toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoCustomerBookingsHub queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(CustomerBookingsHubWithRelay);
