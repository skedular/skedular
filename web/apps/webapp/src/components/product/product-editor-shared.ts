import { listingMetadataSchemaShape } from '@/components/listingMetadata';
import { array, boolean, object, string } from 'yup';
import { v7 as uuid } from 'uuid';

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
};

export type CancellationRefundRuleForm = {
  minutesBefore: string;
  refundPercentage: string;
};

export const cancellationRefundRuleSchema = object({
  minutesBefore: string()
    .required('Minutes before is required.')
    .test('is-number', 'Minutes before must be a valid number.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
    .test('is-not-negative', 'Minutes before must be greater than or equal to 0.', (value) => Number(value) >= 0),
  refundPercentage: string()
    .required('Refund percentage is required.')
    .test('is-number', 'Refund percentage must be a valid number.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
    .test('is-range', 'Refund percentage must be between 0 and 100.', (value) => Number(value) >= 0 && Number(value) <= 100),
});

export const createCancellationRefundRule = (refundPercentage = '100'): CancellationRefundRuleForm => ({
  minutesBefore: '',
  refundPercentage,
});

export const normalizeCancellationRefundRules = (
  cancellationPolicyType: string,
  cancellationRefundRules: readonly { readonly minutesBefore: string | number; readonly refundPercentage: string | number }[] | CancellationRefundRuleForm[] | null | undefined,
): CancellationRefundRuleForm[] => {
  const rules = (cancellationRefundRules ?? []).map((rule) => ({
    minutesBefore: rule.minutesBefore.toString(),
    refundPercentage: rule.refundPercentage.toString(),
  }));

  if (cancellationPolicyType === 'NO_CANCELLATION') {
    return [];
  }

  if (cancellationPolicyType === 'FULL_REFUND_BEFORE_CUTOFF') {
    return [
      {
        minutesBefore: rules[0]?.minutesBefore?.toString() ?? '',
        refundPercentage: '100',
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
  cadence: 'ONE_TIME',
  price: '',
  numberOfResourcesToBook: '1',
  minDurationMinutes: '',
  maxDurationMinutes: '',
  cancellationPolicyType: 'NO_CANCELLATION',
  cancellationRefundRules: [],
  isTaxInclusive: true,
  supportsSubscriptionAutoRenewal: false,
  maxAllowedResourcesLockTimePaidViaCard: defaultMaxAllowedResourcesLockTimePaidViaCard.toString(),
  maxAllowedResourcesLockTimePaidViaBankTransfer: (defaultMaxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString(),
  billingMode: 'NOT_SET',
  acceptedPaymentMethods: [],
});

export const isSubscriptionCadence = (cadence?: string | null) =>
  !!cadence && new Set(['DAILY', 'WEEKLY', 'FORTNIGHTLY', 'MONTHLY', 'TWO_MONTHS', 'QUARTERLY', 'FOUR_MONTHS', 'FIVE_MONTHS', 'SIX_MONTHS', 'YEARLY']).has(cadence);

export const isEventType = (type?: string | null) => type === 'EVENT';

export const getDurationStepDetails = (cadence: string, bookingSlotSizeInMinutes: number) => {
  switch (cadence) {
    case 'PER15_MINUTE':
      return {
        durationStepMinutes: 15,
        durationStepLabel: '15 minutes',
      };

    case 'PER30_MINUTES':
      return {
        durationStepMinutes: 30,
        durationStepLabel: '30 minutes',
      };

    case 'PER_HOUR':
      return {
        durationStepMinutes: 60,
        durationStepLabel: '1 hour (60 minutes)',
      };

    default:
      return {
        durationStepMinutes: bookingSlotSizeInMinutes,
        durationStepLabel: `${bookingSlotSizeInMinutes} minutes`,
      };
  }
};

export const productSchema = (bookingSlotSizeInMinutes: number) =>
  object({
    ...listingMetadataSchemaShape,
    type: string().required('Product type is required.'),
    currency: string().required('Currency is required.'),
    mustBookAllLocationResources: boolean(),
    productTagIds: array().min(1, 'At least one product tag must be selected.').required('Product tags are required.'),
    amenityIds: array().nullable(),
    pricingOptions: array()
      .of(
        object({
          ...listingMetadataSchemaShape,
          cadence: string().required('Pricing cadence is required.'),
          price: string()
            .matches(/^\d+(\.\d{1,2})?$/, 'Price must be a valid decimal number.')
            .required('Price is required.')
            .test('is-greater-than-zero', 'Price must be greater than zero.', (value) => Number(value) > 0),
          numberOfResourcesToBook: string()
            .required('Number of resources to book is required.')
            .test('is-number', 'Number of resources to book must be a valid number.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
            .test('min', 'Number of resources to book must be greater than 0.', (value) => Number(value) > 0),
          minDurationMinutes: string()
            .required('Minimum duration in minutes is required.')
            .test('is-number', 'Minimum duration in minutes must be a valid number.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Minimum duration in minutes must be greater than 0.', (value) => Number(value) > 0)
            .test('is-not-greater-than-a-day', 'Minimum duration cannot be longer than one day.', (value) => Number(value) <= 60 * 24)
            .test('is-valid-duration-step', function (value) {
              const { cadence } = this.parent;
              const { durationStepMinutes, durationStepLabel } = getDurationStepDetails(cadence, bookingSlotSizeInMinutes);
              const minDurationMinutes = Number(value);
              if (isNaN(minDurationMinutes)) {
                return true;
              }

              if (minDurationMinutes % durationStepMinutes !== 0) {
                return this.createError({ message: `Minimum duration in minutes must be in ${durationStepLabel} increments.` });
              }

              return true;
            })
            .test('is-less-than-maxDurationMinutes', 'Minimum duration in minutes must be less or equal than maximum duration in minutes.', function (value) {
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
            .required('Maximum duration in minutes is required.')
            .test('is-number', 'Maximum duration in minutes must be a valid number.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Maximum duration in minutes must be greater than 0.', (value) => Number(value) > 0)
            .test('is-not-greater-than-a-day', 'Maximum duration cannot be longer than one day.', (value) => Number(value) <= 60 * 24)
            .test('is-valid-duration-step', function (value) {
              const { cadence } = this.parent;
              const { durationStepMinutes, durationStepLabel } = getDurationStepDetails(cadence, bookingSlotSizeInMinutes);
              const maxDurationMinutes = Number(value);
              if (isNaN(maxDurationMinutes)) {
                return true;
              }

              if (maxDurationMinutes % durationStepMinutes !== 0) {
                return this.createError({ message: `Maximum duration in minutes must be in ${durationStepLabel} increments.` });
              }

              return true;
            })
            .test('is-less-than-minDurationMinutes', 'Maximum duration in minutes must be greater or equal than minimum duration in minutes.', function (value) {
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
            .required('Cancellation policy is required.')
            .test('is-not-not-set', 'Cancellation policy is required.', (value) => value !== 'NOT_SET'),
          cancellationRefundRules: array()
            .when('cancellationPolicyType', ([cancellationPolicyType], schema) =>
              cancellationPolicyType === 'NO_CANCELLATION' ? schema.of(object()) : schema.of(cancellationRefundRuleSchema),
            )
            .required()
            .test('matches-cancellation-policy', 'Cancellation refund rules do not match the selected cancellation policy.', function (value) {
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
            .required('Max allowed resources lock time paid via card is required.')
            .test('is-number', 'Max allowed resources lock time must be a valid number.', (value) => !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Max allowed resources lock time must be greater than 0.', (value) => Number(value) > 0),
          maxAllowedResourcesLockTimePaidViaBankTransfer: string()
            .required('Max allowed resources lock time paid via bank transfer is required.')
            .test('is-number', 'Max allowed resources lock time must be a valid number.', (value) => !isNaN(Number(value)))
            .test('is-greater-than-zero', 'Max allowed resources lock time must be greater than 0.', (value) => Number(value) > 0),
          billingMode: string()
            .required('Billing mode is required.')
            .test('is-not-not-set', 'Billing mode is required.', (value) => value !== 'NOT_SET'),
          acceptedPaymentMethods: array().min(1, 'At least one accepted booking payment method must be selected.').required('Booking payment methods are required.'),
        }),
      )
      .min(1, 'At least one pricing option is required.')
      .test(
        'is-unique-cadence-numberOfResourcesToBook-and-billingMode',
        'Cadence, number of resources to book, and billing mode combination must be unique for each pricing option.',
        (value) => {
          if (!value || value.length === 0) {
            return true;
          }

          const combinations = new Set<string>();
          for (const pricingOption of value) {
            const combination = `${pricingOption.cadence}|${pricingOption.numberOfResourcesToBook}|${pricingOption.billingMode}`;
            if (combinations.has(combination)) {
              return false;
            }

            combinations.add(combination);
          }

          return true;
        },
      )
      .test('events-cannot-enable-subscriptions', 'Event products cannot enable subscription auto renewal.', function (value) {
        const { type } = this.parent as ProductDetails;
        const pricingOptions = value as PricingOptionForm[] | undefined;
        if (!isEventType(type) || !pricingOptions) {
          return true;
        }

        return pricingOptions.every((pricingOption) => !isSubscriptionCadence(pricingOption.cadence) && !pricingOption.supportsSubscriptionAutoRenewal);
      }),
  });
