import { PaletteModeContext, RelayError, getRelayErrorMessage, toRootError } from '@skedular/shared';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';

import type { editOrganizationProductTagDialog_rootQuery } from '@/queries/__generated__/editOrganizationProductTagDialog_rootQuery.graphql';
import type { editOrganizationProductTagDialog_updateProductTagMutation } from '@/queries/__generated__/editOrganizationProductTagDialog_updateProductTagMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';

import { ColorPicker, FormFieldLabel, FormStackColumn, PageHeaderPanel, SettingsSectionCard, SmallIconTypography, StackColumn, StickyReviewRail } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { useDebounceCallback } from 'usehooks-ts';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<editOrganizationProductTagDialog_rootQuery, Record<string, unknown>>;
  productTagId: string;
  onSaved: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationProductTagDialog_rootQuery($productTagId: String!) {
    productTag(id: $productTagId) {
      id
      name
      description
      color
    }
  }
`;

type ProductTagDetails = {
  name: string;
  description: string | null | undefined;
};

type TagPatchField = 'NAME' | 'DESCRIPTION' | 'COLOR';

type ProductTagPatchDetails = ProductTagDetails & {
  color: string | null | undefined;
};

const inlinePatchDebounceTimeout = 1000;

const getChangedTagFields = (left: ProductTagPatchDetails, right: ProductTagPatchDetails): TagPatchField[] => {
  const fieldsToUpdate: TagPatchField[] = [];
  if (left.name !== right.name) {
    fieldsToUpdate.push('NAME');
  }
  if (left.description !== right.description) {
    fieldsToUpdate.push('DESCRIPTION');
  }
  if (left.color !== right.color) {
    fieldsToUpdate.push('COLOR');
  }

  return fieldsToUpdate;
};

const productTagSchema = object({
  name: string().required('Product tag name is required'),
  description: string().nullable(),
});

const getValidTagPatchFields = (fieldsToUpdate: TagPatchField[], values: ProductTagPatchDetails): TagPatchField[] =>
  fieldsToUpdate.filter((field) => {
    if (field === 'COLOR') {
      return true;
    }

    const formField = field === 'NAME' ? 'name' : 'description';
    try {
      productTagSchema.validateSyncAt(formField, values);
      return true;
    } catch {
      return false;
    }
  });

const EditOrganizationProductTagPageComponent = ({ queryReference, productTagId, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationProductTagDialog_rootQuery>(RootQuery, queryReference);
  const [commitUpdateProductTagPatch] = useMutation<editOrganizationProductTagDialog_updateProductTagMutation>(graphql`
    mutation editOrganizationProductTagDialog_updateProductTagMutation($input: UpdateOrganizationTagInput!) @raw_response_type {
      updateProductTag(input: $input) {
        organizationTag {
          id
          name
          description
          color
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(productTagSchema);
  const requiredFields = makeRequired(productTagSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.productTag?.color);
  const initialProductTagValues = useMemo<ProductTagPatchDetails>(
    () => ({
      name: rootData.productTag?.name ?? '',
      description: rootData.productTag?.description,
      color: rootData.productTag?.color,
    }),
    [rootData.productTag],
  );
  const draftProductTagValues = useRef(initialProductTagValues);
  const submittedProductTagValues = useRef(initialProductTagValues);

  const commitProductTagPatch = useCallback(
    (fieldsToUpdate: TagPatchField[], values: ProductTagPatchDetails) => {
      const productTag = rootData.productTag;
      const validFieldsToUpdate = getValidTagPatchFields(fieldsToUpdate, values);
      if (!productTag || validFieldsToUpdate.length === 0) {
        return;
      }

      const previousValues = submittedProductTagValues.current;
      if (getChangedTagFields(previousValues, values).filter((field) => validFieldsToUpdate.includes(field)).length === 0) {
        return;
      }
      submittedProductTagValues.current = values;

      commitUpdateProductTagPatch({
        variables: {
          input: {
            clientMutationId: uuid(),
            id: productTagId,
            fieldsToUpdate: validFieldsToUpdate,
            name: values.name,
            description: values.description,
            color: values.color,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            submittedProductTagValues.current = previousValues;
            themedToast(<NotificationContent content={`Failed to update product tag '${productTag.name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          }
        },
        onError: (error) => {
          submittedProductTagValues.current = previousValues;
          themedToast(<NotificationContent content={`Failed to update product tag '${productTag.name}'. Error: ${error.message}.`} />, errorNotificationOptions);
        },
        optimisticResponse: {
          updateProductTag: {
            organizationTag: {
              id: productTagId,
              name: values.name,
              description: values.description,
              color: values.color,
            },
          },
        },
      });
    },
    [commitUpdateProductTagPatch, productTagId, rootData.productTag, themedToast],
  );
  const debouncedCommitProductTagPatch = useDebounceCallback(commitProductTagPatch, inlinePatchDebounceTimeout);

  if (!rootData.productTag) {
    return null;
  }

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel
            title="Edit product tag"
            description="Update the product tag name, description, and colour used across marketplace products and resources."
            actions={
              <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                Cancel
              </Button>
            }
          />

          <Form
            onSubmit={() => undefined}
            initialValues={{ name: rootData.productTag.name, description: rootData.productTag.description }}
            validate={validate}
            render={({ handleSubmit, values }) => {
              const productTagValues = values as ProductTagDetails;
              const nextProductTagValues = { ...productTagValues, color: selectedColor };
              const changedFields = getChangedTagFields(draftProductTagValues.current, nextProductTagValues);
              if (changedFields.length > 0) {
                draftProductTagValues.current = nextProductTagValues;
                debouncedCommitProductTagPatch(changedFields, nextProductTagValues);
              }

              return (
                <FormStackColumn onSubmit={handleSubmit}>
                  <SettingsSectionCard title="Product tag details" description="Keep the product tag label clear for operators applying it across marketplace setup.">
                    <StackColumn spacing={2}>
                      <FormFieldLabel label="Name">
                        <TextField name="name" required={requiredFields.name} helperText="Use a short, recognisable product tag name." />
                      </FormFieldLabel>

                      <FormFieldLabel label="Description">
                        <TextField name="description" required={requiredFields.description} multiline rows={3} />
                      </FormFieldLabel>
                    </StackColumn>
                  </SettingsSectionCard>

                  <SettingsSectionCard title="Appearance" description="Choose a colour so this product tag is easy to recognise in lists and filters.">
                    <FormFieldLabel label="Colour">
                      <ColorPicker onChange={setSelectedColor} defaultColor={rootData.productTag?.color} />
                    </FormFieldLabel>
                  </SettingsSectionCard>
                </FormStackColumn>
              );
            }}
          />
        </StackColumn>

        <StickyReviewRail title="Product tag help" description="Changes apply wherever this product tag is already used.">
          <SettingsSectionCard title="Before saving" description="Avoid renaming tags in a way that changes their operational meaning unexpectedly.">
            <StackColumn spacing={1}>
              <SmallIconTypography label="Keep names consistent with how operators search and filter." />
              <SmallIconTypography label="Use the description for usage rules or edge cases." />
            </StackColumn>
          </SettingsSectionCard>
        </StickyReviewRail>
      </Box>
    </Box>
  );
};

const MemoEditOrganizationProductTagPage = memo(EditOrganizationProductTagPageComponent);

type RelayProps = {
  productTagId: string;
  onSaved: () => void;
  onCancel: () => void;
};

const EditOrganizationProductTagPageWithRelay = ({ productTagId, onSaved, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationProductTagDialog_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery({ productTagId }, { fetchPolicy: 'store-and-network' });
  }, [productTagId, loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoEditOrganizationProductTagPage queryReference={queryReference} productTagId={productTagId} onSaved={onSaved} onCancel={onCancel} />
    </ErrorBoundary>
  );
};

export const EditOrganizationProductTagPage = memo(EditOrganizationProductTagPageWithRelay);

export default EditOrganizationProductTagPage;
