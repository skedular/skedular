import { AppBarWithStackColumn, BodyIconTypography, FormFieldLabel, FormStackColumn, SectionIconTypography, StackColumn, StackRow } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { MultipleChoicesLocationTags, MultipleChoicesProductTags, SingleChoicesCurrency, SingleChoicesPriceUnit } from '@/components/organization';
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
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useParams } from 'next/navigation';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<addProduct_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  onAdded: (locationId: string) => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query addProduct_rootQuery(
    $organizationId: String!
    $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]
  ) {
    ...multipleChoicesProductTags_query
    ...multipleChoicesLocationTags_query
    ...singleChoicePriceUnit_query
    ...singleChoiceCurrency_query
  }
`;

type ProductDetails = {
  name: string;
  description: string | null;
  priceUnit: string;
  currency: string;
  productTagIds: string[];
  locationTagIds: string[];
};

const productSchema = object({
  name: string().min(3, 'Product name must be at least three characters long.').required('Product name is required'),
  description: string().nullable(),
  priceUnit: string().required('Price Unit is required'),
  currency: string().required('Currency is required'),
  productTagIds: array().nullable(),
  locationTagIds: array().nullable(),
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
          minDurationMinutes
          maxDurationMinutes
          bookAllLocationResources
          recurrenceIntervalDays
          forceContinuousSlots
          maxSpreadDays
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
  const validateProductDetails = makeValidate(productSchema);
  const requiredFields = makeRequired(productSchema);

  const handleCloseClick = () => {
    onCancel();
    onReloadRequired();
  };

  const handleProductAddClick = ({ name, description, priceUnit, currency, productTagIds, locationTagIds }: ProductDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding product '${name}'...`} />, infoNotificationOptions);

    commitAddProduct({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          description,
          price: '2.99',
          priceUnit: priceUnit as PriceUnit,
          currency: currency as Currency,
          minDurationMinutes: null,
          maxDurationMinutes: null,
          bookAllLocationResources: false,
          recurrenceIntervalDays: 1,
          forceContinuousSlots: false,
          maxSpreadDays: null,
          productTagIds,
          locationTagIds,
          organizationId,
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
            price: '2.99',
            priceUnit: {
              type: priceUnit as PriceUnit,
              name: '',
            },
            currency: {
              type: currency as Currency,
              name: '',
            },
            minDurationMinutes: null,
            maxDurationMinutes: null,
            bookAllLocationResources: false,
            recurrenceIntervalDays: 1,
            forceContinuousSlots: false,
            maxSpreadDays: null,
            productTags: [],
            locationTags: [],
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
              name: '',
              description: '',
              priceUnit: '',
              currency: '',
              productTagIds: [],
              locationTagIds: [],
            }}
            validate={validateProductDetails}
            render={({ handleSubmit }) => (
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

                    <FormFieldLabel label="Price Unit">
                      <SingleChoicesPriceUnit rootDataRelay={rootData} name="priceUnit" required={requiredFields.priceUnit} />
                    </FormFieldLabel>

                    <FormFieldLabel label="Currency">
                      <SingleChoicesCurrency rootDataRelay={rootData} name="currency" required={requiredFields.currency} />
                    </FormFieldLabel>

                  <FormFieldLabel label="Product Tags">
                    <MultipleChoicesProductTags rootDataRelay={rootData} name="productTagIds" required={requiredFields.productTagIds} organizationId={organizationId} />
                  </FormFieldLabel>

                  <FormFieldLabel label="Location Tags">
                    <MultipleChoicesLocationTags rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} organizationId={organizationId} />
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
            )}
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
