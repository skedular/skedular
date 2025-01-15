import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Paper from '@mui/material/Paper';
import Tooltip from '@mui/material/Tooltip';
import {
  BodyIconTypography,
  ColorPicker,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { CustomTagIcon, DeleteIcon, EditIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
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
import type { customTagCard_OrganizationTagDetails$key } from './__generated__/customTagCard_OrganizationTagDetails.graphql';
import type { customTagCard_Query$key } from './__generated__/customTagCard_Query.graphql';
import type { customTagCard_addCustomerDefaultOrganizationTagMutation } from './__generated__/customTagCard_addCustomerDefaultOrganizationTagMutation.graphql';
import type { customTagCard_deleteCustomTagMutation } from './__generated__/customTagCard_deleteCustomTagMutation.graphql';
import type { customTagCard_removeCustomerDefaultOrganizationTagMutation } from './__generated__/customTagCard_removeCustomerDefaultOrganizationTagMutation.graphql';
import type { customTagCard_updateCustomTagMutation } from './__generated__/customTagCard_updateCustomTagMutation.graphql';

type Props = {
  rootDataRelay: customTagCard_Query$key;
  organizationTagDetailsRelay: customTagCard_OrganizationTagDetails$key;
  connectionIds: string[];
};

type OrganizationTagDetails = {
  name: string;
  description: string;
};

const customTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const CustomTagCard = ({ rootDataRelay, organizationTagDetailsRelay, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment customTagCard_Query on Query {
        me {
          id
          preferredCustomTags {
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
      fragment customTagCard_OrganizationTagDetails on OrganizationTagDetails {
        id
        name
        description
        color
      }
    `,
    organizationTagDetailsRelay,
  );

  const [commitUpdateCustomTag] = useMutation<customTagCard_updateCustomTagMutation>(graphql`
    mutation customTagCard_updateCustomTagMutation($input: UpdateCustomTagInput!) {
      updateCustomTag(input: $input) {
        organizationTag {
          id
          name
          description
          color
        }
      }
    }
  `);

  const [commitDeleteCustomTag] = useMutation<customTagCard_deleteCustomTagMutation>(graphql`
    mutation customTagCard_deleteCustomTagMutation($connectionIds: [ID!]!, $input: DeleteCustomTagInput!) {
      deleteCustomTag(input: $input) {
        organizationTag {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultOrganizationTag] = useMutation<customTagCard_addCustomerDefaultOrganizationTagMutation>(graphql`
    mutation customTagCard_addCustomerDefaultOrganizationTagMutation($input: AddCustomerDefaultOrganizationTagInput!) {
      addCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredCustomTags {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultOrganizationTag] = useMutation<customTagCard_removeCustomerDefaultOrganizationTagMutation>(graphql`
    mutation customTagCard_removeCustomerDefaultOrganizationTagMutation($input: RemoveCustomerDefaultOrganizationTagInput!) {
      removeCustomerDefaultOrganizationTag(input: $input) {
        customer {
          id
          preferredCustomTags {
            uniqueId
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(customTagSchema);
  const requiredFields = makeRequired(customTagSchema);
  const [customTagRemoveConfirmationDialogOpen, setCustomTagRemoveConfirmationDialogOpen] = useState(false);
  const isPreferredCustomTag = useMemo(
    () => !!rootData.me?.preferredCustomTags.find((customTag) => customTag.uniqueId === organizationTagDetails.id),
    [rootData.me?.preferredCustomTags, organizationTagDetails.id],
  );
  const [selectedColor, setSelectedColor] = useState(organizationTagDetails.color);

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleDeleteClick = () => {
    setCustomTagRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingCustomTagClick = () => {
    setCustomTagRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingCustomTagClick = () => {
    setCustomTagRemoveConfirmationDialogOpen(false);

    const toastId = themedToast(<NotificationContent content={`Removing tag '${organizationTagDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteCustomTag({
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
            render: <NotificationContent content={`Failed to remove tag '${organizationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag ${organizationTagDetails.name} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove tag '${organizationTagDetails.name}'. Error: ${error.message}.`} />,
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

  const handleSaveClick = ({ name, description }: OrganizationTagDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating tag '${organizationTagDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateCustomTag({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: organizationTagDetails.id,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update tag '${organizationTagDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag ${name} updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update tag '${organizationTagDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateOrganizationTag: {
          organizationTag: {
            id: organizationTagDetails.id,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  const handleSetAsPreferredCustomTagClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting tag '${organizationTagDetails.name}' as your preferred tag...`} />,
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
                content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag '${organizationTagDetails.name}' has been set as the preferred tag.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to set tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`} />
          ),
        });
      },
      optimisticResponse: {
        addCustomerDefaultOrganizationTag: {
          customer: {
            id: rootData.me.id,
            preferredCustomTags: rootData.me.preferredCustomTags.concat([
              {
                uniqueId: organizationTagDetails.id,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredCustomTagClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing tag '${organizationTagDetails.name}' as your preferred tag...`} />,
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
                content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag '${organizationTagDetails.name}' has been removed as your preferred tag.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to remove the tag '${organizationTagDetails.name}' as your preferred tag. Error: ${error.message}.`}
            />
          ),
        });
      },
      optimisticResponse: {
        removeCustomerDefaultOrganizationTag: {
          customer: {
            id: rootData.me.id,
            preferredCustomTags: rootData.me.preferredCustomTags.filter(({ uniqueId }) => uniqueId === organizationTagDetails.id),
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
          <CardHeader title={<BodyIconTypography label={organizationTagDetails.name} startElement={<CustomTagIcon />} invertDefaultColor />} />

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.organization.canModify && (
              <Tooltip title={'Edit tag'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}
            {rootData.organization.canModify && (
              <Tooltip title={'Remove tag'}>
                <Button size="small" color="warning" onClick={handleDeleteClick}>
                  <DeleteIcon />
                </Button>
              </Tooltip>
            )}
            {isPreferredCustomTag && (
              <Tooltip title={'Remove as preferred tag'}>
                <Button size="small" color="primary" onClick={handleRemoveAsPreferredCustomTagClicked}>
                  <PreferredIcon />
                </Button>
              </Tooltip>
            )}
            {!isPreferredCustomTag && (
              <Tooltip title={'Set as preferred tag'}>
                <Button size="small" color="primary" onClick={handleSetAsPreferredCustomTagClicked}>
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
              description: organizationTagDetails.description,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} helperText="Add your tag name" />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color" useWiderSpace>
                  <ColorPicker onChange={handleColorChange} defaultColor={organizationTagDetails.color} />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </Paper>
      )}

      <Dialog TransitionComponent={DialogTransition} open={customTagRemoveConfirmationDialogOpen} onClose={handleCancelRemovingCustomTagClick}>
        <DefaultDialogTitle title="Remove Tag" />
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove the tag "${organizationTagDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingCustomTagClick}
            onSecondaryClicked={handleCancelRemovingCustomTagClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(CustomTagCard);
