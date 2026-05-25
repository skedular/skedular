import {
  ColorPicker,
  EditorActionBar,
  FormFieldLabel,
  FormStackColumn,
  PageHeaderPanel,
  SettingsSectionCard,
  SmallIconTypography,
  StackColumn,
  StickyReviewRail,
} from '@skedular/ui';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { PaletteModeContext } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { editOrganizationProductTagDialog_rootQuery } from '@/queries/__generated__/editOrganizationProductTagDialog_rootQuery.graphql';
import type { editOrganizationProductTagDialog_updateProductTagMutation } from '@/queries/__generated__/editOrganizationProductTagDialog_updateProductTagMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
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

const productTagSchema = object({
  name: string().required('Product tag name is required'),
  description: string().nullable(),
});

const EditOrganizationProductTagPageComponent = ({ queryReference, productTagId, onSaved, onCancel }: Props) => {
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

  const handleAddClick = ({ name, description }: ProductTagDetails) => {
    if (!rootData.productTag) {
      return;
    }

    const oldName = rootData.productTag.name;
    const fieldsToUpdate: TagPatchField[] = [];
    if (rootData.productTag.name !== name) {
      fieldsToUpdate.push('NAME');
    }
    if (rootData.productTag.description !== description) {
      fieldsToUpdate.push('DESCRIPTION');
    }
    if (rootData.productTag.color !== selectedColor) {
      fieldsToUpdate.push('COLOR');
    }
    if (fieldsToUpdate.length === 0) {
      onSaved();
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating product tag '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateProductTagPatch({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: productTagId,
          fieldsToUpdate,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update product tag '${oldName}'. Error: ${getRelayErrorMessage(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product tag ${name} updated.`} />,
        });

        onSaved();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update product tag '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateProductTag: {
          organizationTag: {
            id: productTagId,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  if (!rootData.productTag) {
    return null;
  }

  return (
    <Box sx={{ px: { xs: 2, md: 3 }, py: 3 }}>
      <Box sx={{ maxWidth: 1320, mx: 'auto', display: 'grid', gridTemplateColumns: { xs: 'minmax(0, 1fr)', xl: 'minmax(0, 2fr) 320px' }, gap: 2 }}>
        <StackColumn spacing={2.5} sx={{ minWidth: 0 }}>
          <PageHeaderPanel title="Edit product tag" description="Update the product tag name, description, and colour used across marketplace products and resources." />

          <Form
            onSubmit={handleAddClick}
            initialValues={{ name: rootData.productTag.name, description: rootData.productTag.description }}
            validate={validate}
            render={({ handleSubmit }) => (
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

                <EditorActionBar
                  secondaryActions={
                    <Button type="button" variant="text" onClick={onCancel} sx={{ textTransform: 'none' }}>
                      Cancel
                    </Button>
                  }
                  primaryAction="Save product tag"
                />
              </FormStackColumn>
            )}
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
