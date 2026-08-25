import { formatPriceForDisplay, getRelayErrorMessage, startOfDay, toShortDate, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { getMarketplaceProductLink, getMarketplaceSubscriptionDetailsLink } from '@/components/links';
import { CustomerTermsAndConditionsPanel } from '@/components/marketplaceProduct';
import { getAvailableDaysGuidance, isDateAvailableForPrice } from '@/components/marketplaceProduct/available-days';
import { isSubscriptionCadence } from '@/components/marketplaceProductSubscription/subscription-utils';
import { isWeeklySelectionComplete, toggleWeeklySelectedDay, toWeeklySelectedDaysInput } from '@/components/marketplaceProductSubscription/weekly-selection';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import type { marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation } from '@/queries/__generated__/marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation.graphql';
import type { DayOfWeek, marketplaceProductSubscribeForm_query$key, PaymentMethod } from '@/queries/__generated__/marketplaceProductSubscribeForm_query.graphql';
import Alert from '@mui/material/Alert';
import Autocomplete from '@mui/material/Autocomplete';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import FormControlLabel from '@mui/material/FormControlLabel';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import TextField from '@mui/material/TextField';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { Dayjs } from 'dayjs';
import { useRouter, useSearchParams } from 'next/navigation';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MarketplaceProductSubscribeSummary from './marketplace-product-subscribe-summary';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  bookingAvailable: boolean;
  bookingAvailabilityMessage: string;
  rootDataRelay: marketplaceProductSubscribeForm_query$key;
};

const MarketplaceProductSubscribeForm = ({ bookingAvailable, bookingAvailabilityMessage, rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment marketplaceProductSubscribeForm_query on Query @argumentDefinitions(productId: { type: "String!" }) {
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
            price
            isTaxInclusive
            supportsSubscriptionAutoRenewal
            billingMode
            acceptedPaymentMethods
            availableDays
            requiredDaysPerWeek
            numberOfResourcesToBook
            cancellationPolicyType
            cancellationRefundRules {
              minutesBefore
              refundPercentage
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitAddSubscription, isInFlight] = useMutation<marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation>(graphql`
    mutation marketplaceProductSubscribeForm_addMarketplaceBookingSubscriptionMutation($input: AddMarketplaceBookingSubscriptionInput!) {
      addMarketplaceBookingSubscription(input: $input) {
        accessError {
          message
          upgradeRequired
        }
        marketplaceBookingSubscription {
          id
          startedAt
          nextRenewalAt
          autoRenew
          status {
            type
            name
          }
          recurringBookings {
            startDate
            marketplaceBooking {
              id
              isPaymentRequired
              paymentExpiry
              bookingCheckoutSession {
                checkoutUrl
              }
              paymentStatus {
                type
                name
              }
              quantity
              invoiceUrl
              paymentMethod {
                type
                name
              }
            }
          }
          marketplaceBooking {
            id
            isPaymentRequired
            paymentExpiry
            bookingCheckoutSession {
              checkoutUrl
            }
            paymentStatus {
              type
              name
            }
            quantity
            invoiceUrl
            invoiceNumber
            totalAmountToDisplay
            totalAmountExcludeTaxToDisplay
            taxAmountToDisplay
            billingMode
            paymentMethod {
              type
              name
            }
            invoiceEmailList
          }
        }
      }
    }
  `);

  const router = useRouter();
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

  const subscriptionPricingOptions = useMemo(
    () => [...(rootData.product?.pricingOptions ?? [])].filter((option) => isSubscriptionCadence(option.purchaseCadence)).sort((left, right) => left.index - right.index),
    [rootData.product?.pricingOptions],
  );

  const [selectedPricingId, setSelectedPricingId] = useState(initialPricingOptionId ?? '');
  const [startedAt, setStartedAt] = useState<Dayjs>(startOfDay());
  const [quantity, setQuantity] = useState(1);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod | ''>('');
  const [invoiceEmailList, setInvoiceEmailList] = useState<string[]>(() => [...(rootData.me?.emails ?? [])]);
  const [autoRenew, setAutoRenew] = useState(true);
  const [weeklySelectedDays, setWeeklySelectedDays] = useState<DayOfWeek[]>([]);
  const [hasAcceptedTermsAndConditions, setHasAcceptedTermsAndConditions] = useState(false);

  const effectiveSelectedPricingId = useMemo(() => {
    if (subscriptionPricingOptions.some((item) => item.id === selectedPricingId)) {
      return selectedPricingId;
    }

    if (initialPricingOptionId && subscriptionPricingOptions.some((item) => item.id === initialPricingOptionId)) {
      return initialPricingOptionId;
    }

    return subscriptionPricingOptions[0]?.id ?? '';
  }, [initialPricingOptionId, selectedPricingId, subscriptionPricingOptions]);

  const selectedPricingOption = useMemo(
    () => subscriptionPricingOptions.find((item) => item.id === effectiveSelectedPricingId) ?? subscriptionPricingOptions[0] ?? null,
    [effectiveSelectedPricingId, subscriptionPricingOptions],
  );
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
      subscriptionPricingOptions.map((option) => ({
        id: option.id,
        label: option.listingMetadata.title ?? option.listingMetadata.subTitle ?? 'Pricing option',
        description: option.listingMetadata.title ? (option.listingMetadata.subTitle ?? '') : '',
      })),
    [subscriptionPricingOptions],
  );

  const totalLabel = selectedPricingOption ? formatPriceForDisplay(currencyLabel, Number(selectedPricingOption.price) * quantity, selectedPricingOption.purchaseCadence) : '';
  const billingModeLabel = selectedPricingOption?.billingMode === 'IN_ARREARS' ? 'First invoice due now, later cycles billed in arrears' : 'Payment due at checkout';
  const requiredDaysPerWeek = selectedPricingOption?.requiredDaysPerWeek ?? 0;
  const weeklySelectionRequired = selectedPricingOption?.purchaseCadence === 'WEEKLY' && selectedPricingOption?.requiredDaysPerWeek != null;
  const eligibleSelectedDays = (
    selectedPricingOption?.availableDays?.length ? selectedPricingOption.availableDays : ['MONDAY', 'TUESDAY', 'WEDNESDAY', 'THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY']
  ) as DayOfWeek[];
  const weeklySelectionGuidance = weeklySelectionRequired ? `Choose exactly ${requiredDaysPerWeek} days per week.` : null;

  const handleSubmit = () => {
    if (!rootData.product || !selectedPricingOption) {
      return;
    }

    if (!effectivePaymentMethod) {
      toast.error(<NotificationContent content="Select a payment method to continue." />);
      return;
    }

    if (!isWeeklySelectionComplete(weeklySelectionRequired, weeklySelectedDays, requiredDaysPerWeek)) {
      toast.error(<NotificationContent content={weeklySelectionGuidance ?? 'Choose the required days per week to continue.'} />);
      return;
    }

    if (rootData.product.organization.customerFacingTermsAndConditionsUrl && !hasAcceptedTermsAndConditions) {
      toast.error(<NotificationContent content="Accept the space terms and conditions before continuing." />);
      return;
    }
    const id = uuid();
    const subscriptionDetailsLink = getMarketplaceSubscriptionDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, id);

    commitAddSubscription({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
          customerIds: [rootData.me.id],
          organizationIds: [],
          organizationCustomDomains: [organizationCustomDomain],
          teamIds: [],
          requestedResourceIds: selectedResourceIds,
          startedAt: startedAt.utc().startOf('day').toISOString(),
          autoRenew: selectedPricingOption.supportsSubscriptionAutoRenewal ? autoRenew : false,
          cancelAtPeriodEnd: false,
          paymentMethod: effectivePaymentMethod as PaymentMethod,
          invoiceEmailList,
          quantity,
          productVersionId: rootData.product.latestProductVersionId,
          pricingId: selectedPricingOption.id,
          weeklySelectedDays: toWeeklySelectedDaysInput(weeklySelectionRequired, weeklySelectedDays),
          checkoutReturnUrl: new URL(subscriptionDetailsLink, window.location.origin).toString(),
        },
      },
      onCompleted: (response, errors) => {
        if (errors?.length) {
          toast(<NotificationContent content={`We couldn't start this plan. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);
          return;
        }

        const accessError = response.addMarketplaceBookingSubscription?.accessError;
        if (accessError) {
          toast(<NotificationContent content={accessError.message} />, errorNotificationOptions);
          return;
        }

        const subscription = response.addMarketplaceBookingSubscription?.marketplaceBookingSubscription;
        const subscriptionId = subscription?.id ?? '';

        if (subscriptionId) {
          router.push(getMarketplaceSubscriptionDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, subscriptionId));
        }
      },
      onError: (error) => {
        toast(<NotificationContent content={`We couldn't start this plan. ${error.message}`} />, errorNotificationOptions);
      },
    });
  };

  if (!rootData.product) {
    return null;
  }

  if (rootData.product.type.type === 'EVENT') {
    return (
      <Alert severity="info" sx={{ borderRadius: 3 }}>
        Event products support timed bookings only. Use the booking flow instead of starting a recurring plan.
      </Alert>
    );
  }

  if (subscriptionPricingOptions.length === 0) {
    return (
      <Alert severity="info" sx={{ borderRadius: 3 }}>
        This product does not have a recurring plan available yet.
      </Alert>
    );
  }

  const productLink = getMarketplaceProductLink(integratedPlatform, isCustomDomain, organizationCustomDomain, rootData.product.id, selectedResourceIds);

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
          <CaptionIconTypography label="Complete your plan" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
          <LeadIconTypography label="Reserve your access window" sx={{ mt: 1 }} />
          <BodyIconTypography
            label={
              selectedResourceIds.length > 0
                ? 'Choose the plan, start date, and contact emails. This subscription will only try to book the floor-plan resource selection you made, and it will fail instead of switching to another resource.'
                : 'Choose the plan, start date, and contact emails. Resource allocation is handled in the background after purchase.'
            }
            sx={{ mt: 1, opacity: 0.82 }}
          />

          <StackColumn spacing={2.25} sx={{ mt: 3 }}>
            <TextField
              select
              label="Plan"
              value={effectiveSelectedPricingId}
              onChange={(event) => {
                setSelectedPricingId(event.target.value);
                setPaymentMethod('');
                setWeeklySelectedDays([]);
              }}
            >
              {pricingChoices.map((choice) => (
                <MenuItem key={choice.id} value={choice.id}>
                  {choice.label} {choice.description ? `- ${choice.description}` : ''}
                </MenuItem>
              ))}
            </TextField>

            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <DatePicker
                label="Start date"
                value={startedAt}
                onChange={(value) => value && setStartedAt(value)}
                shouldDisableDate={(date) => !isDateAvailableForPrice(date, selectedPricingOption?.availableDays)}
                slotProps={{ textField: { helperText: getAvailableDaysGuidance(selectedPricingOption?.availableDays) } }}
                sx={{ flex: 1 }}
              />
            </Stack>

            {weeklySelectionRequired ? (
              <Box>
                <BodyIconTypography label={weeklySelectionGuidance ?? ''} sx={{ mb: 1 }} />
                <StackRow spacing={1} sx={{ flexWrap: 'wrap' }}>
                  {eligibleSelectedDays.map((day) => {
                    const selected = weeklySelectedDays.includes(day);
                    const atMaximum = weeklySelectedDays.length >= requiredDaysPerWeek;

                    return (
                      <Button
                        key={day}
                        size="small"
                        variant={selected ? 'contained' : 'outlined'}
                        disabled={!selected && atMaximum}
                        onClick={() => setWeeklySelectedDays((current) => toggleWeeklySelectedDay(current, day, requiredDaysPerWeek))}
                        sx={{ textTransform: 'none' }}
                      >
                        {day.charAt(0) + day.slice(1).toLowerCase()}
                      </Button>
                    );
                  })}
                </StackRow>
              </Box>
            ) : null}

            <TextField
              label="Quantity"
              type="number"
              value={quantity}
              onChange={(event) => setQuantity(Math.max(1, Number(event.target.value || '1')))}
              slotProps={{ htmlInput: { min: 1 } }}
              sx={{ width: { xs: '100%', sm: 160 } }}
            />

            <TextField select label="Payment method" value={effectivePaymentMethod} onChange={(event) => setPaymentMethod(event.target.value as PaymentMethod)}>
              {availablePaymentMethods.map((method) => (
                <MenuItem key={method.type} value={method.type}>
                  {method.name}
                </MenuItem>
              ))}
            </TextField>

            <Autocomplete
              multiple
              freeSolo
              options={rootData.me?.emails ?? []}
              value={invoiceEmailList}
              onChange={(_, value) => setInvoiceEmailList(value)}
              renderInput={(params) => <TextField {...params} label="Invoice emails" helperText="We'll send invoice updates to these addresses." />}
            />

            {selectedPricingOption?.supportsSubscriptionAutoRenewal ? (
              <FormControlLabel
                control={<Switch checked={autoRenew} onChange={(event) => setAutoRenew(event.target.checked)} />}
                label="Auto-renew this plan using the latest matching pricing option"
              />
            ) : null}

            <CustomerTermsAndConditionsPanel
              accepted={hasAcceptedTermsAndConditions}
              onAcceptedChange={setHasAcceptedTermsAndConditions}
              termsAndConditionsUrl={rootData.product.organization.customerFacingTermsAndConditionsUrl}
            />

            <Box
              sx={{
                display: 'flex',
                flexWrap: 'wrap',
                gap: 1.25,
                justifyContent: 'space-between',
                alignItems: 'center',
                mt: 1,
              }}
            >
              <Box>
                <SubtitleIconTypography label={rootData.product.listingMetadata.title ?? ''} />
                <BodyIconTypography
                  label={selectedPricingOption?.listingMetadata.title ?? selectedPricingOption?.listingMetadata.subTitle ?? 'Pricing option'}
                  sx={{ opacity: 0.72 }}
                />
              </Box>
              <StackRow spacing={1.25}>
                <Button variant="text" onClick={() => router.push(productLink)} sx={{ textTransform: 'none' }}>
                  Back to product
                </Button>
                <Button
                  variant="contained"
                  onClick={handleSubmit}
                  disabled={
                    !bookingAvailable ||
                    isInFlight ||
                    !selectedPricingOption ||
                    !isWeeklySelectionComplete(weeklySelectionRequired, weeklySelectedDays, requiredDaysPerWeek) ||
                    (!!rootData.product.organization.customerFacingTermsAndConditionsUrl && !hasAcceptedTermsAndConditions)
                  }
                  sx={{ textTransform: 'none' }}
                >
                  Start plan
                </Button>
              </StackRow>
            </Box>
            {!bookingAvailable ? <Alert severity="info">{bookingAvailabilityMessage}</Alert> : null}
          </StackColumn>
        </CardContent>
      </Card>

      <MarketplaceProductSubscribeSummary
        amountLabel={totalLabel}
        autoRenew={selectedPricingOption?.supportsSubscriptionAutoRenewal ? autoRenew : false}
        billingModeLabel={billingModeLabel}
        cancellationPolicyType={selectedPricingOption?.cancellationPolicyType}
        cancellationRefundRules={selectedPricingOption?.cancellationRefundRules}
        productType={rootData.product.type.type}
        quantity={quantity}
        startsOnLabel={toShortDate(startedAt.toISOString())}
        taxLabel={selectedPricingOption?.isTaxInclusive ? 'Tax included' : 'Tax added at invoice'}
        title={selectedPricingOption?.listingMetadata.title ?? rootData.product.listingMetadata.title ?? ''}
      />
    </Box>
  );
};

export default memo(MarketplaceProductSubscribeForm);
