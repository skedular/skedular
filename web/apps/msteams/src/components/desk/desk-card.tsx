import MoreVertIcon from '@mui/icons-material/MoreVert';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { useTheme } from '@mui/material/styles';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { DangerIcon, DeleteIcon, DeskIcon, EditIcon, InfoIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { ZonesLine } from 'components/zone';
import { makeRequired, makeValidate } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import { array, object, string } from 'yup';
import type { deskCard_DeskDetails$key } from './__generated__/deskCard_DeskDetails.graphql';
import type { deskCard_addCustomerDefaultDeskMutation } from './__generated__/deskCard_addCustomerDefaultDeskMutation.graphql';
import type { deskCard_deleteLocationMutation } from './__generated__/deskCard_deleteLocationMutation.graphql';
import type { deskCard_query$key } from './__generated__/deskCard_query.graphql';
import type { deskCard_removeCustomerDefaultDeskMutation } from './__generated__/deskCard_removeCustomerDefaultDeskMutation.graphql';
import type { deskCard_updateDeskMutation } from './__generated__/deskCard_updateDeskMutation.graphql';
import type { deskMultipleChoicesZones_query$key } from './__generated__/deskMultipleChoicesZones_query.graphql';
import DeskMultipleChoicesZones from './desk-multiple-choices-zones';
import DeskName from './desk-name';

type Props = {
  rootDataRelay: deskCard_query$key;
  deskDetailsRelay: deskCard_DeskDetails$key;
  deskMultipleChoicesZonesData: deskMultipleChoicesZones_query$key;
  connectionIds: string[];
  customerDetails: CustomerDetails | null;
};

interface CustomerDetails {
  familyName: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  name: string | null | undefined;
  photoUrl: string | null | undefined;
  uniqueId: string;
}

interface DeskDetails {
  name: string;
  locationTagIds: string[];
}

const deskSchema = object({
  name: string().required('Desk name is required'),
  locationTagIds: array().nullable(),
});

enum MoreActionsMenuOptionType {
  ActivateDesk,
  DeactivateDesk,
  EnableDeskApprovalRequirement,
  RemoveDeskApprovalRequirement,
}

interface MoreActionsMenuItemType {
  id: MoreActionsMenuOptionType;
  label: String;
}

const moreActionsMenuAllOptions = {
  [MoreActionsMenuOptionType.ActivateDesk]:{
    id: MoreActionsMenuOptionType.ActivateDesk,
    label: 'Activate desk',
  },
  [MoreActionsMenuOptionType.DeactivateDesk]:{
    id: MoreActionsMenuOptionType.DeactivateDesk,
    label: 'Dectivate desk',
  },
  [MoreActionsMenuOptionType.EnableDeskApprovalRequirement]:{
    id: MoreActionsMenuOptionType.EnableDeskApprovalRequirement,
    label: 'Enable desk approval requirement',
  },
  [MoreActionsMenuOptionType.RemoveDeskApprovalRequirement]:{
    id: MoreActionsMenuOptionType.RemoveDeskApprovalRequirement,
    label: 'Remove desk approval requirement',
  },
};

const DeskCard = ({ rootDataRelay, deskDetailsRelay, deskMultipleChoicesZonesData, connectionIds, customerDetails }: Props) => {
  const rootData = useFragment<deskCard_query$key>(
    graphql`
      fragment deskCard_query on Query {
        me {
          id
          preferredDesks {
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

  const deskDetails = useFragment(
    graphql`
      fragment deskCard_DeskDetails on DeskDetails {
        id
        name
        deactivated
        requireBookingApproval
        locationTags {
          id
          name
        }
      }
    `,
    deskDetailsRelay,
  );

  const [commitUpdateDesk] = useMutation<deskCard_updateDeskMutation>(graphql`
    mutation deskCard_updateDeskMutation($input: UpdateDeskInput!) @raw_response_type {
      updateDesk(input: $input) {
        desk {
          id
          name
          deactivated
          requireBookingApproval
          locationTags {
            id
            name
          }
        }
      }
    }
  `);

  const [commitDeleteDesk] = useMutation<deskCard_deleteLocationMutation>(graphql`
    mutation deskCard_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteDeskInput!) {
      deleteDesk(input: $input) {
        desk {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultDesk] = useMutation<deskCard_addCustomerDefaultDeskMutation>(graphql`
    mutation deskCard_addCustomerDefaultDeskMutation($input: AddCustomerDefaultDeskInput!) {
      addCustomerDefaultDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultDesk] = useMutation<deskCard_removeCustomerDefaultDeskMutation>(graphql`
    mutation deskCard_removeCustomerDefaultDeskMutation($input: RemoveCustomerDefaultDeskInput!) {
      removeCustomerDefaultDesk(input: $input) {
        customer {
          id
          preferredDesks {
            uniqueId
          }
        }
      }
    }
  `);

  const theme = useTheme();
  const { enqueueSnackbar } = useSnackbar();
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [deskRemoveConfirmationDialogOpen, setDeskRemoveConfirmationDialogOpen] = useState(false);
  const [deskDeactivateConfirmationDialogOpen, setDeskDeactivateConfirmationDialogOpen] = useState(false);
  const [deskActivateConfirmationDialogOpen, setDeskActivateConfirmationDialogOpen] = useState(false);
  const [setDeskApprovalRequirementConfirmationDialogOpen, setSetDeskApprovalRequirementConfirmationDialogOpen] = useState(false);
  const [removeDeskApprovalRequirementConfirmationDialogOpen, setRemoveDeskApprovalRequirementConfirmationDialogOpen] = useState(false);
  const isPreferredDesk = useMemo(
    () => !!rootData.me?.preferredDesks.find((desk) => desk.uniqueId === deskDetails.id),
    [rootData.me?.preferredDesks, deskDetails.id],
  );

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.ActivateDesk:
        handleActivateDeskConfirmationDialogClick();

        break;

      case MoreActionsMenuOptionType.DeactivateDesk:
        handleDeactivateDeskConfirmationDialogClick();

        break;

      case MoreActionsMenuOptionType.EnableDeskApprovalRequirement:
        handleSetDeskApprovalRequirementConfirmationDialogClick();

        break;

      case MoreActionsMenuOptionType.RemoveDeskApprovalRequirement:
        handleRemoveDeskApprovalRequirementConfirmationDialogClick();

        break;
    }
  };

  const handleDeleteConfirmationDialogClick = () => {
    setDeskRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingDeskClick = () => {
    setDeskRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingDeskClick = () => {
    setDeskRemoveConfirmationDialogOpen(false);

    commitDeleteDesk({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: uuidv4(),
          id: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to delete desk '${deskDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to delete desk '${deskDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  const handleDeactivateDeskConfirmationDialogClick = () => {
    setDeskDeactivateConfirmationDialogOpen(true);
  };

  const handleCancelDeactivateDeskClick = () => {
    setDeskDeactivateConfirmationDialogOpen(false);
  };

  const handleConfirmDeactivatingDeskClick = () => {
    setDeskDeactivateConfirmationDialogOpen(false);

    const locationTagIds = deskDetails.locationTags.map(({ id }) => id);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: true,
          requireBookingApproval: deskDetails.requireBookingApproval,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to deactivate Desk '${deskDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to deactivate Desk '${deskDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: true,
            requireBookingApproval: deskDetails.requireBookingApproval,
            locationTags: deskDetails.locationTags,
          },
        },
      },
    });
  };

  const handleActivateDeskConfirmationDialogClick = () => {
    setDeskActivateConfirmationDialogOpen(true);
  };

  const handleCancelActivateDeskClick = () => {
    setDeskActivateConfirmationDialogOpen(false);
  };

  const handleConfirmActivatingDeskClick = () => {
    setDeskActivateConfirmationDialogOpen(false);

    const locationTagIds = deskDetails.locationTags.map(({ id }) => id);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: false,
          requireBookingApproval: deskDetails.requireBookingApproval,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to deactivate Desk '${deskDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to deactivate Desk '${deskDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: false,
            requireBookingApproval: deskDetails.requireBookingApproval,
            locationTags: deskDetails.locationTags,
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

  const handleSaveClick = ({ name, locationTagIds }: DeskDetails) => {
    setEditing(false);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: deskDetails.id,
          name,
          deactivated: deskDetails.deactivated,
          requireBookingApproval: deskDetails.requireBookingApproval,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update Desk '${deskDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update Desk '${deskDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name,
            deactivated: deskDetails.deactivated,
            requireBookingApproval: deskDetails.requireBookingApproval,
            locationTags: [],
          },
        },
      },
    });
  };

  const handleDefaultDeskStateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!rootData.me) {
      return;
    }

    if (event.target.checked) {
      commitAddCustomerDefaultDesk({
        variables: {
          input: {
            clientMutationId: uuidv4(),
            deskId: deskDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to set desk '${deskDetails.name}' as preferred. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to set desk '${deskDetails.name}' as preferred. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          addCustomerDefaultDesk: {
            customer: {
              id: rootData.me.id,
              preferredDesks: rootData.me.preferredDesks.concat([
                {
                  uniqueId: deskDetails.id,
                },
              ]),
            },
          },
        },
      });
    } else {
      commitRemoveCustomerDefaultDesk({
        variables: {
          input: {
            clientMutationId: uuidv4(),
            deskId: deskDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to clear preferred desk '${deskDetails.name}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to clear preferred desk '${deskDetails.name}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          removeCustomerDefaultDesk: {
            customer: {
              id: rootData.me.id,
              preferredDesks: rootData.me.preferredDesks.filter(({ uniqueId }) => uniqueId === deskDetails.id),
            },
          },
        },
      });
    }
  };

  const handleSetDeskApprovalRequirementConfirmationDialogClick = () => {
    setSetDeskApprovalRequirementConfirmationDialogOpen(true);
  };

  const handleCancelSetDeskApprovalRequirementClick = () => {
    setSetDeskApprovalRequirementConfirmationDialogOpen(false);
  };

  const handleSetDeskApprovalRequirementClick = () => {
    setSetDeskApprovalRequirementConfirmationDialogOpen(false);

    const locationTagIds = deskDetails.locationTags.map(({ id }) => id);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: deskDetails.deactivated,
          requireBookingApproval: true,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to set Desk '${deskDetails.name}' require approval property. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to set Desk '${deskDetails.name}' require approval property. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: deskDetails.deactivated,
            requireBookingApproval: true,
            locationTags: deskDetails.locationTags,
          },
        },
      },
    });
  };

  const handleRemoveDeskApprovalRequirementConfirmationDialogClick = () => {
    setRemoveDeskApprovalRequirementConfirmationDialogOpen(true);
  };

  const handleCancelRemoveDeskApprovalRequirementDeskClick = () => {
    setRemoveDeskApprovalRequirementConfirmationDialogOpen(false);
  };

  const handleConfirmRemoveDeskApprovalRequirementDeskClick = () => {
    setRemoveDeskApprovalRequirementConfirmationDialogOpen(false);

    const locationTagIds = deskDetails.locationTags.map(({ id }) => id);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: deskDetails.deactivated,
          requireBookingApproval: false,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to unset Desk '${deskDetails.name}' require approval property. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to unset Desk '${deskDetails.name}' require approval property. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: deskDetails.deactivated,
            requireBookingApproval: false,
            locationTags: deskDetails.locationTags,
          },
        },
      },
    });
  };

  if (!rootData.location) {
    return <></>;
  }

  let moreActionsOption: MoreActionsMenuItemType[] = [];

  if (rootData.location.canModify) {
    if (deskDetails.deactivated) {
      moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.ActivateDesk]);
    } else {
      moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeactivateDesk]);
    }

    if (deskDetails.requireBookingApproval) {
      moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveDeskApprovalRequirement]);
    } else {
      moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.EnableDeskApprovalRequirement]);
    }
  }

  let extraInfo: String[] = [];

  if (deskDetails.deactivated) {
    extraInfo = extraInfo.concat('Inactive');
  }

  if (deskDetails.requireBookingApproval) {
    extraInfo = extraInfo.concat('Approval Required');
  }

  return (
    <>
      {!editing && (
        <Card elevation={24} sx={{ minWidth: 320, height: '100%' }}>
          {moreActionsOption.length > 0 && (
            <CardHeader
              title={
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                  <DeskIcon />
                  <Typography gutterBottom variant="body1">
                    {deskDetails.name}
                  </Typography>
                </Stack>
              }
              action={
                <IconButton onClick={handleMoreActionsMenuClick}>
                  <MoreVertIcon />
                </IconButton>
              }
            />
          )}

          <CardContent>
            <ZonesLine zones={deskDetails.locationTags} />

            {extraInfo.length > 0 && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <InfoIcon />
                <Typography gutterBottom variant="body1">
                  {extraInfo.join(', ')}
                </Typography>
              </Stack>
            )}

            {customerDetails && (
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                <CustomerAvatar
                  name={{
                    name: null,
                    givenName: customerDetails.givenName,
                    middleName: customerDetails.middleName,
                    familyName: customerDetails.familyName,
                  }}
                  photo={{
                    url: customerDetails.photoUrl,
                  }}
                />
                <Typography gutterBottom variant="body1">
                  {getCustomerFullName(customerDetails)}
                </Typography>
              </Stack>
            )}
          </CardContent>

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.location.canModify && (
              <Tooltip title={'Edit desk details'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}

            {rootData.location.canModify && (
              <Tooltip title={'Delete desk'}>
                <Button size="small" color="warning" onClick={handleDeleteConfirmationDialogClick}>
                  <DeleteIcon />
                </Button>
              </Tooltip>
            )}

            <Tooltip title={isPreferredDesk ? 'Remove preferred desk' : 'Set as preferred desk'}>
              <Switch checked={isPreferredDesk} onChange={handleDefaultDeskStateChange} />
            </Tooltip>
          </CardActions>
        </Card>
      )}
      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSaveClick}
            initialValues={{
              name: deskDetails.name,
              locationTagIds: deskDetails.locationTags.map(({ id }) => id),
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
                <DeskName name="name" required={requiredFields.name} />
                <DeskMultipleChoicesZones
                  rootDataRelay={deskMultipleChoicesZonesData}
                  name="locationTagIds"
                  required={requiredFields.locationTagIds}
                />
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

      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            {option.label}
          </MenuItem>
        ))}
      </Menu>

      <Dialog fullWidth={true} open={deskRemoveConfirmationDialogOpen} onClose={handleCancelRemovingDeskClick}>
        <DialogTitle color={theme.palette.warning.main}>Remove desk</DialogTitle>
        <DialogContent>
          <DialogContentText color={theme.palette.warning.main}>{`Are you sure you want to remove desk "${deskDetails.name}"?`}</DialogContentText>

          <DialogActions>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingDeskClick}>
              Remove
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingDeskClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
      <Dialog fullWidth={true} open={deskDeactivateConfirmationDialogOpen} onClose={handleCancelDeactivateDeskClick}>
        <DialogTitle color={theme.palette.info.main}>Deactivate desk</DialogTitle>
        <DialogContent>
          <DialogContentText color={theme.palette.info.main}>{`Are you sure you want to deactivate desk "${deskDetails.name}"?`}</DialogContentText>

          <DialogActions>
            <Button color="info" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmDeactivatingDeskClick}>
              Deactivate
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelDeactivateDeskClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
      <Dialog fullWidth={true} open={deskActivateConfirmationDialogOpen} onClose={handleCancelActivateDeskClick}>
        <DialogTitle color={theme.palette.info.main}>Activate desk</DialogTitle>
        <DialogContent>
          <DialogContentText color={theme.palette.info.main}>{`Are you sure you want to activate desk "${deskDetails.name}"?`}</DialogContentText>

          <DialogActions>
            <Button color="info" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmActivatingDeskClick}>
              Activate
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelActivateDeskClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
      <Dialog fullWidth={true} open={setDeskApprovalRequirementConfirmationDialogOpen} onClose={handleCancelSetDeskApprovalRequirementClick}>
        <DialogTitle color={theme.palette.info.main}>Set Desk Approval Requirement</DialogTitle>
        <DialogContent>
          <DialogContentText
            color={theme.palette.info.main}
          >{`Are you sure you want to enable approval for desk "${deskDetails.name}"?`}</DialogContentText>

          <DialogActions>
            <Button color="info" variant="contained" startIcon={<DangerIcon />} onClick={handleSetDeskApprovalRequirementClick}>
              Enable
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelSetDeskApprovalRequirementClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>

      <Dialog
        fullWidth={true}
        open={removeDeskApprovalRequirementConfirmationDialogOpen}
        onClose={handleCancelRemoveDeskApprovalRequirementDeskClick}
      >
        <DialogTitle color={theme.palette.info.main}>Remove Approval Requirement for Desk</DialogTitle>
        <DialogContent>
          <DialogContentText
            color={theme.palette.info.main}
          >{`Are you sure you want to remove approval for desk "${deskDetails.name}"?`}</DialogContentText>

          <DialogActions>
            <Button color="info" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemoveDeskApprovalRequirementDeskClick}>
              Remove
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemoveDeskApprovalRequirementDeskClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(DeskCard);
