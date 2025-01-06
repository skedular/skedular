import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Paper from '@mui/material/Paper';
import Tooltip from '@mui/material/Tooltip';
import { BodyIconTypography, DefaultDialogTitle, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { DeleteIcon, EditIcon, NotPreferredIcon, PreferredIcon, ZoneIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { zoneCard_OrganizationTagDetails$key } from './__generated__/zoneCard_OrganizationTagDetails.graphql';
import type { zoneCard_Query$key } from './__generated__/zoneCard_Query.graphql';
import type { zoneCard_addCustomerDefaultOrganizationTagMutation } from './__generated__/zoneCard_addCustomerDefaultOrganizationTagMutation.graphql';
import type { zoneCard_deleteZoneMutation } from './__generated__/zoneCard_deleteZoneMutation.graphql';
import type { zoneCard_removeCustomerDefaultOrganizationTagMutation } from './__generated__/zoneCard_removeCustomerDefaultOrganizationTagMutation.graphql';
import type { zoneCard_updateZoneMutation } from './__generated__/zoneCard_updateZoneMutation.graphql';

type Props = {
  rootDataRelay: zoneCard_Query$key;
  organizationTagDetailsRelay: zoneCard_OrganizationTagDetails$key;
  connectionIds: string[];
};

type OrganizationTagDetails = {
  name: string;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
});

const ZoneCard = ({ rootDataRelay, organizationTagDetailsRelay, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment zoneCard_Query on Query {
        me {
          id
          preferredZones {
            uniqueId
          }
        }
        organization(id: $organizationId) {
          canModify
        }
      }
    `,
    rootDataRelay,
  );

  const organizationTagDetails = useFragment(
    graphql`
      fragment zoneCard_OrganizationTagDetails on OrganizationTagDetails {
        id
        name
      }
    `,
    organizationTagDetailsRelay,
  );

  const [commitUpdateZone] = useMutation<zoneCard_updateZoneMutation>(graphql`
    mutation zoneCard_updateZoneMutation($input: UpdateZoneInput!) {
      updateZone(input: $input) {
        organizationTag {
          id
          name
        }
      }
    }
  `);

  const [commitDeleteZone] = useMutation<zoneCard_deleteZoneMutation>(graphql`
    mutation zoneCard_deleteZoneMutation($connectionIds: [ID!]!, $input: DeleteZoneInput!) {
      deleteZone(input: $input) {
        organizationTag {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultOrganizationTag] = useMutation<zoneCard_addCustomerDefaultOrganizationTagMutation>(graphql`
    mutation zoneCard_addCustomerDefaultOrganizationTagMutation($input: AddCustomerDefaultOrganizationTagInput!) {
      addCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultOrganizationTag] = useMutation<zoneCard_removeCustomerDefaultOrganizationTagMutation>(graphql`
    mutation zoneCard_removeCustomerDefaultOrganizationTagMutation($input: RemoveCustomerDefaultOrganizationTagInput!) {
      removeCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [zoneRemoveConfirmationDialogOpen, setZoneRemoveConfirmationDialogOpen] = useState(false);
  const isPreferredZone = useMemo(
    () => !!rootData.me?.preferredZones.find((zone) => zone.uniqueId === organizationTagDetails.id),
    [rootData.me?.preferredZones, organizationTagDetails.id],
  );

  const handleDeleteClick = () => {
    setZoneRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingZoneClick = () => {
    setZoneRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingZoneClick = () => {
    setZoneRemoveConfirmationDialogOpen(false);

    const toastId = themedToast(<NotificationContent content={`Removing zone '${organizationTagDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteZone({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zone '${organizationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone ${organizationTagDetails.name} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zone '${organizationTagDetails.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const handleSaveClick = ({ name }: OrganizationTagDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating zone '${organizationTagDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateZone({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organizationTagDetails.id,
          name,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update zone '${organizationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone ${name} updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update zone '${organizationTagDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationTag: {
          organizationTag: {
            id: organizationTagDetails.id,
            name,
          },
        },
      },
    });
  };

  const handleSetAsPreferredZoneClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting zone '${organizationTagDetails.name}' as your preferred zone...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been set as the preferred zone.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to set zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />
          ),
        });
      },
      optimisticResponse: {
        addCustomerDefaultOrganizationTag: {
          customer: {
            id: rootData.me.id,
            preferredZones: rootData.me.preferredZones.concat([
              {
                uniqueId: organizationTagDetails.id,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredZoneClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing zone '${organizationTagDetails.name}' as your preferred zone...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultOrganizationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationTagId: organizationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${organizationTagDetails.name}' has been removed as your preferred zone.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to remove the zone '${organizationTagDetails.name}' as your preferred zone. Error: ${error.message}.`}
            />
          ),
        });
      },
      optimisticResponse: {
        removeCustomerDefaultOrganizationTag: {
          customer: {
            id: rootData.me.id,
            preferredZones: rootData.me.preferredZones.filter(({ uniqueId }) => uniqueId === organizationTagDetails.id),
          },
        },
      },
    });
  };

  if (!rootData.organization) {
    return <></>;
  }

  return (
    <>
      {!editing && (
        <Card sx={{ minWidth: 200, height: '100%' }}>
          <CardHeader title={<BodyIconTypography label={organizationTagDetails.name} startElement={<ZoneIcon />} invertDefaultColor />} />

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.organization.canModify && (
              <Tooltip title={'Edit zone'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}
            {rootData.organization.canModify && (
              <Tooltip title={'Remove zone'}>
                <Button size="small" color="warning" onClick={handleDeleteClick}>
                  <DeleteIcon />
                </Button>
              </Tooltip>
            )}
            {isPreferredZone && (
              <Tooltip title={'Remove as preferred zone'}>
                <Button size="small" color="primary" onClick={handleRemoveAsPreferredZoneClicked}>
                  <PreferredIcon />
                </Button>
              </Tooltip>
            )}
            {!isPreferredZone && (
              <Tooltip title={'Set as preferred zone'}>
                <Button size="small" color="primary" onClick={handleSetAsPreferredZoneClicked}>
                  <NotPreferredIcon />
                </Button>
              </Tooltip>
            )}
          </CardActions>
        </Card>
      )}

      {editing && (
        <Paper sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              name: organizationTagDetails.name,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} helperText="Add your zone name" />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </Paper>
      )}

      <Dialog TransitionComponent={DialogTransition} open={zoneRemoveConfirmationDialogOpen} onClose={handleCancelRemovingZoneClick}>
        <DefaultDialogTitle title="Remove Zone" />
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove the zone "${organizationTagDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingZoneClick}
            onSecondaryClicked={handleCancelRemovingZoneClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(ZoneCard);
