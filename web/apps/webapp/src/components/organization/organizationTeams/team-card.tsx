import { CustomerAvatar } from '@/components/avatars';
import { DefaultDialogTitle, LeadIconTypography, PushToRight, SmallIconTypography, StackColumn, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import { EllipseMenuIcon, NotPreferredIcon, PreferredIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationTeamSetupBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { teamCard_addCustomerPreferredTeamMutation } from '@/queries/__generated__/teamCard_addCustomerPreferredTeamMutation.graphql';
import type { teamCard_deleteTeamMutation } from '@/queries/__generated__/teamCard_deleteTeamMutation.graphql';
import type { teamCard_query$key } from '@/queries/__generated__/teamCard_query.graphql';
import type { teamCard_removeCustomerPreferredTeamMutation } from '@/queries/__generated__/teamCard_removeCustomerPreferredTeamMutation.graphql';
import type { teamCard_TeamDetails$key } from '@/queries/__generated__/teamCard_TeamDetails.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import CardMedia from '@mui/material/CardMedia';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

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
          preferredTeams {
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
          uniqueAlphanumericName
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
        primaryFeatureImage {
          thumbnail {
            url
            height
            width
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

  const [commitAddCustomerPreferredTeam] = useMutation<teamCard_addCustomerPreferredTeamMutation>(graphql`
    mutation teamCard_addCustomerPreferredTeamMutation($input: AddCustomerPreferredTeamInput!) {
      addCustomerPreferredTeam(input: $input) {
        customer {
          id
          preferredTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerPreferredTeam] = useMutation<teamCard_removeCustomerPreferredTeamMutation>(graphql`
    mutation teamCard_removeCustomerPreferredTeamMutation($input: RemoveCustomerPreferredTeamInput!) {
      removeCustomerPreferredTeam(input: $input) {
        customer {
          id
          preferredTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);
  const isPreferred = useMemo(() => rootData.me?.preferredTeams.some((item) => item.uniqueId === teamDetails.id), [teamDetails.id, rootData.me?.preferredTeams]);

  let moreActionsOption: MoreActionsMenuItemType[] = [moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditTeam]];

  if (teamDetails.canDelete) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteTeam]);
  }

  moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.ViewTeamBookings]);

  const editLink = getOrganizationTeamSetupBaseLink(integratedPlatrform, teamDetails.organization!.uniqueAlphanumericName!, teamDetails.id);

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditTeam:
        router.push(editLink);
        break;

      case MoreActionsMenuOptionType.DeleteTeam:
        handleRemoveTeamClicked();
        break;

      case MoreActionsMenuOptionType.ViewTeamBookings:
        router.push(getOrganizationBookingsBaseLink(integratedPlatrform, teamDetails.organization!.uniqueAlphanumericName!, { teamId: teamDetails.id }));
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
          clientMutationId: uuid(),
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
    const toastId = themedToast(<NotificationContent content={`Setting team '${teamDetails.name}' as your preferred team...`} />, infoNotificationOptions);

    commitAddCustomerPreferredTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
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
    const toastId = themedToast(<NotificationContent content={`Removing team '${teamDetails.name}' as your preferred team...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredTeam({
      variables: {
        input: {
          clientMutationId: uuid(),
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
        {teamDetails.primaryFeatureImage && teamDetails.primaryFeatureImage.thumbnail && <CardMedia component="img" image={teamDetails.primaryFeatureImage.thumbnail.url} />}
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
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
          <StackColumn sx={{ paddingTop: 1, paddingBottom: 1 }}>
            <SmallIconTypography label="Members of this team" />
            <StackRow>
              <AvatarGroup max={5}>
                {teammates.map((item) => (
                  <CustomerAvatar key={item.uniqueId} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                ))}
              </AvatarGroup>
            </StackRow>
          </StackColumn>
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
