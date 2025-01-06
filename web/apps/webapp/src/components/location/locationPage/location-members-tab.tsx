import type { locationMembersTab_inviteCustomersToJoinLocationMutation } from '@/queries/__generated__/locationMembersTab_inviteCustomersToJoinLocationMutation.graphql';
import type { locationMembersTab_locationMembers_query$key } from '@/queries/__generated__/locationMembersTab_locationMembers_query.graphql';
import type {
  LocationMemberOrderField,
  LocationMemberOrderInput,
  locationMembersTab_locationMembers_refetchableFragment,
} from '@/queries/__generated__/locationMembersTab_locationMembers_refetchableFragment.graphql';
import type { locationMembersTab_query$key } from '@/queries/__generated__/locationMembersTab_query.graphql';
import type { locationMembersTab_rootQuery } from '@/queries/__generated__/locationMembersTab_rootQuery.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import {
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  GridContainer,
  PushToRight,
  StackRow,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
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
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useCallback, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, graphql, useFragment, useMutation, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import LocationMemberCard from './location-member-card';

type Props = {
  queryReference: PreloadedQuery<locationMembersTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootQuery = graphql`
  query locationMembersTab_rootQuery(
    $locationId: String!
    $peopleNameSearchText: String
    $locationMembersSortingValues: [LocationMemberOrderInput!]
  ) {
    ...locationMembersTab_query
    ...locationMembersTab_locationMembers_query
  }
`;

type PeopleToJoin = {
  emails: (string | undefined)[];
};

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

const LocationMembersTab = ({ queryReference, locationId }: Props) => {
  const rootDataRelay = usePreloadedQuery<locationMembersTab_rootQuery>(RootQuery, queryReference);
  const rootData = useFragment<locationMembersTab_query$key>(
    graphql`
      fragment locationMembersTab_query on Query {
        location(id: $locationId) {
          id
          name
        }
        ...locationSingleChoiceMemberRole_query
      }
    `,
    rootDataRelay,
  );
  const {
    data: rootDataPaginatedLocationMembers,
    loadNext: loadNextPaginatedLocationMembers,
    isLoadingNext: isLoadingNextPaginatedLocationMembers,
    refetch: refetchLocationMembers,
  } = usePaginationFragment<locationMembersTab_locationMembers_refetchableFragment, locationMembersTab_locationMembers_query$key>(
    graphql`
      fragment locationMembersTab_locationMembers_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationMembersTab_locationMembers_refetchableFragment") {
        locationMembers(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationMembersSortingValues
        ) @connection(key: "locationMembersTab_locationMembers") {
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [, startTransition] = useTransition();
  const [commitInviteCustomersToJoinLocation] = useMutation<locationMembersTab_inviteCustomersToJoinLocationMutation>(graphql`
    mutation locationMembersTab_inviteCustomersToJoinLocationMutation($input: InviteCustomersToJoinLocationInput!) {
      inviteCustomersToJoinLocation(input: $input) {
        clientMutationId
      }
    }
  `);

  const [sortingLocationMemberOrder, setSortingLocationMemberOrder] = useState<LocationMemberOrderInput>({
    direction: 'Ascending',
    field: 'Name',
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

    handleRefetch(pageSize, sortingLocationMemberOrder, peopleNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, locationMemberOrder: LocationMemberOrderInput, peopleNameSearchText: string) => {
      startTransition(() => {
        refetchLocationMembers(
          {
            count: pageSize,
            locationMembersSortingValues: [locationMemberOrder],
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
    [refetchLocationMembers],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNextPaginatedLocationMembers) {
      return;
    }

    loadNextPaginatedLocationMembers(pageSize);
  }, [loadNextPaginatedLocationMembers, isLoadingNextPaginatedLocationMembers, pageSize]);

  const [peopleNameSearchText, setPeopleNameSearchText] = useState<string>('');
  const [invitePeopleDialogOpen, setInvitePeopleDialogOpen] = useState(false);
  const validateMembersToInvite = makeValidate(peopleToInviteSchema);
  const requiredMembersToInviteFields = makeRequired(peopleToInviteSchema);

  const handleSearchTextChange = (str: string) => {
    setPeopleNameSearchText(str);

    handleRefetch(pageSize, sortingLocationMemberOrder, str);
  };

  const connectionIds = useMemo(() => {
    return rootDataPaginatedLocationMembers.locationMembers ? [rootDataPaginatedLocationMembers.locationMembers.__id] : [];
  }, [rootDataPaginatedLocationMembers.locationMembers]);

  if (!rootData.location || !rootDataPaginatedLocationMembers.locationMembers) {
    return <></>;
  }

  const locationMemberEdges = rootDataPaginatedLocationMembers.locationMembers.edges;
  const count = rootDataPaginatedLocationMembers.locationMembers.totalCount ? rootDataPaginatedLocationMembers.locationMembers.totalCount : 0;
  const slicedLocationMemberEdges = locationMemberEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > locationMemberEdges.length ? locationMemberEdges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingLocationMemberOrder({
      direction,
      field: value as unknown as LocationMemberOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as LocationMemberOrderField,
      },
      peopleNameSearchText,
    );
  };

  const handleInvitePeopleDialogOpenClick = () => {
    setInvitePeopleDialogOpen(true);
  };

  const handleInvitePeopleClick = ({ emails: originalEmailsStr }: PeopleToJoin) => {
    if (!rootData.location || !originalEmailsStr) {
      return;
    }

    const emails = originalEmailsStr as unknown as string;
    if (!emails) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Inviting people to join location '${rootData.location.name}'...`} />,
      infoNotificationOptions,
    );

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to invite people to join location '${rootData.location?.name}'. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation sent to people to join location ${rootData.location?.name}.`} />,
        });

        setInvitePeopleDialogOpen(false);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to invite people to join location '${rootData.location?.name}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  const handleCancelInvitingPeopleClick = () => {
    setInvitePeopleDialogOpen(false);
  };

  return (
    <>
      <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
        Invite People
      </Button>

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
            { id: 'Role', label: 'Role' },
          ]}
          defaultOption={sortingLocationMemberOrder.field}
          defaultSortingDirectionValue={sortingLocationMemberOrder.direction as unknown as Direction}
          onValueChange={handleSortingChanged}
        />
      </StackRow>

      <GridContainer>
        {slicedLocationMemberEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <LocationMemberCard data={rootData} locationMemberDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </GridContainer>

      <Dialog TransitionComponent={DialogTransition} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
        <DefaultDialogTitle title="Invite people to join your location" />
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
                <FormFieldLabel label="Emails" useWiderSpace>
                  <TextField name="emails" required={requiredMembersToInviteFields.emails} helperText="member1@example.com,member2@example.com" />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={handleCancelInvitingPeopleClick} primaryLabel="Invite" secondaryLabel="Cancel" />
              </FormStackColumn>
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
  locationId: string;
};

const LocationMembersTabWithRelay = ({ onReloadRequired, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationMembersTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
        locationMembersSortingValues: [
          {
            direction: 'Descending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
      <MemoLocationMembersTab queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationMembersTabWithRelay);
