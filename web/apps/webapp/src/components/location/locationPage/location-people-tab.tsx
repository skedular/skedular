import { CustomerCard } from '@/components/customer';
import type {
  LocationMemberOrderField,
  LocationMemberOrderInput,
  locationMembers_PaginationQuery,
} from '@/queries/__generated__/locationMembers_PaginationQuery.graphql';
import type { locationPeopleTab_inviteCustomersToJoinLocationMutation } from '@/queries/__generated__/locationPeopleTab_inviteCustomersToJoinLocationMutation.graphql';
import type { locationPeopleTab_query$key } from '@/queries/__generated__/locationPeopleTab_query.graphql';
import type { locationPeopleTab_query_organizationMembers$key } from '@/queries/__generated__/locationPeopleTab_query_organizationMembers.graphql';
import type {
  CustomerOrderField,
  CustomerOrderInput,
  locationPeopleTab_query_paginatedCustomersByDefaultLocation,
} from '@/queries/__generated__/locationPeopleTab_query_paginatedCustomersByDefaultLocation.graphql';
import type { locationPeopleTab_rootQuery } from '@/queries/__generated__/locationPeopleTab_rootQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import MUITextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { DialogTransition } from '@repo/shared/components/transitions';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, graphql, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { array, object, string } from 'yup';
import LocationMemberCard from './location-member-card';

type Props = {
  queryReference: PreloadedQuery<locationPeopleTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const RootQuery = graphql`
  query locationPeopleTab_rootQuery(
    $locationId: String!
    $locationExists: Boolean!
    $peopleNameSearchText: String
    $locationPeopleSortingValues: [LocationMemberOrderInput!]
    $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]
  ) {
    ...locationPeopleTab_query
    ...locationPeopleTab_query_organizationMembers
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

const LocationPeopleTab = ({ queryReference, onReloadRequired, organizationId, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationPeopleTab_rootQuery>(RootQuery, queryReference);
  const {
    data: rootDataLocation,
    loadNext: loadNextLocationMembers,
    isLoadingNext: isLoadingNextLocationMembers,
    refetch: refetchLocationMembers,
  } = usePaginationFragment<locationMembers_PaginationQuery, locationPeopleTab_query$key>(
    graphql`
      fragment locationPeopleTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationMembers_PaginationQuery") {
        location(id: $locationId) {
          id
          name
        }
        paginatedLocationMembers(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationPeopleSortingValues
        ) @connection(key: "locationPeopleTab_paginatedLocationMembers") @include(if: $locationExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...locationMemberCard_LocationMemberDetails
            }
          }
        }
        ...locationSingleChoiceMembershipType_query
      }
    `,
    rootDataRelay,
  );

  const {
    data: rootDataPaginatedCustomersByDefaultLocation,
    loadNext: loadNextpaginatedCustomersByDefaultLocation,
    isLoadingNext: isLoadingNextpaginatedCustomersByDefaultLocation,
    refetch: refetchPaginatedCustomersByDefaultLocation,
  } = usePaginationFragment<locationPeopleTab_query_paginatedCustomersByDefaultLocation, locationPeopleTab_query_organizationMembers$key>(
    graphql`
      fragment locationPeopleTab_query_organizationMembers on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationPeopleTab_query_paginatedCustomersByDefaultLocation") {
        paginatedCustomersByDefaultLocation(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationOrganizationPeopleSortingValues
        ) @connection(key: "locationPeopleTab_paginatedCustomersByDefaultLocation") @include(if: $locationExists) {
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
  const [commitInviteCustomersToJoinLocation] = useMutation<locationPeopleTab_inviteCustomersToJoinLocationMutation>(graphql`
    mutation locationPeopleTab_inviteCustomersToJoinLocationMutation($input: InviteCustomersToJoinLocationInput!) {
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
  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
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
              locationOrganizationPeopleSortingValues: [customerOrder],
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
              locationPeopleSortingValues: [locationMemberOrder],
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
      if (isLoadingNextpaginatedCustomersByDefaultLocation) {
        return;
      }

      loadNextpaginatedCustomersByDefaultLocation(pageSize);
    } else {
      if (isLoadingNextLocationMembers) {
        return;
      }

      loadNextLocationMembers(pageSize);
    }
  }, [
    organizationId,
    loadNextLocationMembers,
    isLoadingNextLocationMembers,
    loadNextpaginatedCustomersByDefaultLocation,
    isLoadingNextpaginatedCustomersByDefaultLocation,
    pageSize,
  ]);

  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validateMembersToInvite = makeValidate(membersToInviteSchema);
  const requiredMembersToInviteFields = makeRequired(membersToInviteSchema);

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingLocationMemberOrder, sortingCustomerOrder, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

  const connectionIds = useMemo(() => {
    if (organizationId) {
      if (!rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation) {
        return [];
      }

      return [rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.__id];
    } else {
      return rootDataLocation.paginatedLocationMembers ? [rootDataLocation.paginatedLocationMembers.__id] : [];
    }
  }, [organizationId, rootDataLocation.paginatedLocationMembers, rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation]);

  if (
    !rootDataLocation.location ||
    !rootDataLocation.paginatedLocationMembers ||
    !rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation
  ) {
    return <></>;
  }

  const locationMemberEdges = rootDataLocation.paginatedLocationMembers.edges;
  const organizationMemberEdges = rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.edges;
  const count = organizationId
    ? rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.totalCount
      ? rootDataPaginatedCustomersByDefaultLocation.paginatedCustomersByDefaultLocation.totalCount
      : 0
    : rootDataLocation.paginatedLocationMembers.totalCount
      ? rootDataLocation.paginatedLocationMembers.totalCount
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
    if (!rootDataLocation.location || !originalEmailsStr) {
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
          locationId: rootDataLocation.location.id,
          emails: emails
            .split(/[\s,]+/)
            .map((email) => email.trim())
            .filter((email) => email),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to invite people to join location '${rootDataLocation.location?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to invite people to join location '${rootDataLocation.location?.name}'. Error: ${error.message}`, {
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
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
            Invite People
          </Button>
        </Stack>
      )}

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />} />
        <AccordionDetails>
          <MUITextField
            defaultValue={peopleNameSearchText}
            helperText="Enter name to narrow down the people list"
            onChange={(event) => debounceSearchTextChange(event?.target.value)}
          />
        </AccordionDetails>
      </Accordion>

      <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
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
            organizationId ? (sortingCustomerOrder.direction as unknown as Direction) : (sortingLocationMemberOrder.direction as unknown as Direction)
          }
          onValueChange={handleSortingChanged}
        />
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
              <LocationMemberCard data={rootDataLocation} locationMemberDetailsRelay={edge.node} connectionIds={connectionIds} />
            </Grid>
          ))}
      </Grid>

      <Dialog TransitionComponent={DialogTransition} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
        <DialogTitle>Invite people to join your location</DialogTitle>
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

const MemoLocationPeopleTab = memo(LocationPeopleTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const LocationPeopleTabWithRelay = ({ onReloadRequired, organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationPeopleTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        locationPeopleSortingValues: [
          {
            direction: 'Descending',
            field: 'name',
          },
        ],
        locationOrganizationPeopleSortingValues: [
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
      <MemoLocationPeopleTab
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationId={organizationId}
        locationId={locationId}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationPeopleTabWithRelay);
