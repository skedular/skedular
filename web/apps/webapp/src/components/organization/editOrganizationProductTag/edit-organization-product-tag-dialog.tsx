import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { RelayError, toRootError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { editOrganizationProductTagDialog_rootQuery } from '@/queries/__generated__/editOrganizationProductTagDialog_rootQuery.graphql';
import type { editOrganizationProductTagDialog_updateProductTagMutation } from '@/queries/__generated__/editOrganizationProductTagDialog_updateProductTagMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<editOrganizationProductTagDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  productTagId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
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

const productTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const EditOrganizationProductTagDialog = ({ queryReference, productTagId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationProductTagDialog_rootQuery>(RootQuery, queryReference);

  const [commitUpdateProductTag] = useMutation<editOrganizationProductTagDialog_updateProductTagMutation>(graphql`
    mutation editOrganizationProductTagDialog_updateProductTagMutation($input: UpdateProductTagInput!) @raw_response_type {
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

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ProductTagDetails) => {
    if (!rootData.productTag) {
      return;
    }

    const oldName = rootData.productTag.name;
    const toastId = themedToast(<NotificationContent content={`Updating product tag '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateProductTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: productTagId,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update product tag '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Product tag ${name} updated.`} />,
        });

        onAddClicked();
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
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Edit Product Tag" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: rootData.productTag.name,
            description: rootData.productTag.description,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Edit product tag details" />
                <SmallIconTypography label="Enter the name of the product tag to update." />

                <FormFieldLabel label="Name">
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description">
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color">
                  <ColorPicker onChange={handleColorChange} defaultColor={rootData.productTag?.color} />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Save" secondaryLabel="Cancel" />
              </FormStackColumn>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

const MemoEditOrganizationProductTagDialog = memo(EditOrganizationProductTagDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  productTagId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const EditOrganizationProductTagDialogWithRelay = ({ onReloadRequired, productTagId, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationProductTagDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        productTagId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, productTagId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      if (onReloadRequired) {
        onReloadRequired();
      }
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoEditOrganizationProductTagDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        productTagId={productTagId}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(EditOrganizationProductTagDialogWithRelay);
