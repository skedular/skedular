import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Grid from '@mui/material/Grid2';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { AddIcon, EditIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { DialogTransition } from '@repo/shared/components/transitions';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMemberSelector } from 'components/organization';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useFragment, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { array, object, string } from 'yup';
import type { teamPeopleTab_inviteCustomersToJoinTeamMutation } from './__generated__/teamPeopleTab_inviteCustomersToJoinTeamMutation.graphql';
import type { teamPeopleTab_paginatedTeamMembers_query$key } from './__generated__/teamPeopleTab_paginatedTeamMembers_query.graphql';
import type {
  TeamMemberOrderField,
  TeamMemberOrderInput,
  teamPeopleTab_paginatedTeamMembers_refetchableFragment,
} from './__generated__/teamPeopleTab_paginatedTeamMembers_refetchableFragment.graphql';
import type { teamPeopleTab_query$key } from './__generated__/teamPeopleTab_query.graphql';
import type { teamPeopleTab_rootQuery } from './__generated__/teamPeopleTab_rootQuery.graphql';
import type { teamPeopleTab_updateTeamMutation } from './__generated__/teamPeopleTab_updateTeamMutation.graphql';
import TeamMemberCard from './team-member-card';

type Props = {
  queryReference: PreloadedQuery<teamPeopleTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

const RootQuery = graphql`
  query teamPeopleTab_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $teamId: String!
    $teamExists: Boolean!
    $bookingPeopleNameSearchText: String
    $teamPeopleSortingValues: [TeamMemberOrderInput!]
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $peopleNameSearchText: String
  ) {
    ...teamPeopleTab_query
    ...teamPeopleTab_paginatedTeamMembers_query
  }
`;

type TeamDetails = {
  organizationMemberIds: string[];
};

type MembersToJoin = {
  emails: (string | undefined)[];
};

const teamSchema = object({
  organizationMemberIds: array().nullable(),
});

const membersToInviteSchema = object({
  emails: array()
    .transform(function (value, originalValue) {
      if (this.isType(value) && value !== null) {
        return value;
      }

      return originalValue ? originalValue.split(/[\s,]+/) : [];
    })
    .of(string().email(({ value }) => `${value} is not a valid email`))
    .required('List of emails separated by comma is required'),
});

const TeamPeopleTab = ({ queryReference, organizationId, teamId }: Props) => {
  const rootDataRelay = usePreloadedQuery<teamPeopleTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<teamPeopleTab_query$key>(
    graphql`
      fragment teamPeopleTab_query on Query {
        team(id: $teamId) {
          id
          name
          about
          organization {
            name
          }
          canModify
          members {
            customer {
              uniqueId
            }
            organizationMember {
              uniqueId
            }
          }
        }
        ...teamMemberCard_query
        ...organizationMemberSelector_query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedTeamMembers,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<teamPeopleTab_paginatedTeamMembers_refetchableFragment, teamPeopleTab_paginatedTeamMembers_query$key>(
    graphql`
      fragment teamPeopleTab_paginatedTeamMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "teamPeopleTab_paginatedTeamMembers_refetchableFragment") {
        paginatedTeamMembers(
          first: $count
          after: $cursor
          where: { teamId: $teamId, nameContains: $peopleNameSearchText }
          orderBy: $teamPeopleSortingValues
        ) @connection(key: "teamPeopleTab_paginatedTeamMembers") @include(if: $teamExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...teamMemberCard_TeamMemberDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateTeam] = useMutation<teamPeopleTab_updateTeamMutation>(graphql`
    mutation teamPeopleTab_updateTeamMutation($input: UpdateTeamInput!) @raw_response_type {
      updateTeam(input: $input) {
        team {
          id
          name
          about
          organization {
            name
          }
          members {
            customer {
              uniqueId
            }
            organizationMember {
              uniqueId
            }
          }
        }
      }
    }
  `);

  const [commitInviteCustomersToJoinTeam] = useMutation<teamPeopleTab_inviteCustomersToJoinTeamMutation>(graphql`
    mutation teamPeopleTab_inviteCustomersToJoinTeamMutation($input: InviteCustomersToJoinTeamInput!) {
      inviteCustomersToJoinTeam(input: $input) {
        clientMutationId
      }
    }
  `);

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<TeamMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const { enqueueSnackbar } = useSnackbar();
  const [editingOrganizationMembers, setEditingOrganizationMembers] = useState(false);
  const validateTeam = makeValidate(teamSchema);
  const requiredTeamFields = makeRequired(teamSchema);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validateMembersToInvite = makeValidate(membersToInviteSchema);
  const requiredMembersToInviteFields = makeRequired(membersToInviteSchema);

  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, peopleNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: TeamMemberOrderInput, peopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            teamPeopleSortingValues: [order],
            peopleNameSearchText,
            teamExists: !!teamId,
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {
              setPage(0);
            },
          },
        );
      });
    },
    [refetch, teamId],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  useMemo(
    () => (rootDataPaginatedTeamMembers.paginatedTeamMembers ? [rootDataPaginatedTeamMembers.paginatedTeamMembers.__id] : []),
    [rootDataPaginatedTeamMembers.paginatedTeamMembers],
  );

  // Workaround to ensure we have all the zones if new zones added using zone dialog
  useEffect(() => {
    handleRefetch(pageSize, sortingOrder, peopleNameSearchText);
  }, [handleRefetch, pageSize, sortingOrder, peopleNameSearchText]);

  const handleEditOrganizationMembersClick = () => {
    setEditingOrganizationMembers(true);
  };

  const handleTeamUpdateClick = ({ organizationMemberIds }: TeamDetails) => {
    if (!rootData.team) {
      return;
    }

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.team.id,
          name: rootData.team.name,
          about: rootData.team.about,
          customerIds: [],
          organizationId,
          organizationMemberIds: [...new Set(organizationMemberIds)],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update team '${rootData.team?.name}' members. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setEditingOrganizationMembers(false);
        handleRefetch(pageSize, sortingOrder, peopleNameSearchText);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update team '${rootData.team?.name}' members. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateTeam: {
          team: {
            id: rootData.team.id,
            name: rootData.team.name,
            about: rootData.team.about,
            organization: null,
            members: [],
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    setEditingOrganizationMembers(false);
  };

  const handleInvitePeopleDialogOpenClick = () => {
    setInvitePeopleDialogOpen(true);
  };

  const handleInvitePeopleClick = ({ emails: originalEmailsStr }: MembersToJoin) => {
    if (!rootData.team || !originalEmailsStr) {
      return;
    }

    const emails = originalEmailsStr as unknown as string;
    if (!emails) {
      return;
    }

    commitInviteCustomersToJoinTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: rootData.team.id,
          emails: emails
            .split(/[\s,]+/)
            .map((email) => email.trim())
            .filter((email) => email),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to invite people to join team '${rootData.team?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to invite people to join team '${rootData.team?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  const handleCancelInvitingPeopleClick = () => {
    setInvitePeopleDialogOpen(false);
  };

  if (!rootData.team || !rootDataPaginatedTeamMembers.paginatedTeamMembers) {
    return <></>;
  }

  const teamMemberEdges = rootDataPaginatedTeamMembers.paginatedTeamMembers.edges;
  const count = rootDataPaginatedTeamMembers.paginatedTeamMembers.totalCount ? rootDataPaginatedTeamMembers.paginatedTeamMembers.totalCount : 0;
  const slicedrEdges = teamMemberEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > teamMemberEdges.length ? teamMemberEdges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as TeamMemberOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as TeamMemberOrderField,
      },
      peopleNameSearchText,
    );
  };

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  return (
    <>
      {!organizationId && (
        <Stack direction="row" sx={{ justifyContent: 'flex-start' }} spacing={1}>
          <Grid>
            <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
              Invite People
            </Button>
          </Grid>
        </Stack>
      )}

      {rootData.team?.organization && (
        <Stack direction="row" sx={{ justifyContent: 'flex-end' }} spacing={1}>
          {!editingOrganizationMembers && rootData.team.canModify && (
            <Button size="large" color="primary" onClick={handleEditOrganizationMembersClick}>
              <EditIcon />
            </Button>
          )}
        </Stack>
      )}

      {!editingOrganizationMembers && (
        <>
          <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
            <Search size="small" placeholder="Find a person..." defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <TablePagination
                count={count}
                page={page}
                onPageChange={handleChangePage}
                rowsPerPage={pageSize}
                onRowsPerPageChange={handlePageSizeChange}
              />
              <Sorting
                options={[
                  { id: 'createdAt', label: 'Join date' },
                  { id: 'name', label: 'Name' },
                  { id: 'givenName', label: 'Given name' },
                  { id: 'middleName', label: 'Middle name' },
                  { id: 'familyName', label: 'Family Name' },
                ]}
                defaultOption={sortingOrder.field}
                defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
                onValueChange={handleSortingChanged}
              />
            </Stack>
          </Stack>

          <Grid container spacing={1}>
            {slicedrEdges.map((edge) => (
              <Grid key={edge.node.id}>
                <TeamMemberCard
                  teamMemberDetailsRelay={edge.node}
                  rootDataRelay={rootData}
                  organizationId={organizationId}
                  onRefetchNeeded={() => handleRefetch(pageSize, sortingOrder, peopleNameSearchText)}
                />
              </Grid>
            ))}
          </Grid>
        </>
      )}

      {editingOrganizationMembers && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleTeamUpdateClick}
            initialValues={{
              organizationMemberIds: rootData.team.members
                .filter((member) => member.organizationMember)
                .map(({ organizationMember }) => organizationMember!.uniqueId),
            }}
            validate={validateTeam}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                {rootData.team?.organization && (
                  <OrganizationMemberSelector
                    rootDataRelay={rootData}
                    organizationId={organizationId}
                    name="organizationMemberIds"
                    required={requiredTeamFields.organizationMemberIds}
                    multiple={true}
                    useMemberId={true}
                  />
                )}
                <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Update
                  </Button>
                </Stack>
              </Stack>
            )}
          />
        </Paper>
      )}

      <Dialog TransitionComponent={DialogTransition} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
        <DialogTitle>Invite people to join your team</DialogTitle>
        <DialogContent>
          <DialogContentText>You can enter the list of emails separated by comma</DialogContentText>

          <Form
            onSubmit={handleInvitePeopleClick}
            initialValues={{
              emails: '',
            }}
            validate={validateMembersToInvite}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                <TextField
                  label="Emails"
                  name="emails"
                  required={requiredMembersToInviteFields.emails}
                  multiline={true}
                  helperText="member1@example.com,member2@example.com"
                />
                <DialogActions>
                  <Button color="secondary" variant="contained" onClick={handleCancelInvitingPeopleClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Invite
                  </Button>
                </DialogActions>
              </Stack>
            )}
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

const MemoTeamPeopleTab = memo(TeamPeopleTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

const TeamPeopleTabWithRelay = ({ onReloadRequired, organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teamPeopleTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamId,
        teamExists: !!teamId,
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        teamPeopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        organizationMemberSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId, teamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamPeopleTab queryReference={queryReference} onReloadRequired={handleReloadRequired} teamId={teamId} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamPeopleTabWithRelay);
