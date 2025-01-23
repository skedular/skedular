import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import {
  ColorPicker,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  SmallIconTypography,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { editOrganizationCustomTagDialog_rootQuery } from './__generated__/editOrganizationCustomTagDialog_rootQuery.graphql';
import type { editOrganizationCustomTagDialog_updateCustomTagMutation } from './__generated__/editOrganizationCustomTagDialog_updateCustomTagMutation.graphql';

type Props = {
  queryReference: PreloadedQuery<editOrganizationCustomTagDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  customTagId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationCustomTagDialog_rootQuery($customTagId: String!) {
    customTag(id: $customTagId) {
      id
      name
      description
      color
    }
  }
`;

type CustomTagDetails = {
  name: string;
  description: string;
};

const customTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const EditOrganizationCustomTagDialog = ({ queryReference, customTagId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationCustomTagDialog_rootQuery>(RootQuery, queryReference);

  const [commitUpdateCustomTag] = useMutation<editOrganizationCustomTagDialog_updateCustomTagMutation>(graphql`
    mutation editOrganizationCustomTagDialog_updateCustomTagMutation($input: UpdateCustomTagInput!) @raw_response_type {
      updateCustomTag(input: $input) {
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
  const validate = makeValidate(customTagSchema);
  const requiredFields = makeRequired(customTagSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.customTag?.color);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: CustomTagDetails) => {
    if (!rootData.customTag) {
      return;
    }

    const oldName = rootData.customTag.name;
    const toastId = themedToast(<NotificationContent content={`Updating tag '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateCustomTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: customTagId,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update tag '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag ${name} updated.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update tag '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateCustomTag: {
          organizationTag: {
            id: customTagId,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  if (!rootData.customTag) {
    return <></>;
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Edit Tag" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: rootData.customTag.name,
            description: rootData.customTag.description,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Edit tag details" />
                <SmallIconTypography label="Enter the name of the tag to update." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color" useWiderSpace>
                  <ColorPicker onChange={handleColorChange} defaultColor={rootData.customTag?.color} />
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

const MemoEditOrganizationCustomTagDialog = memo(EditOrganizationCustomTagDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  customTagId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const EditOrganizationCustomTagDialogWithRelay = ({ onReloadRequired, customTagId, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationCustomTagDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        customTagId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, customTagId]);

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
      <MemoEditOrganizationCustomTagDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        customTagId={customTagId}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(EditOrganizationCustomTagDialogWithRelay);
