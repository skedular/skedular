import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Link from '@mui/material/Link';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationCard } from 'components/location';
import debounce from 'lodash.debounce';
import { memo, useCallback, useMemo, useState } from 'react';
import { usePaginationFragment } from 'react-relay';
import type {
  LocationOrderField,
  LocationOrderInput,
  organizationLocationsPaginationQuery,
} from './__generated__/organizationLocationsPaginationQuery.graphql';
import type { organizationLocationsTab_query$key } from './__generated__/organizationLocationsTab_query.graphql';

type Props = {
  rootDataRelay: organizationLocationsTab_query$key;
};

const OrganizationLocationsTab = ({ rootDataRelay }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationLocationsPaginationQuery, organizationLocationsTab_query$key>(
    graphql`
      fragment organizationLocationsTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationLocationsPaginationQuery") {
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
              ...locationCard_LocationDetails
            }
          }
        }
        ...locationCard_Query
        organization(id: $organizationId) {
          id
          canModify
        }
      }
    `,
    rootDataRelay,
  );

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
    },
    [refetch],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const connectionIds = useMemo(() => [rootData.locations?.__id], [rootData.locations]);
  const locationEdges = rootData.locations.edges;
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
    <>
      {rootData.organization.canModify && (
        <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
          <Grid item sx={{ marginRight: 1 }}>
            <Link href={`/organization/${rootData.organization.id}/location/add`}>
              <Button variant="contained" startIcon={<AddIcon />}>
                Add Location
              </Button>
            </Link>
          </Grid>
        </Grid>
      )}

      <Grid item sx={{ marginTop: 1 }}>
        <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />} />
          <AccordionDetails>
            <TextField
              defaultValue={locationNameSearchText}
              helperText="Enter location name to narrow down the locations list"
              onChange={(event) => debounceSearchTextChange(event?.target.value)}
            />
          </AccordionDetails>
        </Accordion>
      </Grid>

      <Grid container sx={{ justifyContent: 'flex-end' }}>
        <Grid item>
          <TablePagination
            count={rootData.locations?.totalCount ? rootData.locations.totalCount : 0}
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
        </Grid>
        <Grid item>
          <Sorting
            options={[{ id: 'name', label: 'Name' }]}
            // @ts-expect-error
            defaultOption={sortingOrder.field}
            defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
            onValueChange={handleSortingChanged}
          />
        </Grid>
      </Grid>
      <Grid container spacing={{ xs: 2, md: 3 }}>
        {slicedEdges.map((edge) => (
          <Grid item key={edge.node.id}>
            <LocationCard rootDataRelay={rootData} locationDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>
    </>
  );
};

export default memo(OrganizationLocationsTab);
