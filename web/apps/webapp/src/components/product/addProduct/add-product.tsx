import { FileUploadResponse } from '@/clients/openapi/skedular/v1/core/fetch';
import { MultipleChoicesBookingPaymentMethodTypes } from '@/components/booking';
import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags, MultipleChoicesProductTags, SingleChoiceCurrency, SingleChoicePriceUnit } from '@/components/organization';
import { productFeatureImageHeight, productFeatureImageWidth } from '@/components/product';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { ImageFileUploaderWithCropper } from '@/libs/image-file-uploader';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors, keyboardTextFieldDebounceTimeout } from '@/libs/utils';
import type { addProduct_addProductMutation, Currency, PaymentMethod, PriceUnit } from '@/queries/__generated__/addProduct_addProductMutation.graphql';
import type { addProduct_rootQuery } from '@/queries/__generated__/addProduct_rootQuery.graphql';
import { Box, Button } from '@mui/material';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import Image from 'next/image';
import { useParams } from 'next/navigation';
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
  organizationId: string;
  onAdded: (productId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addProduct_rootQuery(
    $organizationId: String!
    $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]
  ) {
    openingHoursMinutesStep
    defaultMaxAllowedResourcesLockTimePaidViaCard
    defaultMaxAllowedResourcesLockTimePaidViaBankTransfer
    ...multipleChoicesProductTags_query
    ...multipleChoicesLocationTags_query
    ...singleChoicePriceUnit_query
    ...singleChoiceCurrency_query
    ...multipleChoicesBookingPaymentMethodTypes_query
  }
`;

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
        var price = Number(value);
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
        var minDurationMinutes = Number(value);
        if (isNaN(minDurationMinutes)) {
          return true;
        }

        return minDurationMinutes % openingHoursMinutesStep === 0;
      })
      .test('is-less-than-maxDurationMinutes', 'Minimum duration in minutes must be less or equal than maximum duration in minutes.', function (value) {
        const { maxDurationMinutes: maxDurationMinutesStr } = this.parent;
        var maxDurationMinutes = Number(maxDurationMinutesStr);
        if (isNaN(maxDurationMinutes)) {
          return true;
        }

        var minDurationMinutes = Number(value);
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

        var minDurationMinutes = Number(value);
        if (isNaN(minDurationMinutes)) {
          return true;
        }

        return minDurationMinutes % 60 === 0;
      }),
    maxDurationMinutes: string()
      .nullable()
      .test('is-multiple-of-openingHoursMinutesStep', `Maximum duration in minutes must be in ${openingHoursMinutesStep}-minutes increments.`, function (value) {
        var maxDurationMinutes = Number(value);
        if (isNaN(maxDurationMinutes)) {
          return true;
        }

        return maxDurationMinutes % openingHoursMinutesStep === 0;
      })
      .test('is-less-than-minDurationMinutes', 'Maximum duration in minutes must be greater or equal than minimum duration in minutes.', function (value) {
        const { minDurationMinutes: minDurationMinutesStr } = this.parent;
        var minDurationMinutes = Number(minDurationMinutesStr);
        if (isNaN(minDurationMinutes)) {
          return true;
        }

        var maxDurationMinutes = Number(value);
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

        var maxDurationMinutes = Number(value);
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

        var maxBookingSpreadDays = Number(value);
        if (isNaN(maxBookingSpreadDays)) {
          return true;
        }

        var recurrenceWindowDays = Number(recurrenceWindowDaysStr);
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

        var maxBookingSpreadDays = Number(value);
        if (isNaN(maxBookingSpreadDays)) {
          return true;
        }

        return maxBookingSpreadDays > 0;
      }),
    productTagIds: array().min(1, 'At least one product tag must be selected.').required('Product tags are required.'),
    locationTagIds: array().nullable(),
    maxAllowedResourcesLockTimePaidViaCard: string()
      .test('is-number', 'Max allowed resources lock time must be a valid number.', function (value) {
        var maxAllowedResourcesLockTimePaidViaCard = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaCard)) {
          return false;
        }

        return true;
      })
      .test('is-greater-than-zero', 'Max allowed resources lock time must be greater than 0.', function (value) {
        var maxAllowedResourcesLockTimePaidViaCard = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaCard)) {
          return false;
        }

        return maxAllowedResourcesLockTimePaidViaCard > 0;
      }),
    maxAllowedResourcesLockTimePaidViaBankTransfer: string()
      .test('is-number', 'Max allowed resources lock time must be a valid number.', function (value) {
        var maxAllowedResourcesLockTimePaidViaBankTransfer = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaBankTransfer)) {
          return false;
        }

        return true;
      })
      .test('is-greater-than-zero', 'Max allowed resources lock time must be greater than 0.', function (value) {
        var maxAllowedResourcesLockTimePaidViaBankTransfer = Number(value);
        if (isNaN(maxAllowedResourcesLockTimePaidViaBankTransfer)) {
          return false;
        }

        return maxAllowedResourcesLockTimePaidViaBankTransfer > 0;
      }),
    acceptedBookingPaymentMethods: array().min(1, 'At least one accepted booking payment method must be selected".').required('Booking payment methods are required.'),
    isPriceTaxInclusive: boolean().required(),
  });

const AddProduct = ({ queryReference, onReloadRequired, organizationId, onAdded, onCancel }: Props) => {
  const rootData = usePreloadedQuery<addProduct_rootQuery>(RootQuery, queryReference);
  const [commitAddProduct] = useMutation<addProduct_addProductMutation>(graphql`
    mutation addProduct_addProductMutation($input: AddProductInput!) @raw_response_type {
      addProduct(input: $input) {
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
            uniqueId
            name
            color
          }
          locationTags {
            uniqueId
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProductDetails = makeValidate(productSchema(rootData.openingHoursMinutesStep));
  const requiredFields = makeRequired(productSchema(rootData.openingHoursMinutesStep));
  const [name, setName] = useState('');
  const debounceSetName = useDebounceCallback(setName, keyboardTextFieldDebounceTimeout);

  const [description, setDescription] = useState<string | null>('');
  const debounceSetDescription = useDebounceCallback(setDescription, keyboardTextFieldDebounceTimeout);

  const [price, setPrice] = useState('0.00');
  const debounceSetPrice = useDebounceCallback(setPrice, keyboardTextFieldDebounceTimeout);

  const [priceUnit, setPriceUnit] = useState('');
  const debounceSetPriceUnit = useDebounceCallback(setPriceUnit, keyboardTextFieldDebounceTimeout);

  const [currency, setCurrency] = useState('');
  const debounceSetCurrency = useDebounceCallback(setCurrency, keyboardTextFieldDebounceTimeout);

  const [numberOfResourcesToBook, setNumberOfResourcesToBook] = useState('1');
  const debounceSetNumberOfResourcesToBook = useDebounceCallback(setNumberOfResourcesToBook, keyboardTextFieldDebounceTimeout);

  const [minDurationMinutes, setMinDurationMinutes] = useState<string | null>(null);
  const debounceSetMinDurationMinutes = useDebounceCallback(setMinDurationMinutes, keyboardTextFieldDebounceTimeout);

  const [maxDurationMinutes, setMaxDurationMinutes] = useState<string | null>(null);
  const debounceSetMaxDurationMinutes = useDebounceCallback(setMaxDurationMinutes, keyboardTextFieldDebounceTimeout);

  const [bookAllLocationResources, setBookAllLocationResources] = useState(false);
  const debounceSetBookAllLocationResources = useDebounceCallback(setBookAllLocationResources, keyboardTextFieldDebounceTimeout);

  const [recurrenceWindowDays, setRecurrenceWindowDays] = useState('1');
  const debounceSetRecurrenceWindowDays = useDebounceCallback(setRecurrenceWindowDays, keyboardTextFieldDebounceTimeout);

  const [requireConsecutiveDays, setRequireConsecutiveDays] = useState(false);
  const debounceSetRequireConsecutiveDays = useDebounceCallback(setRequireConsecutiveDays, keyboardTextFieldDebounceTimeout);

  const [maxBookingSpreadDays, setMaxBookingSpreadDays] = useState<string | null>('1');
  const debounceSetMaxBookingSpreadDays = useDebounceCallback(setMaxBookingSpreadDays, keyboardTextFieldDebounceTimeout);

  const [productTagIds, setProductTagIds] = useState<string[]>([]);
  const debounceSetProductTagIds = useDebounceCallback(setProductTagIds, keyboardTextFieldDebounceTimeout);

  const [locationTagIds, setLocationTagIds] = useState<string[]>([]);
  const debounceSetLocationTagIds = useDebounceCallback(setLocationTagIds, keyboardTextFieldDebounceTimeout);

  const [maxAllowedResourcesLockTimePaidViaCard, setMaxAllowedResourcesLockTimePaidViaCard] = useState<string>(rootData.defaultMaxAllowedResourcesLockTimePaidViaCard.toString());
  const debounceSetMaxAllowedResourcesLockTimePaidViaCard = useDebounceCallback(setMaxAllowedResourcesLockTimePaidViaCard, keyboardTextFieldDebounceTimeout);

  const [maxAllowedResourcesLockTimePaidViaBankTransfer, setMaxAllowedResourcesLockTimePaidViaBankTransfer] = useState<string>(
    (rootData.defaultMaxAllowedResourcesLockTimePaidViaBankTransfer / (60 * 24)).toString(),
  );
  const debounceSetMaxAllowedResourcesLockTimePaidViaBankTransfer = useDebounceCallback(setMaxAllowedResourcesLockTimePaidViaBankTransfer, keyboardTextFieldDebounceTimeout);

  const [acceptedBookingPaymentMethods, setAcceptedBookingPaymentMethods] = useState<string[]>([]);
  const debounceSetAcceptedBookingPaymentMethods = useDebounceCallback(setAcceptedBookingPaymentMethods, keyboardTextFieldDebounceTimeout);

  const [isPriceTaxInclusive, setIsPriceTaxInclusive] = useState(true);
  const debounceSetIsPriceTaxInclusive = useDebounceCallback(setIsPriceTaxInclusive, keyboardTextFieldDebounceTimeout);

  const [primaryFeatureImage, setPrimaryFeatureImage] = useState<FileUploadResponse>();

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleProductAddClick = ({
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
    const id = uuid();
    const numberOfResourcesToBook = Number(numberOfResourcesToBookStr);
    const minDurationMinutes = minDurationMinutesStr ? Number(minDurationMinutesStr) : null;
    const maxDurationMinutes = maxDurationMinutesStr ? Number(maxDurationMinutesStr) : null;
    const recurrenceWindowDays = recurrenceWindowDaysStr ? Number(recurrenceWindowDaysStr) : 1;
    const maxBookingSpreadDays = maxBookingSpreadDaysStr ? Number(maxBookingSpreadDaysStr) : null;
    const toastId = themedToast(<NotificationContent content={`Adding product '${name}'...`} />, infoNotificationOptions);
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

    commitAddProduct({
      variables: {
        input: {
          clientMutationId: uuid(),
          id,
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
          organizationId,
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

  const handleFeatureImageUploadCompleted = (response: FileUploadResponse) => {
    setPrimaryFeatureImage(response);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBarWithStackColumn onClose={handleCloseClick} label="Add Product">
          <Form
            onSubmit={handleProductAddClick}
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
                    <SectionIconTypography label="Product Setup" />
                    <BodyIconTypography label="Edit your product name and details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <FormFieldLabel label="Feature image">
                      <StackColumn>
                        {primaryFeatureImage?.thumbnail && primaryFeatureImage.original.height && primaryFeatureImage.original.width && (
                          <Image src={primaryFeatureImage.original.url} height={primaryFeatureImage.original.height} width={primaryFeatureImage.original.width} alt="" />
                        )}
                        <ImageFileUploaderWithCropper
                          defaultAspectRatio={productFeatureImageWidth / productFeatureImageHeight}
                          previewImageHeight={productFeatureImageHeight}
                          previewImageWidth={productFeatureImageWidth}
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
                      <MultipleChoicesProductTags rootDataRelay={rootData} name="productTagIds" required={requiredFields.productTagIds} organizationId={organizationId} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Location Tags">
                      <MultipleChoicesLocationTags rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} organizationId={organizationId} />
                    </FormFieldLabel>

                    <FormFieldLabel>
                      <Switches
                        name="bookAllLocationResources"
                        required={requiredFields.bookAllLocationResources}
                        data={{ label: 'Book all location resources', value: 'bookAllLocationResources' }}
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
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        multipleChoicesProductTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesLocationTagsSortingValues: [
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
  }, [loadQuery, triggerReloadId, finalOrganizationId]);

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
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoAddProduct queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} onAdded={onAdded} onCancel={onCancel} />
    </ErrorBoundary>
  );
};

export default memo(AddProductWithRelay);
