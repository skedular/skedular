import { CustomerAvatar } from '@/components/avatars';
import { CardMediaCarousel } from '@/components/CardMediaCarousel';
import { DefaultDialogTitle, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, TwoButtonsDialogActions } from '@/components/commons';
import { EllipseMenuIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationTeamSetupBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, sandstone } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { teamCard_deleteTeamMutation } from '@/queries/__generated__/teamCard_deleteTeamMutation.graphql';
import type { teamCard_TeamDetails$key } from '@/queries/__generated__/teamCard_TeamDetails.graphql';
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
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  teamDetailsRelay: teamCard_TeamDetails$key;
  connectionIds: string[];
  teammates: CustomerDetails[];
};

type CustomerDetails = {
  id: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

const TeamCard = ({ teamDetailsRelay, connectionIds, teammates }: Props) => {
  const teamDetails = useFragment(
    graphql`
      fragment teamCard_TeamDetails on TeamDetails {
        id
        name
        organization {
          uniqueAlphanumericName
        }
        members {
          edges {
            node {
              organizationMember {
                uniqueId
                customer {
                  id
                  givenName
                  middleName
                  familyName
                  name
                  photoUrl
                }
              }
            }
          }
        }
        featureImages {
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

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);

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

  return (
    <>
      <Card sx={{ width: { xs: '100%', sm: 400 } }}>
        <CardMediaCarousel images={teamDetails.featureImages ?? []} />
        <CardHeader
          title={
            <StackRow>
              <Link component={NextLink} href={editLink}>
                <LeadIconTypography startElement={<TeamIcon excludeTooltip />} label={teamDetails.name} sx={{ flexWrap: undefined }} invertDefaultColor />
              </Link>
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
                  <CustomerAvatar key={item.id} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
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
