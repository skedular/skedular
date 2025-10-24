import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { editOrganizationLocationTagDialog_rootQuery } from '@/queries/__generated__/editOrganizationLocationTagDialog_rootQuery.graphql';
import type { editOrganizationLocationTagDialog_updateLocationTagMutation } from '@/queries/__generated__/editOrganizationLocationTagDialog_updateLocationTagMutation.graphql';
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
  queryReference: PreloadedQuery<editOrganizationLocationTagDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  locationTagId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationLocationTagDialog_rootQuery($locationTagId: String!) {
    locationTag(id: $locationTagId) {
      id
      name
      description
      color
    }
  }
`;

type LocationTagDetails = {
  name: string;
  description: string | null | undefined;
};

const locationTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const EditOrganizationLocationTagDialog = ({ queryReference, locationTagId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationLocationTagDialog_rootQuery>(RootQuery, queryReference);

  const [commitUpdateLocationTag] = useMutation<editOrganizationLocationTagDialog_updateLocationTagMutation>(graphql`
    mutation editOrganizationLocationTagDialog_updateLocationTagMutation($input: UpdateLocationTagInput!) @raw_response_type {
      updateLocationTag(input: $input) {
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
  const validate = makeValidate(locationTagSchema);
  const requiredFields = makeRequired(locationTagSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.locationTag?.color);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: LocationTagDetails) => {
    if (!rootData.locationTag) {
      return;
    }

    const oldName = rootData.locationTag.name;
    const toastId = themedToast(<NotificationContent content={`Updating location tag '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateLocationTag({
      variables: {
        input: {
          clientMutationId: uuid(),
          id: locationTagId,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update location tag '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location tag ${name} updated.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update location tag '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocationTag: {
          organizationTag: {
            id: locationTagId,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  if (!rootData.locationTag) {
    return null;
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Edit Location Tag" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: rootData.locationTag.name,
            description: rootData.locationTag.description,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Edit location tag details" />
                <SmallIconTypography label="Enter the name of the location tag to update." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color" useWiderSpace>
                  <ColorPicker onChange={handleColorChange} defaultColor={rootData.locationTag?.color} />
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

const MemoEditOrganizationLocationTagDialog = memo(EditOrganizationLocationTagDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  locationTagId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const EditOrganizationLocationTagDialogWithRelay = ({ onReloadRequired, locationTagId, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationLocationTagDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationTagId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationTagId]);

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
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoEditOrganizationLocationTagDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        locationTagId={locationTagId}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(EditOrganizationLocationTagDialogWithRelay);
