import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { editOrganizationResourceTypeDialog_rootQuery } from '@/queries/__generated__/editOrganizationResourceTypeDialog_rootQuery.graphql';
import type { editOrganizationResourceTypeDialog_updateResourceTypeMutation } from '@/queries/__generated__/editOrganizationResourceTypeDialog_updateResourceTypeMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<editOrganizationResourceTypeDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  resourceTypeId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationResourceTypeDialog_rootQuery($resourceTypeId: String!) {
    resourceType(id: $resourceTypeId) {
      id
      name
      description
      color
      systemType
    }
  }
`;

type ResourceTypeDetails = {
  name: string;
  description: string;
};

const resourceTypeSchema = object({
  name: string().required('Resource Type name is required'),
  description: string().nullable(),
});

const EditOrganizationResourceTypeDialog = ({ queryReference, resourceTypeId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationResourceTypeDialog_rootQuery>(RootQuery, queryReference);

  const [commitUpdateResourceType] = useMutation<editOrganizationResourceTypeDialog_updateResourceTypeMutation>(graphql`
    mutation editOrganizationResourceTypeDialog_updateResourceTypeMutation($input: UpdateResourceTypeInput!) @raw_response_type {
      updateResourceType(input: $input) {
        organizationResourceType {
          id
          name
          description
          color
          systemType
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(resourceTypeSchema);
  const requiredFields = makeRequired(resourceTypeSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.resourceType?.color);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ResourceTypeDetails) => {
    if (!rootData.resourceType) {
      return;
    }

    const oldName = rootData.resourceType.name;
    const toastId = themedToast(<NotificationContent content={`Updating resource type '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateResourceType({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: resourceTypeId,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update resource type '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Resource type ${name} updateed.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update resource type '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateResourceType: {
          organizationResourceType: {
            id: resourceTypeId,
            name,
            description,
            color: selectedColor,
            systemType: null,
          },
        },
      },
    });
  };

  if (!rootData.resourceType) {
    return <></>;
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Edit Resource Type" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: rootData.resourceType.name,
            description: rootData.resourceType.description,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Edit resource type details" />
                <SmallIconTypography label="Enter the name of the resource type to update." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color" useWiderSpace>
                  <ColorPicker onChange={handleColorChange} defaultColor={rootData.resourceType?.color} />
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

const MemoEditOrganizationResourceTypeDialog = memo(EditOrganizationResourceTypeDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  resourceTypeId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const EditOrganizationResourceTypeDialogWithRelay = ({ onReloadRequired, resourceTypeId, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationResourceTypeDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        resourceTypeId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, resourceTypeId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      if (onReloadRequired) {
        onReloadRequired();
      }
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoEditOrganizationResourceTypeDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        resourceTypeId={resourceTypeId}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(EditOrganizationResourceTypeDialogWithRelay);
