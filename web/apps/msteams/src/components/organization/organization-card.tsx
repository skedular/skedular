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
import { default as Link, default as MuiLink } from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { useTheme } from '@mui/material/styles';
import { OrganizationAvatar } from '@repo/shared/components/avatars';
import { AboutIcon, DangerIcon, DeleteIcon, EditIcon, ViewIcon, WebsiteIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, now } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import type { organizationCard_OrganizationDetails$key } from './__generated__/organizationCard_OrganizationDetails.graphql';
import type { organizationCard_Query$key } from './__generated__/organizationCard_Query.graphql';
import type { organizationCard_clearCustomerDefaultOrganizationMutation } from './__generated__/organizationCard_clearCustomerDefaultOrganizationMutation.graphql';
import type { organizationCard_deleteOrganizationMutation } from './__generated__/organizationCard_deleteOrganizationMutation.graphql';
import type { organizationCard_setCustomerDefaultOrganizationMutation } from './__generated__/organizationCard_setCustomerDefaultOrganizationMutation.graphql';

type Props = {
  rootDataRelay: organizationCard_Query$key;
  organizationDetailsRelay: organizationCard_OrganizationDetails$key;
  connectionIds: string[];
};

const OrganizationCard = ({ rootDataRelay, organizationDetailsRelay, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationCard_Query on Query {
        me {
          id
          defaultOrganization {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );

  const organizationDetails = useFragment(
    graphql`
      fragment organizationCard_OrganizationDetails on OrganizationDetails {
        id
        name
        about
        website
        logoUrl
        hasFutureBooking
        hasLocation
        canModify
        canDelete
      }
    `,
    organizationDetailsRelay,
  );

  const [commitDeleteOrganization] = useMutation<organizationCard_deleteOrganizationMutation>(graphql`
    mutation organizationCard_deleteOrganizationMutation($connectionIds: [ID!]!, $input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitSetCustomerDefaultOrganization] = useMutation<organizationCard_setCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationCard_setCustomerDefaultOrganizationMutation($input: SetCustomerDefaultOrganizationInput!) {
      setCustomerDefaultOrganization(input: $input) {
        customer {
          id
          defaultOrganization {
            uniqueId
            name
          }
        }
      }
    }
  `);

  const [commitClearCustomerDefaultOrganization] = useMutation<organizationCard_clearCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationCard_clearCustomerDefaultOrganizationMutation($input: ClearCustomerDefaultOrganizationInput!) {
      clearCustomerDefaultOrganization(input: $input) {
        customer {
          id
          defaultOrganization {
            uniqueId
            name
          }
        }
      }
    }
  `);

  const theme = useTheme();
  const { enqueueSnackbar } = useSnackbar();
  const [organizationRemoveConfirmationDialogOpen, setOrganizationRemoveConfirmationDialogOpen] = useState(false);
  const isDefaultOrganization = useMemo(
    () => (rootData.me?.defaultOrganization ? rootData.me.defaultOrganization.uniqueId === organizationDetails.id : false),
    [rootData.me?.defaultOrganization, organizationDetails.id],
  );

  const handleDeleteClick = () => {
    setOrganizationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingOrganizationClick = () => {
    setOrganizationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingOrganizatioClick = () => {
    setOrganizationRemoveConfirmationDialogOpen(false);

    commitDeleteOrganization({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: uuidv4(),
          id: organizationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to delete organization '${organizationDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to delete organization '${organizationDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        deleteOrganization: {
          organization: {
            id: organizationDetails.id,
            deletedAt: now().toISOString(),
          },
        },
      },
    });
  };

  const handleDefaultOrganizationStateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!rootData.me) {
      return;
    }

    if (event.target.checked) {
      commitSetCustomerDefaultOrganization({
        variables: {
          input: {
            clientMutationId: uuidv4(),
            organizationId: organizationDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to set organization '${organizationDetails.name}' as default. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to set organization '${organizationDetails.name}' as default. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          addCustomerDefaultLocation: {
            customer: {
              id: rootData.me.id,
              defaultOrganization: {
                uniqueId: organizationDetails.id,
                name: organizationDetails.name,
              },
            },
          },
        },
      });
    } else {
      commitClearCustomerDefaultOrganization({
        variables: {
          input: {
            clientMutationId: uuidv4(),
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to clear default organization '${organizationDetails.name}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to clear default organization '${organizationDetails.name}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          removeCustomerDefaultLocation: {
            customer: {
              id: rootData.me.id,
              defaultOrganization: null,
            },
          },
        },
      });
    }
  };

  let organizationDeletionMessage = '';

  if (organizationDetails.hasFutureBooking && organizationDetails.hasLocation) {
    organizationDeletionMessage = `Bookings have been scheduled for this organization "${organizationDetails.name}" and there are locations under this organization. Are you sure you want to remove it?`;
  } else if (organizationDetails.hasFutureBooking && !organizationDetails.hasLocation) {
    organizationDeletionMessage = `Bookings have been scheduled for this organization "${organizationDetails.name}". Are you sure you want to remove it?`;
  } else if (!organizationDetails.hasFutureBooking && organizationDetails.hasLocation) {
    organizationDeletionMessage = `There are locations under this organization "${organizationDetails.name}". Are you sure you want to remove it?`;
  } else {
    organizationDeletionMessage = `Are you sure you want to remove organization "${organizationDetails.name}"?`;
  }

  return (
    <>
      <Card elevation={24} sx={{ minWidth: 350, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <OrganizationAvatar name={{ name: organizationDetails.name }} photo={{ url: organizationDetails.logoUrl }} />
              {organizationDetails.name && (
                <Typography gutterBottom variant="h5" noWrap={true}>
                  {organizationDetails.name}
                </Typography>
              )}
            </Stack>
          }
        />

        <CardContent>
          {organizationDetails.about && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <AboutIcon />
              <Typography gutterBottom variant="h5" noWrap={true}>
                {organizationDetails.about}
              </Typography>
            </Stack>
          )}

          {organizationDetails.website && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <WebsiteIcon />
              <MuiLink href={organizationDetails.website} target="_blank" rel="noopener noreferrer">
                {organizationDetails.website}
              </MuiLink>
            </Stack>
          )}
        </CardContent>

        <CardActions sx={{ justifyContent: 'flex-end' }}>
          <Tooltip title={organizationDetails.canModify ? 'Edit organization details' : 'View organization details'}>
            <Link href={`/organization/${organizationDetails.id}`}>
              <Button size="small" color="primary">
                {organizationDetails.canModify ? <EditIcon /> : <ViewIcon />}
              </Button>
            </Link>
          </Tooltip>
          {organizationDetails.canDelete && (
            <Tooltip title={'Delete organization'}>
              <Button size="small" color="warning" onClick={handleDeleteClick}>
                <DeleteIcon />
              </Button>
            </Tooltip>
          )}
          <Tooltip title={isDefaultOrganization ? 'Remove default organization' : 'Set as default organization'}>
            <Switch checked={isDefaultOrganization} onChange={handleDefaultOrganizationStateChange} />
          </Tooltip>
        </CardActions>
      </Card>

      <Dialog fullWidth={true} open={organizationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingOrganizationClick}>
        <DialogTitle color={theme.palette.warning.main}>Remove organization</DialogTitle>
        <DialogContent>
          <DialogContentText color={theme.palette.warning.main}>{organizationDeletionMessage}</DialogContentText>
          <DialogActions>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingOrganizatioClick}>
              Remove
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingOrganizationClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(OrganizationCard);
