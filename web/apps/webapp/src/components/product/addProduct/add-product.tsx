import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon } from '@/components/icons';
import { ListingMetadata, listingMetadataSchemaShape } from '@/components/listingMetadata';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import {
  MultipleChoicesPaymentMethodTypes,
  MultipleChoicesProductTags,
  SingleChoiceCurrency,
  SingleChoiceProductPricingBillingMode,
  SingleChoiceProductPricingCadence,
} from '@/components/organization';
import MultipleChoicesAmenities from '@/components/organization/multiple-choices-amenities';
import { RelayError, toRootError } from '@/components/relayError';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext, useKnownParams } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { addProduct_addProductMutation, Currency, PaymentMethod, ProductPricingCadence } from '@/queries/__generated__/addProduct_addProductMutation.graphql';
import type { addProduct_rootQuery } from '@/queries/__generated__/addProduct_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import { makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addProduct_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  onAdded: (productId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addProduct_rootQuery($organizationUniqueAlphanumericName: String!, $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]) {
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
    ...multipleChoicesProductPricingBillingModes_query
    ...multipleChoicesProductTags_query
    ...singleChoiceCurrency_query
    ...multipleChoicesPaymentMethodTypes_query
    ...singleChoiceProductPricingCadence_query
    ...multipleChoicesAmenities_query
  }
`;

type ProductDetails = {
  title: string | null;
  subTitle: string | null;
  includedFeatures: string | null;
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
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaCard: string;
  maxAllowedResourcesLockTimePaidViaBankTransfer: string;
  billingMode: string;
  acceptedPaymentMethods: string[];
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
  isTaxInclusive: true,
  maxAllowedResourcesLockTimePaidViaCard: defaultMaxAllowedResourcesLockTimePaidViaCard.toString(),
  maxAllowedResourcesLockTimePaidViaBankTransfer: (defaultMaxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString(),
  billingMode: 'NOT_SET',
  acceptedPaymentMethods: [],
});

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
          isTaxInclusive: boolean().required(),
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
      .required('Pricing options are required.'),
  });

const AddProduct = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName, onAdded, onCancel }: Props) => {
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
            cadence
            price
            numberOfResourcesToBook
            minDurationMinutes
            maxDurationMinutes
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

  const [title, setTitle] = useState<string | null>(null);
  const debounceSetTitle = useDebounceCallback(setTitle, keyboardTextFieldDebounceTimeout);
  const [subTitle, setSubTitle] = useState<string | null>(null);
  const debounceSetSubTitle = useDebounceCallback(setSubTitle, keyboardTextFieldDebounceTimeout);
  const [includedFeatures, setIncludedFeatures] = useState<string | null>(null);
  const debounceSetIncludedFeatures = useDebounceCallback(setIncludedFeatures, keyboardTextFieldDebounceTimeout);

  const [currency, setCurrency] = useState('');
  const debounceSetCurrency = useDebounceCallback(setCurrency, keyboardTextFieldDebounceTimeout);

  const [productTagIds, setProductTagIds] = useState<string[]>([]);
  const debounceSetProductTagIds = useDebounceCallback(setProductTagIds, keyboardTextFieldDebounceTimeout);

  const [amenityIds, setAmenityIds] = useState<string[]>([]);
  const debounceSetAmenityIds = useDebounceCallback(setAmenityIds, keyboardTextFieldDebounceTimeout);

  const [featureImages, setFeatureImages] = useState<FileUploadResponse[]>([]);
  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(null);

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleProductAddClick = ({ title, subTitle, includedFeatures, currency, productTagIds, amenityIds, pricingOptions }: ProductDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding product '${title}'...`} />, infoNotificationOptions);
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
          currency: currency as Currency,
          tagIds: productTagIds.concat(amenityIds),
          organizationUniqueAlphanumericName,
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
            cadence: pricingOption.cadence as ProductPricingCadence,
            price: Number(pricingOption.price),
            numberOfResourcesToBook: Number(pricingOption.numberOfResourcesToBook),
            minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
            maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add new product '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${name} added.`} />,
        });

        onAdded(id);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add new product '${name}'. Error: ${error.message}.`} />,
        });
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
                about: '',
                title: pricingOption.title ?? '',
                subTitle: pricingOption.subTitle ?? '',
                includedFeatures: [],
              },
              cadence: pricingOption.cadence as ProductPricingCadence,
              price: Number(pricingOption.price),
              numberOfResourcesToBook: Number(pricingOption.numberOfResourcesToBook),
              minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
              maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
              isTaxInclusive: pricingOption.isTaxInclusive,
              maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
              maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer) * 60 * 24,
              billingMode: pricingOption.billingMode as never,
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
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Product">
          <Form
            onSubmit={handleProductAddClick}
            initialValues={{
              title,
              subTitle,
              includedFeatures,
              currency,
              productTagIds,
              amenityIds,
              pricingOptions: [createPricingOption(rootData.defaultMaxAllowedResourcesLockTimePaidViaCard, rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer)],
            }}
            validate={validateProductDetails}
            render={({ handleSubmit, values, form, errors }) => {
              debounceSetTitle(values!.title);
              debounceSetSubTitle(values!.subTitle);
              debounceSetIncludedFeatures(values!.includedFeatures);
              debounceSetCurrency(values!.currency);
              debounceSetProductTagIds(values!.productTagIds);
              debounceSetAmenityIds(values!.amenityIds);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Product Setup" />
                    <BodyIconTypography label="Edit your product name and details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Feature Images">
                      <StackColumn>
                        <Box
                          sx={{
                            display: 'grid',
                            gridTemplateColumns: { xs: 'repeat(auto-fill, minmax(140px, 1fr))', sm: 'repeat(auto-fill, minmax(180px, 1fr))' },
                            gap: 2,
                          }}
                        >
                          {featureImages.map((image, index) => (
                            <Box
                              key={index}
                              sx={{
                                position: 'relative',
                                borderRadius: 2,
                                overflow: 'hidden',
                                border: 1,
                                borderColor: 'divider',
                                backgroundColor: paletteMode === 'dark' ? 'grey.900' : 'grey.50',
                              }}
                            >
                              {/* eslint-disable-next-line @next/next/no-img-element */}
                              <img src={image.original?.url ?? image.thumbnail?.url ?? ''} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                              <StackRow sx={{ position: 'absolute', top: 8, right: 8 }}>
                                <IconButton size="small" aria-label="Remove feature image" onClick={() => handleRemoveFeatureImage(image)}>
                                  <DeleteIcon fontSize="small" />
                                </IconButton>
                              </StackRow>
                              <StackRow sx={{ position: 'absolute', left: 8, bottom: 8 }}>
                                {primaryFeatureImage?.original?.url === image.original?.url ? (
                                  <Chip size="small" color="success" label="Cover image" />
                                ) : (
                                  <Button variant="contained" size="small" onClick={() => handleSetPrimaryFeatureImage(image)} sx={{ textTransform: 'none' }}>
                                    Make cover
                                  </Button>
                                )}
                              </StackRow>
                            </Box>
                          ))}
                        </Box>
                        <ImageFileUploaderWithCropper onUploadCompleted={handleFeatureImageUploadCompleted} />
                      </StackColumn>
                    </FormFieldLabel>

                    <ListingMetadata
                      fields={['title', 'subTitle', 'includedFeatures']}
                      onChange={({ includedFeatures, subTitle, title }) => {
                        debounceSetTitle(title);
                        debounceSetSubTitle(subTitle);
                        debounceSetIncludedFeatures(includedFeatures);
                      }}
                      requiredFields={requiredFields}
                    />

                    <FormFieldLabel label="Currency">
                      <SingleChoiceCurrency rootDataRelay={rootData} name="currency" required={requiredFields.currency} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Product Tags">
                      <MultipleChoicesProductTags
                        rootDataRelay={rootData}
                        name="productTagIds"
                        required={requiredFields.productTagIds}
                        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Amenities">
                      <MultipleChoicesAmenities rootDataRelay={rootData} name="amenityIds" required={requiredFields.amenityIds} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Pricing Options" />
                    <StackColumn>
                      {(values?.pricingOptions ?? []).map((pricingOption: PricingOptionForm, index: number) => (
                        <Box
                          key={pricingOption.id}
                          sx={{
                            border: 1,
                            borderColor: 'divider',
                            borderRadius: 2,
                            paddingLeft: 2,
                            paddingBottom: 2,
                          }}
                        >
                          <StackColumn>
                            <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                              {(values?.pricingOptions ?? []).length > 1 ? (
                                <Button
                                  color="error"
                                  onClick={() => {
                                    const nextPricingOptions = (values?.pricingOptions ?? []).filter((_: PricingOptionForm, itemIndex: number) => itemIndex !== index);
                                    form.change('pricingOptions', nextPricingOptions);
                                  }}
                                >
                                  Remove
                                </Button>
                              ) : null}
                            </StackRow>

                            <ListingMetadata fields={['title', 'subTitle']} namePrefix={`pricingOptions[${index}]`} requiredFields={requiredFields} />

                            <FormFieldLabel label="Cadence">
                              <SingleChoiceProductPricingCadence rootDataRelay={rootData} name={`pricingOptions[${index}].cadence`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Price">
                              <TextField name={`pricingOptions[${index}].price`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Number of Resources to Book">
                              <TextField name={`pricingOptions[${index}].numberOfResourcesToBook`} required />
                            </FormFieldLabel>

                            <FormFieldLabel>
                              <Switches name={`pricingOptions[${index}].isTaxInclusive`} data={{ label: 'Is price tax inclusive?', value: 'isTaxInclusive' }} />
                            </FormFieldLabel>

                            <FormFieldLabel label="Minimum Duration (minutes)">
                              <TextField name={`pricingOptions[${index}].minDurationMinutes`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Maximum Duration (minutes)">
                              <TextField name={`pricingOptions[${index}].maxDurationMinutes`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Maximum Permitted Resource Lock Duration Paid via Card (minutes)">
                              <TextField name={`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaCard`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Maximum Permitted Resource Lock Duration Paid via Bank Transfer (days)">
                              <TextField name={`pricingOptions[${index}].maxAllowedResourcesLockTimePaidViaBankTransfer`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Accepted Payment Methods">
                              <MultipleChoicesPaymentMethodTypes rootDataRelay={rootData} name={`pricingOptions[${index}].acceptedPaymentMethods`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Billing Mode">
                              <SingleChoiceProductPricingBillingMode rootDataRelay={rootData as never} name={`pricingOptions[${index}].billingMode`} required />
                            </FormFieldLabel>
                          </StackColumn>
                        </Box>
                      ))}

                      {typeof errors?.pricingOptions === 'string' ? <BodyIconTypography label={errors.pricingOptions} sx={{ color: 'error.main' }} /> : null}

                      <StackRow>
                        <Button
                          variant="outlined"
                          onClick={() => {
                            const nextPricingOptions = [
                              ...(values?.pricingOptions ?? []),
                              createPricingOption(rootData.defaultMaxAllowedResourcesLockTimePaidViaCard, rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer),
                            ];
                            form.change('pricingOptions', nextPricingOptions);
                          }}
                        >
                          Add Pricing Option
                        </Button>
                      </StackRow>
                    </StackColumn>
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <StackRow>
                      <Button variant="contained" type="submit" sx={defaultButtonStyle}>
                        <BodyIconTypography label="Add" invertDefaultColor={paletteMode === 'dark'} />
                      </Button>
                    </StackRow>
                  </StackColumn>
                </FormStackColumn>
              );
            }}
          />
        </AppBarWithStackColumn>
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
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
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
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

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
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
        onAdded={onAdded}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(AddProductWithRelay);
