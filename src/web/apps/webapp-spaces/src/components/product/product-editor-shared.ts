import { listingMetadataSchemaShape } from '@/components/listingMetadata';
import { v7 as uuid } from 'uuid';
import { array, boolean, object, string } from 'yup';

export type ProductDetails = {
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
  type: string;
  currency: string;
  productTagIds: string[];
  amenityIds: string[];
  pricingOptions: PricingOptionForm[];
};

export type PricingOptionForm = {
  id: string;
  title: string | null;
  subTitle: string | null;
  cadence: string;
  price: string;
  numberOfResourcesToBook: string;
  minDurationMinutes: string;
  maxDurationMinutes: string;
  cancellationPolicyType: string;
  cancellationRefundRules: CancellationRefundRuleForm[];
  isTaxInclusive: boolean;
  supportsSubscriptionAutoRenewal: boolean;
  maxAllowedResourcesLockTimePaidViaCard: string;
  maxAllowedResourcesLockTimePaidViaBankTransfer: string;
  billingMode: string;
  acceptedPaymentMethods: string[];
  /** Empty means this price is available every calendar day. */
  availableDays: string[];
  requiredDaysPerWeek: string;
  fulfillmentType?: string;
  entitlementCreditQuantity?: string;
  entitlementValidityDays?: string;
  minDurationDisplayUnit?: string | null;
  maxDurationDisplayUnit?: string | null;
  maxAllowedResourcesLockTimePaidViaCardDisplayUnit?: string | null;
  maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit?: string | null;
};

export type CancellationRefundRuleForm = {
  minutesBefore: string;
  refundPercentage: string;
  displayUnit?: string | null;
};

export const cancellationRefundRuleSchema = object({
  minutesBefore: string()
    .required('Please enter how long before the booking this rule applies.')
    .test('is-number', 'Enter a valid number of minutes.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
    .test('is-not-negative', 'Minutes before must be 0 or more.', (value) => Number(value) >= 0),
  refundPercentage: string()
    .required('Please enter the refund percentage.')
    .test('is-number', 'Enter a valid refund percentage.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
    .test('is-range', 'Refund percentage must be between 0 and 100.', (value) => Number(value) >= 0 && Number(value) <= 100),
});

export const createCancellationRefundRule = (refundPercentage = '100'): CancellationRefundRuleForm => ({
  minutesBefore: '',
  refundPercentage,
  displayUnit: null,
});

export const normalizeCancellationRefundRules = (
  cancellationPolicyType: string,
  cancellationRefundRules:
    | readonly { readonly minutesBefore: string | number; readonly refundPercentage: string | number; readonly displayUnit?: 'MINUTES' | 'HOURS' | null }[]
    | CancellationRefundRuleForm[]
    | null
    | undefined,
): CancellationRefundRuleForm[] => {
  const rules = (cancellationRefundRules ?? []).map((rule) => ({
    minutesBefore: rule.minutesBefore.toString(),
    refundPercentage: rule.refundPercentage.toString(),
    displayUnit: rule.displayUnit ?? null,
  }));

  if (cancellationPolicyType === 'NO_CANCELLATION') {
    return [];
  }

  if (cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF') {
    return [
      {
        minutesBefore: rules[0]?.minutesBefore?.toString() ?? '',
        refundPercentage: '100',
        displayUnit: rules[0]?.displayUnit ?? null,
      },
    ];
  }

  if (cancellationPolicyType === 'TIERED_REFUND') {
    return rules.length > 0 ? rules : [createCancellationRefundRule('100')];
  }

  return rules;
};

export const createPricingOption = (defaultMaxAllowedResourcesLockTimePaidViaCard: number, defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number): PricingOptionForm => ({
  id: uuid(),
  title: null,
  subTitle: null,
  cadence: 'DAILY',
  price: '',
  numberOfResourcesToBook: '1',
  minDurationMinutes: '',
  maxDurationMinutes: '',
  cancellationPolicyType: 'NO_CANCELLATION',
  cancellationRefundRules: [],
  isTaxInclusive: true,
  supportsSubscriptionAutoRenewal: false,
  maxAllowedResourcesLockTimePaidViaCard: defaultMaxAllowedResourcesLockTimePaidViaCard.toString(),
  maxAllowedResourcesLockTimePaidViaBankTransfer: defaultMaxAllowedResourcesLockTimePaidViaBankTransfer.toString(),
  billingMode: 'NOT_SET',
  acceptedPaymentMethods: [],
  availableDays: [],
  requiredDaysPerWeek: '',
  fulfillmentType: 'RESERVATION',
  entitlementCreditQuantity: '',
  entitlementValidityDays: '',
  minDurationDisplayUnit: null,
  maxDurationDisplayUnit: null,
  maxAllowedResourcesLockTimePaidViaCardDisplayUnit: null,
  maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: null,
});

export const isSubscriptionCadence = (cadence?: string | null) =>
  !!cadence && new Set(['DAILY', 'WEEKLY', 'FORTNIGHTLY', 'MONTHLY', 'TWO_MONTHS', 'QUARTERLY', 'FOUR_MONTHS', 'FIVE_MONTHS', 'SIX_MONTHS', 'YEARLY']).has(cadence);

export const isEventType = (type?: string | null) => type === 'EVENT';

export const toRequiredDaysPerWeekInput = (cadence: string, requiredDaysPerWeek: string) =>
  cadence !== 'DAILY' && requiredDaysPerWeek.trim() ? Number(requiredDaysPerWeek) : null;

export const sanitizeWeeklyRequiredDays = (value: string) => value.replace(/[^0-9]/g, '').slice(0, 1);

export const getDurationStepDetails = (_cadence: string) => {
  switch (_cadence) {
    default:
      return {
        durationStepMinutes: 1,
        durationStepLabel: '1 minute',
      };
  }
};

export const productSchema = () =>
  object({
    ...listingMetadataSchemaShape,
    type: string().required('Please choose a product type.'),
    currency: string().required('Please choose a currency.'),
    mustBookAllLocationResources: boolean(),
    productTagIds: array().min(1, 'Choose at least one booking group.').required('Please choose at least one booking group.'),
    amenityIds: array().nullable(),
    pricingOptions: array()
      .of(
        object({
          ...listingMetadataSchemaShape,
          cadence: string().required('Please choose how often this pricing applies.'),
          price: string()
            .matches(/^\d+(\.\d{1,2})?$/, 'Enter a valid price.')
            .required('Please enter a price.')
            .test('is-greater-than-zero', 'Price must be more than zero.', (value) => Number(value) > 0),
          numberOfResourcesToBook: string()
            .required('Please enter how many resources can be booked.')
            .test('is-number', 'Enter a valid number of resources.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
            .test('min', 'The number of resources must be at least 1.', (value) => Number(value) > 0),
          minDurationMinutes: string()
            .required('Please enter the minimum booking length.')
            .test('is-number', 'Enter a valid minimum booking length.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Minimum booking length must be more than zero.', (value) => Number(value) > 0)
            .test('is-not-greater-than-a-day', 'Minimum duration cannot be longer than one day.', (value) => Number(value) <= 60 * 24)
            .test('is-valid-duration-step', function (value) {
              const { cadence } = this.parent;
              const { durationStepMinutes, durationStepLabel } = getDurationStepDetails(cadence);
              const minDurationMinutes = Number(value);
              if (isNaN(minDurationMinutes)) {
                return true;
              }

              if (minDurationMinutes % durationStepMinutes !== 0) {
                return this.createError({ message: `Minimum booking length must use ${durationStepLabel} steps.` });
              }

              return true;
            })
            .test('is-less-than-maxDurationMinutes', 'Minimum booking length cannot be longer than the maximum booking length.', function (value) {
              const { maxDurationMinutes: maxDurationMinutesStr } = this.parent;
              const maxDurationMinutes = Number(maxDurationMinutesStr);
              if (isNaN(maxDurationMinutes)) {
                return true;
              }

              const minDurationMinutes = Number(value);
              if (isNaN(minDurationMinutes)) {
                return true;
              }

              return minDurationMinutes <= maxDurationMinutes;
            }),
          maxDurationMinutes: string()
            .required('Please enter the maximum booking length.')
            .test('is-number', 'Enter a valid maximum booking length.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Maximum booking length must be more than zero.', (value) => Number(value) > 0)
            .test('is-not-greater-than-a-day', 'Maximum duration cannot be longer than one day.', (value) => Number(value) <= 60 * 24)
            .test('is-valid-duration-step', function (value) {
              const { cadence } = this.parent;
              const { durationStepMinutes, durationStepLabel } = getDurationStepDetails(cadence);
              const maxDurationMinutes = Number(value);
              if (isNaN(maxDurationMinutes)) {
                return true;
              }

              if (maxDurationMinutes % durationStepMinutes !== 0) {
                return this.createError({ message: `Maximum booking length must use ${durationStepLabel} steps.` });
              }

              return true;
            })
            .test('is-less-than-minDurationMinutes', 'Maximum booking length cannot be shorter than the minimum booking length.', function (value) {
              const { minDurationMinutes: minDurationMinutesStr } = this.parent;
              const minDurationMinutes = Number(minDurationMinutesStr);
              if (isNaN(minDurationMinutes)) {
                return true;
              }

              const maxDurationMinutes = Number(value);
              if (isNaN(maxDurationMinutes)) {
                return true;
              }

              return maxDurationMinutes >= minDurationMinutes;
            }),
          cancellationPolicyType: string()
            .required('Please choose a cancellation policy.')
            .test('is-not-not-set', 'Please choose a cancellation policy.', (value) => value !== 'NOT_SET'),
          cancellationRefundRules: array()
            .when('cancellationPolicyType', ([cancellationPolicyType], schema) =>
              cancellationPolicyType === 'NO_CANCELLATION' ? schema.of(object()) : schema.of(cancellationRefundRuleSchema),
            )
            .required()
            .test('matches-cancellation-policy', 'The refund rules do not match the selected cancellation policy.', function (value) {
              const { cancellationPolicyType } = this.parent as PricingOptionForm;
              const rules = value ?? [];

              if (cancellationPolicyType === 'NO_CANCELLATION') {
                return rules.length === 0;
              }

              if (cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF') {
                return true;
              }

              if (cancellationPolicyType === 'TIERED_REFUND') {
                return rules.length > 0 && new Set(rules.map((item) => item.minutesBefore)).size === rules.length;
              }

              return false;
            }),
          isTaxInclusive: boolean().required(),
          supportsSubscriptionAutoRenewal: boolean().required(),
          maxAllowedResourcesLockTimePaidViaCard: string()
            .required('Please enter the hold time for card payments.')
            .test('is-number', 'Enter a valid hold time.', (value) => !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Hold time must be more than zero.', (value) => Number(value) > 0),
          maxAllowedResourcesLockTimePaidViaBankTransfer: string()
            .required('Please enter the hold time for bank transfers.')
            .test('is-number', 'Enter a valid hold time.', (value) => !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Hold time must be more than zero.', (value) => Number(value) > 0),
          billingMode: string()
            .required('Please choose a billing mode.')
            .test('is-not-not-set', 'Please choose a billing mode.', (value) => value !== 'NOT_SET'),
          acceptedPaymentMethods: array().min(1, 'Choose at least one accepted payment method.').required('Please choose at least one accepted payment method.'),
          requiredDaysPerWeek: string().test('weekly-day-selection', 'Set the required number of selected days from 1 up to the enabled weekdays.', function (value) {
            const { cadence, availableDays } = this.parent as PricingOptionForm;
            if (cadence === 'DAILY') return !value;
            if (!value) return true;
            const required = Number(value);
            const availableCount = availableDays.length || 7;
            return Number.isInteger(required) && required > 0 && required <= availableCount;
          }),
        }),
      )
      .min(1, 'Add at least one pricing option.')
      .test(
        'is-unique-cadence-numberOfResourcesToBook-and-billingMode',
        'Each pricing option must use a different combination of frequency, quantity, and billing mode.',
        (value) => {
          if (!value || value.length === 0) {
            return true;
          }

          const combinations = new Set<string>();
          for (const pricingOption of value) {
            const combination = `${pricingOption.cadence}|${pricingOption.numberOfResourcesToBook}|${pricingOption.billingMode}|${pricingOption.cadence === 'DAILY' ? '' : pricingOption.requiredDaysPerWeek}`;
            if (combinations.has(combination)) {
              return false;
            }

            combinations.add(combination);
          }

          return true;
        },
      )
      .test('events-cannot-enable-subscriptions', 'Event products cannot use subscription auto-renewal.', function (value) {
        const { type } = this.parent as ProductDetails;
        const pricingOptions = value as PricingOptionForm[] | undefined;
        if (!isEventType(type) || !pricingOptions) {
          return true;
        }

        return pricingOptions.every((pricingOption) => !isSubscriptionCadence(pricingOption.cadence) && !pricingOption.supportsSubscriptionAutoRenewal);
      }),
  });
