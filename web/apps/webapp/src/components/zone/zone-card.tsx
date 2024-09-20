import { TAG_TYPE_LOCATION_ZONE } from '@/components/zone';
import type { zoneCard_LocationTagDetails$key } from '@/queries/__generated__/zoneCard_LocationTagDetails.graphql';
import type { zoneCard_Query$key } from '@/queries/__generated__/zoneCard_Query.graphql';
import type { zoneCard_addCustomerDefaultLocationTagMutation } from '@/queries/__generated__/zoneCard_addCustomerDefaultLocationTagMutation.graphql';
import type { zoneCard_deleteLocationMutation } from '@/queries/__generated__/zoneCard_deleteLocationMutation.graphql';
import type { zoneCard_removeCustomerDefaultLocationTagMutation } from '@/queries/__generated__/zoneCard_removeCustomerDefaultLocationTagMutation.graphql';
import type { zoneCard_updateZoneMutation } from '@/queries/__generated__/zoneCard_updateZoneMutation.graphql';
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
import Switch from '@mui/material/Switch';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { useTheme } from '@mui/material/styles';
import { DangerIcon, DeleteIcon, EditIcon, ZoneIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, now } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';
import ZoneName from './zone-name';

type Props = {
  rootDataRelay: zoneCard_Query$key;
  locationTagDetailsRelay: zoneCard_LocationTagDetails$key;
  connectionIds: string[];
};

interface LocationTagDetails {
  name: string;
}

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

  const [commitUpdateZone] = useMutation<zoneCard_updateZoneMutation>(graphql`
    mutation zoneCard_updateZoneMutation($input: UpdateLocationTagInput!) {
      updateLocationTag(input: $input) {
        locationTag {
          id
          name
        }
      }
    }
  `);

  const [commitDeleteZone] = useMutation<zoneCard_deleteLocationMutation>(graphql`
    mutation zoneCard_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationTagInput!) {
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

  const theme = useTheme();
  const { enqueueSnackbar } = useSnackbar();
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

    commitDeleteZone({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: locationTagDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to delete location '${locationTagDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to delete location '${locationTagDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        deleteLocationTag: {
          locationTag: {
            id: locationTagDetails.id,
            deletedAt: now().toISOString(),
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
    setEditing(false);

    commitUpdateZone({
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
          enqueueSnackbar(`Failed to update Zone '${locationTagDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update Zone '${locationTagDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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

  const handleDefaultLocationTagStateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!rootData.me) {
      return;
    }

    if (event.target.checked) {
      commitAddCustomerDefaultLocationTag({
        variables: {
          input: {
            clientMutationId: nanoid(),
            locationTagId: locationTagDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to set zone '${locationTagDetails.name}' as default. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to set zone '${locationTagDetails.name}' as default. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
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
    } else {
      commitRemoveCustomerDefaultLocationTag({
        variables: {
          input: {
            clientMutationId: nanoid(),
            locationTagId: locationTagDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to clear default zone '${locationTagDetails.name}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to clear default zone '${locationTagDetails.name}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
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
    }
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
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <ZoneIcon />
                <Typography gutterBottom variant="body1">
                  {locationTagDetails.name}
                </Typography>
              </Stack>
            }
          />

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.location.canModify && (
              <Tooltip title={'Edit zone details'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}
            {rootData.location.canModify && (
              <Tooltip title={'Delete zone'}>
                <Button size="small" color="warning" onClick={handleDeleteClick}>
                  <DeleteIcon />
                </Button>
              </Tooltip>
            )}
            <Tooltip title={isPreferredZone ? 'Remove preferred zone' : 'Set as preferred zone'}>
              <Switch checked={isPreferredZone} onChange={handleDefaultLocationTagStateChange} />
            </Tooltip>
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
              <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
                <ZoneName name="name" required={requiredFields.name} />

                <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                  <Button color="primary" variant="contained" type="submit">
                    Save
                  </Button>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                </Stack>
                <Stack sx={{ flex: 1 }} direction="row" spacing={2} />
              </Stack>
            )}
          />
        </Paper>
      )}

      <Dialog fullWidth={true} open={zoneRemoveConfirmationDialogOpen} onClose={handleCancelRemovingZoneClick}>
        <DialogTitle color={theme.palette.warning.main}>Remove zone</DialogTitle>
        <DialogContent>
          <DialogContentText
            color={theme.palette.warning.main}
          >{`Are you sure you want to remove this zone "${locationTagDetails.name}"?`}</DialogContentText>

          <DialogActions>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingZoneClick}>
              Remove
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingZoneClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(ZoneCard);
