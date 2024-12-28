import { OrganizationMemberSelector } from '@/components/organization';
import type { teamMembersTab_inviteCustomersToJoinTeamMutation } from '@/queries/__generated__/teamMembersTab_inviteCustomersToJoinTeamMutation.graphql';
import type { teamMembersTab_query$key } from '@/queries/__generated__/teamMembersTab_query.graphql';
import type { teamMembersTab_refetchableFragment } from '@/queries/__generated__/teamMembersTab_refetchableFragment.graphql';
import type { teamMembersTab_rootQuery } from '@/queries/__generated__/teamMembersTab_rootQuery.graphql';
import type { teamMembersTab_teamMembers_query$key } from '@/queries/__generated__/teamMembersTab_teamMembers_query.graphql';
import type {
  TeamMemberOrderField,
  TeamMemberOrderInput,
  teamMembersTab_teamMembers_refetchableFragment,
} from '@/queries/__generated__/teamMembersTab_teamMembers_refetchableFragment.graphql';
import type { teamMembersTab_updateTeamMembersMutation } from '@/queries/__generated__/teamMembersTab_updateTeamMembersMutation.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Grid from '@mui/material/Grid2';
import Paper from '@mui/material/Paper';
import TablePagination from '@mui/material/TablePagination';
import { FormStackColumn, GridContainer, PushToRight, StackRow, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { AddIcon, EditIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, graphql, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import TeamMemberCard from './team-member-card';

type Props = {
  queryReference: PreloadedQuery<teamMembersTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
};

const RootQuery = graphql`
  query teamMembersTab_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $teamId: String!
    $bookingPeopleNameSearchText: String
    $teamMembersSortingValues: [TeamMemberOrderInput!]
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $peopleNameSearchText: String
  ) {
    ...teamMembersTab_query
    ...teamMembersTab_teamMembers_query
  }
`;

type TeamDetails = {
  organizationMemberIds: string[];
};

type PeopleToJoin = {
  emails: (string | undefined)[];
};

const teamSchema = object({
  organizationMemberIds: array().nullable(),
});

const peopleToInviteSchema = object({
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

const TeamMembersTab = ({ queryReference, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<teamMembersTab_rootQuery>(RootQuery, queryReference);
  const [rootData, refetch] = useRefetchableFragment<teamMembersTab_refetchableFragment, teamMembersTab_query$key>(
    graphql`
      fragment teamMembersTab_query on Query @refetchable(queryName: "teamMembersTab_refetchableFragment") {
        team(id: $teamId) {
          id
          name
          about
          timezone
          organization {
            name
          }
          canModify
          members {
            id
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
    refetch: refetchTeamMembers,
  } = usePaginationFragment<teamMembersTab_teamMembers_refetchableFragment, teamMembersTab_teamMembers_query$key>(
    graphql`
      fragment teamMembersTab_teamMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "teamMembersTab_teamMembers_refetchableFragment") {
        teamMembers(
          first: $count
          after: $cursor
          where: { teamId: $teamId, nameContains: $peopleNameSearchText }
          orderBy: $teamMembersSortingValues
        ) @connection(key: "teamMembersTab_teamMembers") {
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

  const [commitUpdateTeamMembers] = useMutation<teamMembersTab_updateTeamMembersMutation>(graphql`
    mutation teamMembersTab_updateTeamMembersMutation($input: UpdateTeamMembersInput!) @raw_response_type {
      updateTeamMembers(input: $input) {
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

  const [commitInviteCustomersToJoinTeam] = useMutation<teamMembersTab_inviteCustomersToJoinTeamMutation>(graphql`
    mutation teamMembersTab_inviteCustomersToJoinTeamMutation($input: InviteCustomersToJoinTeamInput!) {
      inviteCustomersToJoinTeam(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<TeamMemberOrderInput>({
    direction: 'Ascending',
    field: 'Name',
  });
  const [editingOrganizationMembers, setEditingOrganizationMembers] = useState(false);
  const validateTeam = makeValidate(teamSchema);
  const requiredTeamFields = makeRequired(teamSchema);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validateMembersToInvite = makeValidate(peopleToInviteSchema);
  const requiredMembersToInviteFields = makeRequired(peopleToInviteSchema);
  const connectionIds = useMemo(
    () => (rootDataPaginatedTeamMembers.teamMembers ? [rootDataPaginatedTeamMembers.teamMembers.__id] : []),
    [rootDataPaginatedTeamMembers.teamMembers],
  );
  const teamMemberEdges = useMemo(
    () => (rootDataPaginatedTeamMembers.teamMembers ? rootDataPaginatedTeamMembers.teamMembers.edges : []),
    [rootDataPaginatedTeamMembers.teamMembers],
  );

  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetchTeamMembers(pageSize, sortingOrder, peopleNameSearchText);
  };

  const handleRefetch = useCallback(() => {
    startTransition(() => {
      refetch(
        {},
        {
          fetchPolicy: 'store-and-network',
          onComplete: () => {
            setPage(0);
          },
        },
      );
    });
  }, [refetch]);

  const handleRefetchTeamMembers = useCallback(
    (pageSize: number, order: TeamMemberOrderInput, peopleNameSearchText: string) => {
      startTransition(() => {
        refetchTeamMembers(
          {
            count: pageSize,
            teamMembersSortingValues: [order],
            peopleNameSearchText,
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
    [refetchTeamMembers],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const handleEditOrganizationMembersClick = () => {
    setEditingOrganizationMembers(true);
  };

  const handleTeamUpdateClick = ({ organizationMemberIds }: TeamDetails) => {
    if (!rootData.team) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating team '${rootData.team.name}' members...`} />, infoNotificationOptions);

    commitUpdateTeamMembers({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.team.id,
          customerIds: [],
          organizationMemberIds: [...new Set(organizationMemberIds)],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to update team '${rootData.team?.name}' members. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team ${rootData.team?.name} members updated.`} />,
        });

        setEditingOrganizationMembers(false);
        handleRefetchTeamMembers(pageSize, sortingOrder, peopleNameSearchText);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update team '${rootData.team?.name}' members. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        updateTeamMembers: {
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

  const handleInvitePeopleClick = ({ emails: originalEmailsStr }: PeopleToJoin) => {
    if (!rootData.team || !originalEmailsStr) {
      return;
    }

    const emails = originalEmailsStr as unknown as string;
    if (!emails) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Inviting people to join team '${rootData.team.name}'...`} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to invite people to join team '${rootData.team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation sent to people to join team ${rootData.team?.name}.`} />,
        });

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to invite people to join team '${rootData.team?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCancelInvitingPeopleClick = () => {
    setInvitePeopleDialogOpen(false);
  };

  if (!rootData.team || !rootDataPaginatedTeamMembers.teamMembers) {
    return <></>;
  }

  const count = rootDataPaginatedTeamMembers.teamMembers.totalCount ? rootDataPaginatedTeamMembers.teamMembers.totalCount : 0;
  const slicedrEdges = teamMemberEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > teamMemberEdges.length ? teamMemberEdges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as TeamMemberOrderField,
    });

    handleRefetchTeamMembers(
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

    handleRefetchTeamMembers(pageSize, sortingOrder, str);
  };
  return (
    <>
      {!organizationId && (
        <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
          Invite People
        </Button>
      )}

      {!editingOrganizationMembers && (
        <>
          {rootData.team?.organization && rootData.team.canModify && (
            <Button variant="contained" size="small" color="primary" startIcon={<EditIcon />} onClick={handleEditOrganizationMembersClick}>
              Edit Members
            </Button>
          )}

          <StackRow>
            <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
            <PushToRight />
            <TablePagination
              component="div"
              count={count}
              page={page}
              onPageChange={handleChangePage}
              rowsPerPage={pageSize}
              onRowsPerPageChange={handlePageSizeChange}
            />
            <Sorting
              options={[
                { id: 'Name', label: 'Name' },
                { id: 'GivenName', label: 'Given name' },
                { id: 'MiddleName', label: 'Middle name' },
                { id: 'FamilyName', label: 'Family Name' },
              ]}
              defaultOption={sortingOrder.field}
              defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
              onValueChange={handleSortingChanged}
            />
          </StackRow>

          <GridContainer>
            {slicedrEdges.map((edge) => (
              <Grid key={edge.node.id}>
                <TeamMemberCard
                  teamMemberDetailsRelay={edge.node}
                  rootDataRelay={rootData}
                  connectionIds={connectionIds}
                  onRefetchNeeded={handleRefetch}
                />
              </Grid>
            ))}
          </GridContainer>
        </>
      )}

      {editingOrganizationMembers && (
        <Paper sx={{ padding: 2 }}>
          <Form
            onSubmit={handleTeamUpdateClick}
            initialValues={{
              organizationMemberIds: rootData.team.members
                .filter((member) => member.organizationMember)
                .map(({ organizationMember }) => organizationMember!.uniqueId),
            }}
            validate={validateTeam}
            render={({ handleSubmit }) => (
              <FormStackColumn onSubmit={handleSubmit}>
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
                <TwoButtonsDialogActions onSecondaryClicked={handleCancelClick} primaryLabel="Update" secondaryLabel="Cancel" />
              </FormStackColumn>
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
              <FormStackColumn onSubmit={handleSubmit}>
                <TextField
                  label="Emails"
                  name="emails"
                  required={requiredMembersToInviteFields.emails}
                  multiline={true}
                  helperText="member1@example.com,member2@example.com"
                />
                <TwoButtonsDialogActions onSecondaryClicked={handleCancelInvitingPeopleClick} primaryLabel="Invite" secondaryLabel="Cancel" />
              </FormStackColumn>
            )}
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

const MemoTeamMembersTab = memo(TeamMembersTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  teamId: string;
};

const TeamMembersTabWithRelay = ({ onReloadRequired, organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teamMembersTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        teamId,
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        teamMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
        organizationMemberSelectorOrganizationMembersSortingValues: [
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
      <MemoTeamMembersTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamMembersTabWithRelay);
