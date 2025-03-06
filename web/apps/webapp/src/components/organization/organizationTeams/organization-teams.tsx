import { CustomerAvatar } from '@/components/avatars';
import { DefaultDialogTitle, GridContainer, PushToRight, SectionIconTypography, SmallIconTypography, StackColumn, TwoButtonsDialogActions } from '@/components/commons';
import { EllipseMenuIcon, NotPreferredIcon, PreferredIcon } from '@/components/icons';
import { getOrganizationBookingsBaseLink, getOrganizationTeamSetupBaseLink } from '@/components/links';
import { ListGridToggle } from '@/components/listGridToggle';
import { Loading } from '@/components/loading';
import { LocationSelector } from '@/components/location/locationSelector';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { NewTeamButton } from '@/components/team/addTeam';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { defaultGridStyle, defaultPadding, maxScreenWidth } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { organizationTeams_addCustomerPreferredTeamMutation } from '@/queries/__generated__/organizationTeams_addCustomerPreferredTeamMutation.graphql';
import type { organizationTeams_deleteTeamMutation } from '@/queries/__generated__/organizationTeams_deleteTeamMutation.graphql';
import type { organizationTeams_removeCustomerPreferredTeamMutation } from '@/queries/__generated__/organizationTeams_removeCustomerPreferredTeamMutation.graphql';
import type { organizationTeams_rootQuery } from '@/queries/__generated__/organizationTeams_rootQuery.graphql';
import type { organizationTeams_teams_query$key } from '@/queries/__generated__/organizationTeams_teams_query.graphql';
import type { organizationTeams_teams_refetchableFragment } from '@/queries/__generated__/organizationTeams_teams_refetchableFragment.graphql';
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
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import TeamCard from './team-card';

type Props = {
  queryReference: PreloadedQuery<organizationTeams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationTeams_rootQuery(
    $organizationId: String!
    $primaryLocationIds: [String!]
    $teamsSortingValues: [TeamOrderInput!]
    $locationsSortingValues: [LocationOrderInput!]
  ) {
    me {
      id
      preferredTeams {
        uniqueId
      }
    }
    ...teamCard_query
    ...locationSelector_allLocations_query
    ...organizationTeams_teams_query
  }
`;

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
  preferred: boolean;
};

const Teams = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationTeams_rootQuery>(RootQuery, queryReference);
  const [rootDataRefetchable, refetch] = useRefetchableFragment<organizationTeams_teams_refetchableFragment, organizationTeams_teams_query$key>(
    graphql`
      fragment organizationTeams_teams_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "organizationTeams_teams_refetchableFragment") {
        teams(first: $count, after: $cursor, where: { organizationId: $organizationId, primaryLocationIds: $primaryLocationIds }, orderBy: $teamsSortingValues)
          @connection(key: "organizationTeams_teams") {
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
              ...teamCard_TeamDetails
            }
          }
        }
      }
    `,
    rootData,
  );

  const [commitDeleteTeam] = useMutation<organizationTeams_deleteTeamMutation>(graphql`
    mutation organizationTeams_deleteTeamMutation($connectionIds: [ID!]!, $input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerPreferredTeam] = useMutation<organizationTeams_addCustomerPreferredTeamMutation>(graphql`
    mutation organizationTeams_addCustomerPreferredTeamMutation($input: AddCustomerPreferredTeamInput!) {
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

  const [commitRemoveCustomerPreferredTeam] = useMutation<organizationTeams_removeCustomerPreferredTeamMutation>(graphql`
    mutation organizationTeams_removeCustomerPreferredTeamMutation($input: RemoveCustomerPreferredTeamInput!) {
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

  const [locationIds, setLocationIds] = useState<string[]>([]);
  const [viewMode, setViewMode] = useState<'list' | 'grid'>('grid');
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const connectionIds = useMemo(() => (rootDataRefetchable.teams ? [rootDataRefetchable.teams.__id] : []), [rootDataRefetchable.teams]);
  const [selectedTeamId, setSelectedTeamId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);
  const [preferredTeams, setPreferredTeams] = useState(rootData.me?.preferredTeams.map(({ uniqueId }) => uniqueId) ?? []);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditTeam],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteTeam],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.ViewTeamBookings],
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

  useEffect(() => handleRefetch(locationIds), [handleRefetch, locationIds]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.EditTeam:
        if (!teamDetails) {
          return;
        }

        router.push(getOrganizationTeamSetupBaseLink(teamDetails.organization?.uniqueId!, teamDetails.id));
        break;

      case MoreActionsMenuOptionType.DeleteTeam:
        handleRemoveTeamClicked();
        break;

      case MoreActionsMenuOptionType.ViewTeamBookings:
        if (!teamDetails) {
          return;
        }

        router.push(getOrganizationBookingsBaseLink(teamDetails.organization?.uniqueId!, { teamId: teamDetails.id }));
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

    const toastId = themedToast(<NotificationContent content={`Setting team '${teamDetails.name}' as your preferred team...`} />, infoNotificationOptions);

    commitAddCustomerPreferredTeam({
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

        setPreferredTeams(preferredTeams.concat([teamDetails.id]));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set team '${teamDetails.name}' as your preferred team. Error: ${error.message}.`} />,
        });
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

    const toastId = themedToast(<NotificationContent content={`Removing team '${teamDetails.name}' as your preferred team...`} />, infoNotificationOptions);

    commitRemoveCustomerPreferredTeam({
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

        setPreferredTeams(preferredTeams.filter((item) => item !== teamDetails.id));
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handlLocationChanged = (id?: string) => {
    setLocationIds(id ? [id] : []);
  };

  const handlViewModeChanged = (newViewMode: 'list' | 'grid') => {
    setViewMode(newViewMode);
  };

  const rows: RowType[] = teams.map((team) => {
    return {
      id: team.id,
      team,
      teammates: team.members.filter(({ organizationMember }) => !!organizationMember).map(({ organizationMember }) => organizationMember!.customer),
      preferred: preferredTeams.includes(team.id),
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
      field: 'preferred',
      headerName: 'Preferred?',
      editable: false,
      renderCell: (params) => {
        const id = params.id as string;
        if (params.value) {
          return (
            <IconButton onClick={() => handleRemoveAsPreferredTeamClicked(id)}>
              <PreferredIcon />
            </IconButton>
          );
        }

        return (
          <IconButton onClick={() => handleSetAsPreferredTeamClicked(id)}>
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
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
          <LocationSelector rootDataRelay={rootData} onChange={handlLocationChanged} />
          <ListGridToggle defaultValue={viewMode} onChange={handlViewModeChanged} />
          <PushToRight />
          <NewTeamButton organizationId={organizationId} />
        </GridContainer>
        <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding }}>
          <SectionIconTypography label="Teams" />
          <Divider />
          <Box sx={{ paddingBottom: defaultPadding }} />

          {viewMode === 'grid' && (
            <GridContainer>
              {teams.map((team) => (
                <Grid key={team.id}>
                  <TeamCard
                    rootDataRelay={rootData}
                    teamDetailsRelay={team}
                    connectionIds={connectionIds}
                    teammates={team.members.filter(({ organizationMember }) => !!organizationMember)!.map(({ organizationMember }) => organizationMember!.customer)}
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
              localeText={{ noRowsLabel: 'No team found' }}
            />
          )}
        </StackColumn>
      </StackColumn>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />

      {teamDetails && (
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
      )}
    </>
  );
};

const MemoTeams = memo(Teams);

type RelayProps = {
  organizationId: string;
};

const TeamsWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationTeams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        teamsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        locationsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeams queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamsWithRelay);
