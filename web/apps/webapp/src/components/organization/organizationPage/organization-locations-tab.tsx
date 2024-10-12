import { getLocationAddLink } from '@/components/location';
import { LocationBookingsCard } from '@/components/location/locationBookingCard';
import type {
  LocationOrderField,
  LocationOrderInput,
  organizationLocations_PaginationQuery,
} from '@/queries/__generated__/organizationLocations_PaginationQuery.graphql';
import type { organizationLocationsTab_query$key } from '@/queries/__generated__/organizationLocationsTab_query.graphql';
import type { organizationLocationsTab_rootQuery } from '@/queries/__generated__/organizationLocationsTab_rootQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { memo, useCallback, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<organizationLocationsTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationLocationsTab_rootQuery(
    $organizationId: String!
    $organizationLocationsSortingValues: [LocationOrderInput!]!
    $locationNameSearchText: String
  ) {
    ...organizationLocationsTab_query
  }
`;

const OrganizationLocationsTab = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<organizationLocationsTab_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationLocations_PaginationQuery, organizationLocationsTab_query$key>(
    graphql`
      fragment organizationLocationsTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationLocations_PaginationQuery") {
        locations(
          first: $count
          after: $cursor
          where: { organizationId: $organizationId, nameContains: $locationNameSearchText }
          orderBy: $organizationLocationsSortingValues
        ) @connection(key: "organizationLocationsTab_locations") {
          __id
          totalCount
          edges {
            node {
              id
              name
              organization {
                uniqueId
                name
              }
            }
          }
        }
        organization(id: $organizationId) {
          id
          canModify
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<LocationOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [locationNameSearchText, setLocationNameSearchText] = useState<string>('');

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    if (newPage > page) {
      loadNextPage();
    }

    setPage(newPage);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const pageSize = parseInt(event.target.value, 10);

    setPageSize(parseInt(event.target.value, 10));

    handleRefetch(pageSize, sortingOrder, locationNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: LocationOrderInput, locationNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            organizationLocationsSortingValues: [order],
            locationNameSearchText,
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
    [refetch],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const connectionIds = useMemo(() => (rootData.locations ? [rootData.locations.__id] : []), [rootData.locations]);
  const locationEdges = rootData.locations ? rootData.locations.edges : [];
  const slicedEdges = locationEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > locationEdges.length ? locationEdges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as LocationOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as LocationOrderField,
      },
      locationNameSearchText,
    );
  };

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleSearchTextChange = (str: string) => {
    setLocationNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };
  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

  if (!rootData.organization) {
    return <></>;
  }

  return (
    <Stack direction="column" spacing={1}>
      {rootData.organization.canModify && (
        <Stack direction="row" sx={{ width: 'auto' }}>
          <Link component={NextLink} href={getLocationAddLink(rootData.organization.id)}>
            <Button variant="contained" startIcon={<AddIcon />}>
              Add Location
            </Button>
          </Link>
        </Stack>
      )}

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />} />
        <AccordionDetails>
          <TextField
            defaultValue={locationNameSearchText}
            helperText="Enter location name to narrow down the locations list"
            onChange={(event) => debounceSearchTextChange(event?.target.value)}
          />
        </AccordionDetails>
      </Accordion>

      <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
        <TablePagination
          count={rootData.locations?.totalCount ? rootData.locations.totalCount : 0}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={pageSize}
          onRowsPerPageChange={handlePageSizeChange}
        />
        <Sorting
          options={[{ id: 'name', label: 'Name' }]}
          defaultOption={sortingOrder.field}
          defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
          onValueChange={handleSortingChanged}
        />
      </Stack>
      <Grid container spacing={1}>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <LocationBookingsCard
              organizationId={edge.node.organization?.uniqueId}
              organizationName={edge.node.organization?.name}
              locationId={edge.node.id}
              locationName={edge.node.name}
              locationsConnectionIds={connectionIds}
            />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

const MemoOrganizationLocationsTab = memo(OrganizationLocationsTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
};

const OrganizationLocationsTabWithRelay = ({ onReloadRequired, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationLocationsTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId,
        organizationLocationsSortingValues: [
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
      <MemoOrganizationLocationsTab queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationLocationsTabWithRelay);
