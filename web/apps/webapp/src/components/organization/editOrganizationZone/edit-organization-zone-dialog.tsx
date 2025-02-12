import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { Loading } from '@/components/loading';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { editOrganizationZoneDialog_rootQuery } from '@/queries/__generated__/editOrganizationZoneDialog_rootQuery.graphql';
import type { editOrganizationZoneDialog_updateZoneMutation } from '@/queries/__generated__/editOrganizationZoneDialog_updateZoneMutation.graphql';
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
  queryReference: PreloadedQuery<editOrganizationZoneDialog_rootQuery, Record<string, unknown>>;
  onReloadRequired?: () => void;
  zoneId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const RootQuery = graphql`
  query editOrganizationZoneDialog_rootQuery($zoneId: String!) {
    zone(id: $zoneId) {
      id
      name
      description
      color
    }
  }
`;

type ZoneDetails = {
  name: string;
  description: string;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
  description: string().nullable(),
});

const EditOrganizationZoneDialog = ({ queryReference, zoneId, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const rootData = usePreloadedQuery<editOrganizationZoneDialog_rootQuery>(RootQuery, queryReference);

  const [commitUpdateZone] = useMutation<editOrganizationZoneDialog_updateZoneMutation>(graphql`
    mutation editOrganizationZoneDialog_updateZoneMutation($input: UpdateZoneInput!) @raw_response_type {
      updateZone(input: $input) {
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
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [selectedColor, setSelectedColor] = useState(rootData.zone?.color);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ZoneDetails) => {
    if (!rootData.zone) {
      return;
    }

    const oldName = rootData.zone.name;
    const toastId = themedToast(<NotificationContent content={`Updating zone '${oldName}'...`} />, infoNotificationOptions);

    commitUpdateZone({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: zoneId,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update zone '${oldName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone ${name} updateed.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update zone '${oldName}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateZone: {
          organizationTag: {
            id: zoneId,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  if (!rootData.zone) {
    return <></>;
  }

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Edit Zone" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: rootData.zone.name,
            description: rootData.zone.description,
          }}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Edit zone details" />
                <SmallIconTypography label="Enter the name of the zone to update." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color" useWiderSpace>
                  <ColorPicker onChange={handleColorChange} defaultColor={rootData.zone?.color} />
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

const MemoEditOrganizationZoneDialog = memo(EditOrganizationZoneDialog);

type RelayProps = {
  onReloadRequired?: () => void;
  zoneId: string;
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

const EditOrganizationZoneDialogWithRelay = ({ onReloadRequired, zoneId, isDialogOpen, onAddClicked, onCancel }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<editOrganizationZoneDialog_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        zoneId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, zoneId]);

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
      <MemoEditOrganizationZoneDialog
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        zoneId={zoneId}
        isDialogOpen={isDialogOpen}
        onAddClicked={onAddClicked}
        onCancel={onCancel}
      />
    </ErrorBoundary>
  );
};

export default memo(EditOrganizationZoneDialogWithRelay);
