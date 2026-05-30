import { PaletteModeContext, RelayError, getRelayErrorMessage, toRootError } from '@skedular/shared';
import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/core/fetch';
import { listingMetadataSchemaShape } from '@/components/listingMetadata';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import ProductEditorForm from '@/components/product/product-editor-form';

import useKnownParams from '@/hooks/use-known-params';
import type { addProduct_addProductMutation, Currency, PaymentMethod, ProductPricingCadence, ProductType } from '@/queries/__generated__/addProduct_addProductMutation.graphql';
import type { addProduct_rootQuery } from '@/queries/__generated__/addProduct_rootQuery.graphql';
import Box from '@mui/material/Box';

import { makeRequired, makeValidate } from 'mui-rff';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { array, boolean, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addProduct_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  onAdded: (productId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addProduct_rootQuery($organizationCustomDomain: String!, $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]) {
    bookingSlotSizeInMinutes
    defaultMaxAllowedResourcesLockTimePaidViaCard
    defaultMaxAllowedResourcesLockTimePaidViaBankTransfer
    currencies {
      type
      name
    }
    paymentMethods {
      type
      name
    }
    ...singleChoiceProductPricingBillingMode_query
    ...multipleChoicesProductTags_query
    ...singleChoiceCurrency_query
    ...multipleChoicesPaymentMethodTypes_query
    ...singleChoiceProductPricingCadence_query
    ...singleChoiceProductPricingCancellationType_query
    ...multipleChoicesAmenities_query
    ...singleChoiceProductType_query
  }
`;

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
};

type CancellationRefundRuleForm = {
  minutesBefore: string;
  refundPercentage: string;
};

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
  maxAllowedResourcesLockTimePaidViaBankTransfer: (defaultMaxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString(),
  billingMode: 'NOT_SET',
  acceptedPaymentMethods: [],
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
            const combination = `${pricingOption.cadence}|${pricingOption.numberOfResourcesToBook}|${pricingOption.billingMode}`;
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

const AddProduct = (props: Props) => {
  const { queryReference, onReloadRequired, organizationCustomDomain, onAdded } = props;
  const rootData = usePreloadedQuery<addProduct_rootQuery>(RootQuery, queryReference);
  const [commitAddProduct] = useMutation<addProduct_addProductMutation>(graphql`
    mutation addProduct_addProductMutation($input: AddProductInput!) @raw_response_type {
      addProduct(input: $input) {
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
            numberOfResourcesToBook
            minDurationMinutes
            maxDurationMinutes
            cancellationPolicyType
            cancellationRefundRules {
              minutesBefore
              refundPercentage
            }
            isTaxInclusive
            maxAllowedResourcesLockTimePaidViaCard
            maxAllowedResourcesLockTimePaidViaBankTransfer
            billingMode
            acceptedPaymentMethods
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProductDetails = makeValidate(productSchema(rootData.bookingSlotSizeInMinutes));
  const requiredFields = makeRequired(productSchema(rootData.bookingSlotSizeInMinutes));

  const [initialProductValues] = useState(() => ({
    title: null as string | null,
    subTitle: null as string | null,
    includedFeatures: null as string | null,
    type: '',
    currency: '',
    productTagIds: [] as string[],
    amenityIds: [] as string[],
    pricingOptions: [createPricingOption(rootData.defaultMaxAllowedResourcesLockTimePaidViaCard, rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer)],
  }));

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>([]);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(null);

  const handleProductAddClick = ({ title, subTitle, includedFeatures, type, currency, productTagIds, amenityIds, pricingOptions }: ProductDetails) => {
    const id = uuid();
    const productTitle = title?.trim() || 'product';
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitAddProduct({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
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
          organizationCustomDomain,
          featureImages: finalFeatureImages,
          pricingOptions: pricingOptions.map((pricingOption, index) => ({
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
            supportsSubscriptionAutoRenewal: isEventType(type) ? false : pricingOption.supportsSubscriptionAutoRenewal,
            numberOfResourcesToBook: isEventType(type) ? 1 : Number(pricingOption.numberOfResourcesToBook),
            minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
            maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
            cancellationPolicyType: pricingOption.cancellationPolicyType as never,
            cancellationRefundRules: pricingOption.cancellationRefundRules.map((item) => ({
              minutesBefore: Number(item.minutesBefore),
              refundPercentage: Number(item.refundPercentage),
            })),
            isTaxInclusive: pricingOption.isTaxInclusive,
            maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
            maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer) * 60 * 24,
            billingMode: pricingOption.billingMode as never,
            acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((type) => type as PaymentMethod),
          })),
        },
      } as never,
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`We couldn't add ${productTitle}. ${getRelayErrorMessage(errors)}`} />, errorNotificationOptions);

          return;
        }

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`We couldn't add ${productTitle}. ${error.message}`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addProduct: {
          product: {
            id,
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
            pricingOptions: pricingOptions.map((pricingOption, index) => ({
              index,
              listingMetadata: {
                title: pricingOption.title ?? '',
                subTitle: pricingOption.subTitle ?? '',
              },
              purchaseCadence: pricingOption.cadence as ProductPricingCadence,
              bookingCadence: pricingOption.cadence as ProductPricingCadence,
              price: Number(pricingOption.price),
              supportsSubscriptionAutoRenewal: isEventType(type) ? false : pricingOption.supportsSubscriptionAutoRenewal,
              numberOfResourcesToBook: isEventType(type) ? 1 : Number(pricingOption.numberOfResourcesToBook),
              minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
              maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
              cancellationPolicyType: pricingOption.cancellationPolicyType as never,
              cancellationRefundRules: pricingOption.cancellationRefundRules.map((item) => ({
                minutesBefore: Number(item.minutesBefore),
                refundPercentage: Number(item.refundPercentage),
              })),
              isTaxInclusive: pricingOption.isTaxInclusive,
              maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
              maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer) * 60 * 24,
              acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((type) => type as PaymentMethod),
            })),
          },
        },
      } as never,
    });
  };

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

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <Form
          onSubmit={handleProductAddClick}
          initialValues={initialProductValues}
          validate={validateProductDetails}
          render={({ handleSubmit, values, form, errors }) => {
            return (
              <ProductEditorForm
                mode="add"
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

const MemoAddProduct = memo(AddProduct);

type RelayProps = {
  onReloadRequired: () => void;
  onAdded: (id: string) => void;
  onCancel: () => void;
};

const AddProductWithRelay = ({ onReloadRequired, onAdded, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<addProduct_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        multipleChoicesProductTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoAddProduct
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationCustomDomain={organizationCustomDomain}
        onAdded={onAdded}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(AddProductWithRelay);
