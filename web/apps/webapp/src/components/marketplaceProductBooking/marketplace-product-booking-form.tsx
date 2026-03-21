import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { getMarketplaceProductBookingDetailsLink, getMarketplaceProductLink, getSignInLink } from '@/components/links';
import { isSubscriptionCadence } from '@/components/marketplaceProductSubscription/subscription-utils';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import { joinErrors, toShortDate } from '@/libs/utils';
import type {
  BookingCategory,
  marketplaceProductBookingForm_addMarketplaceBookingMutation,
  PaymentMethod,
} from '@/queries/__generated__/marketplaceProductBookingForm_addMarketplaceBookingMutation.graphql';
import type { marketplaceProductBookingForm_query$key } from '@/queries/__generated__/marketplaceProductBookingForm_query.graphql';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import MenuItem from '@mui/material/MenuItem';
import TextField from '@mui/material/TextField';
import { TimeRangePicker } from '@mui/x-date-pickers-pro/TimeRangePicker';
import type { DateRange } from '@mui/x-date-pickers-pro/models';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { Dayjs } from 'dayjs';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MarketplaceProductBookingSummary from './marketplace-product-booking-summary';

type Props = {
  onDateChange: (value: Dayjs) => void;
  onTimeRangeChange: (value: DateRange<Dayjs>) => void;
  rootDataRelay: marketplaceProductBookingForm_query$key;
  selectedDate: Dayjs;
  timeRange: DateRange<Dayjs>;
};

const bookingCategory = 'WORKING_FROM_COWORKING_SPACE' as BookingCategory;

const MarketplaceProductBookingForm = ({ onDateChange, onTimeRangeChange, rootDataRelay, selectedDate, timeRange }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment marketplaceProductBookingForm_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        me {
          id
          emails
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
        productPricingCadences {
          type
          name
        }
        product(id: $productId) {
          id
          latestProductVersionId
          type {
            type
            name
          }
          organization {
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
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitAddBooking, isInFlight] = useMutation<marketplaceProductBookingForm_addMarketplaceBookingMutation>(graphql`
    mutation marketplaceProductBookingForm_addMarketplaceBookingMutation($input: AddMarketplaceBookingInput!) {
      addMarketplaceBooking(input: $input) {
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

  const router = useRouter();
  const searchParams = useSearchParams();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const initialPricingOptionId = searchParams.get('pricingOptionId');

  const bookingPricingOptions = useMemo(
    () => [...(rootData.product?.pricingOptions ?? [])].filter((option) => !isSubscriptionCadence(option.purchaseCadence)).sort((left, right) => left.index - right.index),
    [rootData.product?.pricingOptions],
  );

  const [selectedPricingId, setSelectedPricingId] = useState(initialPricingOptionId ?? '');
  const [quantity, setQuantity] = useState(1);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod | ''>('');
  const [invoiceEmailList, setInvoiceEmailList] = useState<string[]>(() => [...(rootData.me?.emails ?? [])]);
  const [notes, setNotes] = useState('');

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
  const isInArrearsBilling = selectedPricingOption?.billingMode === 'IN_ARREARS';

  useEffect(() => {
    if (!selectedPricingOption) {
      return;
    }

    // Half-day pricing is a fixed-duration product in this booking flow. Keep the selected
    // start time but normalize the range to four hours so the summary and pricing stay valid.
    if (selectedPricingOption.bookingCadence !== 'HALF_DAY') {
      return;
    }

    const [timeFrom, timeUntil] = timeRange;
    if (!timeFrom || !timeUntil) {
      return;
    }

    if (timeUntil.diff(timeFrom, 'minutes') === 240) {
      return;
    }

    onTimeRangeChange([timeFrom, timeFrom.add(4, 'hours')]);
  }, [onTimeRangeChange, selectedPricingOption, timeRange]);

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
        label: rootData.productPricingCadences.find((item) => item.type === option.purchaseCadence)?.name ?? option.purchaseCadence,
        description: option.listingMetadata.title ?? option.listingMetadata.subTitle ?? '',
      })),
    [bookingPricingOptions, rootData.productPricingCadences],
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

    return `${currencyLabel} ${total.toFixed(2)}`;
  }, [currencyLabel, dateRangeValidation, effectiveQuantity, selectedPricingOption]);

  const durationLabel = dateRangeValidation.valid ? `${dateRangeValidation.until.diff(dateRangeValidation.from, 'minutes')} minutes` : 'Invalid time';
  const paymentLabel = isInArrearsBilling
    ? 'Invoice sent on billing cycle'
    : (availablePaymentMethods.find((item) => item.type === effectivePaymentMethod)?.name ?? 'Select payment method');
  const productLink = rootData.product ? getMarketplaceProductLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, rootData.product.id) : '';
  const handleSignInClick = () => {
    const returnTo = `${window.location.pathname}${window.location.search}`;
    router.push(`${getSignInLink()}?returnTo=${encodeURIComponent(returnTo)}`);
  };

  const handleSubmit = () => {
    if (!rootData.product || !selectedPricingOption) {
      return;
    }

    const product = rootData.product;

    if (!rootData.me?.id) {
      handleSignInClick();
      return;
    }

    if (!dateRangeValidation.valid) {
      toast.error(<NotificationContent content={dateRangeValidation.errorMessage} />);
      return;
    }

    const submittedPaymentMethod = isInArrearsBilling ? (availablePaymentMethods[0]?.type ?? '') : effectivePaymentMethod;

    if (!submittedPaymentMethod) {
      toast.error(<NotificationContent content="Select a payment method to continue." />);
      return;
    }

    const toastId = toast(<NotificationContent content={`Making your booking for ${toShortDate(dateRangeValidation.from.toISOString())}...`} />, infoNotificationOptions);
    const id = uuid();

    commitAddBooking({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          customerIds: [rootData.me.id],
          from: dateRangeValidation.from.toISOString(),
          until: dateRangeValidation.until.toISOString(),
          notes,
          organizationCustomDomains: [organizationCustomDomain],
          organizationIds: [],
          teamIds: [],
          resourceIds: [],
          category: bookingCategory,
          paymentMethod: submittedPaymentMethod as PaymentMethod,
          invoiceEmailList,
          quantity: effectiveQuantity,
          productVersionId: product.latestProductVersionId,
          pricingId: selectedPricingOption.id,
          checkoutReturnUrl: new URL(
            getMarketplaceProductBookingDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, product.id, id),
            window.location.origin,
          ).toString(),
        },
      },
      onCompleted: (response, errors) => {
        if (errors?.length) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't complete this booking. ${joinErrors(errors)}`} />,
          });
          return;
        }

        const booking = response.addMarketplaceBooking?.booking;
        if (!booking?.id) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content="The booking was created, but we couldn't open its details page." />,
          });
          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking confirmed for ${toShortDate(booking?.from)}.`} />,
        });

        router.push(getMarketplaceProductBookingDetailsLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, product.id, booking.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't complete this booking. ${error.message}`} />,
        });
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

            <DatePicker label="Booking date" value={selectedDate} onChange={(value) => value && onDateChange(value)} />

            <TimeRangePicker
              minutesStep={rootData.bookingSlotSizeInMinutes}
              value={timeRange}
              onChange={(value) => {
                if (value[0] && value[1]) {
                  onTimeRangeChange(value);
                }
              }}
            />

            {!isEventProduct && (
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

            {dateRangeValidation.errorMessage ? <Alert severity="warning">{dateRangeValidation.errorMessage}</Alert> : null}

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

            <TextField label="Notes" multiline minRows={3} value={notes} onChange={(event) => setNotes(event.target.value)} helperText="Optional notes for the workspace team." />

            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1.25, justifyContent: 'space-between', alignItems: 'center', mt: 1 }}>
              <Box>
                <SubtitleIconTypography label={rootData.product.listingMetadata.title ?? ''} />
                <BodyIconTypography label={selectedPricingOption?.listingMetadata.title ?? ''} sx={{ opacity: 0.72 }} />
              </Box>
              <StackRow spacing={1.25}>
                <Button variant="text" onClick={() => router.push(productLink)} sx={{ textTransform: 'none' }}>
                  Back to product
                </Button>
                <Button variant="contained" onClick={handleSubmit} disabled={isInFlight || !selectedPricingOption} sx={{ textTransform: 'none' }}>
                  {rootData.me ? 'Book now' : 'Sign in to continue'}
                </Button>
              </StackRow>
            </Box>
          </StackColumn>
        </CardContent>
      </Card>

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
        termsAndConditionsUrl={rootData.product?.organization.customerFacingTermsAndConditionsUrl}
        title={selectedPricingOption?.listingMetadata.title ?? rootData.product.listingMetadata.title ?? ''}
      />
    </Box>
  );
};

export default memo(MarketplaceProductBookingForm);
