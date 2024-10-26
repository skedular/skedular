import type { zoneCard_LocationTagDetails$key } from '@/queries/__generated__/zoneCard_LocationTagDetails.graphql';
import type { zoneCard_Query$key } from '@/queries/__generated__/zoneCard_Query.graphql';
import type { zoneCard_addCustomerDefaultLocationTagMutation } from '@/queries/__generated__/zoneCard_addCustomerDefaultLocationTagMutation.graphql';
import type { zoneCard_deleteLocationTagMutation } from '@/queries/__generated__/zoneCard_deleteLocationTagMutation.graphql';
import type { zoneCard_removeCustomerDefaultLocationTagMutation } from '@/queries/__generated__/zoneCard_removeCustomerDefaultLocationTagMutation.graphql';
import type { zoneCard_updateLocationTagMutation } from '@/queries/__generated__/zoneCard_updateLocationTagMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { DangerIcon, DeleteIcon, EditIcon, NotPreferredIcon, PreferredIcon, ZoneIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { TAG_TYPE_LOCATION_ZONE, ZoneName } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  rootDataRelay: zoneCard_Query$key;
  locationTagDetailsRelay: zoneCard_LocationTagDetails$key;
  connectionIds: string[];
};

type LocationTagDetails = {
  name: string;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
});

const ZoneCard = ({ rootDataRelay, locationTagDetailsRelay, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment zoneCard_Query on Query {
        me {
          id
          preferredZones {
            uniqueId
          }
        }
        location(id: $locationId) {
          canModify
        }
      }
    `,
    rootDataRelay,
  );

  const locationTagDetails = useFragment(
    graphql`
      fragment zoneCard_LocationTagDetails on LocationTagDetails {
        id
        name
      }
    `,
    locationTagDetailsRelay,
  );

  const [commitUpdateLocationTag] = useMutation<zoneCard_updateLocationTagMutation>(graphql`
    mutation zoneCard_updateLocationTagMutation($input: UpdateLocationTagInput!) {
      updateLocationTag(input: $input) {
        locationTag {
          id
          name
        }
      }
    }
  `);

  const [commitDeleteLocationTag] = useMutation<zoneCard_deleteLocationTagMutation>(graphql`
    mutation zoneCard_deleteLocationTagMutation($connectionIds: [ID!]!, $input: DeleteLocationTagInput!) {
      deleteLocationTag(input: $input) {
        locationTag {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocationTag] = useMutation<zoneCard_addCustomerDefaultLocationTagMutation>(graphql`
    mutation zoneCard_addCustomerDefaultLocationTagMutation($input: AddCustomerDefaultLocationTagInput!) {
      addCustomerDefaultLocationTag(input: $input) {
        customer {
          id
          preferredZones {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultLocationTag] = useMutation<zoneCard_removeCustomerDefaultLocationTagMutation>(graphql`
    mutation zoneCard_removeCustomerDefaultLocationTagMutation($input: RemoveCustomerDefaultLocationTagInput!) {
      removeCustomerDefaultLocationTag(input: $input) {
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
    () => !!rootData.me?.preferredZones.find((zone) => zone.uniqueId === locationTagDetails.id),
    [rootData.me?.preferredZones, locationTagDetails.id],
  );

  const handleDeleteClick = () => {
    setZoneRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingZoneClick = () => {
    setZoneRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingZoneClick = () => {
    setZoneRemoveConfirmationDialogOpen(false);

    const toastId = themedToast(<NotificationContent content={`Removing zone '${locationTagDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteLocationTag({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: locationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove zone '${locationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone ${locationTagDetails.name} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove zone '${locationTagDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        deleteLocationTag: {
          locationTag: {
            id: locationTagDetails.id,
          },
        },
      },
    });
  };

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  const handleSaveClick = ({ name }: LocationTagDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating zone '${locationTagDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateLocationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: locationTagDetails.id,
          name,
          tagType: TAG_TYPE_LOCATION_ZONE,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update zone '${locationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
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
          render: <NotificationContent content={`Failed to update zone '${locationTagDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateLocationTag: {
          locationTag: {
            id: locationTagDetails.id,
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
      <NotificationContent content={`Setting zone '${locationTagDetails.name}' as your preferred zone...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultLocationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationTagId: locationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to set zone '${locationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${locationTagDetails.name}' has been set as the preferred zone.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to set zone '${locationTagDetails.name}' as your preferred zone. Error: ${error.message}.`} />
          ),
        });
      },
      optimisticResponse: {
        addCustomerDefaultLocationTag: {
          customer: {
            id: rootData.me.id,
            preferredZones: rootData.me.preferredZones.concat([
              {
                uniqueId: locationTagDetails.id,
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
      <NotificationContent content={`Removing zone '${locationTagDetails.name}' as your preferred zone...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultLocationTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationTagId: locationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the zone '${locationTagDetails.name}' as your preferred zone. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone '${locationTagDetails.name}' has been removed as your preferred zone.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to remove the zone '${locationTagDetails.name}' as your preferred zone. Error: ${error.message}.`}
            />
          ),
        });
      },
      optimisticResponse: {
        removeCustomerDefaultLocationTag: {
          customer: {
            id: rootData.me.id,
            preferredZones: rootData.me.preferredZones.filter(({ uniqueId }) => uniqueId === locationTagDetails.id),
          },
        },
      },
    });
  };

  if (!rootData.location) {
    return <></>;
  }

  return (
    <>
      {!editing && (
        <Card elevation={24} sx={{ minWidth: 200, height: '100%' }}>
          <CardHeader
            title={
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <ZoneIcon />
                <Typography variant="body1">{locationTagDetails.name}</Typography>
              </Stack>
            }
          />

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.location.canModify && (
              <Tooltip title={'Edit zone'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}
            {rootData.location.canModify && (
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
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              name: locationTagDetails.name,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                <ZoneName name="name" required={requiredFields.name} />

                <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Update
                  </Button>
                </Stack>
              </Stack>
            )}
          />
        </Paper>
      )}

      <Dialog TransitionComponent={DialogTransition} open={zoneRemoveConfirmationDialogOpen} onClose={handleCancelRemovingZoneClick}>
        <DialogTitle>Remove zone</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove the zone "${locationTagDetails.name}"?`}</DialogContentText>
          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingZoneClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingZoneClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(ZoneCard);
