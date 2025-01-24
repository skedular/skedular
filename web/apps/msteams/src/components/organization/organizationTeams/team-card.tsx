import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { DefaultDialogTitle, LeadIconTypography, PushToRight, SmallIconTypography, StackRow, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { EllipseMenuIcon, NotPreferredIcon, PreferredIcon, TeamIcon } from '@repo/shared/components/icons';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@repo/shared/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, sandstone } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { getOrganizationTeamSetupBaseLink } from 'components/links';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import type { teamCard_addCustomerDefaultTeamMutation } from './__generated__/teamCard_addCustomerDefaultTeamMutation.graphql';
import type { teamCard_deleteTeamMutation } from './__generated__/teamCard_deleteTeamMutation.graphql';
import type { teamCard_query$key } from './__generated__/teamCard_query.graphql';
import type { teamCard_removeCustomerDefaultTeamMutation } from './__generated__/teamCard_removeCustomerDefaultTeamMutation.graphql';
import type { teamCard_TeamDetails$key } from './__generated__/teamCard_TeamDetails.graphql';

type Props = {
  rootDataRelay: teamCard_query$key;
  teamDetailsRelay: teamCard_TeamDetails$key;
  connectionIds: string[];
  teammates: CustomerDetails[];
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const TeamCard = ({ rootDataRelay, teamDetailsRelay, connectionIds, teammates }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment teamCard_query on Query {
        me {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );

  const teamDetails = useFragment(
    graphql`
      fragment teamCard_TeamDetails on TeamDetails {
        id
        name
        organization {
          uniqueId
        }
        members {
          organizationMember {
            uniqueId
            customer {
              uniqueId
              givenName
              middleName
              familyName
              name
              photoUrl
            }
          }
        }
        hasFutureBooking
        canModify
        canDelete
      }
    `,
    teamDetailsRelay,
  );

  const [commitDeleteTeam] = useMutation<teamCard_deleteTeamMutation>(graphql`
    mutation teamCard_deleteTeamMutation($connectionIds: [ID!]!, $input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultTeam] = useMutation<teamCard_addCustomerDefaultTeamMutation>(graphql`
    mutation teamCard_addCustomerDefaultTeamMutation($input: AddCustomerDefaultTeamInput!) {
      addCustomerDefaultTeam(input: $input) {
        customer {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultTeam] = useMutation<teamCard_removeCustomerDefaultTeamMutation>(graphql`
    mutation teamCard_removeCustomerDefaultTeamMutation($input: RemoveCustomerDefaultTeamInput!) {
      removeCustomerDefaultTeam(input: $input) {
        customer {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const navigate = useNavigate();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);
  const isPreferred = useMemo(() => rootData.me?.defaultTeams.some((item) => item.uniqueId === teamDetails.id), [teamDetails.id, rootData.me?.defaultTeams]);

  let moreActionsOption: MoreActionsMenuItemType[] = [moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditTeam]];

  if (teamDetails.canDelete) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteTeam]);
  }

  const editLink = getOrganizationTeamSetupBaseLink(teamDetails.organization?.uniqueId!, teamDetails.id);

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditTeam:
        navigate(editLink);
        break;

      case MoreActionsMenuOptionType.DeleteTeam:
        handleRemoveTeamClicked();
        break;
    }
  };

  const handleRemoveTeamClicked = () => {
    setTeamRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingTeamClick = () => {
    setTeamRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingTeamClick = () => {
    const toastId = themedToast(<NotificationContent content={`Removing team '${teamDetails.name}'...`} />, infoNotificationOptions);

    commitDeleteTeam({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: teamDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove team '${teamDetails.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team '${teamDetails.name}' has been successfully removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove team '${teamDetails.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleSetAsPreferredTeamClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Setting team '${teamDetails.name}' as your preferred team...`} />, infoNotificationOptions);

    commitAddCustomerDefaultTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: teamDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to set team '${teamDetails.name}' as your preferred team. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team '${teamDetails.name}' has been set as the preferred team.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set team '${teamDetails.name}' as your preferred team. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleRemoveAsPreferredTeamClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing team '${teamDetails.name}' as your preferred team...`} />, infoNotificationOptions);

    commitRemoveCustomerDefaultTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: teamDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team '${teamDetails.name}' has been removed as your preferred team.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 400 } }}>
        <CardHeader
          title={
            <StackRow>
              <Link href={editLink}>
                <LeadIconTypography startElement={<TeamIcon />} label={teamDetails.name} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>
              <PushToRight />
              <Box color={paletteMode === 'dark' ? coal : sandstone}>
                {isPreferred && (
                  <IconButton onClick={handleRemoveAsPreferredTeamClicked} color="inherit">
                    <PreferredIcon />
                  </IconButton>
                )}
                {!isPreferred && (
                  <IconButton onClick={handleSetAsPreferredTeamClicked} color="inherit">
                    <NotPreferredIcon />
                  </IconButton>
                )}
              </Box>
            </StackRow>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone} sx={{ paddingTop: 0.5 }}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
          }
        />
        <CardContent>
          <StackRow>
            <SmallIconTypography label="Members of this team" />
            <AvatarGroup max={5}>
              {teammates.map((item) => (
                <CustomerAvatar key={item.uniqueId} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
              ))}
            </AvatarGroup>
          </StackRow>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      <Dialog slots={{ transition: DialogTransition }} open={teamRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
        <DefaultDialogTitle title="Remove Team" />
        <DialogContent sx={{ marginTop: 2 }}>
          <DialogContentText>
            {teamDetails.hasFutureBooking
              ? `Bookings are scheduled for the team "${teamDetails.name}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the team "${teamDetails.name}"?`}
          </DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingTeamClick}
            onSecondaryClicked={handleCancelRemovingTeamClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamCard);
