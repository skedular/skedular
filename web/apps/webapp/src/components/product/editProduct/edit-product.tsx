import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { MultipleChoicesBookingPaymentMethodTypes } from '@/components/booking';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags, MultipleChoicesProductTags, SingleChoiceCurrency, SingleChoicePriceUnit } from '@/components/organization';
import { productFeatureImageHeight, productFeatureImageWidth } from '@/components/product';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { editProduct_query$key } from '@/queries/__generated__/editProduct_query.graphql';
import type { Currency, editProduct_updateProductMutation, PaymentMethod, PriceUnit } from '@/queries/__generated__/editProduct_updateProductMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import Image from 'next/image';
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
  price: string;
  priceUnit: string;
  currency: string;
  numberOfResourcesToBook: string;
  minDurationMinutes: string | null;
  maxDurationMinutes: string | null;
  bookAllLocationResources: boolean;
  recurrenceWindowDays: string;
  requireConsecutiveDays: boolean;
  maxBookingSpreadDays: string | null;
  productTagIds: string[];
  locationTagIds: string[];
  maxAllowedResourcesLockTimePaidViaCard: string;
  maxAllowedResourcesLockTimePaidViaBankTransfer: string;
  acceptedBookingPaymentMethods: string[];
  isPriceTaxInclusive: boolean;
};

const productSchema = (openingHoursMinutesStep: number) =>
  object({
    name: string().min(3, 'Product name must be at least three characters long.').required('Product name is required'),
    description: string().nullable(),
    price: string()
      .matches(/^\d+(\.\d{1,2})?$/, 'Price must be a valid decimal number.')
      .required('Price is required.')
      .test('is-greater-than-zero', 'Price must be greater than zero.', function (value) {
        const price = Number(value);
        if (isNaN(price)) {
          return true;
        }

        return price > 0;
      }),
    priceUnit: string().required('Price Unit is required.'),
    currency: string().required('Currency is required.'),
    numberOfResourcesToBook: string()
      .required('Number of resources to book is required.')
      .test('is-number', 'Number of resources to book must be a valid number.', (value) => value !== undefined && value.trim() !== '' && !isNaN(Number(value)))
      .test('min', 'Number of resources to book must be greater than 0.', (value) => Number(value) > 0),
    minDurationMinutes: string()
      .nullable()
      .test('is-multiple-of-openingHoursMinutesStep', `Minimum duration in minutes must be in ${openingHoursMinutesStep}-minutes increments.`, function (value) {
        const minDurationMinutes = Number(value);
        if (isNaN(minDurationMinutes)) {
          return true;
        }

        return minDurationMinutes % openingHoursMinutesStep === 0;
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
      })
      .test('is-following-hour-price-unit-rules', 'Minimum duration in minutes must be in hour increments when price unit is hourly.', function (value) {
        const { priceUnit } = this.parent;
        if (!priceUnit) {
          return true;
        }

        if (priceUnit !== 'PerHour') {
          return true;
        }

        const minDurationMinutes = Number(value);
        if (isNaN(minDurationMinutes)) {
          return true;
        }

        return minDurationMinutes % 60 === 0;
      }),
    maxDurationMinutes: string()
      .nullable()
      .test('is-multiple-of-openingHoursMinutesStep', `Maximum duration in minutes must be in ${openingHoursMinutesStep}-minutes increments.`, function (value) {
        const maxDurationMinutes = Number(value);
        if (isNaN(maxDurationMinutes)) {
          return true;
        }

        return maxDurationMinutes % openingHoursMinutesStep === 0;
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
      })
      .test('is-following-hour-price-unit-rules', 'Maximum duration in minutes must be in hour increments when price unit is hourly.', function (value) {
        const { priceUnit } = this.parent;
        if (!priceUnit) {
          return true;
        }

        if (priceUnit !== 'PerHour') {
          return true;
        }

        const maxDurationMinutes = Number(value);
        if (isNaN(maxDurationMinutes)) {
          return true;
        }

        return maxDurationMinutes % 60 === 0;
      }),
    mustBookAllLocationResources: boolean(),
    recurrenceWindowDays: string()
      .test('is-required', 'Recurrence window days is required.', function (value) {
        const { bookAllLocationResources } = this.parent;
        if (bookAllLocationResources) {
          return true;
        }

        return !!value;
      })
      .test('is-greater-than-zero', 'Recurrence window days must be greater than 0.', function (value) {
        const { bookAllLocationResources } = this.parent;
        if (bookAllLocationResources) {
          return true;
        }

        return Number(value) > 0;
      }),
    requireConsecutiveDays: boolean(),
    maxBookingSpreadDays: string()
      .nullable()
      .test('is-greater-than-recurrence', 'Max booking spread days must be greater than or equal to recurrence window days.', function (value) {
        const { bookAllLocationResources, requireConsecutiveDays, recurrenceWindowDays: recurrenceWindowDaysStr } = this.parent;

        if (bookAllLocationResources || requireConsecutiveDays) {
          return true;
        }

        const maxBookingSpreadDays = Number(value);
        if (isNaN(maxBookingSpreadDays)) {
          return true;
        }

        const recurrenceWindowDays = Number(recurrenceWindowDaysStr);
        if (isNaN(recurrenceWindowDaysStr)) {
          return true;
        }

        return maxBookingSpreadDays >= recurrenceWindowDays;
      })
      .test('is-greater-than-zero', 'Max booking spread days must be greater than 0.', function (value) {
        const { bookAllLocationResources, requireConsecutiveDays } = this.parent;
        if (bookAllLocationResources || requireConsecutiveDays) {
          return true;
        }

        const maxBookingSpreadDays = Number(value);
        if (isNaN(maxBookingSpreadDays)) {
          return true;
        }

        return maxBookingSpreadDays > 0;
      }),
    productTagIds: array().min(1, 'At least one product tag must be selected.').required('Product tags are required.'),
    locationTagIds: array().nullable(),
    maxAllowedResourcesLockTimePaidViaCard: string()
      .test('is-number', 'Max allowed resources lock time must be a valid number.', function (value) {
        const maxAllowedResourcesLockTimePaidViaCard = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaCard)) {
          return false;
        }

        return true;
      })
      .test('is-greater-than-zero', 'Max allowed resources lock time must be greater than 0.', function (value) {
        const maxAllowedResourcesLockTimePaidViaCard = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaCard)) {
          return false;
        }

        return maxAllowedResourcesLockTimePaidViaCard > 0;
      }),
    maxAllowedResourcesLockTimePaidViaBankTransfer: string()
      .test('is-number', 'Max allowed resources lock time must be a valid number.', function (value) {
        const maxAllowedResourcesLockTimePaidViaBankTransfer = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaBankTransfer)) {
          return false;
        }

        return true;
      })
      .test('is-greater-than-zero', 'Max allowed resources lock time must be greater than 0.', function (value) {
        const maxAllowedResourcesLockTimePaidViaBankTransfer = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaBankTransfer)) {
          return false;
        }

        return maxAllowedResourcesLockTimePaidViaBankTransfer > 0;
      }),
    acceptedBookingPaymentMethods: array().min(1, 'At least one accepted booking payment method must be selected".').required('Booking payment methods are required.'),
    isPriceTaxInclusive: boolean().required(),
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
          price
          priceUnit {
            type
            name
          }
          currency {
            type
            name
          }
          numberOfResourcesToBook
          minDurationMinutes
          maxDurationMinutes
          bookAllLocationResources
          recurrenceWindowDays
          requireConsecutiveDays
          maxBookingSpreadDays
          productTags {
            id
            name
            color
          }
          locationTags {
            id
            name
            color
          }
          organization {
            id
          }
          maxAllowedResourcesLockTimePaidViaCard
          maxAllowedResourcesLockTimePaidViaBankTransfer
          acceptedBookingPaymentMethods {
            type
          }
          primaryFeatureImage {
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
          isPriceTaxInclusive
        }
        openingHoursMinutesStep
        defaultMaxAllowedResourcesLockTimePaidViaCard
        defaultMaxAllowedResourcesLockTimePaidViaBankTransfer
        ...multipleChoicesProductTags_query
        ...multipleChoicesLocationTags_query
        ...singleChoicePriceUnit_query
        ...singleChoiceCurrency_query
        ...multipleChoicesBookingPaymentMethodTypes_query
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
          price
          priceUnit {
            type
            name
          }
          currency {
            type
            name
          }
          numberOfResourcesToBook
          minDurationMinutes
          maxDurationMinutes
          bookAllLocationResources
          recurrenceWindowDays
          requireConsecutiveDays
          maxBookingSpreadDays
          productTags {
            id
            name
            color
          }
          locationTags {
            id
            name
            color
          }
          acceptedBookingPaymentMethods {
            type
          }
          maxAllowedResourcesLockTimePaidViaCard
          maxAllowedResourcesLockTimePaidViaBankTransfer
          primaryFeatureImage {
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
          isPriceTaxInclusive
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProductDetails = makeValidate(productSchema(rootData.openingHoursMinutesStep));
  const requiredFields = makeRequired(productSchema(rootData.openingHoursMinutesStep));

  const [name, setName] = useState(rootData.product ? rootData.product.name : '');
  const debounceSetName = useDebounceCallback(setName, keyboardTextFieldDebounceTimeout);

  const [description, setDescription] = useState<string | null>(rootData.product && rootData.product.description ? rootData.product.description : null);
  const debounceSetDescription = useDebounceCallback(setDescription, keyboardTextFieldDebounceTimeout);

  const [price, setPrice] = useState(rootData.product ? rootData.product.price : '');
  const debounceSetPrice = useDebounceCallback(setPrice, keyboardTextFieldDebounceTimeout);

  const [priceUnit, setPriceUnit] = useState(rootData.product ? rootData.product.priceUnit.type : '');
  const debounceSetPriceUnit = useDebounceCallback(setPriceUnit, keyboardTextFieldDebounceTimeout);

  const [currency, setCurrency] = useState(rootData.product ? rootData.product.currency.type : '');
  const debounceSetCurrency = useDebounceCallback(setCurrency, keyboardTextFieldDebounceTimeout);

  const [numberOfResourcesToBook, setNumberOfResourcesToBook] = useState(rootData.product ? rootData.product.numberOfResourcesToBook.toString() : '1');
  const debounceSetNumberOfResourcesToBook = useDebounceCallback(setNumberOfResourcesToBook, keyboardTextFieldDebounceTimeout);

  const [minDurationMinutes, setMinDurationMinutes] = useState<string | null>(
    rootData.product && rootData.product.minDurationMinutes ? rootData.product.minDurationMinutes.toString() : null,
  );
  const debounceSetMinDurationMinutes = useDebounceCallback(setMinDurationMinutes, keyboardTextFieldDebounceTimeout);

  const [maxDurationMinutes, setMaxDurationMinutes] = useState<string | null>(
    rootData.product && rootData.product.maxDurationMinutes ? rootData.product.maxDurationMinutes.toString() : null,
  );
  const debounceSetMaxDurationMinutes = useDebounceCallback(setMaxDurationMinutes, keyboardTextFieldDebounceTimeout);

  const [bookAllLocationResources, setBookAllLocationResources] = useState(rootData.product ? rootData.product.bookAllLocationResources : false);
  const debounceSetBookAllLocationResources = useDebounceCallback(setBookAllLocationResources, keyboardTextFieldDebounceTimeout);

  const [recurrenceWindowDays, setRecurrenceWindowDays] = useState(rootData.product ? rootData.product.recurrenceWindowDays.toString() : '1');
  const debounceSetRecurrenceWindowDays = useDebounceCallback(setRecurrenceWindowDays, keyboardTextFieldDebounceTimeout);

  const [requireConsecutiveDays, setRequireConsecutiveDays] = useState(rootData.product ? rootData.product.requireConsecutiveDays : false);
  const debounceSetRequireConsecutiveDays = useDebounceCallback(setRequireConsecutiveDays, keyboardTextFieldDebounceTimeout);

  const [maxBookingSpreadDays, setMaxBookingSpreadDays] = useState<string | null>(
    rootData.product && rootData.product.maxBookingSpreadDays ? rootData.product.maxBookingSpreadDays.toString() : '1',
  );
  const debounceSetMaxBookingSpreadDays = useDebounceCallback(setMaxBookingSpreadDays, keyboardTextFieldDebounceTimeout);

  const [productTagIds, setProductTagIds] = useState<string[]>(rootData.product ? rootData.product.productTags.map(({ id }) => id) : []);
  const debounceSetProductTagIds = useDebounceCallback(setProductTagIds, keyboardTextFieldDebounceTimeout);

  const [locationTagIds, setLocationTagIds] = useState<string[]>(rootData.product ? rootData.product.locationTags.map(({ id }) => id) : []);
  const debounceSetLocationTagIds = useDebounceCallback(setLocationTagIds, keyboardTextFieldDebounceTimeout);

  const [maxAllowedResourcesLockTimePaidViaCard, setMaxAllowedResourcesLockTimePaidViaCard] = useState<string>(
    rootData.product ? rootData.product.maxAllowedResourcesLockTimePaidViaCard.toString() : rootData.defaultMaxAllowedResourcesLockTimePaidViaCard.toString(),
  );
  const debounceSetMaxAllowedResourcesLockTimePaidViaCard = useDebounceCallback(setMaxAllowedResourcesLockTimePaidViaCard, keyboardTextFieldDebounceTimeout);

  const [maxAllowedResourcesLockTimePaidViaBankTransfer, setMaxAllowedResourcesLockTimePaidViaBankTransfer] = useState<string>(
    (rootData.product
      ? (rootData.product.maxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString()
      : rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)
    ).toString(),
  );
  const debounceSetMaxAllowedResourcesLockTimePaidViaBankTransfer = useDebounceCallback(setMaxAllowedResourcesLockTimePaidViaBankTransfer, keyboardTextFieldDebounceTimeout);

  const [acceptedBookingPaymentMethods, setAcceptedBookingPaymentMethods] = useState<string[]>(
    rootData.product ? rootData.product.acceptedBookingPaymentMethods.map(({ type }) => type) : [],
  );
  const debounceSetAcceptedBookingPaymentMethods = useDebounceCallback(setAcceptedBookingPaymentMethods, keyboardTextFieldDebounceTimeout);

  const [isPriceTaxInclusive, setIsPriceTaxInclusive] = useState(rootData.product ? rootData.product.isPriceTaxInclusive : true);
  const debounceSetIsPriceTaxInclusive = useDebounceCallback(setIsPriceTaxInclusive, keyboardTextFieldDebounceTimeout);

  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse | null>(
    rootData.product?.primaryFeatureImage && rootData.product?.primaryFeatureImage.original
      ? {
          id: '',
          original: {
            url: rootData.product?.primaryFeatureImage.original.url,
            height: rootData.product?.primaryFeatureImage.original.height,
            width: rootData.product?.primaryFeatureImage.original.width,
          },
          thumbnail: rootData.product?.primaryFeatureImage.thumbnail
            ? {
                url: rootData.product?.primaryFeatureImage.thumbnail.url,
                height: rootData.product?.primaryFeatureImage.thumbnail.height,
                width: rootData.product?.primaryFeatureImage.thumbnail.width,
              }
            : null,
        }
      : null,
  );

  const handleProductDetailUpdateClick = ({
    name,
    description,
    price,
    priceUnit,
    currency,
    numberOfResourcesToBook: numberOfResourcesToBookStr,
    minDurationMinutes: minDurationMinutesStr,
    maxDurationMinutes: maxDurationMinutesStr,
    bookAllLocationResources,
    recurrenceWindowDays: recurrenceWindowDaysStr,
    requireConsecutiveDays,
    maxBookingSpreadDays: maxBookingSpreadDaysStr,
    productTagIds,
    locationTagIds,
    maxAllowedResourcesLockTimePaidViaCard: maxAllowedResourcesLockTimePaidViaCardStr,
    maxAllowedResourcesLockTimePaidViaBankTransfer: maxAllowedResourcesLockTimePaidViaBankTransferStr,
    acceptedBookingPaymentMethods,
    isPriceTaxInclusive,
  }: ProductDetails) => {
    const product = rootData.product;
    if (!product) {
      return;
    }

    const numberOfResourcesToBook = Number(numberOfResourcesToBookStr);
    const minDurationMinutes = minDurationMinutesStr ? Number(minDurationMinutesStr) : null;
    const maxDurationMinutes = maxDurationMinutesStr ? Number(maxDurationMinutesStr) : null;
    const recurrenceWindowDays = recurrenceWindowDaysStr ? Number(recurrenceWindowDaysStr) : 1;
    const maxBookingSpreadDays = maxBookingSpreadDaysStr ? Number(maxBookingSpreadDaysStr) : null;
    const toastId = themedToast(<NotificationContent content={`Updating product '${product.name}'...`} />, infoNotificationOptions);
    const finalPrimaryFeatureImage = primaryFeatureImage
      ? {
          original: primaryFeatureImage.original
            ? { url: primaryFeatureImage.original.url, height: primaryFeatureImage.original.height, width: primaryFeatureImage.original.width }
            : null,
          thumbnail: primaryFeatureImage.thumbnail
            ? { url: primaryFeatureImage.thumbnail.url, height: primaryFeatureImage.thumbnail.height, width: primaryFeatureImage.thumbnail.width }
            : null,
        }
      : null;
    const maxAllowedResourcesLockTimePaidViaCard = maxAllowedResourcesLockTimePaidViaCardStr
      ? Number(maxAllowedResourcesLockTimePaidViaCardStr)
      : rootData.defaultMaxAllowedResourcesLockTimePaidViaCard;
    const maxAllowedResourcesLockTimePaidViaBankTransfer = maxAllowedResourcesLockTimePaidViaBankTransferStr
      ? Number(maxAllowedResourcesLockTimePaidViaBankTransferStr) * 60 * 24
      : rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer;

    commitUpdateProduct({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: product.id,
          name,
          description,
          price,
          isPriceTaxInclusive,
          priceUnit: priceUnit as PriceUnit,
          currency: currency as Currency,
          numberOfResourcesToBook,
          bookAllLocationResources,
          minDurationMinutes,
          maxDurationMinutes,
          recurrenceWindowDays,
          requireConsecutiveDays,
          maxBookingSpreadDays,
          productTagIds,
          locationTagIds,
          primaryFeatureImage: finalPrimaryFeatureImage,
          maxAllowedResourcesLockTimePaidViaCard,
          maxAllowedResourcesLockTimePaidViaBankTransfer,
          acceptedBookingPaymentMethods: acceptedBookingPaymentMethods.map((type) => type as PaymentMethod),
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
            price,
            isPriceTaxInclusive,
            priceUnit: {
              type: priceUnit as PriceUnit,
              name: '',
            },
            currency: {
              type: currency as Currency,
              name: '',
            },
            numberOfResourcesToBook,
            bookAllLocationResources,
            minDurationMinutes,
            maxDurationMinutes,
            recurrenceWindowDays,
            requireConsecutiveDays,
            maxBookingSpreadDays,
            productTags: [],
            locationTags: [],
            primaryFeatureImage: finalPrimaryFeatureImage,
            maxAllowedResourcesLockTimePaidViaCard,
            maxAllowedResourcesLockTimePaidViaBankTransfer,
            acceptedBookingPaymentMethods: [],
          },
        },
      },
    });
  };

  const handleCloseClick = () => {
    router.back();
  };

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setPrimaryFeatureImage(response);
  };

  if (!rootData.product) {
    return <></>;
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
              price,
              priceUnit,
              currency,
              minDurationMinutes,
              maxDurationMinutes,
              bookAllLocationResources,
              requireConsecutiveDays,
              recurrenceWindowDays,
              maxBookingSpreadDays,
              numberOfResourcesToBook,
              productTagIds,
              locationTagIds,
              maxAllowedResourcesLockTimePaidViaCard,
              maxAllowedResourcesLockTimePaidViaBankTransfer,
              acceptedBookingPaymentMethods,
              isPriceTaxInclusive,
            }}
            validate={validateProductDetails}
            render={({ handleSubmit, values }) => {
              debounceSetName(values!.name);
              debounceSetDescription(values!.description);
              debounceSetPrice(values!.price);
              debounceSetPriceUnit(values!.priceUnit);
              debounceSetCurrency(values!.currency);
              debounceSetMinDurationMinutes(values!.minDurationMinutes);
              debounceSetMaxDurationMinutes(values!.maxDurationMinutes);
              debounceSetBookAllLocationResources(values!.bookAllLocationResources);
              debounceSetRequireConsecutiveDays(values!.requireConsecutiveDays);
              debounceSetRecurrenceWindowDays(values!.recurrenceWindowDays);
              debounceSetMaxBookingSpreadDays(values!.maxBookingSpreadDays);
              debounceSetNumberOfResourcesToBook(values!.numberOfResourcesToBook);
              debounceSetProductTagIds(values!.productTagIds);
              debounceSetLocationTagIds(values!.locationTagIds);
              debounceSetMaxAllowedResourcesLockTimePaidViaCard(values!.maxAllowedResourcesLockTimePaidViaCard);
              debounceSetMaxAllowedResourcesLockTimePaidViaBankTransfer(values!.maxAllowedResourcesLockTimePaidViaBankTransfer);
              debounceSetAcceptedBookingPaymentMethods(values!.acceptedBookingPaymentMethods);
              debounceSetIsPriceTaxInclusive(values!.isPriceTaxInclusive);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Edit Product" />
                    <BodyIconTypography label="Edit your product details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Feature Image">
                      <StackColumn>
                        {primaryFeatureImage?.thumbnail && primaryFeatureImage.original.height && primaryFeatureImage.original.width && (
                          <Image src={primaryFeatureImage.original.url} height={primaryFeatureImage.original.height} width={primaryFeatureImage.original.width} alt="" />
                        )}
                        <ImageFileUploaderWithCropper
                          defaultAspectRatio={productFeatureImageWidth / productFeatureImageHeight}
                          onUploadCompleted={handleFeatureImageUploadCompleted}
                        />
                      </StackColumn>
                    </FormFieldLabel>

                    <FormFieldLabel label="Name">
                      <TextField name="name" required={requiredFields.name} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Description">
                      <TextField name="description" required={requiredFields.description} multiline rows={3} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Price">
                      <TextField name="price" required={requiredFields.price} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Price Unit">
                      <SingleChoicePriceUnit rootDataRelay={rootData} name="priceUnit" required={requiredFields.priceUnit} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Currency">
                      <SingleChoiceCurrency rootDataRelay={rootData} name="currency" required={requiredFields.currency} />
                    </FormFieldLabel>

                    <FormFieldLabel>
                      <Switches
                        name="isPriceTaxInclusive"
                        required={requiredFields.isPriceTaxInclusive}
                        data={{ label: 'Is price tax inclusive?', value: 'isPriceTaxInclusive' }}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Minimum Duration (minutes)">
                      <TextField name="minDurationMinutes" required={requiredFields.minDurationMinutes} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Maximum Duration (minutes)">
                      <TextField name="maxDurationMinutes" required={requiredFields.maxDurationMinutes} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Accepted Payment Methods">
                      <MultipleChoicesBookingPaymentMethodTypes
                        rootDataRelay={rootData}
                        name="acceptedBookingPaymentMethods"
                        required={requiredFields.acceptedBookingPaymentMethods}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Product Tags">
                      <MultipleChoicesProductTags
                        rootDataRelay={rootData}
                        name="productTagIds"
                        required={requiredFields.productTagIds}
                        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel label="Location Tags">
                      <MultipleChoicesLocationTags
                        rootDataRelay={rootData}
                        name="locationTagIds"
                        required={requiredFields.locationTagIds}
                        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
                      />
                    </FormFieldLabel>

                    <FormFieldLabel>
                      <Switches
                        name="bookAllLocationResources"
                        required={requiredFields.bookAllLocationResources}
                        data={{
                          label: 'Book all location resources',
                          value: 'bookAllLocationResources',
                        }}
                        helperText="If checked, all location resources will be booked for this product."
                      />
                    </FormFieldLabel>

                    {!bookAllLocationResources && (
                      <FormFieldLabel label="Number of Resources to Book">
                        <TextField name="numberOfResourcesToBook" required={requiredFields.numberOfResourcesToBook} />
                      </FormFieldLabel>
                    )}

                    {!bookAllLocationResources && (
                      <FormFieldLabel label="Recurrence Window Days">
                        <TextField name="recurrenceWindowDays" required={requiredFields.recurrenceWindowDays} />
                      </FormFieldLabel>
                    )}

                    {!bookAllLocationResources && (
                      <FormFieldLabel>
                        <Switches
                          name="requireConsecutiveDays"
                          required={requiredFields.requireConsecutiveDays}
                          data={{ label: 'Must book consecutive days', value: 'requireConsecutiveDays' }}
                          helperText="If checked, only consecutive days booking allowed for this product."
                        />
                      </FormFieldLabel>
                    )}

                    {!bookAllLocationResources && !requireConsecutiveDays && (
                      <FormFieldLabel label="Max Booking Spread Days">
                        <TextField name="maxBookingSpreadDays" required={requiredFields.maxBookingSpreadDays} />
                      </FormFieldLabel>
                    )}

                    <FormFieldLabel label="Maximum Permitted Resource Lock Duration Paid via Card (minutes)">
                      <TextField name="maxAllowedResourcesLockTimePaidViaCard" required={requiredFields.maxAllowedResourcesLockTimePaidViaCard} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Maximum Permitted Resource Lock Duration Paid via Bank Transfer (days)">
                      <TextField name="maxAllowedResourcesLockTimePaidViaBankTransfer" required={requiredFields.maxAllowedResourcesLockTimePaidViaBankTransfer} />
                    </FormFieldLabel>
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
