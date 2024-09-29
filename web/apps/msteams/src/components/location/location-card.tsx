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
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import Tooltip from '@mui/material/Tooltip';
import Typography from '@mui/material/Typography';
import { LocationAvatar } from '@repo/shared/components/avatars';
import { AboutIcon, DangerIcon, DeleteIcon, DeskIcon, EditIcon, OrganizationIcon, ViewIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, now } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import type { locationCard_LocationDetails$key } from './__generated__/locationCard_LocationDetails.graphql';
import type { locationCard_Query$key } from './__generated__/locationCard_Query.graphql';
import type { locationCard_addCustomerDefaultLocationMutation } from './__generated__/locationCard_addCustomerDefaultLocationMutation.graphql';
import type { locationCard_deleteLocationMutation } from './__generated__/locationCard_deleteLocationMutation.graphql';
import type { locationCard_removeCustomerDefaultLocationMutation } from './__generated__/locationCard_removeCustomerDefaultLocationMutation.graphql';

type Props = {
  rootDataRelay: locationCard_Query$key;
  locationDetailsRelay: locationCard_LocationDetails$key;
  connectionIds: string[];
};

const LocationCard = ({ rootDataRelay, locationDetailsRelay: location, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationCard_Query on Query {
        me {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );

  const locationDetails = useFragment(
    graphql`
      fragment locationCard_LocationDetails on LocationDetails {
        id
        name
        about
        organization {
          uniqueId
          name
        }
        deskCapacity
        hasFutureBooking
        canModify
        canDelete
      }
    `,
    location,
  );

  const [commitDeleteLocation] = useMutation<locationCard_deleteLocationMutation>(graphql`
    mutation locationCard_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocation] = useMutation<locationCard_addCustomerDefaultLocationMutation>(graphql`
    mutation locationCard_addCustomerDefaultLocationMutation($input: AddCustomerDefaultLocationInput!) {
      addCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultLocation] = useMutation<locationCard_removeCustomerDefaultLocationMutation>(graphql`
    mutation locationCard_removeCustomerDefaultLocationMutation($input: RemoveCustomerDefaultLocationInput!) {
      removeCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const isPreferredLocation = useMemo(
    () => !!rootData.me?.defaultLocations.find((location) => location.uniqueId === locationDetails.id),
    [rootData.me?.defaultLocations, locationDetails.id],
  );

  const handleDeleteClick = () => {
    setLocationRemoveConfirmationDialogOpen(true);
  };

  const handleDefaultLocationStateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!rootData.me) {
      return;
    }

    if (event.target.checked) {
      commitAddCustomerDefaultLocation({
        variables: {
          input: {
            clientMutationId: nanoid(),
            locationId: locationDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to set location '${locationDetails.name}' as your preferred location. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          addCustomerDefaultLocation: {
            customer: {
              id: rootData.me.id,
              defaultLocations: rootData.me.defaultLocations.concat([
                {
                  uniqueId: locationDetails.id,
                },
              ]),
            },
          },
        },
      });
    } else {
      commitRemoveCustomerDefaultLocation({
        variables: {
          input: {
            clientMutationId: nanoid(),
            locationId: locationDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to remove the location '${locationDetails.name}' as your preferred location. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          removeCustomerDefaultLocation: {
            customer: {
              id: rootData.me.id,
              defaultLocations: rootData.me.defaultLocations.filter(({ uniqueId }) => uniqueId === locationDetails.id),
            },
          },
        },
      });
    }
  };

  const handleCancelRemovingLocationClick = () => {
    setLocationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingLocationClick = () => {
    setLocationRemoveConfirmationDialogOpen(false);

    commitDeleteLocation({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: locationDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to delete location '${locationDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to delete location '${locationDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        deleteLocation: {
          location: {
            id: locationDetails.id,
            deletedAt: now().toISOString(),
          },
        },
      },
    });
  };

  return (
    <>
      <Card elevation={24} sx={{ minWidth: 350, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <LocationAvatar name={{ name: locationDetails.name }} photo={{ url: null }} sx={{ marginBottom: 1 }} />

              {locationDetails.name && (
                <Typography variant="h5" noWrap={true}>
                  {locationDetails.name}
                </Typography>
              )}
            </Stack>
          }
        />

        <CardContent>
          {locationDetails.about && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <AboutIcon />
              <Typography variant="body1" noWrap={true}>
                {locationDetails.about}
              </Typography>
            </Stack>
          )}

          {locationDetails.organization && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <OrganizationIcon />
              <Typography variant="body1" noWrap={true}>
                {locationDetails.organization.name}
              </Typography>
            </Stack>
          )}

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <DeskIcon />
            <Typography variant="body1" noWrap={true}>
              {locationDetails.deskCapacity === 0 ? 'No desk available' : `Desk Capacity: ${locationDetails.deskCapacity}`}
            </Typography>
          </Stack>
        </CardContent>

        <CardActions sx={{ justifyContent: 'flex-end' }}>
          <Tooltip title={locationDetails.canModify ? 'Edit location details' : 'View location'}>
            <Link
              href={
                locationDetails.organization
                  ? `/organization/${locationDetails.organization.uniqueId}/location/${locationDetails.id}`
                  : `/location/${locationDetails.id}`
              }
            >
              <Button size="small" color="primary">
                {locationDetails.canModify ? <EditIcon /> : <ViewIcon />}
              </Button>
            </Link>
          </Tooltip>

          {locationDetails.canDelete && (
            <Tooltip title={'Remove location'}>
              <Button size="small" color="warning" onClick={handleDeleteClick}>
                <DeleteIcon />
              </Button>
            </Tooltip>
          )}

          <Tooltip title={isPreferredLocation ? 'Remove preferred location' : 'Set as preferred location'}>
            <Switch checked={isPreferredLocation} onChange={handleDefaultLocationStateChange} />
          </Tooltip>
        </CardActions>
      </Card>

      <Dialog fullWidth={true} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
        <DialogTitle>Remove location</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {locationDetails.hasFutureBooking
              ? `Bookings are scheduled for the location "${locationDetails.name}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the location "${locationDetails.name}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingLocationClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingLocationClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(LocationCard);
