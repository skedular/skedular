import { OrganizationMemberSelector } from '@/components/organization';
import type {
  TeamMemberOrderField,
  TeamMemberOrderInput,
  teamMembersPaginationQuery,
} from '@/queries/__generated__/teamMembersPaginationQuery.graphql';
import type { teamPeopleTab_inviteCustomersToJoinTeamMutation } from '@/queries/__generated__/teamPeopleTab_inviteCustomersToJoinTeamMutation.graphql';
import type { teamPeopleTab_query$key } from '@/queries/__generated__/teamPeopleTab_query.graphql';
import type { teamPeopleTab_updateTeamMutation } from '@/queries/__generated__/teamPeopleTab_updateTeamMutation.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Box from '@mui/material/Box';
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
import MUITextField from '@mui/material/TextField';
import { AddIcon, EditIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation, usePaginationFragment } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import { array, object, string } from 'yup';
import TeamMemberCard from './team-member-card';

type Props = {
  rootDataRelay: teamPeopleTab_query$key;
  organizationId: string | null;
};

interface TeamDetails {
  organizationMemberIds: string[];
}

interface MembersToJoin {
  emails: (string | undefined)[];
}

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

const TeamPeopleTab = ({ rootDataRelay, organizationId }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<teamMembersPaginationQuery, teamPeopleTab_query$key>(
    graphql`
      fragment teamPeopleTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "teamMembersPaginationQuery") {
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
        paginatedTeamMembers(
          first: $count
          after: $cursor
          where: { teamId: $teamId, nameContains: $peopleNameSearchText }
          orderBy: $teamPeopleSortingValues
        ) @connection(key: "teamPeopleTab_paginatedTeamMembers") {
          __id
          totalCount
          edges {
            node {
              id
              ...teamMemberCard_TeamMemberDetails
            }
          }
        }
        ...teamMemberCard_query
        ...organizationMemberSelector_query
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
  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validateMembersToInvite = makeValidate(membersToInviteSchema);
  const requiredMembersToInviteFields = makeRequired(membersToInviteSchema);

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
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
      refetch(
        {
          count: pageSize,
          teamPeopleSortingValues: [order],
          peopleNameSearchText,
        },
        {
          fetchPolicy: 'store-and-network',
          onComplete: () => {
            setPage(0);
          },
        },
      );
    },
    [refetch],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const connectionIds = useMemo(() => (rootData.paginatedTeamMembers ? [rootData.paginatedTeamMembers.__id] : []), [rootData.paginatedTeamMembers]);

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
          clientMutationId: uuidv4(),
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
        } else {
          setEditingOrganizationMembers(false);
          handleRefetch(pageSize, sortingOrder, peopleNameSearchText);
        }
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
          clientMutationId: uuidv4(),
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
        } else {
          setInvitePeopleDialogOpen(false);
        }
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

  if (!rootData.team) {
    return <></>;
  }

  const teamMemberEdges = rootData.paginatedTeamMembers.edges;
  const count = rootData.paginatedTeamMembers.totalCount ? rootData.paginatedTeamMembers.totalCount : 0;
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

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handlePoepleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };
  const debouncePeopleSearchTextChange = debounce(handlePoepleSearchTextChange, keyboardDebounceTimeout);

  return (
    <>
      {!organizationId && (
        <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
          <Grid>
            <Button variant="contained" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
              Invite People
            </Button>
          </Grid>
        </Grid>
      )}

      {rootData.team?.organization && (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          {!editingOrganizationMembers && rootData.team.canModify && (
            <Button size="large" color="primary" onClick={handleEditOrganizationMembersClick}>
              <EditIcon />
            </Button>
          )}
        </Box>
      )}

      {!editingOrganizationMembers && (
        <>
          <Grid sx={{ marginTop: 1 }}>
            <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
              <AccordionSummary expandIcon={<ExpandMoreIcon />} />
              <AccordionDetails>
                <MUITextField
                  defaultValue={peopleNameSearchText}
                  helperText="Enter name to narrow down the members list"
                  onChange={(event) => debouncePeopleSearchTextChange(event?.target.value)}
                />
              </AccordionDetails>
            </Accordion>
          </Grid>

          <Grid container sx={{ justifyContent: 'flex-end' }}>
            <Grid>
              <TablePagination
                count={count}
                page={page}
                onPageChange={handleChangePage}
                rowsPerPage={pageSize}
                onRowsPerPageChange={handlePageSizeChange}
              />
            </Grid>

            <Grid>
              <Sorting
                options={[
                  { id: 'createdAt', label: 'Join date' },
                  { id: 'name', label: 'Name' },
                  { id: 'givenName', label: 'Given name' },
                  { id: 'middleName', label: 'Middle name' },
                  { id: 'familyName', label: 'Family Name' },
                ]}
                // @ts-expect-error
                defaultOption={sortingOrder.field}
                defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
                onValueChange={handleSortingChanged}
              />
            </Grid>
          </Grid>

          <Grid container spacing={{ xs: 2, md: 3 }}>
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
        <Grid
          container
          sx={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            marginBottom: 1,
          }}
        >
          <Paper elevation={24} sx={{ padding: 3 }}>
            <Form
              onSubmit={handleTeamUpdateClick}
              initialValues={{
                organizationMemberIds: rootData.team.members
                  .filter((member) => member.organizationMember)
                  // @ts-expect-error
                  .map(({ organizationMember }) => organizationMember.uniqueId),
              }}
              validate={validateTeam}
              render={({ handleSubmit }) => (
                <Box
                  component="form"
                  sx={{
                    '& > :not(style)': { m: 1 },
                  }}
                  autoComplete="off"
                  noValidate
                  onSubmit={handleSubmit}
                >
                  {rootData.team?.organization && (
                    <OrganizationMemberSelector
                      rootDataRelay={rootData}
                      name="organizationMemberIds"
                      required={requiredTeamFields.organizationMemberIds}
                      multiple={true}
                      useMemberId={true}
                    />
                  )}
                  <Stack sx={{ flex: 1, justifyContent: 'flex-end' }} direction="row" spacing={2}>
                    <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                      Cancel
                    </Button>
                    <Button color="primary" variant="contained" type="submit">
                      Update
                    </Button>
                  </Stack>
                </Box>
              )}
            />
          </Paper>
        </Grid>
      )}

      <Dialog fullWidth={true} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
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
              <Box
                component="form"
                sx={{
                  '& > :not(style)': { m: 1 },
                }}
                autoComplete="off"
                noValidate
                onSubmit={handleSubmit}
              >
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
              </Box>
            )}
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamPeopleTab);
