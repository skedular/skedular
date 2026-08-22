import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { listingMetadataSchemaShape } from '@/components/listingMetadata';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import ProductEditorForm from '@/components/product/product-editor-form';
import { toRequiredDaysPerWeekInput } from '@/components/product/product-editor-shared';
import type { editProduct_query$key } from '@/queries/__generated__/editProduct_query.graphql';
import type {
  Currency,
  editProduct_updateProductMutation,
  PaymentMethod,
  ProductPatchField,
  ProductPricingCadence,
  ProductType,
} from '@/queries/__generated__/editProduct_updateProductMutation.graphql';
import Box from '@mui/material/Box';
import { getRelayErrorMessage, keyboardTextFieldDebounceTimeout, PaletteModeContext } from '@skedular/shared';
import { makeRequired, makeValidate } from 'mui-rff';
import { memo, useContext, useMemo, useRef, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: editProduct_query$key;
  onReloadRequired?: () => void;
  organizationCustomDomain: string;
};

type ProductDetails = {
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
  type: string;
  currency: string;
  productTagIds: string[];
  amenityIds: string[];
  pricingOptions: PricingOptionForm[];
};

type PricingOptionForm = {
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
  availableDays: string[];
  requiredDaysPerWeek: string;
  fulfillmentType: string;
  entitlementCreditQuantity: string;
  entitlementValidityDays: string;
  minDurationDisplayUnit?: string | null;
  maxDurationDisplayUnit?: string | null;
  maxAllowedResourcesLockTimePaidViaCardDisplayUnit?: string | null;
  maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit?: string | null;
};

type CancellationRefundRuleForm = {
  minutesBefore: string;
  refundPercentage: string;
  displayUnit?: 'MINUTES' | 'HOURS' | null;
};
const productAutosaveDebounceTimeout = 1000;

const productFieldGroups: ReadonlyArray<[ProductPatchField, ReadonlyArray<keyof ProductDetails>]> = [
  ['LISTING_METADATA', ['title', 'subTitle', 'includedFeatures']],
  ['TYPE', ['type']],
  ['CURRENCY', ['currency']],
  ['TAGS', ['productTagIds', 'amenityIds']],
  ['PRICING_OPTIONS', ['pricingOptions']],
];

const getComparableProductFieldValue = (productDetails: ProductDetails, field: keyof ProductDetails) => {
  if (field === 'pricingOptions') {
    return productDetails.pricingOptions.map(({ id, ...pricingOption }) => {
      void id;

      return pricingOption;
    });
  }

  return productDetails[field];
};

const getChangedProductFields = (
  left: ProductDetails | null,
  right: ProductDetails,
  leftFeatureImages: FileUploadResponse[],
  rightFeatureImages: FileUploadResponse[],
): ProductPatchField[] => {
  if (!left) return [];
  const changed: ProductPatchField[] = [];
  for (const [patchField, formFields] of productFieldGroups) {
    if (formFields.some((f) => JSON.stringify(getComparableProductFieldValue(left, f)) !== JSON.stringify(getComparableProductFieldValue(right, f)))) {
      changed.push(patchField);
    }
  }
  if (JSON.stringify(leftFeatureImages) !== JSON.stringify(rightFeatureImages)) changed.push('FEATURE_IMAGES');
  return changed;
};

const getValidProductPatchFields = (
  productDetailsSchema: ReturnType<typeof productSchema>,
  fieldsToUpdate: ProductPatchField[],
  productDetails: ProductDetails,
): ProductPatchField[] =>
  fieldsToUpdate.filter((patchField) => {
    if (patchField === 'FEATURE_IMAGES') {
      return true;
    }

    const formFields = productFieldGroups.find(([field]) => field === patchField)?.[1] ?? [];

    try {
      for (const formField of formFields) {
        productDetailsSchema.validateSyncAt(formField, productDetails);
      }

      return true;
    } catch {
      return false;
    }
  });

const cancellationRefundRuleSchema = object({
  minutesBefore: string()
    .required('Please enter how long before the booking this rule applies.')
    .test('is-number', 'Enter a valid number of minutes.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
    .test('is-not-negative', 'Minutes before must be 0 or more.', (value) => Number(value) >= 0),
  refundPercentage: string()
    .required('Please enter the refund percentage.')
    .test('is-number', 'Enter a valid refund percentage.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
    .test('is-range', 'Refund percentage must be between 0 and 100.', (value) => Number(value) >= 0 && Number(value) <= 100),
});

const createCancellationRefundRule = (refundPercentage = '100'): CancellationRefundRuleForm => ({
  minutesBefore: '',
  refundPercentage,
});

const normalizeCancellationRefundRules = (
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

const createPricingOption = (defaultMaxAllowedResourcesLockTimePaidViaCard: number, defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number): PricingOptionForm => ({
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
  maxAllowedResourcesLockTimePaidViaBankTransfer: defaultMaxAllowedResourcesLockTimePaidViaBankTransfer.toString(),
  billingMode: 'NOT_SET',
  acceptedPaymentMethods: [],
  availableDays: [],
  requiredDaysPerWeek: '',
  fulfillmentType: 'RESERVATION',
  entitlementCreditQuantity: '',
  entitlementValidityDays: '',
});

const isSubscriptionCadence = (cadence?: string | null) =>
  !!cadence && new Set(['DAILY', 'WEEKLY', 'FORTNIGHTLY', 'MONTHLY', 'TWO_MONTHS', 'QUARTERLY', 'FOUR_MONTHS', 'FIVE_MONTHS', 'SIX_MONTHS', 'YEARLY']).has(cadence);

const isEventType = (type?: string | null) => type === 'EVENT';

const getDurationStepDetails = (cadence: string, bookingSlotSizeInMinutes: number) => {
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

const productSchema = (bookingSlotSizeInMinutes: number) =>
  object({
    ...listingMetadataSchemaShape,
    type: string().required('Please choose a product type.'),
    currency: string().required('Please choose a currency.'),
    mustBookAllLocationResources: boolean(),
    productTagIds: array().min(1, 'Choose at least one product tag.').required('Please choose at least one product tag.'),
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
              const { durationStepMinutes, durationStepLabel } = getDurationStepDetails(cadence, bookingSlotSizeInMinutes);

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
              const { durationStepMinutes, durationStepLabel } = getDurationStepDetails(cadence, bookingSlotSizeInMinutes);

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

          const seenCombinations = new Set<string>();
          for (const pricingOption of value as PricingOptionForm[]) {
            const combination = `${pricingOption.cadence}|${pricingOption.numberOfResourcesToBook}|${pricingOption.billingMode}|${pricingOption.cadence === 'WEEKLY' ? pricingOption.requiredDaysPerWeek : ''}`;
            if (seenCombinations.has(combination)) {
              return false;
            }

            seenCombinations.add(combination);
          }

          return true;
        },
      )
      .test('event-products-must-use-explicit-time-bookings', 'Event products must use pricing with specific booking times.', function (value) {
        const { type } = this.parent as ProductDetails;
        const pricingOptions = value as PricingOptionForm[] | undefined;
        if (!isEventType(type) || !pricingOptions) {
          return true;
        }

        return pricingOptions.every((pricingOption) => !isSubscriptionCadence(pricingOption.cadence) && !pricingOption.supportsSubscriptionAutoRenewal);
      })
      .required('Please add at least one pricing option.'),
  });

const EditProduct = ({ rootDataRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<editProduct_query$key>(
    graphql`
      fragment editProduct_query on Query {
        product(id: $productId) {
          id
          inactive
          listingMetadata {
            title
            subTitle
            includedFeatures
          }
          type {
            type
            name
          }
          currency {
            type
            name
          }
          productTags {
            id
            name
            color
          }
          amenities {
            id
            name
            color
          }
          featureImages {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
          pricingOptions {
            id
            index
            listingMetadata {
              title
              subTitle
            }
            supportsSubscriptionAutoRenewal
            purchaseCadence
            bookingCadence
            price
            availableDays
            requiredDaysPerWeek
            numberOfResourcesToBook
            minDurationMinutes
            minDurationDisplayUnit
            maxDurationMinutes
            maxDurationDisplayUnit
            cancellationPolicyType
            cancellationRefundRules {
              minutesBefore
              displayUnit
              refundPercentage
            }
            isTaxInclusive
            maxAllowedResourcesLockTimePaidViaCard
            maxAllowedResourcesLockTimePaidViaCardDisplayUnit
            maxAllowedResourcesLockTimePaidViaBankTransfer
            maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit
            billingMode
            acceptedPaymentMethods
            fulfillmentType
            entitlementCreditQuantity
            entitlementValidityDays
          }
        }
        productPricingCadences {
          type
          name
        }
        ...singleChoiceProductPricingBillingMode_query
        currencies {
          type
          name
        }
        bookingSlotSizeInMinutes
        defaultMaxAllowedResourcesLockTimePaidViaCard
        defaultMaxAllowedResourcesLockTimePaidViaBankTransfer
        ...multipleChoicesProductTags_query
        ...singleChoiceCurrency_query
        ...multipleChoicesPaymentMethodTypes_query
        ...singleChoiceProductPricingCadence_query
        ...singleChoiceProductPricingCancellationType_query
        ...multipleChoicesAmenities_query
        ...singleChoiceProductType_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateProduct] = useMutation<editProduct_updateProductMutation>(graphql`
    mutation editProduct_updateProductMutation($input: UpdateProductInput!) @raw_response_type {
      updateProduct(input: $input) {
        product {
          id
          inactive
          listingMetadata {
            title
            subTitle
            includedFeatures
          }
          type {
            type
            name
          }
          currency {
            type
            name
          }
          productTags {
            id
            name
            color
          }
          amenities {
            id
            name
            color
          }
          featureImages {
            original {
              url
              height
              width
            }
            thumbnail {
              url
              height
              width
            }
          }
          pricingOptions {
            index
            listingMetadata {
              title
              subTitle
            }
            supportsSubscriptionAutoRenewal
            purchaseCadence
            bookingCadence
            price
            availableDays
            requiredDaysPerWeek
            numberOfResourcesToBook
            minDurationMinutes
            minDurationDisplayUnit
            maxDurationMinutes
            maxDurationDisplayUnit
            cancellationPolicyType
            cancellationRefundRules {
              minutesBefore
              displayUnit
              refundPercentage
            }
            isTaxInclusive
            maxAllowedResourcesLockTimePaidViaCard
            maxAllowedResourcesLockTimePaidViaCardDisplayUnit
            maxAllowedResourcesLockTimePaidViaBankTransfer
            maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit
            acceptedPaymentMethods
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const productDetailsSchema = productSchema(rootData.bookingSlotSizeInMinutes);
  const validateProductDetails = makeValidate(productDetailsSchema);
  const requiredFields = makeRequired(productDetailsSchema);

  const [title, setTitle] = useState<string | null>(rootData.product?.listingMetadata.title ?? null);
  const debounceSetTitle = useDebounceCallback(setTitle, keyboardTextFieldDebounceTimeout);
  const [subTitle, setSubTitle] = useState<string | null>(rootData.product?.listingMetadata.subTitle ?? null);
  const debounceSetSubTitle = useDebounceCallback(setSubTitle, keyboardTextFieldDebounceTimeout);
  const [includedFeatures, setIncludedFeatures] = useState<string | null>(rootData.product?.listingMetadata.includedFeatures?.join('\n') ?? null);
  const debounceSetIncludedFeatures = useDebounceCallback(setIncludedFeatures, keyboardTextFieldDebounceTimeout);

  const [type, setType] = useState(rootData.product ? rootData.product.type.type : '');
  const debounceSetType = useDebounceCallback(setType, keyboardTextFieldDebounceTimeout);
  const [currency, setCurrency] = useState(rootData.product ? rootData.product.currency.type : '');
  const debounceSetCurrency = useDebounceCallback(setCurrency, keyboardTextFieldDebounceTimeout);

  const [productTagIds, setProductTagIds] = useState<string[]>(rootData.product ? rootData.product.productTags.map(({ id }) => id) : []);
  const debounceSetProductTagIds = useDebounceCallback(setProductTagIds, keyboardTextFieldDebounceTimeout);

  const [amenityIds, setAmenityIds] = useState<string[]>(rootData.product?.amenities.map((item) => item.id) ?? []);
  const debounceSetAmenityIds = useDebounceCallback(setAmenityIds, keyboardTextFieldDebounceTimeout);

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>(
    rootData.product
      ? rootData.product.featureImages
          .filter((item) => !!item.original)
          .map((item) => ({
            id: '',
            original: {
              url: item.original!.url,
              height: item.original!.height,
              width: item.original!.width,
            },
            thumbnail: item.thumbnail
              ? {
                  url: item.thumbnail.url,
                  height: item.thumbnail.height,
                  width: item.thumbnail.width,
                }
              : null,
          }))
      : [],
  );
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(featureImages[0] ?? null);
  const initialProductValues = useMemo<ProductDetails | null>(
    () =>
      rootData.product
        ? {
            title,
            subTitle,
            includedFeatures,
            type,
            currency,
            productTagIds,
            amenityIds,
            pricingOptions:
              rootData.product.pricingOptions.length > 0
                ? rootData.product.pricingOptions.map((pricingOption) => ({
                    id: pricingOption.id,
                    title: pricingOption.listingMetadata.title ?? null,
                    subTitle: pricingOption.listingMetadata.subTitle ?? null,
                    cadence: pricingOption.purchaseCadence,
                    price: pricingOption.price.toString(),
                    fulfillmentType: (pricingOption as unknown as { fulfillmentType?: string }).fulfillmentType ?? 'RESERVATION',
                    entitlementCreditQuantity: (pricingOption as unknown as { entitlementCreditQuantity?: number }).entitlementCreditQuantity?.toString() ?? '',
                    entitlementValidityDays: (pricingOption as unknown as { entitlementValidityDays?: number }).entitlementValidityDays?.toString() ?? '',
                    supportsSubscriptionAutoRenewal: pricingOption.supportsSubscriptionAutoRenewal,
                    numberOfResourcesToBook: pricingOption.numberOfResourcesToBook.toString(),
                    minDurationMinutes: pricingOption.minDurationMinutes ? pricingOption.minDurationMinutes.toString() : '',
                    minDurationDisplayUnit: pricingOption.minDurationDisplayUnit ?? null,
                    maxDurationMinutes: pricingOption.maxDurationMinutes ? pricingOption.maxDurationMinutes.toString() : '',
                    maxDurationDisplayUnit: pricingOption.maxDurationDisplayUnit ?? null,
                    cancellationPolicyType: pricingOption.cancellationPolicyType,
                    cancellationRefundRules: normalizeCancellationRefundRules(pricingOption.cancellationPolicyType, pricingOption.cancellationRefundRules).map((item) => ({
                      minutesBefore: item.minutesBefore.toString(),
                      displayUnit: item.displayUnit ?? null,
                      refundPercentage: item.refundPercentage.toString(),
                    })),
                    isTaxInclusive: pricingOption.isTaxInclusive,
                    maxAllowedResourcesLockTimePaidViaCard: pricingOption.maxAllowedResourcesLockTimePaidViaCard.toString(),
                    maxAllowedResourcesLockTimePaidViaCardDisplayUnit: pricingOption.maxAllowedResourcesLockTimePaidViaCardDisplayUnit ?? null,
                    maxAllowedResourcesLockTimePaidViaBankTransfer: pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer.toString(),
                    maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: pricingOption.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit ?? null,
                    billingMode: (pricingOption as unknown as { billingMode?: string }).billingMode ?? 'NOT_SET',
                    acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((item) => item),
                    availableDays: (pricingOption as unknown as { availableDays?: readonly string[] }).availableDays?.slice() ?? [],
                    requiredDaysPerWeek: pricingOption.requiredDaysPerWeek?.toString() ?? '',
                  }))
                : [createPricingOption(rootData.defaultMaxAllowedResourcesLockTimePaidViaCard, rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer)],
          }
        : null,
    [
      amenityIds,
      currency,
      includedFeatures,
      productTagIds,
      rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer,
      rootData.defaultMaxAllowedResourcesLockTimePaidViaCard,
      rootData.product,
      subTitle,
      title,
      type,
    ],
  );
  const previousProductValues = useRef<ProductDetails | null>(initialProductValues);
  const previousFeatureImages = useRef<FileUploadResponse[]>(featureImages);

  const handleProductDetailUpdateClick = (fieldsToUpdate: ProductPatchField[], productDetails: ProductDetails) => {
    const { title, subTitle, includedFeatures, type, currency, productTagIds, amenityIds, pricingOptions } = productDetails;
    const product = rootData.product;
    const validFieldsToUpdate = getValidProductPatchFields(productDetailsSchema, fieldsToUpdate, productDetails);
    if (!product || validFieldsToUpdate.length === 0) {
      return;
    }

    const productTitle = product.listingMetadata.title || 'product';
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    const normalizedPricingOptions = pricingOptions.map((pricingOption) => ({
      ...pricingOption,
      cancellationRefundRules: normalizeCancellationRefundRules(pricingOption.cancellationPolicyType, pricingOption.cancellationRefundRules),
    }));

    commitUpdateProduct({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: product.id,
          fieldsToUpdate: validFieldsToUpdate,
          listingMetadata: {
            about: '',
            title: title ?? '',
            subTitle: subTitle ?? '',
            includedFeatures: (includedFeatures ?? '')
              .split('\n')
              .map((feature) => feature.trim())
              .filter((feature) => feature !== ''),
          },
          type: type as ProductType,
          currency: currency as Currency,
          tagIds: productTagIds.concat(amenityIds),
          featureImages: finalFeatureImages,
          pricingOptions: normalizedPricingOptions.map((pricingOption, index) => ({
            id: pricingOption.id,
            index,
            listingMetadata: {
              about: '',
              title: pricingOption.title ?? '',
              subTitle: pricingOption.subTitle ?? '',
              includedFeatures: [],
            },
            purchaseCadence: pricingOption.cadence as ProductPricingCadence,
            bookingCadence: pricingOption.cadence as ProductPricingCadence,
            price: Number(pricingOption.price),
            fulfillmentType: pricingOption.fulfillmentType as never,
            entitlementCreditQuantity:
              pricingOption.fulfillmentType === 'ENTITLEMENT' && pricingOption.entitlementCreditQuantity ? Number(pricingOption.entitlementCreditQuantity) : null,
            entitlementValidityDays:
              pricingOption.fulfillmentType === 'ENTITLEMENT' && pricingOption.entitlementValidityDays ? Number(pricingOption.entitlementValidityDays) : null,
            availableDays: pricingOption.availableDays,
            requiredDaysPerWeek: toRequiredDaysPerWeekInput(pricingOption.cadence, pricingOption.requiredDaysPerWeek),
            supportsSubscriptionAutoRenewal: isEventType(type) ? false : pricingOption.supportsSubscriptionAutoRenewal,
            numberOfResourcesToBook: isEventType(type) ? 1 : Number(pricingOption.numberOfResourcesToBook),
            minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
            minDurationDisplayUnit: pricingOption.minDurationDisplayUnit,
            maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
            maxDurationDisplayUnit: pricingOption.maxDurationDisplayUnit,
            cancellationPolicyType: pricingOption.cancellationPolicyType as never,
            cancellationRefundRules: pricingOption.cancellationRefundRules.map((item) => ({
              minutesBefore: Number(item.minutesBefore),
              displayUnit: item.displayUnit,
              refundPercentage: Number(item.refundPercentage),
            })),
            isTaxInclusive: pricingOption.isTaxInclusive,
            maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
            maxAllowedResourcesLockTimePaidViaCardDisplayUnit: pricingOption.maxAllowedResourcesLockTimePaidViaCardDisplayUnit,
            maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer),
            maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: pricingOption.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit,
            billingMode: pricingOption.billingMode as never,
            acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((type) => type as PaymentMethod),
          })),
        },
      } as never,
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't update ${productTitle}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't update ${productTitle}. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        updateProduct: {
          product: {
            id: product.id,
            inactive: false,
            listingMetadata: {
              title: title ?? '',
              subTitle: subTitle ?? '',
              includedFeatures: (includedFeatures ?? '')
                .split('\n')
                .map((feature) => feature.trim())
                .filter((feature) => feature !== ''),
            },
            type: {
              type: type as ProductType,
              name: '',
            },
            currency: {
              type: currency as Currency,
              name: '',
            },
            productTags: [],
            amenities: [],
            featureImages: finalFeatureImages,
            pricingOptions: normalizedPricingOptions.map((pricingOption, index) => ({
              index,
              listingMetadata: {
                title: pricingOption.title ?? '',
                subTitle: pricingOption.subTitle ?? '',
              },
              purchaseCadence: pricingOption.cadence as ProductPricingCadence,
              bookingCadence: pricingOption.cadence as ProductPricingCadence,
              price: Number(pricingOption.price),
              fulfillmentType: pricingOption.fulfillmentType as never,
              entitlementCreditQuantity:
                pricingOption.fulfillmentType === 'ENTITLEMENT' && pricingOption.entitlementCreditQuantity ? Number(pricingOption.entitlementCreditQuantity) : null,
              entitlementValidityDays:
                pricingOption.fulfillmentType === 'ENTITLEMENT' && pricingOption.entitlementValidityDays ? Number(pricingOption.entitlementValidityDays) : null,
              availableDays: pricingOption.availableDays,
              requiredDaysPerWeek: toRequiredDaysPerWeekInput(pricingOption.cadence, pricingOption.requiredDaysPerWeek),
              supportsSubscriptionAutoRenewal: isEventType(type) ? false : pricingOption.supportsSubscriptionAutoRenewal,
              numberOfResourcesToBook: isEventType(type) ? 1 : Number(pricingOption.numberOfResourcesToBook),
              minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
              minDurationDisplayUnit: pricingOption.minDurationDisplayUnit,
              maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
              maxDurationDisplayUnit: pricingOption.maxDurationDisplayUnit,
              cancellationPolicyType: pricingOption.cancellationPolicyType as never,
              cancellationRefundRules: pricingOption.cancellationRefundRules.map((item) => ({
                minutesBefore: Number(item.minutesBefore),
                displayUnit: item.displayUnit,
                refundPercentage: Number(item.refundPercentage),
              })),
              isTaxInclusive: pricingOption.isTaxInclusive,
              maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
              maxAllowedResourcesLockTimePaidViaCardDisplayUnit: pricingOption.maxAllowedResourcesLockTimePaidViaCardDisplayUnit,
              maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer),
              maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit: pricingOption.maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit,
              acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((type) => type as PaymentMethod),
            })),
          },
        },
      } as never,
    });
  };
  const debouncedProductDetailUpdate = useDebounceCallback(handleProductDetailUpdateClick, productAutosaveDebounceTimeout);

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setFeatureImages((prev) => [response, ...prev]);
    setPrimaryFeatureImage((prevPrimary) => prevPrimary ?? response);
  };

  const handleRemoveFeatureImage = (image: FileUploadResponse) => {
    setFeatureImages((prev) => {
      const next = prev.filter((item) => item.original?.url !== image.original?.url);

      if (primaryFeatureImage?.original?.url === image.original?.url) {
        setPrimaryFeatureImage(next[0] ?? null);
      }

      return next;
    });
  };

  const handleSetPrimaryFeatureImage = (image: FileUploadResponse) => {
    setPrimaryFeatureImage(image);
    setFeatureImages((prev) => [image, ...prev.filter((item) => item.original?.url !== image.original?.url)]);
  };

  if (!rootData.product) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <Form
          onSubmit={() => undefined}
          initialValues={initialProductValues ?? undefined}
          initialValuesEqual={() => true}
          validate={validateProductDetails}
          render={({ handleSubmit, values, form, errors }) => {
            debounceSetTitle(values?.title ?? null);
            debounceSetSubTitle(values?.subTitle ?? null);
            debounceSetIncludedFeatures(values?.includedFeatures ?? null);
            debounceSetType(values?.type ?? null);
            debounceSetCurrency(values?.currency ?? null);
            debounceSetProductTagIds(values?.productTagIds ?? []);
            debounceSetAmenityIds(values?.amenityIds ?? []);
            const productValues = values as ProductDetails;
            const changedFields = getChangedProductFields(previousProductValues.current, productValues, previousFeatureImages.current, featureImages);
            if (changedFields.length > 0) {
              previousProductValues.current = productValues;
              previousFeatureImages.current = featureImages;
              debouncedProductDetailUpdate(changedFields, productValues);
            }

            return (
              <ProductEditorForm
                mode="edit"
                onSubmit={handleSubmit}
                rootDataRelay={rootData}
                values={values as ProductDetails}
                errors={errors}
                form={form as unknown as { change: (name: string, nextValue: unknown) => void }}
                requiredFields={requiredFields}
                organizationCustomDomain={organizationCustomDomain}
                featureImages={featureImages}
                primaryFeatureImage={primaryFeatureImage}
                onUploadCompleted={handleFeatureImageUploadCompleted}
                onRemoveFeatureImage={handleRemoveFeatureImage}
                onSetPrimaryFeatureImage={handleSetPrimaryFeatureImage}
                paletteMode={paletteMode}
              />
            );
          }}
        />
      </Box>
    </Box>
  );
};

export default memo(EditProduct);
