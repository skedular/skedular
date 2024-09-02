import { CustomerCard } from '@/components/customer';
import type {
  LocationMemberOrderField,
  LocationMemberOrderInput,
  locationMembersPaginationQuery,
} from '@/queries/__generated__/locationMembersPaginationQuery.graphql';
import type { locationPeopleTab_inviteCustomersToJoinLocationMutation } from '@/queries/__generated__/locationPeopleTab_inviteCustomersToJoinLocationMutation.graphql';
import type { locationPeopleTab_query$key } from '@/queries/__generated__/locationPeopleTab_query.graphql';
import type {
  CustomerOrderField,
  CustomerOrderInput,
  locationPeopleTab_query_customersByDefaultLocation,
} from '@/queries/__generated__/locationPeopleTab_query_customersByDefaultLocation.graphql';
import type { locationPeopleTab_query_organizationMembers$key } from '@/queries/__generated__/locationPeopleTab_query_organizationMembers.graphql';
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
import TablePagination from '@mui/material/TablePagination';
import MUITextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { TextField, makeRequired, makeValidate } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation, usePaginationFragment } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import { array, object, string } from 'yup';
import LocationMemberCard from './location-member-card';

type Props = {
  rootDataLocationMembersRelay: locationPeopleTab_query$key;
  rootDataOrganizationMembersRelay: locationPeopleTab_query_organizationMembers$key;
  organizationId: string;
};

interface MembersToJoin {
  emails: (string | undefined)[];
}

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

const LocationPeopleTab = ({ rootDataLocationMembersRelay, rootDataOrganizationMembersRelay, organizationId }: Props) => {
  const {
    data: rootDataLocation,
    loadNext: loadNextLocationMembers,
    isLoadingNext: isLoadingNextLocationMembers,
    refetch: refetchLocationMembers,
  } = usePaginationFragment<locationMembersPaginationQuery, locationPeopleTab_query$key>(
    graphql`
      fragment locationPeopleTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationMembersPaginationQuery") {
        location(id: $locationId) {
          id
          name
        }
        paginatedLocationMembers(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationPeopleSortingValues
        ) @connection(key: "locationPeopleTab_paginatedLocationMembers") {
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
    rootDataLocationMembersRelay,
  );

  const {
    data: rootDatacustomersByDefaultLocation,
    loadNext: loadNextcustomersByDefaultLocation,
    isLoadingNext: isLoadingNextcustomersByDefaultLocation,
    refetch: refetchcustomersByDefaultLocation,
  } = usePaginationFragment<locationPeopleTab_query_customersByDefaultLocation, locationPeopleTab_query_organizationMembers$key>(
    graphql`
      fragment locationPeopleTab_query_organizationMembers on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationPeopleTab_query_customersByDefaultLocation") {
        customersByDefaultLocation(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $locationOrganizationPeopleSortingValues
        ) @connection(key: "locationPeopleTab_customersByDefaultLocation") {
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
    rootDataOrganizationMembersRelay,
  );

  const [isPending, startTransition] = useTransition();
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

  const loadNext = organizationId ? loadNextcustomersByDefaultLocation : loadNextLocationMembers;
  const isLoadingNext = organizationId ? isLoadingNextcustomersByDefaultLocation : isLoadingNextLocationMembers;

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
          refetchcustomersByDefaultLocation(
            {
              count: pageSize,
              locationOrganizationPeopleSortingValues: [customerOrder],
              peopleNameSearchText,
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
    [organizationId, refetchLocationMembers, refetchcustomersByDefaultLocation],
  );

  const loadNextPage = useCallback(() => {
    if (organizationId) {
      if (isLoadingNextcustomersByDefaultLocation) {
        return;
      }

      loadNextcustomersByDefaultLocation(pageSize);
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
    loadNextcustomersByDefaultLocation,
    isLoadingNextcustomersByDefaultLocation,
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
      return [rootDatacustomersByDefaultLocation.customersByDefaultLocation.__id];
    } else {
      return rootDataLocation.paginatedLocationMembers ? [rootDataLocation.paginatedLocationMembers.__id] : [];
    }
  }, [organizationId, rootDataLocation.paginatedLocationMembers, rootDatacustomersByDefaultLocation.customersByDefaultLocation]);

  if (!rootDataLocation.location) {
    return <></>;
  }

  const locationMemberEdges = rootDataLocation.paginatedLocationMembers.edges;
  const organizationMemberEdges = rootDatacustomersByDefaultLocation.customersByDefaultLocation.edges;
  const count = organizationId
    ? rootDatacustomersByDefaultLocation.customersByDefaultLocation.totalCount
      ? rootDatacustomersByDefaultLocation.customersByDefaultLocation.totalCount
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
          clientMutationId: uuidv4(),
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
        } else {
          setInvitePeopleDialogOpen(false);
        }
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
        <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
          <Grid>
            <Button variant="contained" startIcon={<AddIcon />} onClick={handleInvitePeopleDialogOpenClick}>
              Invite People
            </Button>
          </Grid>
        </Grid>
      )}

      <Grid sx={{ marginTop: 1 }}>
        <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />} />
          <AccordionDetails>
            <MUITextField
              defaultValue={peopleNameSearchText}
              helperText="Enter name to narrow down the people list"
              onChange={(event) => debounceSearchTextChange(event?.target.value)}
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
              { id: 'name', label: 'Name' },
              { id: 'givenName', label: 'Given name' },
              { id: 'middleName', label: 'Middle name' },
              { id: 'familyName', label: 'Family Name' },
              { id: 'membershipType', label: 'Membership type' },
              { id: 'createdAt', label: 'Join date' },
            ]}
            // @ts-expect-error
            defaultOption={organizationId ? sortingCustomerOrder.field : sortingLocationMemberOrder.field}
            defaultSortingDirectionValue={
              organizationId
                ? (sortingCustomerOrder.direction as unknown as Direction)
                : (sortingLocationMemberOrder.direction as unknown as Direction)
            }
            onValueChange={handleSortingChanged}
          />
        </Grid>
      </Grid>

      <Grid container spacing={{ xs: 2, md: 3 }}>
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

      <Dialog fullWidth={true} open={invitePeopleDialogOpen} onClose={handleCancelInvitingPeopleClick}>
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

export default memo(LocationPeopleTab);
