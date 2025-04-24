import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import {
  MultipleChoicesLocationTags,
  MultipleChoicesProductTags,
  SingleChoiceOrganizationStripeConnectAccount,
  SingleChoicesCurrency,
  SingleChoicesPriceUnit,
} from '@/components/organization';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { PaletteModeContext } from '@/libs/providers';
import { defaultButtonStyle, defaultPadding } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addProduct_addProductMutation, Currency, PriceUnit } from '@/queries/__generated__/addProduct_addProductMutation.graphql';
import type { addProduct_rootQuery } from '@/queries/__generated__/addProduct_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import { makeRequired, makeValidate, Switches, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, boolean, number, object, string } from 'yup';

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
    $singleChoiceOrganizationStripeConnectAccountSortingValues: [OrganizationStripeConnectAccountOrderInput!]
  ) {
    openingHoursMinutesStep
    ...multipleChoicesProductTags_query
    ...multipleChoicesLocationTags_query
    ...singleChoicePriceUnit_query
    ...singleChoiceCurrency_query
    ...singleChoiceOrganizationStripeConnectAccount_query
  }
`;

type ProductDetails = {
  name: string;
  description: string | null;
  price: string;
  priceUnit: string;
  currency: string;
  numberOfResourcesToBook: number;
  minDurationMinutes: number | null;
  maxDurationMinutes: number | null;
  bookAllLocationResources: boolean;
  recurrenceWindowDays: number;
  requireConsecutiveDays: boolean;
  maxBookingSpreadDays: number | null;
  productTagIds: string[];
  locationTagIds: string[];
  organizationStripeConnectAccountId?: string;
};

const productSchema = (openingHoursMinutesStep: number) =>
  object({
    name: string().min(3, 'Product name must be at least three characters long.').required('Product name is required'),
    description: string().nullable(),
    price: string()
      .matches(/^\d+(\.\d{1,2})?$/, 'Price must be a valid decimal number')
      .required('Price is required'),
    priceUnit: string().required('Price Unit is required'),
    currency: string().required('Currency is required'),
    numberOfResourcesToBook: number().required('Number of resources to book is required').min(1, 'Number of resources to book must be greater than 0'),
    minDurationMinutes: number()
      .nullable()
      .test('is-multiple-of-openingHoursMinutesStep', `Minimum duration in minutes must be in ${openingHoursMinutesStep}-minutes increments`, function (value) {
        if (typeof value !== 'number') {
          return true;
        }

        return value % openingHoursMinutesStep === 0;
      })
      .test('is-less-than-maxDurationMinutes', 'Minimum duration in minutes must be less or equal than maximum duration in minutes', function (value) {
        const { maxDurationMinutes } = this.parent;
        if (!maxDurationMinutes) {
          return true;
        }

        if (typeof value !== 'number') {
          return true;
        }

        return value <= maxDurationMinutes;
      }),
    maxDurationMinutes: number()
      .nullable()
      .test('is-multiple-of-openingHoursMinutesStep', `Maximum duration in minutes must be in ${openingHoursMinutesStep}-minutes increments`, function (value) {
        if (typeof value !== 'number') {
          return true;
        }

        return value % openingHoursMinutesStep === 0;
      })
      .test('is-less-than-minDurationMinutes', 'Maximum duration in minutes must be greater or equal than minimum duration in minutes', function (value) {
        const { minDurationMinutes } = this.parent;
        if (!minDurationMinutes) {
          return true;
        }

        if (typeof value !== 'number') {
          return true;
        }

        return value >= minDurationMinutes;
      }),
    mustBookAllLocationResources: boolean(),
    recurrenceWindowDays: number()
      .test('is-required', 'Recurrence window days is required', function (value) {
        const { bookAllLocationResources } = this.parent;
        if (bookAllLocationResources) {
          return true;
        }

        return !!value;
      })
      .test('is-greater-than-zero', 'Recurrence window days must be greater than 0', function (value) {
        const { bookAllLocationResources } = this.parent;
        if (bookAllLocationResources) {
          return true;
        }

        return typeof value === 'number' && value > 0;
      }),
    requireConsecutiveDays: boolean(),
    maxBookingSpreadDays: number()
      .nullable()
      .test('is-greater-than-recurrence', 'Max booking spread days must be greater than or equal to recurrence window days', function (value) {
        const { bookAllLocationResources, requireConsecutiveDays, recurrenceWindowDays } = this.parent;
        if (bookAllLocationResources || requireConsecutiveDays) {
          return true;
        }

        if (!value) {
          return true;
        }

        return typeof value === 'number' && value >= recurrenceWindowDays;
      })
      .test('is-greater-than-zero', 'Max booking spread days must be greater than 0', function (value) {
        const { bookAllLocationResources, requireConsecutiveDays } = this.parent;
        if (bookAllLocationResources || requireConsecutiveDays) {
          return true;
        }

        if (!value) {
          return true;
        }

        return typeof value === 'number' && value > 0;
      }),
    productTagIds: array().min(1, 'At least one product tag must be selected').required('Product tags are required'),
    locationTagIds: array().nullable(),
    organizationStripeConnectAccountId: string().nullable(),
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
          organizationStripeConnectAccountDetails {
            uniqueId
            name
          }
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
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validateProductDetails = makeValidate(productSchema(rootData.openingHoursMinutesStep));
  const requiredFields = makeRequired(productSchema(rootData.openingHoursMinutesStep));
  const [name, setName] = useState('');
  const [description, setDescription] = useState<string | null>('');
  const [price, setPrice] = useState('0.00');
  const [priceUnit, setPriceUnit] = useState('');
  const [currency, setCurrency] = useState('');
  const [numberOfResourcesToBook, setNumberOfResourcesToBook] = useState(1);
  const [minDurationMinutes, setMinDurationMinutes] = useState<number | null>(null);
  const [maxDurationMinutes, setMaxDurationMinutes] = useState<number | null>(null);
  const [bookAllLocationResources, setBookAllLocationResources] = useState(false);
  const [recurrenceWindowDays, setRecurrenceWindowDays] = useState(1);
  const [requireConsecutiveDays, setRequireConsecutiveDays] = useState(false);
  const [maxBookingSpreadDays, setMaxBookingSpreadDays] = useState<number | null>(1);
  const [productTagIds, setProductTagIds] = useState<string[]>([]);
  const [locationTagIds, setLocationTagIds] = useState<string[]>([]);
  const [organizationStripeConnectAccountId, setOrganizationStripeConnectAccountId] = useState<string | null | undefined>();

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
    organizationStripeConnectAccountId,
  }: ProductDetails) => {
    const id = nanoid();
    const numberOfResourcesToBook = Number(numberOfResourcesToBookStr);
    const minDurationMinutes = minDurationMinutesStr ? Number(minDurationMinutesStr) : null;
    const maxDurationMinutes = maxDurationMinutesStr ? Number(maxDurationMinutesStr) : null;
    const recurrenceWindowDays = recurrenceWindowDaysStr ? Number(recurrenceWindowDaysStr) : 1;
    const maxBookingSpreadDays = maxBookingSpreadDaysStr ? Number(maxBookingSpreadDaysStr) : null;
    const toastId = themedToast(<NotificationContent content={`Adding product '${name}'...`} />, infoNotificationOptions);

    commitAddProduct({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          description,
          price,
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
          organizationStripeConnectAccountId,
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
            organizationStripeConnectAccountDetails: null,
          },
        },
      },
    });
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
              organizationStripeConnectAccountId,
            }}
            validate={validateProductDetails}
            render={({ handleSubmit, values }) => {
              setName(values.name);
              setDescription(values.description);
              setPrice(values.price);
              setPriceUnit(values.priceUnit);
              setCurrency(values.currency);
              setMinDurationMinutes(values.minDurationMinutes);
              setMaxDurationMinutes(values.maxDurationMinutes);
              setBookAllLocationResources(values.bookAllLocationResources);
              setRequireConsecutiveDays(values.requireConsecutiveDays);
              setRecurrenceWindowDays(values.recurrenceWindowDays);
              setMaxBookingSpreadDays(values.maxBookingSpreadDays);
              setNumberOfResourcesToBook(values.numberOfResourcesToBook);
              setProductTagIds(values.productTagIds);
              setLocationTagIds(values.locationTagIds);
              setOrganizationStripeConnectAccountId(values.organizationStripeConnectAccountId);

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
                    <SectionIconTypography label="Product Setup" />
                    <BodyIconTypography label="Edit your product name and details" />
                    <Divider />
                  </StackColumn>

                  <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
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
                      <SingleChoicesPriceUnit rootDataRelay={rootData} name="priceUnit" required={requiredFields.priceUnit} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Currency">
                      <SingleChoicesCurrency rootDataRelay={rootData} name="currency" required={requiredFields.currency} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Minimum Duration (minutes)">
                      <TextField name="minDurationMinutes" required={requiredFields.minDurationMinutes} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Maximum Duration (minutes)">
                      <TextField name="maxDurationMinutes" required={requiredFields.maxDurationMinutes} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Product Tags">
                      <MultipleChoicesProductTags rootDataRelay={rootData} name="productTagIds" required={requiredFields.productTagIds} organizationId={organizationId} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Location Tags">
                      <MultipleChoicesLocationTags rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} organizationId={organizationId} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Stripe Connect Account">
                      <SingleChoiceOrganizationStripeConnectAccount
                        rootDataRelay={rootData}
                        name="organizationStripeConnectAccountId"
                        required={requiredFields.organizationStripeConnectAccountId}
                      />
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
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
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
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        multipleChoicesLocationTagsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        singleChoiceOrganizationStripeConnectAccountSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
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
      setTriggerReloadId(nanoid());

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
