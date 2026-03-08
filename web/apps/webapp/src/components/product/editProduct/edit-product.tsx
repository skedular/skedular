import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { MultipleChoicesBookingPaymentMethodTypes } from '@/components/booking';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { DeleteIcon } from '@/components/icons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesProductTags, SingleChoiceCurrency, SingleChoiceProductPricingCadence } from '@/components/organization';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { editProduct_query$key } from '@/queries/__generated__/editProduct_query.graphql';
import type { Currency, editProduct_updateProductMutation, PaymentMethod, ProductPricingCadence } from '@/queries/__generated__/editProduct_updateProductMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import { makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { array, boolean, object, string } from 'yup';

type Props = {
  rootDataRelay: editProduct_query$key;
  onReloadRequired?: () => void;
  organizationUniqueAlphanumericName: string;
};

type ProductDetails = {
  name: string;
  description: string | null;
  currency: string;
  productTagIds: string[];
  pricingOptions: PricingOptionForm[];
};

type PricingOptionForm = {
  id: string;
  cadence: string;
  name: string;
  description: string;
  price: string;
  numberOfResourcesToBook: string;
  minDurationMinutes: string;
  maxDurationMinutes: string;
  isTaxInclusive: boolean;
  maxAllowedResourcesLockTimePaidViaCard: string;
  maxAllowedResourcesLockTimePaidViaBankTransfer: string;
  acceptedPaymentMethods: string[];
};

const createPricingOption = (defaultMaxAllowedResourcesLockTimePaidViaCard: number, defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number): PricingOptionForm => ({
  id: uuid(),
  cadence: 'ONE_TIME_V1',
  name: '',
  description: '',
  price: '',
  numberOfResourcesToBook: '1',
  minDurationMinutes: '',
  maxDurationMinutes: '',
  isTaxInclusive: true,
  maxAllowedResourcesLockTimePaidViaCard: defaultMaxAllowedResourcesLockTimePaidViaCard.toString(),
  maxAllowedResourcesLockTimePaidViaBankTransfer: (defaultMaxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString(),
  acceptedPaymentMethods: [],
});

const productSchema = (bookingSlotSizeInMinutes: number) =>
  object({
    name: string().min(3, 'Product name must be at least three characters long.').required('Product name is required'),
    description: string().nullable(),
    currency: string().required('Currency is required.'),
    mustBookAllLocationResources: boolean(),
    productTagIds: array().min(1, 'At least one product tag must be selected.').required('Product tags are required.'),
    pricingOptions: array()
      .of(
        object({
          cadence: string().required('Pricing cadence is required.'),
          name: string().required('Pricing option name is required.'),
          description: string().required('Pricing option description is required.'),
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
            .test('is-multiple-of-bookingSlotSizeInMinutes', `Minimum duration in minutes must be in ${bookingSlotSizeInMinutes}-minutes increments.`, function (value) {
              const minDurationMinutes = Number(value);
              if (isNaN(minDurationMinutes)) {
                return true;
              }

              return minDurationMinutes % bookingSlotSizeInMinutes === 0;
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
            .test('is-multiple-of-bookingSlotSizeInMinutes', `Maximum duration in minutes must be in ${bookingSlotSizeInMinutes}-minutes increments.`, function (value) {
              const maxDurationMinutes = Number(value);
              if (isNaN(maxDurationMinutes)) {
                return true;
              }

              return maxDurationMinutes % bookingSlotSizeInMinutes === 0;
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
          acceptedPaymentMethods: array().min(1, 'At least one accepted booking payment method must be selected.').required('Booking payment methods are required.'),
        }),
      )
      .min(1, 'At least one pricing option is required.')
      .test('is-unique-cadence-and-numberOfResourcesToBook', 'Cadence and number of resources to book combination must be unique for each pricing option.', (value) => {
        if (!value || value.length === 0) {
          return true;
        }

        const seenCombinations = new Set<string>();
        for (const pricingOption of value as PricingOptionForm[]) {
          const combination = `${pricingOption.cadence}|${pricingOption.numberOfResourcesToBook}`;
          if (seenCombinations.has(combination)) {
            return false;
          }

          seenCombinations.add(combination);
        }

        return true;
      })
      .required('Pricing options are required.'),
  });

const EditProduct = ({ rootDataRelay, organizationUniqueAlphanumericName }: Props) => {
  const rootData = useFragment<editProduct_query$key>(
    graphql`
      fragment editProduct_query on Query {
        product(id: $productId) {
          id
          inactive
          name
          description
          currency {
            type
            name
          }
          productTags {
            id
            name
            color
          }
          organization {
            id
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
            name
            description
            cadence
            price
            numberOfResourcesToBook
            minDurationMinutes
            maxDurationMinutes
            isTaxInclusive
            maxAllowedResourcesLockTimePaidViaCard
            maxAllowedResourcesLockTimePaidViaBankTransfer
            acceptedPaymentMethods
          }
        }
        productPricingCadences {
          type
          name
        }
        currencies {
          type
          name
        }
        bookingSlotSizeInMinutes
        defaultMaxAllowedResourcesLockTimePaidViaCard
        defaultMaxAllowedResourcesLockTimePaidViaBankTransfer
        ...multipleChoicesProductTags_query
        ...singleChoiceCurrency_query
        ...multipleChoicesBookingPaymentMethodTypes_query
        ...singleChoiceProductPricingCadence_query
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
          name
          description
          currency {
            type
            name
          }
          productTags {
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
            name
            description
            cadence
            price
            numberOfResourcesToBook
            minDurationMinutes
            maxDurationMinutes
            isTaxInclusive
            maxAllowedResourcesLockTimePaidViaCard
            maxAllowedResourcesLockTimePaidViaBankTransfer
            acceptedPaymentMethods
          }
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProductDetails = makeValidate(productSchema(rootData.bookingSlotSizeInMinutes));
  const requiredFields = makeRequired(productSchema(rootData.bookingSlotSizeInMinutes));

  const [name, setName] = useState(rootData.product ? rootData.product.name : '');
  const debounceSetName = useDebounceCallback(setName, keyboardTextFieldDebounceTimeout);

  const [description, setDescription] = useState<string | null>(rootData.product && rootData.product.description ? rootData.product.description : null);
  const debounceSetDescription = useDebounceCallback(setDescription, keyboardTextFieldDebounceTimeout);

  const [currency, setCurrency] = useState(rootData.product ? rootData.product.currency.type : '');
  const debounceSetCurrency = useDebounceCallback(setCurrency, keyboardTextFieldDebounceTimeout);

  const [productTagIds, setProductTagIds] = useState<string[]>(rootData.product ? rootData.product.productTags.map(({ id }) => id) : []);
  const debounceSetProductTagIds = useDebounceCallback(setProductTagIds, keyboardTextFieldDebounceTimeout);

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

  const handleProductDetailUpdateClick = ({ name, description, currency, productTagIds, pricingOptions }: ProductDetails) => {
    const product = rootData.product;
    if (!product) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating product '${product.name}'...`} />, infoNotificationOptions);
    const finalFeatureImages = featureImages.map((image) => ({
      original: image.original ? { url: image.original.url, height: image.original.height, width: image.original.width } : null,
      thumbnail: image.thumbnail ? { url: image.thumbnail.url, height: image.thumbnail.height, width: image.thumbnail.width } : null,
    }));

    commitUpdateProduct({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: product.id,
          name,
          description,
          currency: currency as Currency,
          productTagIds,
          featureImages: finalFeatureImages,
          pricingOptions: pricingOptions.map((pricingOption, index) => ({
            id: pricingOption.id,
            index,
            name: pricingOption.name.trim(),
            description: pricingOption.description.trim(),
            cadence: pricingOption.cadence as ProductPricingCadence,
            price: Number(pricingOption.price),
            numberOfResourcesToBook: Number(pricingOption.numberOfResourcesToBook),
            minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
            maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
            isTaxInclusive: pricingOption.isTaxInclusive,
            maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
            maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer) * 60 * 24,
            acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((type) => type as PaymentMethod),
          })),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update product '${product.name}'. Error: ${joinErrors(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product ${name} updated.`} />,
        });

        router.back();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update product '${product.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateProduct: {
          product: {
            id: product.id,
            inactive: false,
            name,
            description,
            currency: {
              type: currency as Currency,
              name: '',
            },
            productTags: [],
            featureImages: finalFeatureImages,
            pricingOptions: pricingOptions.map((pricingOption, index) => ({
              index,
              name: pricingOption.name.trim(),
              description: pricingOption.description.trim(),
              cadence: pricingOption.cadence as ProductPricingCadence,
              price: Number(pricingOption.price),
              numberOfResourcesToBook: Number(pricingOption.numberOfResourcesToBook),
              minDurationMinutes: pricingOption.minDurationMinutes ? Number(pricingOption.minDurationMinutes) : null,
              maxDurationMinutes: pricingOption.maxDurationMinutes ? Number(pricingOption.maxDurationMinutes) : null,
              isTaxInclusive: pricingOption.isTaxInclusive,
              maxAllowedResourcesLockTimePaidViaCard: Number(pricingOption.maxAllowedResourcesLockTimePaidViaCard),
              maxAllowedResourcesLockTimePaidViaBankTransfer: Number(pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer) * 60 * 24,
              acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((type) => type as PaymentMethod),
            })),
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.back();
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

  if (!rootData.product) {
    return null;
  }

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Edit Product Information">
          <Form
            onSubmit={handleProductDetailUpdateClick}
            initialValues={{
              name,
              description,
              currency,
              productTagIds,
              pricingOptions:
                rootData.product.pricingOptions.length > 0
                  ? rootData.product.pricingOptions.map((pricingOption) => ({
                      id: uuid(),
                      cadence: pricingOption.cadence,
                      name: pricingOption.name ?? '',
                      description: pricingOption.description ?? '',
                      price: pricingOption.price.toString(),
                      numberOfResourcesToBook: pricingOption.numberOfResourcesToBook.toString(),
                      minDurationMinutes: pricingOption.minDurationMinutes ? pricingOption.minDurationMinutes.toString() : '',
                      maxDurationMinutes: pricingOption.maxDurationMinutes ? pricingOption.maxDurationMinutes.toString() : '',
                      isTaxInclusive: pricingOption.isTaxInclusive,
                      maxAllowedResourcesLockTimePaidViaCard: pricingOption.maxAllowedResourcesLockTimePaidViaCard.toString(),
                      maxAllowedResourcesLockTimePaidViaBankTransfer: (pricingOption.maxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString(),
                      acceptedPaymentMethods: pricingOption.acceptedPaymentMethods.map((item) => item),
                    }))
                  : [createPricingOption(rootData.defaultMaxAllowedResourcesLockTimePaidViaCard, rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer)],
            }}
            validate={validateProductDetails}
            render={({ handleSubmit, values, form, errors }) => {
              debounceSetName(values!.name);
              debounceSetDescription(values!.description);
              debounceSetCurrency(values!.currency);
              debounceSetProductTagIds(values!.productTagIds);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Edit Product" />
                    <BodyIconTypography label="Edit your product details" />
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

                        <ImageFileUploaderWithCropper defaultAspectRatio={1} onUploadCompleted={handleFeatureImageUploadCompleted} />
                      </StackColumn>
                    </FormFieldLabel>

                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Description">
                      <TextField name="description" required={requiredFields.description} multiline rows={3} />
                    </FormFieldLabel>

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

                            <FormFieldLabel label="Cadence">
                              <SingleChoiceProductPricingCadence rootDataRelay={rootData} name={`pricingOptions[${index}].cadence`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Name">
                              <TextField name={`pricingOptions[${index}].name`} required />
                            </FormFieldLabel>

                            <FormFieldLabel label="Description">
                              <TextField name={`pricingOptions[${index}].description`} required multiline rows={3} />
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
                              <MultipleChoicesBookingPaymentMethodTypes rootDataRelay={rootData} name={`pricingOptions[${index}].acceptedPaymentMethods`} required />
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
                        Update
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

export default memo(EditProduct);
