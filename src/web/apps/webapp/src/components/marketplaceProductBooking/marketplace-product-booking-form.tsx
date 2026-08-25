import { getMarketplaceEntitlementPurchaseDetailsLink, getMarketplaceProductBookingDetailsLink, getMarketplaceProductLink, getSignInLink } from '@/components/links';
import { CustomerTermsAndConditionsPanel } from '@/components/marketplaceProduct';
import { getAvailableDaysGuidance, isDateAvailableForPrice } from '@/components/marketplaceProduct/available-days';
import MarketplaceProductBookingPaymentPanel from '@/components/marketplaceProductBooking/marketplace-product-booking-payment-panel';
import { isSubscriptionCadence } from '@/components/marketplaceProductSubscription/subscription-utils';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import useKnownParams from '@/hooks/use-known-params';
import type {
  BookingCategory,
  marketplaceProductBookingForm_addMarketplaceBookingMutation,
  PaymentMethod,
} from '@/queries/__generated__/marketplaceProductBookingForm_addMarketplaceBookingMutation.graphql';
import type { marketplaceProductBookingForm_createEntitlementPurchaseMutation } from '@/queries/__generated__/marketplaceProductBookingForm_createEntitlementPurchaseMutation.graphql';
import type { marketplaceProductBookingForm_query$key } from '@/queries/__generated__/marketplaceProductBookingForm_query.graphql';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import FormControlLabel from '@mui/material/FormControlLabel';
import MenuItem from '@mui/material/MenuItem';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { formatPriceForDisplay, getRelayErrorMessage, startOfDay, toOpeningHoursFromTime, toShortDate, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { Dayjs } from 'dayjs';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useRef, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { getFormFailureToastMessage } from './marketplace-booking-failure-eligibility';
import MarketplaceProductBookingSummary from './marketplace-product-booking-summary';

type Props = {
  bookingAvailable: boolean;
  bookingAvailabilityMessage: string;
  onDateChange: (value: Dayjs) => void;
  onTimeRangeChange: (value: DateRange<Dayjs>) => void;
  rootDataRelay: marketplaceProductBookingForm_query$key;
  selectedDate: Dayjs;
  timeRange: DateRange<Dayjs>;
};

type PricingDurationBounds = {
  minDurationMinutes?: number | null;
  maxDurationMinutes?: number | null;
};

const defaultBookingStartTime = '09:00';
const defaultBookingDurationInMinutes = 60;

const roundDurationUpToStep = (durationInMinutes: number, stepInMinutes: number) => Math.ceil(durationInMinutes / stepInMinutes) * stepInMinutes;

const roundDurationDownToStep = (durationInMinutes: number, stepInMinutes: number) => Math.floor(durationInMinutes / stepInMinutes) * stepInMinutes;

const getDefaultBookingDurationInMinutes = (pricingOption: PricingDurationBounds, slotSizeInMinutes: number) => {
  const safeSlotSize = slotSizeInMinutes > 0 ? slotSizeInMinutes : 1;
  const minimumDuration = pricingOption.minDurationMinutes ?? safeSlotSize;
  const maximumDuration = pricingOption.maxDurationMinutes;
  let duration = Math.max(defaultBookingDurationInMinutes, minimumDuration);

  duration = roundDurationUpToStep(duration, safeSlotSize);

  if (maximumDuration != null && duration > maximumDuration) {
    duration = Math.max(safeSlotSize, roundDurationDownToStep(maximumDuration, safeSlotSize));
  }

  if (duration < minimumDuration) {
    duration = minimumDuration;
  }

  return duration;
};

const bookingCategory = 'WORKING_FROM_COWORKING_SPACE' as BookingCategory;
const availabilityQuery = `
  query marketplaceProductBookingFormAvailabilityQuery(
    $organizationCustomDomain: String
    $productId: String
    $requestedResourceIds: [String!]
    $from: DateTime!
    $until: DateTime!
  ) {
    availableResourcesCount(
      where: {
        organizationCustomDomain: $organizationCustomDomain
        productId: $productId
        requestedResourceIds: $requestedResourceIds
        from: $from
        until: $until
      }
    )
  }
`;

type AvailabilityQueryResponse = {
  data?: {
    availableResourcesCount?: number | null;
  };
  errors?: Array<{
    message?: string | null;
  }>;
};

const MarketplaceProductBookingForm = ({ bookingAvailable, bookingAvailabilityMessage, onDateChange, onTimeRangeChange, rootDataRelay, selectedDate, timeRange }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment marketplaceProductBookingForm_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        me {
          id
          emails
        }
        entitlementPurchases {
          id
          paymentStatus
          paymentMethod
          paymentExpiry
          amount
          currency
          paymentAction
          invoiceNumber
          invoiceUrl
          paymentInstructions
          linkedBookings(first: 1) {
            edges {
              node {
                marketplaceBooking {
                  invoiceUrl
                }
              }
            }
          }
        }
        currencies {
          type
          name
        }
        paymentMethodTypes {
          type
          name
        }
        bookingSlotSizeInMinutes
        product(id: $productId) {
          id
          latestProductVersionId
          type {
            type
            name
          }
          organization {
            uniqueId
            customerFacingTermsAndConditionsUrl
          }
          listingMetadata {
            title
          }
          currency {
            type
            name
          }
          pricingOptions {
            id
            index
            listingMetadata {
              title
              subTitle
            }
            purchaseCadence
            bookingCadence
            price
            numberOfResourcesToBook
            minDurationMinutes
            maxDurationMinutes
            cancellationPolicyType
            cancellationRefundRules {
              minutesBefore
              refundPercentage
            }
            isTaxInclusive
            billingMode
            acceptedPaymentMethods
            availableDays
            fulfillmentType
            supportsSubscriptionAutoRenewal
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitAddBooking, isInFlight] = useMutation<marketplaceProductBookingForm_addMarketplaceBookingMutation>(graphql`
    mutation marketplaceProductBookingForm_addMarketplaceBookingMutation($input: AddMarketplaceBookingInput!) {
      addMarketplaceBooking(input: $input) {
        accessError {
          errorCode
          message
        }
        failure {
          category {
            type
            name
          }
          customerAction {
            type
            name
          }
        }
        booking {
          id
          from
          until
          marketplaceBooking {
            isPaymentRequired
            id
            paymentExpiry
            invoiceUrl
            invoiceNumber
            totalAmountToDisplay
            bookingCheckoutSession {
              checkoutUrl
            }
            paymentMethod {
              name
              type
            }
            paymentStatus {
              type
              name
            }
          }
        }
      }
    }
  `);

  const [commitCreateEntitlementPurchase, isEntitlementPurchaseInFlight] = useMutation<marketplaceProductBookingForm_createEntitlementPurchaseMutation>(graphql`
    mutation marketplaceProductBookingForm_createEntitlementPurchaseMutation($input: CreateEntitlementPurchaseInput!) {
      createEntitlementPurchase(input: $input) {
        error
        purchase {
          id
          paymentAction
          paymentInstructions
        }
      }
    }
  `);

  const router = useRouter();
  const [entitlementStartDate, setEntitlementStartDate] = useState(() => startOfDay());
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const initialPricingOptionId = searchParams.get('pricingOptionId');
  const selectedResourceIds = useMemo(() => {
    const resourceIds = searchParams.get('resourceIds');
    if (resourceIds) {
      return resourceIds.split(',').filter(Boolean);
    }

    const resourceId = searchParams.get('resourceId');
    return resourceId ? [resourceId] : [];
  }, [searchParams]);

  const bookingPricingOptions = useMemo(
    () => [...(rootData.product?.pricingOptions ?? [])].filter((option) => !isSubscriptionCadence(option.purchaseCadence)).sort((left, right) => left.index - right.index),
    [rootData.product?.pricingOptions],
  );

  const [selectedPricingId, setSelectedPricingId] = useState(initialPricingOptionId ?? '');
  const [quantity, setQuantity] = useState(1);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod | ''>('');
  const [autoRenew, setAutoRenew] = useState(true);
  const [invoiceEmailList, setInvoiceEmailList] = useState<string[]>(() => [...(rootData.me?.emails ?? [])]);
  const [availableResourcesCount, setAvailableResourcesCount] = useState<number | null>(null);
  const [isCheckingAvailability, setIsCheckingAvailability] = useState(false);
  const [availabilityErrorMessage, setAvailabilityErrorMessage] = useState('');
  const [hasAcceptedTermsAndConditions, setHasAcceptedTermsAndConditions] = useState(false);
  const lastDefaultedPricingId = useRef('');

  const effectiveSelectedPricingId = useMemo(() => {
    if (bookingPricingOptions.some((item) => item.id === selectedPricingId)) {
      return selectedPricingId;
    }

    if (initialPricingOptionId && bookingPricingOptions.some((item) => item.id === initialPricingOptionId)) {
      return initialPricingOptionId;
    }

    return bookingPricingOptions[0]?.id ?? '';
  }, [bookingPricingOptions, initialPricingOptionId, selectedPricingId]);

  const selectedPricingOption = useMemo(
    () => bookingPricingOptions.find((item) => item.id === effectiveSelectedPricingId) ?? bookingPricingOptions[0] ?? null,
    [bookingPricingOptions, effectiveSelectedPricingId],
  );
  const isEventProduct = rootData.product?.type.type === 'EVENT';
  const isEntitlementPricing = selectedPricingOption?.fulfillmentType === 'ENTITLEMENT';
  const isPurchasingEntitlement = isEntitlementPricing;
  const isInArrearsBilling = selectedPricingOption?.billingMode === 'IN_ARREARS';

  useEffect(() => {
    if (!selectedPricingOption || lastDefaultedPricingId.current === selectedPricingOption.id) {
      return;
    }

    lastDefaultedPricingId.current = selectedPricingOption.id;
    const defaultFrom = toOpeningHoursFromTime(defaultBookingStartTime);
    if (!defaultFrom) {
      return;
    }

    const defaultUntil = defaultFrom.add(getDefaultBookingDurationInMinutes(selectedPricingOption, rootData.bookingSlotSizeInMinutes), 'minutes');

    onTimeRangeChange([defaultFrom, defaultUntil]);
  }, [onTimeRangeChange, rootData.bookingSlotSizeInMinutes, selectedPricingOption]);

  const acceptedPaymentMethods = useMemo(() => selectedPricingOption?.acceptedPaymentMethods ?? [], [selectedPricingOption]);
  const availablePaymentMethods = useMemo(
    () => rootData.paymentMethodTypes.filter((item) => acceptedPaymentMethods.length === 0 || acceptedPaymentMethods.includes(item.type)),
    [acceptedPaymentMethods, rootData.paymentMethodTypes],
  );
  const effectivePaymentMethod = useMemo(
    () => (availablePaymentMethods.some((item) => item.type === paymentMethod) ? paymentMethod : (availablePaymentMethods[0]?.type ?? '')),
    [availablePaymentMethods, paymentMethod],
  );

  const currencyLabel = useMemo(
    () => rootData.currencies.find((item) => item.type === rootData.product?.currency.type)?.name ?? rootData.product?.currency.name ?? '',
    [rootData.currencies, rootData.product?.currency.name, rootData.product?.currency.type],
  );

  const pricingChoices = useMemo(
    () =>
      bookingPricingOptions.map((option) => ({
        id: option.id,
        label: option.listingMetadata.title ?? option.listingMetadata.subTitle ?? 'Pricing option',
        description: option.listingMetadata.title ? (option.listingMetadata.subTitle ?? '') : '',
      })),
    [bookingPricingOptions],
  );

  const dateRangeValidation = useMemo(() => {
    const [timeFrom, timeUntil] = timeRange;
    if (!selectedPricingOption || !timeFrom || !timeUntil) {
      return { errorMessage: 'Time required.', from: selectedDate.utc(), until: selectedDate.utc().add(1, 'day'), valid: false };
    }

    const baseDate = selectedDate.utc();
    const from = baseDate.set('hour', timeFrom.hour()).set('minute', timeFrom.minute());
    const until = baseDate.set('hour', timeUntil.hour()).set('minute', timeUntil.minute());

    if (!from.isValid() || !until.isValid() || !until.isAfter(from)) {
      return { errorMessage: 'Select a valid time range.', from, until, valid: false };
    }

    const durationInMinutes = until.diff(from, 'minutes');

    if (selectedPricingOption.minDurationMinutes != null && durationInMinutes < selectedPricingOption.minDurationMinutes) {
      return { errorMessage: `Minimum duration is ${selectedPricingOption.minDurationMinutes} minutes.`, from, until, valid: false };
    }

    if (selectedPricingOption.maxDurationMinutes != null && durationInMinutes > selectedPricingOption.maxDurationMinutes) {
      return { errorMessage: `Maximum duration is ${selectedPricingOption.maxDurationMinutes} minutes.`, from, until, valid: false };
    }

    return { errorMessage: '', from, until, valid: true };
  }, [selectedDate, selectedPricingOption, timeRange]);

  const effectiveQuantity = isEventProduct ? 1 : quantity;
  const requiredResourceCount = useMemo(() => {
    if (!selectedPricingOption) {
      return 0;
    }

    return isEventProduct ? 1 : effectiveQuantity * selectedPricingOption.numberOfResourcesToBook;
  }, [effectiveQuantity, isEventProduct, selectedPricingOption]);
  const hasEnoughResourcesAvailable = useMemo(() => {
    if (!dateRangeValidation.valid || availableResourcesCount === null) {
      return false;
    }

    return availableResourcesCount >= requiredResourceCount;
  }, [availableResourcesCount, dateRangeValidation.valid, requiredResourceCount]);
  const availabilityMessage = useMemo(() => {
    if (!dateRangeValidation.valid || !rootData.product || !organizationCustomDomain || !selectedPricingOption) {
      return '';
    }

    if (availabilityErrorMessage) {
      return availabilityErrorMessage;
    }

    if (isCheckingAvailability) {
      return 'Checking live availability for this time...';
    }

    if (availableResourcesCount === null) {
      return '';
    }

    if (hasEnoughResourcesAvailable) {
      return requiredResourceCount === 1 ? 'A matching resource is available for this time.' : `${availableResourcesCount} matching resources are available for this time.`;
    }

    return requiredResourceCount === 1
      ? 'No matching resource is available for the selected time.'
      : `Only ${availableResourcesCount} matching resources are available, but this booking needs ${requiredResourceCount}.`;
  }, [
    availabilityErrorMessage,
    availableResourcesCount,
    dateRangeValidation.valid,
    hasEnoughResourcesAvailable,
    isCheckingAvailability,
    organizationCustomDomain,
    requiredResourceCount,
    rootData.product,
    selectedPricingOption,
  ]);

  const totalLabel = useMemo(() => {
    if (!selectedPricingOption || !dateRangeValidation.valid) {
      return 'N/A';
    }

    const minutes = dateRangeValidation.until.diff(dateRangeValidation.from, 'minutes');
    const price = Number(selectedPricingOption.price);
    let total = price * effectiveQuantity;

    if (selectedPricingOption.bookingCadence === 'PER_MINUTE') {
      total = price * effectiveQuantity * minutes;
    }

    if (selectedPricingOption.bookingCadence === 'PER_HOUR') {
      total = (price / 60) * effectiveQuantity * minutes;
    }

    return formatPriceForDisplay(currencyLabel, total.toFixed(2), selectedPricingOption.purchaseCadence);
  }, [currencyLabel, dateRangeValidation, effectiveQuantity, selectedPricingOption]);

  const durationLabel = dateRangeValidation.valid ? `${dateRangeValidation.until.diff(dateRangeValidation.from, 'minutes')} minutes` : 'Invalid time';
  const paymentLabel = isInArrearsBilling
    ? 'Invoice sent on billing cycle'
    : (availablePaymentMethods.find((item) => item.type === effectivePaymentMethod)?.name ?? 'Select payment method');
  const productLink = rootData.product ? getMarketplaceProductLink(integratedPlatform, isCustomDomain, organizationCustomDomain, rootData.product.id, selectedResourceIds) : '';
  const handleSignInClick = () => {
    const returnTo = `${window.location.pathname}${window.location.search}`;
    router.push(`${getSignInLink()}?returnTo=${encodeURIComponent(returnTo)}`);
  };

  useEffect(() => {
    if (!rootData.product || !organizationCustomDomain || !selectedPricingOption || !dateRangeValidation.valid) {
      return;
    }

    const abortController = new AbortController();

    const checkAvailability = async () => {
      try {
        setIsCheckingAvailability(true);
        setAvailabilityErrorMessage('');

        const response = await fetch('/api/v1/graphql', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          credentials: 'same-origin',
          signal: abortController.signal,
          body: JSON.stringify({
            query: availabilityQuery,
            variables: {
              organizationCustomDomain,
              productId: rootData.product?.id,
              requestedResourceIds: selectedResourceIds,
              from: dateRangeValidation.from.toISOString(),
              until: dateRangeValidation.until.toISOString(),
            },
          }),
        });

        const payload = (await response.json()) as AvailabilityQueryResponse;
        if (!response.ok) {
          throw new Error(`Availability check failed with status ${response.status}.`);
        }

        if (payload.errors?.length) {
          const errorMessage = payload.errors
            .map((item) => item.message?.trim())
            .filter((message): message is string => Boolean(message))
            .join(' ');

          throw new Error(errorMessage || 'Could not check availability right now.');
        }

        setAvailableResourcesCount(payload.data?.availableResourcesCount ?? 0);
      } catch (error) {
        if (abortController.signal.aborted) {
          return;
        }

        setAvailableResourcesCount(null);
        setAvailabilityErrorMessage(error instanceof Error ? error.message : 'Could not check availability right now.');
      } finally {
        if (!abortController.signal.aborted) {
          setIsCheckingAvailability(false);
        }
      }
    };

    void checkAvailability();

    return () => {
      abortController.abort();
    };
  }, [dateRangeValidation.from, dateRangeValidation.until, dateRangeValidation.valid, organizationCustomDomain, rootData.product, selectedPricingOption, selectedResourceIds]);

  const handleSubmit = () => {
    if (!rootData.product || !selectedPricingOption) {
      return;
    }

    const product = rootData.product;

    if (!rootData.me?.id) {
      handleSignInClick();
      return;
    }

    const submittedPaymentMethod = isInArrearsBilling ? (availablePaymentMethods[0]?.type ?? '') : effectivePaymentMethod;

    if (!submittedPaymentMethod) {
      toast.error(<NotificationContent content="Select a payment method to continue." />);
      return;
    }

    if (isPurchasingEntitlement) {
      if (!rootData.product.organization.uniqueId) {
        toast.error(<NotificationContent content="This entitlement offering is currently unavailable." />);
        return;
      }

      commitCreateEntitlementPurchase({
        variables: {
          input: {
            clientMutationId: uuid(),
            organizationId: rootData.product.organization.uniqueId,
            productVersionId: rootData.product.latestProductVersionId,
            pricingId: selectedPricingOption.id,
            autoRenew: isPurchasingEntitlement && selectedPricingOption.supportsSubscriptionAutoRenewal ? autoRenew : false,
            paymentMethod: submittedPaymentMethod as PaymentMethod,
            serviceStartAt: entitlementStartDate.utc().startOf('day').toISOString(),
            checkoutReturnUrl: new URL(
              getMarketplaceEntitlementPurchaseDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, '__PURCHASE_ID__'),
              window.location.origin,
            ).toString(),
            invoiceEmailList,
          },
        },
        onCompleted: (response, errors) => {
          if (errors?.length) {
            toast(<NotificationContent content={`We couldn't start this entitlement purchase. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
            return;
          }

          const purchase = response.createEntitlementPurchase?.purchase;
          if (response.createEntitlementPurchase?.error || !purchase) {
            toast(<NotificationContent content={response.createEntitlementPurchase?.error ?? "We couldn't start this entitlement purchase."} />, errorNotificationOptions);
            return;
          }

          router.push(getMarketplaceEntitlementPurchaseDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, purchase.id));
        },
        onError: (error) => {
          toast(<NotificationContent content={`We couldn't start this entitlement purchase. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
        },
      });
      return;
    }

    if (!dateRangeValidation.valid) {
      toast.error(<NotificationContent content={dateRangeValidation.errorMessage} />);
      return;
    }

    if (availabilityErrorMessage) {
      toast.error(<NotificationContent content={availabilityErrorMessage} />);
      return;
    }

    if (isCheckingAvailability) {
      toast.error(<NotificationContent content="Availability is still being checked. Please wait a moment and try again." />);
      return;
    }

    if (!hasEnoughResourcesAvailable) {
      toast.error(<NotificationContent content="No matching resources are available for the selected date and time." />);
      return;
    }

    if (rootData.product.organization.customerFacingTermsAndConditionsUrl && !hasAcceptedTermsAndConditions) {
      toast.error(<NotificationContent content="Accept the space terms and conditions before continuing." />);
      return;
    }
    const id = uuid();

    commitAddBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          customerIds: [rootData.me.id],
          from: dateRangeValidation.from.toISOString(),
          until: dateRangeValidation.until.toISOString(),
          organizationCustomDomains: [organizationCustomDomain],
          organizationIds: [],
          teamIds: [],
          resourceIds: selectedResourceIds,
          category: bookingCategory,
          paymentMethod: submittedPaymentMethod as PaymentMethod,
          invoiceEmailList,
          quantity: effectiveQuantity,
          productVersionId: product.latestProductVersionId,
          pricingId: selectedPricingOption.id,
          entitlementId: null,
          checkoutReturnUrl: new URL(
            getMarketplaceProductBookingDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, product.id, id),
            window.location.origin,
          ).toString(),
        },
      },
      onCompleted: (response, errors) => {
        if (errors?.length) {
          toast(<NotificationContent content={`We couldn't complete this booking. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        const booking = response.addMarketplaceBooking?.booking;
        const accessError = response.addMarketplaceBooking?.accessError;
        const failure = response.addMarketplaceBooking?.failure;
        if (accessError) {
          toast(<NotificationContent content="Bookings are currently unavailable for this workspace." />, errorNotificationOptions);
          return;
        }

        if (failure) {
          toast(<NotificationContent content={getFormFailureToastMessage(failure.category.type)} />, errorNotificationOptions);
          return;
        }

        if (!booking?.id) {
          toast(<NotificationContent content="The booking was created, but we couldn't open its details page." />, errorNotificationOptions);
          return;
        }

        router.push(getMarketplaceProductBookingDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, product.id, booking.id));
      },
      onError: (error) => {
        toast(<NotificationContent content={`We couldn't complete this booking. ${getRelayErrorMessage(error)}`} />, errorNotificationOptions);
      },
    });
  };

  if (!rootData.product) {
    return null;
  }

  if (bookingPricingOptions.length === 0) {
    return (
      <Alert severity="info" sx={{ borderRadius: 3 }}>
        This product only has recurring plans. Use the plan purchase page instead.
      </Alert>
    );
  }

  return (
    <Box
      sx={{
        display: 'grid',
        gap: { xs: 3, lg: 4 },
        gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1.2fr) 380px' },
        alignItems: 'start',
      }}
    >
      <Card sx={{ borderRadius: 4, border: 1, borderColor: (theme) => theme.palette.divider }}>
        <CardContent sx={{ p: { xs: 2.5, md: 3 } }}>
          <CaptionIconTypography label="Book a workspace" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
          <LeadIconTypography label="Choose a time for your booking" sx={{ mt: 1 }} />
          <BodyIconTypography
            label={
              rootData.product.type.type === 'EVENT'
                ? 'Use this marketplace booking flow to reserve every matching event resource for the chosen time, including across multiple locations. If one required resource is unavailable, the booking cannot go ahead.'
                : selectedResourceIds.length > 0
                  ? 'Use this marketplace booking flow to reserve the exact floor-plan resource selection you made. If any selected resource is unavailable for the chosen time, the booking cannot go ahead.'
                  : 'Use this marketplace booking flow to choose the date, time, payment method, and checkout details for this product.'
            }
            sx={{ mt: 1, opacity: 0.82 }}
          />

          {!rootData.me && (
            <Alert
              severity="info"
              sx={{ mt: 2.5, borderRadius: 3 }}
              action={
                <Button color="inherit" size="small" onClick={handleSignInClick}>
                  Sign in
                </Button>
              }
            >
              Sign in to complete this booking.
            </Alert>
          )}

          <StackColumn spacing={2.25} sx={{ mt: 3 }}>
            <TextField
              select
              label="Pricing option"
              value={effectiveSelectedPricingId}
              onChange={(event) => {
                setSelectedPricingId(event.target.value);
                setPaymentMethod('');
              }}
            >
              {pricingChoices.map((choice) => (
                <MenuItem key={choice.id} value={choice.id}>
                  {choice.label} {choice.description ? `- ${choice.description}` : ''}
                </MenuItem>
              ))}
            </TextField>

            {isEntitlementPricing ? (
              <Alert severity="info" sx={{ borderRadius: 3 }}>
                {getAvailableDaysGuidance(selectedPricingOption?.availableDays)}
              </Alert>
            ) : null}

            {isPurchasingEntitlement ? (
              <DatePicker
                label="Entitlement start date"
                value={entitlementStartDate}
                onChange={(value) => value && setEntitlementStartDate(value)}
                shouldDisableDate={(date) => date.isBefore(startOfDay(), 'day')}
                slotProps={{ textField: { helperText: 'Credits become available after payment; this date is used for the service period and invoice.' } }}
              />
            ) : null}

            {!isPurchasingEntitlement ? (
              <DatePicker
                label="Booking date"
                value={selectedDate}
                onChange={(value) => value && onDateChange(value)}
                shouldDisableDate={(date) => !isDateAvailableForPrice(date, selectedPricingOption?.availableDays)}
                slotProps={{ textField: { helperText: getAvailableDaysGuidance(selectedPricingOption?.availableDays) } }}
              />
            ) : null}

            {!isPurchasingEntitlement ? (
              <TimeRangePicker
                minutesStep={rootData.bookingSlotSizeInMinutes}
                value={timeRange}
                onChange={(value) => {
                  if (value[0] && value[1]) {
                    onTimeRangeChange(value);
                  }
                }}
              />
            ) : null}

            {!isPurchasingEntitlement && !isEventProduct && (
              <TextField
                label="Quantity"
                type="number"
                value={quantity}
                onChange={(event) => {
                  setQuantity(Math.max(1, Number(event.target.value || '1')));
                }}
                slotProps={{ htmlInput: { min: 1 } }}
                sx={{ width: { xs: '100%', sm: 160 } }}
              />
            )}

            {!isPurchasingEntitlement && dateRangeValidation.errorMessage ? <Alert severity="warning">{dateRangeValidation.errorMessage}</Alert> : null}
            {!isPurchasingEntitlement && !dateRangeValidation.errorMessage && availabilityMessage ? (
              <Alert severity={availabilityErrorMessage ? 'warning' : hasEnoughResourcesAvailable ? 'success' : 'error'}>{availabilityMessage}</Alert>
            ) : null}

            {!isInArrearsBilling ? (
              <TextField select label="Payment method" value={effectivePaymentMethod} onChange={(event) => setPaymentMethod(event.target.value as PaymentMethod)}>
                {availablePaymentMethods.map((method) => (
                  <MenuItem key={method.type} value={method.type}>
                    {method.name}
                  </MenuItem>
                ))}
              </TextField>
            ) : (
              <Alert severity="info" sx={{ borderRadius: 3 }}>
                This pricing option is invoiced in arrears. You will receive an invoice in line with the organization&apos;s billing cycle, so there is nothing to choose here yet.
              </Alert>
            )}

            {isPurchasingEntitlement && selectedPricingOption?.supportsSubscriptionAutoRenewal ? (
              <FormControlLabel control={<Switch checked={autoRenew} onChange={(event) => setAutoRenew(event.target.checked)} />} label="Automatically renew this credit package" />
            ) : null}

            <TextField
              label="Invoice emails"
              value={invoiceEmailList.join(', ')}
              onChange={(event) =>
                setInvoiceEmailList(
                  event.target.value
                    .split(',')
                    .map((item) => item.trim())
                    .filter(Boolean),
                )
              }
              helperText="Separate multiple email addresses with commas."
            />

            <CustomerTermsAndConditionsPanel
              accepted={hasAcceptedTermsAndConditions}
              onAcceptedChange={setHasAcceptedTermsAndConditions}
              termsAndConditionsUrl={rootData.product.organization.customerFacingTermsAndConditionsUrl}
            />

            {!isPurchasingEntitlement && !bookingAvailable ? <Alert severity="info">{bookingAvailabilityMessage}</Alert> : null}
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1.25, justifyContent: 'space-between', alignItems: 'center', mt: 1 }}>
              <Box>
                <SubtitleIconTypography label={rootData.product.listingMetadata.title ?? ''} />
                <BodyIconTypography label={selectedPricingOption?.listingMetadata.title ?? ''} sx={{ opacity: 0.72 }} />
              </Box>
              <StackRow spacing={1.25}>
                <Button variant="text" onClick={() => router.push(productLink)} sx={{ textTransform: 'none' }}>
                  Back to product
                </Button>
                <Button
                  variant="contained"
                  onClick={handleSubmit}
                  disabled={
                    isInFlight ||
                    isEntitlementPurchaseInFlight ||
                    (!isPurchasingEntitlement && !bookingAvailable) ||
                    !selectedPricingOption ||
                    (!isPurchasingEntitlement && isCheckingAvailability) ||
                    (!isPurchasingEntitlement && !dateRangeValidation.valid) ||
                    (!isPurchasingEntitlement && !hasEnoughResourcesAvailable) ||
                    (!!rootData.me?.id && !!rootData.product.organization.customerFacingTermsAndConditionsUrl && !hasAcceptedTermsAndConditions)
                  }
                  sx={{ textTransform: 'none' }}
                >
                  {rootData.me ? (isPurchasingEntitlement ? 'Purchase credits' : 'Book now') : 'Sign in to continue'}
                </Button>
              </StackRow>
            </Box>
          </StackColumn>
        </CardContent>
      </Card>

      <StackColumn spacing={2}>
        {rootData.entitlementPurchases
          .filter((purchase) => purchase.paymentStatus === 'PENDING')
          .map((purchase) => (
            <MarketplaceProductBookingPaymentPanel
              key={purchase.id}
              checkoutUrl={purchase.paymentMethod === 'CARD' ? (purchase.paymentAction ?? null) : null}
              ctaLabel="Pay now"
              entityLabel="credit purchase"
              invoiceNumber={purchase.invoiceNumber}
              invoiceUrl={purchase.invoiceUrl ?? purchase.linkedBookings.edges[0]?.node.marketplaceBooking?.invoiceUrl ?? null}
              isPaymentRequired
              paymentExpiry={purchase.paymentExpiry}
              paymentInstructions={purchase.paymentInstructions}
              paymentMethodType={purchase.paymentMethod}
              paymentStatusLabel={purchase.paymentStatus}
              paymentStatusType={purchase.paymentStatus}
            />
          ))}
        <MarketplaceProductBookingSummary
          amountLabel={totalLabel}
          cancellationPolicyType={selectedPricingOption?.cancellationPolicyType}
          cancellationRefundRules={selectedPricingOption?.cancellationRefundRules}
          dateLabel={toShortDate(selectedDate.toISOString())}
          durationLabel={selectedPricingOption?.purchaseCadence === 'HALF_DAY' ? 'Half-day access' : durationLabel}
          paymentLabel={paymentLabel}
          productType={rootData.product.type.type}
          quantity={effectiveQuantity}
          taxLabel={selectedPricingOption?.isTaxInclusive ? 'Tax included' : 'Tax added at invoice'}
          title={selectedPricingOption?.listingMetadata.title ?? rootData.product.listingMetadata.title ?? ''}
        />
      </StackColumn>
    </Box>
  );
};

export default memo(MarketplaceProductBookingForm);
