import type { deskTypeCard_OrganizationTagDetails$key } from '@/queries/__generated__/deskTypeCard_OrganizationTagDetails.graphql';
import type { deskTypeCard_Query$key } from '@/queries/__generated__/deskTypeCard_Query.graphql';
import type { deskTypeCard_addCustomerDefaultOrganizationTagMutation } from '@/queries/__generated__/deskTypeCard_addCustomerDefaultOrganizationTagMutation.graphql';
import type { deskTypeCard_deleteDeskTypeMutation } from '@/queries/__generated__/deskTypeCard_deleteDeskTypeMutation.graphql';
import type { deskTypeCard_removeCustomerDefaultOrganizationTagMutation } from '@/queries/__generated__/deskTypeCard_removeCustomerDefaultOrganizationTagMutation.graphql';
import type { deskTypeCard_updateDeskTypeMutation } from '@/queries/__generated__/deskTypeCard_updateDeskTypeMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Paper from '@mui/material/Paper';
import Tooltip from '@mui/material/Tooltip';
import { BodyIconTypography, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { DeskTypeName } from '@repo/shared/components/deskType';
import { DeleteIcon, DeskTypeIcon, EditIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
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
  rootDataRelay: deskTypeCard_Query$key;
  organizationTagDetailsRelay: deskTypeCard_OrganizationTagDetails$key;
  connectionIds: string[];
};

type OrganizationTagDetails = {
  name: string;
};

const deskTypeSchema = object({
  name: string().required('Desk type name is required'),
});

const DeskTypeCard = ({ rootDataRelay, organizationTagDetailsRelay, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment deskTypeCard_Query on Query {
        me {
          id
          preferredDeskTypes {
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
      fragment deskTypeCard_OrganizationTagDetails on OrganizationTagDetails {
        id
        name
      }
    `,
    organizationTagDetailsRelay,
  );

  const [commitUpdateDeskType] = useMutation<deskTypeCard_updateDeskTypeMutation>(graphql`
    mutation deskTypeCard_updateDeskTypeMutation($input: UpdateDeskTypeInput!) {
      updateDeskType(input: $input) {
        organizationTag {
          id
          name
        }
      }
    }
  `);

  const [commitDeleteDeskType] = useMutation<deskTypeCard_deleteDeskTypeMutation>(graphql`
    mutation deskTypeCard_deleteDeskTypeMutation($connectionIds: [ID!]!, $input: DeleteDeskTypeInput!) {
      deleteDeskType(input: $input) {
        organizationTag {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultOrganizationTag] = useMutation<deskTypeCard_addCustomerDefaultOrganizationTagMutation>(graphql`
    mutation deskTypeCard_addCustomerDefaultOrganizationTagMutation($input: AddCustomerDefaultOrganizationTagInput!) {
      addCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredDeskTypes {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultOrganizationTag] = useMutation<deskTypeCard_removeCustomerDefaultOrganizationTagMutation>(graphql`
    mutation deskTypeCard_removeCustomerDefaultOrganizationTagMutation($input: RemoveCustomerDefaultOrganizationTagInput!) {
      removeCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredDeskTypes {
            uniqueId
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(deskTypeSchema);
  const requiredFields = makeRequired(deskTypeSchema);
  const [deskTypeRemoveConfirmationDialogOpen, setDeskTypeRemoveConfirmationDialogOpen] = useState(false);
  const isPreferredDeskType = useMemo(
    () => !!rootData.me?.preferredDeskTypes.find((deskType) => deskType.uniqueId === organizationTagDetails.id),
    [rootData.me?.preferredDeskTypes, organizationTagDetails.id],
  );

  const handleDeleteClick = () => {
    setDeskTypeRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingDeskTypeClick = () => {
    setDeskTypeRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingDeskTypeClick = () => {
    setDeskTypeRemoveConfirmationDialogOpen(false);

    const toastId = themedToast(<NotificationContent content={`Removing desk type '${organizationTagDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteDeskType({
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
            render: <NotificationContent content={`Failed to remove desk type '${organizationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type ${organizationTagDetails.name} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desk type '${organizationTagDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        deleteOrganizationTag: {
          organizationTag: {
            id: organizationTagDetails.id,
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

  const handleSaveClick = ({ name }: OrganizationTagDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating desk type '${organizationTagDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateDeskType({
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
            render: <NotificationContent content={`Failed to update desk type '${organizationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type ${name} updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update desk type '${organizationTagDetails.name}'. Error: ${error.message}.`} />,
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

  const handleSetAsPreferredDeskTypeClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting desk type '${organizationTagDetails.name}' as your preferred desk type...`} />,
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
                content={`Failed to set desk type '${organizationTagDetails.name}' as your preferred desk type. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type '${organizationTagDetails.name}' has been set as the preferred desk type.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to set desk type '${organizationTagDetails.name}' as your preferred desk type. Error: ${error.message}.`}
            />
          ),
        });
      },
      optimisticResponse: {
        addCustomerDefaultOrganizationTag: {
          customer: {
            id: rootData.me.id,
            preferredDeskTypes: rootData.me.preferredDeskTypes.concat([
              {
                uniqueId: organizationTagDetails.id,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredDeskTypeClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing desk type '${organizationTagDetails.name}' as your preferred desk type...`} />,
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
                content={`Failed to remove the desk type '${organizationTagDetails.name}' as your preferred desk type. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type '${organizationTagDetails.name}' has been removed as your preferred desk type.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to remove the desk type '${organizationTagDetails.name}' as your preferred desk type. Error: ${error.message}.`}
            />
          ),
        });
      },
      optimisticResponse: {
        removeCustomerDefaultOrganizationTag: {
          customer: {
            id: rootData.me.id,
            preferredDeskTypes: rootData.me.preferredDeskTypes.filter(({ uniqueId }) => uniqueId === organizationTagDetails.id),
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
          <CardHeader title={<BodyIconTypography label={organizationTagDetails.name} startElement={<DeskTypeIcon />} invertDefaultColor />} />

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.organization.canModify && (
              <Tooltip title={'Edit desk type'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}
            {rootData.organization.canModify && (
              <Tooltip title={'Remove desk type'}>
                <Button size="small" color="warning" onClick={handleDeleteClick}>
                  <DeleteIcon />
                </Button>
              </Tooltip>
            )}
            {isPreferredDeskType && (
              <Tooltip title={'Remove as preferred desk type'}>
                <Button size="small" color="primary" onClick={handleRemoveAsPreferredDeskTypeClicked}>
                  <PreferredIcon />
                </Button>
              </Tooltip>
            )}
            {!isPreferredDeskType && (
              <Tooltip title={'Set as preferred desk type'}>
                <Button size="small" color="primary" onClick={handleSetAsPreferredDeskTypeClicked}>
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
                <DeskTypeName name="name" required={requiredFields.name} />
                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </Paper>
      )}

      <Dialog TransitionComponent={DialogTransition} open={deskTypeRemoveConfirmationDialogOpen} onClose={handleCancelRemovingDeskTypeClick}>
        <DialogTitle>Remove desk type</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove the desk type "${organizationTagDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingDeskTypeClick}
            onSecondaryClicked={handleCancelRemovingDeskTypeClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(DeskTypeCard);
