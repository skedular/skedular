import { CustomerAvatar } from '@/components/avatars';
import { DefaultDialogTitle, LeadIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { EllipseMenuIcon, TeamIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationTeamSetupBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal } from '@/libs/theme';
import { getRelayErrorMessage } from '@/libs/utils';
import type { teamCard_deleteTeamMutation } from '@/queries/__generated__/teamCard_deleteTeamMutation.graphql';
import type { teamCard_TeamDetails$key } from '@/queries/__generated__/teamCard_TeamDetails.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import Tooltip from '@mui/material/Tooltip';
import type { SxProps, Theme } from '@mui/system';
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
          customDomain
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

  const editLink = getOrganizationTeamSetupBaseLink(integratedPlatrform, teamDetails.organization!.customDomain!, teamDetails.id);
  const bookingsLink = getOrganizationBookingsBaseLink(integratedPlatrform, teamDetails.organization!.customDomain!, { teamId: teamDetails.id });
  const memberCount = teammates.length;
  const primaryFeatureImage = teamDetails.featureImages[0]?.thumbnail?.url;
  const sectionSx: SxProps<Theme> = {
    border: 1,
    borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
    borderRadius: 3,
    p: 1.25,
    backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.02)' : 'transparent'),
  };

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
        router.push(bookingsLink);
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
            render: <NotificationContent content={`Failed to remove team '${teamDetails.name}'. Error: ${getRelayErrorMessage(errors)}.`} />,
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
      <Card
        sx={{
          width: '100%',
          height: '100%',
          borderRadius: 4,
          border: 1,
          borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
          boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 10px 28px rgba(15, 23, 42, 0.08)' : theme.shadows[1]),
          backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.92)' : theme.palette.background.paper),
        }}
      >
        <CardContent sx={{ p: 2, height: '100%' }}>
          <StackColumn spacing={2} sx={{ height: '100%' }}>
            <StackRow sx={{ alignItems: 'center', flexWrap: 'nowrap', gap: 2, minHeight: 56 }}>
              <Box
                sx={{
                  width: 56,
                  height: 56,
                  borderRadius: 3,
                  border: 1,
                  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  overflow: 'hidden',
                  flexShrink: 0,
                  bgcolor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.04)' : theme.palette.action.hover),
                }}
              >
                {primaryFeatureImage ? (
                  <>
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={primaryFeatureImage} alt="" style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                  </>
                ) : (
                  <TeamIcon excludeTooltip />
                )}
              </Box>

              <StackColumn spacing={0.75} sx={{ minWidth: 0, flexGrow: 1, justifyContent: 'center' }}>
                <Tooltip title={teamDetails.name}>
                  <Link component={NextLink} href={editLink} underline="none" color="inherit" sx={{ display: 'block', minWidth: 0 }}>
                    <LeadIconTypography label={teamDetails.name} noWrap sx={{ minWidth: 0 }} />
                  </Link>
                </Tooltip>
              </StackColumn>

              {moreActionsOption.length > 0 && (
                <IconButton onClick={handleMoreActionsMenuClick} aria-label="Open team actions" sx={{ color: paletteMode === 'dark' ? 'inherit' : coal }}>
                  <EllipseMenuIcon />
                </IconButton>
              )}
            </StackRow>

            <Divider />

            <StackColumn spacing={1.25} sx={{ flexGrow: 1 }}>
              <Box sx={sectionSx}>
                <StackColumn spacing={0.75}>
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1 }}>
                    <SubtitleIconTypography label="Members" />
                    <SmallIconTypography label={`${memberCount} member${memberCount === 1 ? '' : 's'}`} />
                  </StackRow>
                  <AvatarGroup max={5}>
                    {teammates.map((item) => (
                      <CustomerAvatar key={item.id} name={item} photo={{ url: item.photoUrl }} size="medium" showFullName />
                    ))}
                  </AvatarGroup>
                </StackColumn>
              </Box>
            </StackColumn>

            <Box
              sx={{
                mt: 'auto',
                pt: 1.5,
                borderTop: 1,
                borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
              }}
            />
          </StackColumn>
        </CardContent>
      </Card>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      <Dialog slots={{ transition: DialogTransition }} open={teamRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
        <DefaultDialogTitle title="Remove Team" />
        <DialogContent sx={{ marginTop: 2 }}>
          <DialogContentText>{`Are you sure you want to remove the team "${teamDetails.name}"?`}</DialogContentText>
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
