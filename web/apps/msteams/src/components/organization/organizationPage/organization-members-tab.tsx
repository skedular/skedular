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
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMemberCard } from 'components/organization';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useFragment, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import type { organizationMembersTab_inviteCustomersToJoinOrganizationMutation } from './__generated__/organizationMembersTab_inviteCustomersToJoinOrganizationMutation.graphql';
import type { organizationMembersTab_paginatedOrganizationMembers_query$key } from './__generated__/organizationMembersTab_paginatedOrganizationMembers_query.graphql';
import type {
  OrganizationMemberOrderField,
  OrganizationMemberOrderInput,
  organizationMembersTab_paginatedOrganizationMembers_refetchableFragment,
} from './__generated__/organizationMembersTab_paginatedOrganizationMembers_refetchableFragment.graphql';
import type { organizationMembersTab_query$key } from './__generated__/organizationMembersTab_query.graphql';
import type { organizationMembersTab_rootQuery } from './__generated__/organizationMembersTab_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<organizationMembersTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMembersTab_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $peopleNameSearchText: String
    $organizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
    ...organizationMembersTab_query
    ...organizationMembersTab_paginatedOrganizationMembers_query
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

const OrganizationMembersTab = ({ queryReference, organizationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationMembersTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<organizationMembersTab_query$key>(
    graphql`
      fragment organizationMembersTab_query on Query {
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
    organizationMembersTab_paginatedOrganizationMembers_refetchableFragment,
    organizationMembersTab_paginatedOrganizationMembers_query$key
  >(
    graphql`
      fragment organizationMembersTab_paginatedOrganizationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationMembersTab_paginatedOrganizationMembers_refetchableFragment") {
        paginatedOrganizationMembers(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $peopleNameSearchText }
          orderBy: $organizationMembersSortingValues
        ) @connection(key: "organizationMembersTab_paginatedOrganizationMembers") @include(if: $organizationExists) {
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

  const [commitInviteCustomersToJoinOrganization] = useMutation<organizationMembersTab_inviteCustomersToJoinOrganizationMutation>(graphql`
    mutation organizationMembersTab_inviteCustomersToJoinOrganizationMutation($input: InviteCustomersToJoinOrganizationInput!) {
      inviteCustomersToJoinOrganization(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<OrganizationMemberOrderInput>({
    direction: 'Ascending',
    field: 'Name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
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

    const toastId = themedToast(
      <NotificationContent content={`Inviting people to join organization '${rootData.organization.name}'...`} />,
      infoNotificationOptions,
    );

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to invite people to join organization '${rootData.organization?.name}'. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation sent to people to join organization ${rootData.organization?.name}.`} />,
        });

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent
              content={`Failed to invite people to join organization '${rootData.organization?.name}'. Error: ${error.message}.`}
            />
          ),
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
            organizationMembersSortingValues: [order],
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
              { id: 'Name', label: 'Name' },
              { id: 'GivenName', label: 'Given name' },
              { id: 'MiddleName', label: 'Middle name' },
              { id: 'FamilyName', label: 'Family Name' },
              { id: 'MembershipType', label: 'Membership type' },
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
              <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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

const MemoOrganizationMembersTab = memo(OrganizationMembersTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationMembersTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMembersTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        organizationExists: !!organizationId,
        organizationMembersSortingValues: [
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

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationMembersTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMembersTabWithRelay);
