import { OrganizationMemberCard } from '@/components/organization';
import type { organizationPeopleTab_inviteCustomersToJoinOrganizationMutation } from '@/queries/__generated__/organizationPeopleTab_inviteCustomersToJoinOrganizationMutation.graphql';
import type { organizationPeopleTab_paginatedOrganizationMembers_query$key } from '@/queries/__generated__/organizationPeopleTab_paginatedOrganizationMembers_query.graphql';
import type {
  OrganizationMemberOrderField,
  OrganizationMemberOrderInput,
  organizationPeopleTab_paginatedOrganizationMembers_refetchableFragment,
} from '@/queries/__generated__/organizationPeopleTab_paginatedOrganizationMembers_refetchableFragment.graphql';
import type { organizationPeopleTab_query$key } from '@/queries/__generated__/organizationPeopleTab_query.graphql';
import type { organizationPeopleTab_rootQuery } from '@/queries/__generated__/organizationPeopleTab_rootQuery.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Search } from '@repo/shared/components/search';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { DialogTransition } from '@repo/shared/components/transitions';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, graphql, useFragment, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { array, object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<organizationPeopleTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationPeopleTab_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $peopleNameSearchText: String
    $organizationPeopleSortingValues: [OrganizationMemberOrderInput!]
  ) {
    ...organizationPeopleTab_query
    ...organizationPeopleTab_paginatedOrganizationMembers_query
  }
`;

type MembersToJoin = {
  emails: (string | undefined)[];
};

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

const OrganizationPeopleTab = ({ queryReference, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationPeopleTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationPeopleTab_query$key>(
    graphql`
      fragment organizationPeopleTab_query on Query {
        organization(id: $organizationId) {
          id
          name
          canInvitePeople
        }
        ...organizationSingleChoiceMembershipType_query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedOrganizationMembers,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<
    organizationPeopleTab_paginatedOrganizationMembers_refetchableFragment,
    organizationPeopleTab_paginatedOrganizationMembers_query$key
  >(
    graphql`
      fragment organizationPeopleTab_paginatedOrganizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationPeopleTab_paginatedOrganizationMembers_refetchableFragment") {
        paginatedOrganizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $peopleNameSearchText }
          orderBy: $organizationPeopleSortingValues
        ) @connection(key: "organizationPeopleTab_paginatedOrganizationMembers") @include(if: $organizationExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...organizationMemberCard_OrganizationMemberDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [commitInviteCustomersToJoinOrganization] = useMutation<organizationPeopleTab_inviteCustomersToJoinOrganizationMutation>(graphql`
    mutation organizationPeopleTab_inviteCustomersToJoinOrganizationMutation($input: InviteCustomersToJoinOrganizationInput!) {
      inviteCustomersToJoinOrganization(input: $input) {
        clientMutationId
      }
    }
  `);

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<OrganizationMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const { enqueueSnackbar } = useSnackbar();
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validate = makeValidate(membersToInviteSchema);
  const requiredFields = makeRequired(membersToInviteSchema);

  const handleInvitePeopleDialogOpenClick = () => {
    setInvitePeopleDialogOpen(true);
  };

  const handleInvitePeopleClick = ({ emails: originalEmailsStr }: MembersToJoin) => {
    if (!rootData.organization || !originalEmailsStr) {
      return;
    }

    const emails = originalEmailsStr as unknown as string;
    if (!emails) {
      return;
    }

    commitInviteCustomersToJoinOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: rootData.organization.id,
          emails: emails
            .split(/[\s,]+/)
            .map((email) => email.trim())
            .filter((email) => email),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to invite people to join organization '${rootData.organization?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to invite people to join organization '${rootData.organization?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  const handleCancelInvitingPeopleClick = () => {
    setInvitePeopleDialogOpen(false);
  };

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
    (pageSize: number, order: OrganizationMemberOrderInput, peopleNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            organizationExists: !!organizationId,
            organizationPeopleSortingValues: [order],
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
    [refetch, organizationId],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const connectionIds = useMemo(
    () =>
      rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers
        ? [rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers.__id]
        : [],
    [rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers],
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as OrganizationMemberOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as OrganizationMemberOrderField,
      },
      peopleNameSearchText,
    );
  };

  if (!rootData.organization || !rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers) {
    return <></>;
  }

  const organizationMemberEdges = rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers.edges;
  const slicedEdges = organizationMemberEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > organizationMemberEdges.length ? organizationMemberEdges.length : page * pageSize + pageSize,
  );

  return (
    <>
      {rootData.organization.canInvitePeople && (
        <Stack direction="row" sx={{ width: 'auto' }}>
          <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
            Invite People
          </Button>
        </Stack>
      )}

      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap' }}>
        <Search size="small" placeholder="Find a person..." defaultValue={peopleNameSearchText} onChange={handleSearchTextChange} />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TablePagination
            count={
              rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers.totalCount
                ? rootDataPaginatedOrganizationMembers.paginatedOrganizationMembers.totalCount
                : 0
            }
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
          <Sorting
            options={[
              { id: 'name', label: 'Name' },
              { id: 'givenName', label: 'Given name' },
              { id: 'middleName', label: 'Middle name' },
              { id: 'familyName', label: 'Family Name' },
              { id: 'membershipType', label: 'Membership type' },
              { id: 'createdAt', label: 'Join date' },
            ]}
            defaultOption={sortingOrder.field}
            defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
            onValueChange={handleSortingChanged}
          />
        </Stack>
      </Stack>

      <Grid container spacing={1}>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <OrganizationMemberCard data={rootData} organizationMemberDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>

      <Dialog TransitionComponent={DialogTransition} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
        <DialogTitle>Invite people to join your organization</DialogTitle>
        <DialogContent>
          <DialogContentText>You can enter the list of emails separated by comma</DialogContentText>

          <Form
            onSubmit={handleInvitePeopleClick}
            initialValues={{
              emails: '',
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                <TextField
                  label="Emails"
                  name="emails"
                  required={requiredFields.emails}
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

const MemoOrganizationPeopleTab = memo(OrganizationPeopleTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationPeopleTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationPeopleTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        organizationExists: !!organizationId,
        organizationPeopleSortingValues: [
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
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoOrganizationPeopleTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationPeopleTabWithRelay);
