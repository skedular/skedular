import { CustomerCard } from '@/components/customer';
import type { locationMembersTab_inviteCustomersToJoinLocationMutation } from '@/queries/__generated__/locationMembersTab_inviteCustomersToJoinLocationMutation.graphql';
import type {
  CustomerOrderField,
  CustomerOrderInput,
  locationMembersTab_organizationMembers_paginatedCustomersByDefaultLocation_refetchableFragment,
} from '@/queries/__generated__/locationMembersTab_organizationMembers_paginatedCustomersByDefaultLocation_refetchableFragment.graphql';
import type { locationMembersTab_paginatedCustomersByDefaultLocation_query$key } from '@/queries/__generated__/locationMembersTab_paginatedCustomersByDefaultLocation_query.graphql';
import type { locationMembersTab_paginatedLocationMembers_query$key } from '@/queries/__generated__/locationMembersTab_paginatedLocationMembers_query.graphql';
import type {
  LocationMemberOrderField,
  LocationMemberOrderInput,
  locationMembersTab_paginatedLocationMembers_refetchableFragment,
} from '@/queries/__generated__/locationMembersTab_paginatedLocationMembers_refetchableFragment.graphql';
import type { locationMembersTab_query$key } from '@/queries/__generated__/locationMembersTab_query.graphql';
import type { locationMembersTab_rootQuery } from '@/queries/__generated__/locationMembersTab_rootQuery.graphql';
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
import LocationMemberCard from './location-member-card';

type Props = {
  queryReference: PreloadedQuery<locationMembersTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const RootQuery = graphql`
  query locationMembersTab_rootQuery(
    $locationId: String!
    $locationExists: Boolean!
    $peopleNameSearchText: String
    $locationMembersSortingValues: [LocationMemberOrderInput!]
    $locationOrganizationMembersSortingValues: [CustomerOrderInput!]
  ) {
    ...locationMembersTab_query
    ...locationMembersTab_paginatedLocationMembers_query
    ...locationMembersTab_paginatedCustomersByDefaultLocation_query
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

const LocationMembersTab = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationMembersTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<locationMembersTab_query$key>(
    graphql`
      fragment locationMembersTab_query on Query {
        location(id: $locationId) {
          id
          name
        }
        ...locationSingleChoiceMembershipType_query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedLocationMembers,
    loadNext: loadNextPaginatedLocationMembers,
    isLoadingNext: isLoadingNextPaginatedLocationMembers,
    refetch: refetchLocationMembers,
  } = usePaginationFragment<locationMembersTab_paginatedLocationMembers_refetchableFragment, locationMembersTab_paginatedLocationMembers_query$key>(
    graphql`
      fragment locationMembersTab_paginatedLocationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationMembersTab_paginatedLocationMembers_refetchableFragment") {
        paginatedLocationMembers(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationMembersSortingValues
        ) @connection(key: "locationMembersTab_paginatedLocationMembers") @include(if: $locationExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...locationMemberCard_LocationMemberDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const {
    data: rootDataPaginatedCustomersByDefaultLocation,
    loadNext: loadNextPaginatedCustomersByDefaultLocation,
    isLoadingNext: isLoadingNextPaginatedCustomersByDefaultLocation,
    refetch: refetchPaginatedCustomersByDefaultLocation,
  } = usePaginationFragment<
    locationMembersTab_organizationMembers_paginatedCustomersByDefaultLocation_refetchableFragment,
    locationMembersTab_paginatedCustomersByDefaultLocation_query$key
  >(
    graphql`
      fragment locationMembersTab_paginatedCustomersByDefaultLocation_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationMembersTab_organizationMembers_paginatedCustomersByDefaultLocation_refetchableFragment") {
        paginatedCustomersByDefaultLocation(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationOrganizationMembersSortingValues
        ) @connection(key: "locationMembersTab_paginatedCustomersByDefaultLocation") @include(if: $locationExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...customerCard_CustomerDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const { enqueueSnackbar } = useSnackbar();
  const [commitInviteCustomersToJoinLocation] = useMutation<locationMembersTab_inviteCustomersToJoinLocationMutation>(graphql`
    mutation locationMembersTab_inviteCustomersToJoinLocationMutation($input: InviteCustomersToJoinLocationInput!) {
      inviteCustomersToJoinLocation(input: $input) {
        clientMutationId
      }
    }
  `);

  const [sortingLocationMemberOrder, setSortingLocationMemberOrder] = useState<LocationMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [sortingCustomerOrder, setSortingCustomerOrder] = useState<CustomerOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const handleChangePage = (_: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingLocationMemberOrder, sortingCustomerOrder, peopleNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, locationMemberOrder: LocationMemberOrderInput, customerOrder: CustomerOrderInput, peopleNameSearchText: string) => {
      startTransition(() => {
        if (organizationId) {
          refetchPaginatedCustomersByDefaultLocation(
            {
              count: pageSize,
              locationOrganizationMembersSortingValues: [customerOrder],
              peopleNameSearchText,
              locationExists: !!locationId,
            },
            {
              fetchPolicy: 'store-and-network',
              onComplete: () => {
                setPage(0);
              },
            },
          );
        } else {
          refetchLocationMembers(
            {
              count: pageSize,
              locationMembersSortingValues: [locationMemberOrder],
              peopleNameSearchText,
              locationExists: !!locationId,
            },
            {
              fetchPolicy: 'store-and-network',
              onComplete: () => {
                setPage(0);
              },
            },
          );
        }
      });
    },
    [refetchLocationMembers, refetchPaginatedCustomersByDefaultLocation, organizationId, locationId],
  );

  const loadNextPage = useCallback(() => {
    if (organizationId) {
      if (isLoadingNextPaginatedCustomersByDefaultLocation) {
        return;
      }

      loadNextPaginatedCustomersByDefaultLocation(pageSize);
    } else {
      if (isLoadingNextPaginatedLocationMembers) {
        return;
      }

      loadNextPaginatedLocationMembers(pageSize);
    }
  }, [
    organizationId,
    loadNextPaginatedLocationMembers,
    isLoadingNextPaginatedLocationMembers,
    loadNextPaginatedCustomersByDefaultLocation,
    isLoadingNextPaginatedCustomersByDefaultLocation,
    pageSize,
  ]);

  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validateMembersToInvite = makeValidate(membersToInviteSchema);
  const requiredMembersToInviteFields = makeRequired(membersToInviteSchema);

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingLocationMemberOrder, sortingCustomerOrder, str);
  };

  const connectionIds = useMemo(() => {
    if (organizationId) {
      if (!rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation) {
        return [];
      }

      return [rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.__id];
    } else {
      return rootDataPaginatedLocationMembers.paginatedLocationMembers ? [rootDataPaginatedLocationMembers.paginatedLocationMembers.__id] : [];
    }
  }, [
    organizationId,
    rootDataPaginatedLocationMembers.paginatedLocationMembers,
    rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation,
  ]);

  if (
    !rootData.location ||
    !rootDataPaginatedLocationMembers.paginatedLocationMembers ||
    !rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation
  ) {
    return <></>;
  }

  const locationMemberEdges = rootDataPaginatedLocationMembers.paginatedLocationMembers.edges;
  const organizationMemberEdges = rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.edges;
  const count = organizationId
    ? rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.totalCount
      ? rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.totalCount
      : 0
    : rootDataPaginatedLocationMembers.paginatedLocationMembers.totalCount
      ? rootDataPaginatedLocationMembers.paginatedLocationMembers.totalCount
      : 0;
  const slicedLocationMemberEdges = locationMemberEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > locationMemberEdges.length ? locationMemberEdges.length : page * pageSize + pageSize,
  );
  const slicedOrganizationMemberEdges = organizationMemberEdges?.slice(
    page * pageSize,
    page * pageSize + pageSize > organizationMemberEdges.length ? organizationMemberEdges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingLocationMemberOrder({
      direction,
      field: value as unknown as LocationMemberOrderField,
    });

    setSortingCustomerOrder({
      direction,
      field: value as unknown as CustomerOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as LocationMemberOrderField,
      },
      {
        direction,
        field: value as unknown as CustomerOrderField,
      },
      peopleNameSearchText,
    );
  };

  const handleInvitePeopleDialogOpenClick = () => {
    setInvitePeopleDialogOpen(true);
  };

  const handleInvitePeopleClick = ({ emails: originalEmailsStr }: MembersToJoin) => {
    if (!rootData.location || !originalEmailsStr) {
      return;
    }

    const emails = originalEmailsStr as unknown as string;
    if (!emails) {
      return;
    }

    commitInviteCustomersToJoinLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: rootData.location.id,
          emails: emails
            .split(/[\s,]+/)
            .map((email) => email.trim())
            .filter((email) => email),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to invite member to join location '${rootData.location?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to invite member to join location '${rootData.location?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  const handleCancelInvitingPeopleClick = () => {
    setInvitePeopleDialogOpen(false);
  };

  return (
    <>
      {!organizationId && (
        <Stack direction="row" sx={{ justifyContent: 'flex-start' }} spacing={1}>
          <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
            Invite Member
          </Button>
        </Stack>
      )}

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
              { id: 'name', label: 'Name' },
              { id: 'givenName', label: 'Given name' },
              { id: 'middleName', label: 'Middle name' },
              { id: 'familyName', label: 'Family Name' },
              { id: 'membershipType', label: 'Membership type' },
              { id: 'createdAt', label: 'Join date' },
            ]}
            defaultOption={organizationId ? sortingCustomerOrder.field : sortingLocationMemberOrder.field}
            defaultSortingDirectionValue={
              organizationId
                ? (sortingCustomerOrder.direction as unknown as Direction)
                : (sortingLocationMemberOrder.direction as unknown as Direction)
            }
            onValueChange={handleSortingChanged}
          />
        </Stack>
      </Stack>

      <Grid container spacing={1}>
        {organizationId &&
          slicedOrganizationMemberEdges &&
          slicedOrganizationMemberEdges.map((edge) => (
            <Grid key={edge.node.id}>
              <CustomerCard customerDetailsRelay={edge.node} />
            </Grid>
          ))}
        {!organizationId &&
          slicedLocationMemberEdges.map((edge) => (
            <Grid key={edge.node.id}>
              <LocationMemberCard data={rootData} locationMemberDetailsRelay={edge.node} connectionIds={connectionIds} />
            </Grid>
          ))}
      </Grid>

      <Dialog TransitionComponent={DialogTransition} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
        <DialogTitle>Invite member to join your location</DialogTitle>
        <DialogContent>
          <DialogContentText>You can enter the list of emails separated by comma</DialogContentText>

          <Form
            onSubmit={handleInvitePeopleClick}
            initialValues={{
              emails: '',
            }}
            validate={validateMembersToInvite}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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

const MemoLocationMembersTab = memo(LocationMembersTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const LocationMembersTabWithRelay = ({ onReloadRequired, organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationMembersTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        locationMembersSortingValues: [
          {
            direction: 'Descending',
            field: 'name',
          },
        ],
        locationOrganizationMembersSortingValues: [
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
  }, [loadQuery, triggerReloadId, organizationId, locationId]);

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
      <MemoLocationMembersTab
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationMembersTabWithRelay);
