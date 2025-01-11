import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import {
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
import type { editOrganizationDeskTypeDialog_rootQuery } from './__generated__/editOrganizationDeskTypeDialog_rootQuery.graphql';
import type { editOrganizationDeskTypeDialog_updateDeskTypeMutation } from './__generated__/editOrganizationDeskTypeDialog_updateDeskTypeMutation.graphql';

type Props = {
  queryReference: PreloadedQuery<editOrganizationDeskTypeDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  deskTypeId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationDeskTypeDialog_rootQuery($deskTypeId: String!) {
    deskType(id: $deskTypeId) {
      id
      name
      description
    }
  }
`;

type DeskTypeDetails = {
  name: string;
  description: string;
};

const deskTypeSchema = object({
  name: string().required('Desk type name is required'),
  description: string().nullable(),
});

const EditOrganizationDeskTypeDialog = ({ queryReference, deskTypeId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationDeskTypeDialog_rootQuery>(RootQuery, queryReference);

  const [commitUpdateDeskType] = useMutation<editOrganizationDeskTypeDialog_updateDeskTypeMutation>(graphql`
    mutation editOrganizationDeskTypeDialog_updateDeskTypeMutation($input: UpdateDeskTypeInput!) @raw_response_type {
      updateDeskType(input: $input) {
        organizationTag {
          id
          name
          description
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskTypeSchema);
  const requiredFields = makeRequired(deskTypeSchema);

  const handleAddClick = ({ name, description }: DeskTypeDetails) => {
    if (!rootData.deskType) {
      return;
    }

    const oldName = rootData.deskType.name;
    const toastId = themedToast(<NotificationContent content={`Updating desk type '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateDeskType({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: deskTypeId,
          name,
          description,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update desk type '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type ${name} updated.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update desk type '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDeskType: {
          organizationTag: {
            id: deskTypeId,
            name,
            description,
          },
        },
      },
    });
  };

  if (!rootData.deskType) {
    return <></>;
  }

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DefaultDialogTitle title="Edit Desk Type" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: rootData.deskType.name,
            description: rootData.deskType.description,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Edit desk type details" />
                <SmallIconTypography label="Enter the name of the desk type to update." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
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

const MemoEditOrganizationDeskTypeDialog = memo(EditOrganizationDeskTypeDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  deskTypeId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const EditOrganizationDeskTypeDialogWithRelay = ({ onReloadRequired, deskTypeId, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationDeskTypeDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        deskTypeId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, deskTypeId]);

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
      <MemoEditOrganizationDeskTypeDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        deskTypeId={deskTypeId}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(EditOrganizationDeskTypeDialogWithRelay);
