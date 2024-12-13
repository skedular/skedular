import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Paper from '@mui/material/Paper';
import TablePagination from '@mui/material/TablePagination';
import { FormStackColumn, GridContainer, StackRow, StackRowFullWidth } from '@repo/shared/components/commons';
import { EditIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMemberSelector } from 'components/organization';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useFragment, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object } from 'yup';
import type { teamMembersTab_query$key } from './__generated__/teamMembersTab_query.graphql';
import type { teamMembersTab_rootQuery } from './__generated__/teamMembersTab_rootQuery.graphql';
import type { teamMembersTab_teamMembers_query$key } from './__generated__/teamMembersTab_teamMembers_query.graphql';
import type {
  TeamMemberOrderField,
  TeamMemberOrderInput,
  teamMembersTab_teamMembers_refetchableFragment,
} from './__generated__/teamMembersTab_teamMembers_refetchableFragment.graphql';
import type { teamMembersTab_updateTeamMutation } from './__generated__/teamMembersTab_updateTeamMutation.graphql';
import TeamMemberCard from './team-member-card';

type Props = {
  queryReference: PreloadedQuery<teamMembersTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

const RootQuery = graphql`
  query teamMembersTab_rootQuery(
    $organizationId: String!
    $teamId: String!
    $teamExists: Boolean!
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

const teamSchema = object({
  organizationMemberIds: array().nullable(),
});

const TeamMembersTab = ({ queryReference, organizationId, teamId }: Props) => {
  const rootDataRelay = usePreloadedQuery<teamMembersTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<teamMembersTab_query$key>(
    graphql`
      fragment teamMembersTab_query on Query {
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
        ) @connection(key: "teamMembersTab_teamMembers") @include(if: $teamExists) {
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

  const [commitUpdateTeam] = useMutation<teamMembersTab_updateTeamMutation>(graphql`
    mutation teamMembersTab_updateTeamMutation($input: UpdateTeamInput!) @raw_response_type {
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
            teamMembersSortingValues: [order],
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
    () => (rootDataPaginatedTeamMembers.teamMembers ? [rootDataPaginatedTeamMembers.teamMembers.__id] : []),
    [rootDataPaginatedTeamMembers.teamMembers],
  );

  const handleEditOrganizationMembersClick = () => {
    setEditingOrganizationMembers(true);
  };

  const handleTeamUpdateClick = ({ organizationMemberIds }: TeamDetails) => {
    if (!rootData.team) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Updating team '${rootData.team.name}' members...`} />, infoNotificationOptions);

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.team.id,
          name: rootData.team.name,
          about: rootData.team.about,
          timezone: rootData.team.timezone,
          customerIds: [],
          organizationId,
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
        handleRefetch(pageSize, sortingOrder, peopleNameSearchText);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to update team '${rootData.team?.name}' members. Error: ${error.message}.`} />,
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

  if (!rootData.team || !rootDataPaginatedTeamMembers.teamMembers) {
    return <></>;
  }

  const teamMemberEdges = rootDataPaginatedTeamMembers.teamMembers.edges;
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
      {!editingOrganizationMembers && (
        <>
          {rootData.team?.organization && rootData.team.canModify && (
            <Button variant="contained" size="small" color="primary" startIcon={<EditIcon />} onClick={handleEditOrganizationMembersClick}>
              Edit Members
            </Button>
          )}

          <StackRowFullWidth>
            <Search size="small" placeholder="Search for members" defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
            <StackRow>
              <TablePagination
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
          </StackRowFullWidth>

          <GridContainer spacing={1}>
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
          </GridContainer>
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
              <FormStackColumn onSubmit={handleSubmit}>
                {rootData.team?.organization && (
                  <OrganizationMemberSelector
                    rootDataRelay={rootData}
                    name="organizationMemberIds"
                    required={requiredTeamFields.organizationMemberIds}
                    multiple={true}
                    useMemberId={true}
                  />
                )}
                <StackRow sx={{ justifyContent: 'flex-end' }}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Update
                  </Button>
                </StackRow>
              </FormStackColumn>
            )}
          />
        </Paper>
      )}
    </>
  );
};

const MemoTeamMembersTab = memo(TeamMembersTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
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
        teamExists: !!teamId,
        organizationId,
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
      <MemoTeamMembersTab queryReference={queryReference} onReloadRequired={handleReloadRequired} teamId={teamId} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamMembersTabWithRelay);
