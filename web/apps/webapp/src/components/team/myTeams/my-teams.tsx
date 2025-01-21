import { getModernOrganizationTeamSetupBaseLink } from '@/components/organization';
import type { myTeams_addCustomerDefaultTeamMutation } from '@/queries/__generated__/myTeams_addCustomerDefaultTeamMutation.graphql';
import type { myTeams_deleteTeamMutation } from '@/queries/__generated__/myTeams_deleteTeamMutation.graphql';
import type { myTeams_query$key } from '@/queries/__generated__/myTeams_query.graphql';
import type { myTeams_removeCustomerDefaultTeamMutation } from '@/queries/__generated__/myTeams_removeCustomerDefaultTeamMutation.graphql';
import type { myTeams_teams_query$key } from '@/queries/__generated__/myTeams_teams_query.graphql';
import type { myTeams_teams_refetchableFragment } from '@/queries/__generated__/myTeams_teams_refetchableFragment.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  DefaultDialogTitle,
  GridContainer,
  SectionIconTypography,
  SmallIconTypography,
  StackColumn,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
import { EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@repo/shared/components/icons';
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
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import MyTeamCard from './my-team-card';

type Props = {
  rootDataRelay: myTeams_query$key;
  rootDataTeamsRelay: myTeams_teams_query$key;
  onReloadRequired: () => void;
  primaryLocationIds: string[];
  viewMode: 'list' | 'grid';
};

type TeamDetails = {
  name: string;
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

type RowType = {
  id: string;
  team: TeamDetails;
  teammates: ReadonlyArray<CustomerDetails>;
};

const MyTeams = ({ rootDataRelay, rootDataTeamsRelay, primaryLocationIds, viewMode }: Props) => {
  const rootData = useFragment<myTeams_query$key>(
    graphql`
      fragment myTeams_query on Query {
        me {
          id
          defaultTeams {
            uniqueId
          }
        }
        ...myTeamCard__query
      }
    `,
    rootDataRelay,
  );

  const [rootDataRefetchable, refetch] = useRefetchableFragment<myTeams_teams_refetchableFragment, myTeams_teams_query$key>(
    graphql`
      fragment myTeams_teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "myTeams_teams_refetchableFragment") {
        teams(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, primaryLocationIds: $primaryLocationIds }
          orderBy: $teamsSortingValues
        ) @connection(key: "myTeams_teams") {
          __id
          totalCount
          edges {
            node {
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
              ...myTeamCard_TeamDetails
            }
          }
        }
      }
    `,
    rootDataTeamsRelay,
  );

  const [commitDeleteTeam] = useMutation<myTeams_deleteTeamMutation>(graphql`
    mutation myTeams_deleteTeamMutation($connectionIds: [ID!]!, $input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultTeam] = useMutation<myTeams_addCustomerDefaultTeamMutation>(graphql`
    mutation myTeams_addCustomerDefaultTeamMutation($input: AddCustomerDefaultTeamInput!) {
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

  const [commitRemoveCustomerDefaultTeam] = useMutation<myTeams_removeCustomerDefaultTeamMutation>(graphql`
    mutation myTeams_removeCustomerDefaultTeamMutation($input: RemoveCustomerDefaultTeamInput!) {
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

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const connectionIds = useMemo(() => (rootDataRefetchable.teams ? [rootDataRefetchable.teams.__id] : []), [rootDataRefetchable.teams]);
  const [selectedTeamId, setSelectedTeamId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditTeam],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteTeam],
  ];

  const teams = useMemo(() => {
    if (!rootDataRefetchable.teams) {
      return [];
    }

    return rootDataRefetchable.teams.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name));
  }, [rootDataRefetchable.teams]);

  const teamDetails = useMemo(() => teams.find((item) => item.id === selectedTeamId), [selectedTeamId, teams]);

  const handleRefetch = useCallback(
    (primaryLocationIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            primaryLocationIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(primaryLocationIds), [handleRefetch, primaryLocationIds]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditTeam:
        if (teamDetails) {
          router.push(getModernOrganizationTeamSetupBaseLink(teamDetails.organization?.uniqueId!, teamDetails.id));
        }

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
    if (!teamDetails) {
      return;
    }

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

  const handleSetAsPreferredTeamClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const teamDetails = teams.find((item) => item.id === id);
    if (!teamDetails) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting team '${teamDetails.name}' as your preferred team...`} />,
      infoNotificationOptions,
    );

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
            render: (
              <NotificationContent content={`Failed to set team '${teamDetails.name}' as your preferred team. Error: ${joinErrors(errors)}.`} />
            ),
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

      optimisticResponse: {
        addCustomerDefaultTeam: {
          customer: {
            id: rootData.me.id,
            defaultTeams: rootData.me.defaultTeams.concat([
              {
                uniqueId: teamDetails.id,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredTeamClicked = (id: string) => {
    if (!rootData.me) {
      return;
    }

    const teamDetails = teams.find((item) => item.id === id);
    if (!teamDetails) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Removing team '${teamDetails.name}' as your preferred team...`} />,
      infoNotificationOptions,
    );

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
            render: (
              <NotificationContent
                content={`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${joinErrors(errors)}.`}
              />
            ),
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
          render: (
            <NotificationContent content={`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${error.message}.`} />
          ),
        });
      },
      optimisticResponse: {
        addCustomerDefaultTeam: {
          customer: {
            id: rootData.me.id,
            defaultTeams: rootData.me.defaultTeams.filter(({ uniqueId }) => uniqueId === teamDetails.id),
          },
        },
      },
    });
  };

  const rows: RowType[] = teams.map((team) => {
    return {
      id: team.id,
      team,
      teammates: team.members.filter(({ organizationMember }) => !!organizationMember).map(({ organizationMember }) => organizationMember!.customer),
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'team',
      headerName: 'Team',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value.name} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'teammates',
      headerName: 'Members of this team',
      editable: false,
      renderCell: (params) => (
        <AvatarGroup max={5}>
          {params.value.map((customer: CustomerDetails) => (
            <CustomerAvatar key={customer?.uniqueId} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
          ))}
        </AvatarGroup>
      ),
      display: 'flex',
      minWidth: 300,
    },
    {
      field: 'preferredTeam',
      headerName: '',
      editable: false,
      renderCell: (params) => {
        const teamId = params.id as string;
        const isPreferred = rootData.me?.defaultTeams.find((item) => item.uniqueId === teamId);

        if (isPreferred) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredTeamClicked(teamId)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredTeamClicked(teamId)}>
            <NotPreferredIcon />
          </IconButton>
        );
      },
      display: 'flex',
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedTeamId(params.id as string);
              setMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  if (!rootDataRefetchable.teams) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
        <SectionIconTypography label="Teams" />
        <Divider />
        <Box sx={{ paddingBottom: defaultPadding }} />

        {viewMode === 'grid' && (
          <GridContainer>
            {teams.map((team) => (
              <Grid key={team.id}>
                <MyTeamCard
                  rootDataRelay={rootData}
                  teamDetailsRelay={team}
                  connectionIds={connectionIds}
                  teammates={team.members
                    .filter(({ organizationMember }) => !!organizationMember)!
                    .map(({ organizationMember }) => organizationMember!.customer)}
                />
              </Grid>
            ))}
          </GridContainer>
        )}

        {viewMode === 'list' && (
          <DataGrid
            rows={rows}
            columns={columns}
            ignoreDiacritics
            disableRowSelectionOnClick
            hideFooter
            getRowHeight={() => 'auto'}
            rowSpacingType="margin"
            getRowSpacing={() => ({ top: 3, bottom: 3 })}
            sx={defaultGridStyle}
          />
        )}
      </StackColumn>

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />

      {teamDetails && (
        <Dialog TransitionComponent={DialogTransition} open={teamRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
          <DefaultDialogTitle title="Remove Team" />
          <DialogContent>
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
      )}
    </>
  );
};

export default memo(MyTeams);
