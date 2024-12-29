import { MultipleChoicesDeskTypes, MultipleChoicesZones } from '@/components/organization';
import type { deskCard_DeskDetails$key } from '@/queries/__generated__/deskCard_DeskDetails.graphql';
import type { deskCard_addCustomerDefaultDeskMutation } from '@/queries/__generated__/deskCard_addCustomerDefaultDeskMutation.graphql';
import type { deskCard_deleteLocationMutation } from '@/queries/__generated__/deskCard_deleteLocationMutation.graphql';
import type { deskCard_query$key } from '@/queries/__generated__/deskCard_query.graphql';
import type { deskCard_removeCustomerDefaultDeskMutation } from '@/queries/__generated__/deskCard_removeCustomerDefaultDeskMutation.graphql';
import type { deskCard_updateDeskMutation } from '@/queries/__generated__/deskCard_updateDeskMutation.graphql';
import type { multipleChoicesDeskTypes_query$key } from '@/queries/__generated__/multipleChoicesDeskTypes_query.graphql';
import type { multipleChoicesZones_query$key } from '@/queries/__generated__/multipleChoicesZones_query.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Paper from '@mui/material/Paper';
import Tooltip from '@mui/material/Tooltip';
import Box from '@mui/system/Box';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, FormFieldLabel, FormStackColumn, StackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { DeskTypes } from '@repo/shared/components/deskType';
import { DeleteIcon, DeskIcon, EditIcon, EllipseMenuIcon, InfoIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
import {
  MoreActionsMenu,
  moreActionsMenuAllOptions,
  MoreActionsMenuItemType,
  MoreActionsMenuOptionType,
} from '@repo/shared/components/moreActionsMenu';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, sandstone } from '@repo/shared/libs/theme';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: deskCard_query$key;
  deskDetailsRelay: deskCard_DeskDetails$key;
  multipleChoicesDeskTypesData: multipleChoicesDeskTypes_query$key;
  multipleChoicesZonesData: multipleChoicesZones_query$key;
  connectionIds: string[];
  customerDetails: CustomerDetails | null;
};

type CustomerDetails = {
  familyName: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  name: string | null | undefined;
  photoUrl: string | null | undefined;
  uniqueId: string;
};

type DeskDetails = {
  name: string;
  deskTypeIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  name: string().required('Desk name is required'),
  deskTypeIds: array().nullable(),
  zoneIds: array().nullable(),
});

const DeskCard = ({
  rootDataRelay,
  deskDetailsRelay,
  multipleChoicesDeskTypesData,
  multipleChoicesZonesData,
  connectionIds,
  customerDetails,
}: Props) => {
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
        deskTypes {
          uniqueId
          name
        }
        zones {
          uniqueId
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
          deskTypes {
            uniqueId
            name
          }
          zones {
            uniqueId
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
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

    const toastId = themedToast(<NotificationContent content={`Removing desk '${deskDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteDesk({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove desk '${deskDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${deskDetails.name} removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove desk '${deskDetails.name}'. Error: ${error.message}.`} />,
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

    const deskTypeIds = deskDetails.deskTypes.map(({ uniqueId }) => uniqueId);
    const zoneIds = deskDetails.zones.map(({ uniqueId }) => uniqueId);
    const toastId = themedToast(<NotificationContent content={`Deactivating desk '${deskDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: true,
          requireBookingApproval: deskDetails.requireBookingApproval,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to deactivate Desk '${deskDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${deskDetails.name} deactivated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to deactivate Desk '${deskDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: true,
            requireBookingApproval: deskDetails.requireBookingApproval,
            deskTypes: deskDetails.deskTypes,
            zones: deskDetails.zones,
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

    const deskTypeIds = deskDetails.deskTypes.map(({ uniqueId }) => uniqueId);
    const zoneIds = deskDetails.zones.map(({ uniqueId }) => uniqueId);
    const toastId = themedToast(<NotificationContent content={`Activating desk '${deskDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: false,
          requireBookingApproval: deskDetails.requireBookingApproval,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to activate Desk '${deskDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${deskDetails.name} activated.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to activate Desk '${deskDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: false,
            requireBookingApproval: deskDetails.requireBookingApproval,
            deskTypes: deskDetails.deskTypes,
            zones: deskDetails.zones,
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

  const handleSaveClick = ({ name, deskTypeIds, zoneIds }: DeskDetails) => {
    const toastId = themedToast(<NotificationContent content={`Updating desk '${deskDetails.name}'...`} />, infoNotificationOptions);

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: deskDetails.id,
          name,
          deactivated: deskDetails.deactivated,
          requireBookingApproval: deskDetails.requireBookingApproval,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update Desk '${deskDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${name} updated.`} />,
        });

        setEditing(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update Desk '${deskDetails.name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name,
            deactivated: deskDetails.deactivated,
            requireBookingApproval: deskDetails.requireBookingApproval,
            deskTypes: [],
            zones: [],
          },
        },
      },
    });
  };

  const handleSetAsPreferredDeskClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting desk '${deskDetails.name}' as your preferred desk...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to set desk '${deskDetails.name}' as your preferred desk. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskDetails.name}' has been set as the preferred desk.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set desk '${deskDetails.name}' as your preferred desk. Error: ${error.message}.`} />,
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
  };

  const handleRemoveAsPreferredDeskClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing desk '${deskDetails.name}' as your preferred desk...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          deskId: deskDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the desk '${deskDetails.name}' as your preferred desk. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk '${deskDetails.name}' has been removed as your preferred desk.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to remove the desk '${deskDetails.name}' as your preferred desk. Error: ${error.message}.`} />
          ),
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
  };

  const handleSetDeskApprovalRequirementConfirmationDialogClick = () => {
    setSetDeskApprovalRequirementConfirmationDialogOpen(true);
  };

  const handleCancelSetDeskApprovalRequirementClick = () => {
    setSetDeskApprovalRequirementConfirmationDialogOpen(false);
  };

  const handleSetDeskApprovalRequirementClick = () => {
    setSetDeskApprovalRequirementConfirmationDialogOpen(false);

    const deskTypeIds = deskDetails.deskTypes.map(({ uniqueId }) => uniqueId);
    const zoneIds = deskDetails.zones.map(({ uniqueId }) => uniqueId);
    const toastId = themedToast(
      <NotificationContent content={`Setting '${deskDetails.name}' require approval property...`} />,
      infoNotificationOptions,
    );

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: deskDetails.deactivated,
          requireBookingApproval: true,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to set desk '${deskDetails.name}' require approval property. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Set desk '${deskDetails.name}' require approval property.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set desk '${deskDetails.name}' require approval property. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: deskDetails.deactivated,
            requireBookingApproval: true,
            deskTypes: deskDetails.deskTypes,
            zones: deskDetails.zones,
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

    const deskTypeIds = deskDetails.deskTypes.map(({ uniqueId }) => uniqueId);
    const zoneIds = deskDetails.zones.map(({ uniqueId }) => uniqueId);
    const toastId = themedToast(
      <NotificationContent content={`Unsetting '${deskDetails.name}' require approval property...`} />,
      infoNotificationOptions,
    );

    commitUpdateDesk({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: deskDetails.id,
          name: deskDetails.name,
          deactivated: deskDetails.deactivated,
          requireBookingApproval: false,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to unset desk '${deskDetails.name}' require approval property. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Unset desk '${deskDetails.name}' require approval property.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to unset desk '${deskDetails.name}' require approval property. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateDesk: {
          desk: {
            id: deskDetails.id,
            name: deskDetails.name,
            deactivated: deskDetails.deactivated,
            requireBookingApproval: false,
            deskTypes: deskDetails.deskTypes,
            zones: deskDetails.zones,
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
        <Card sx={{ minWidth: 320, height: '100%' }}>
          <CardHeader
            title={<BodyIconTypography label={deskDetails.name} startElement={<DeskIcon />} invertDefaultColor />}
            action={
              <>
                {moreActionsOption.length > 0 && (
                  <Box color={paletteMode === 'dark' ? coal : sandstone}>
                    <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                      <EllipseMenuIcon />
                    </IconButton>
                  </Box>
                )}
              </>
            }
          />

          <CardContent>
            <StackColumn>
              <DeskTypes
                deskTypes={deskDetails.deskTypes.map(({ uniqueId, name }) => ({ id: uniqueId, name }))}
                sx={{ paddingTop: 1, paddingBottom: 1 }}
              />
              <Zones zones={deskDetails.zones.map(({ uniqueId, name }) => ({ id: uniqueId, name }))} sx={{ paddingTop: 1, paddingBottom: 1 }} />
            </StackColumn>

            {extraInfo.length > 0 && <BodyIconTypography label={extraInfo.join(', ')} startElement={<InfoIcon />} />}

            {customerDetails && (
              <BodyIconTypography
                label={getCustomerFullName(customerDetails)}
                startElement={<CustomerAvatar name={customerDetails} photo={{ url: customerDetails.photoUrl }} size="small" />}
              />
            )}
          </CardContent>

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            {rootData.location.canModify && (
              <Tooltip title={'Edit desk'}>
                <Button size="small" color="primary" onClick={handleEditClick}>
                  <EditIcon />
                </Button>
              </Tooltip>
            )}
            {rootData.location.canModify && (
              <Tooltip title={'Remove desk'}>
                <Button size="small" color="warning" onClick={handleDeleteConfirmationDialogClick}>
                  <DeleteIcon />
                </Button>
              </Tooltip>
            )}
            {isPreferredDesk && (
              <Tooltip title={'Remove as preferred desk'}>
                <Button size="small" color="primary" onClick={handleRemoveAsPreferredDeskClicked}>
                  <PreferredIcon />
                </Button>
              </Tooltip>
            )}
            {!isPreferredDesk && (
              <Tooltip title={'Set as preferred desk'}>
                <Button size="small" color="primary" onClick={handleSetAsPreferredDeskClicked}>
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
              name: deskDetails.name,
              deskTypeIds: deskDetails.deskTypes.map(({ uniqueId }) => uniqueId),
              zoneIds: deskDetails.zones.map(({ uniqueId }) => uniqueId),
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
                <FormFieldLabel label="Name">
                  <TextField name="name" required={requiredFields.name} helperText="Add your desk name" />
                </FormFieldLabel>

                <FormFieldLabel label="Desk Types">
                  <MultipleChoicesDeskTypes rootDataRelay={multipleChoicesDeskTypesData} name="deskTypeIds" required={requiredFields.deskTypeIds} />
                </FormFieldLabel>

                <FormFieldLabel label="Zones">
                  <MultipleChoicesZones rootDataRelay={multipleChoicesZonesData} name="zoneIds" required={requiredFields.zoneIds} />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </Paper>
      )}

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />

      <Dialog TransitionComponent={DialogTransition} open={deskRemoveConfirmationDialogOpen} onClose={handleCancelRemovingDeskClick}>
        <DialogTitle>Remove desk</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove desk "${deskDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingDeskClick}
            onSecondaryClicked={handleCancelRemovingDeskClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
      <Dialog TransitionComponent={DialogTransition} open={deskDeactivateConfirmationDialogOpen} onClose={handleCancelDeactivateDeskClick}>
        <DialogTitle>Deactivate desk</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to deactivate desk "${deskDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmDeactivatingDeskClick}
            onSecondaryClicked={handleCancelDeactivateDeskClick}
            primaryLabel="Deactivate"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
      <Dialog TransitionComponent={DialogTransition} open={deskActivateConfirmationDialogOpen} onClose={handleCancelActivateDeskClick}>
        <DialogTitle>Activate desk</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to activate desk "${deskDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmActivatingDeskClick}
            onSecondaryClicked={handleCancelActivateDeskClick}
            primaryLabel="Activate"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
      <Dialog
        TransitionComponent={DialogTransition}
        open={setDeskApprovalRequirementConfirmationDialogOpen}
        onClose={handleCancelSetDeskApprovalRequirementClick}
      >
        <DialogTitle>Set Desk Approval Requirement</DialogTitle>
        <DialogContent>
          <DialogContentText color="info">{`Are you sure you want to enable approval for desk "${deskDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleSetDeskApprovalRequirementClick}
            onSecondaryClicked={handleCancelSetDeskApprovalRequirementClick}
            primaryLabel="Enable"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>

      <Dialog
        TransitionComponent={DialogTransition}
        open={removeDeskApprovalRequirementConfirmationDialogOpen}
        onClose={handleCancelRemoveDeskApprovalRequirementDeskClick}
      >
        <DialogTitle>Remove Approval Requirement for Desk</DialogTitle>
        <DialogContent>
          <DialogContentText color="info">{`Are you sure you want to remove approval for desk "${deskDetails.name}"?`}</DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemoveDeskApprovalRequirementDeskClick}
            onSecondaryClicked={handleCancelRemoveDeskApprovalRequirementDeskClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(DeskCard);
