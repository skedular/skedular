import { LocationCard } from '@/components/location';
import type { LocationOrderField, LocationOrderInput, locationsPaginationQuery } from '@/queries/__generated__/locationsPaginationQuery.graphql';
import type { locations_query$key } from '@/queries/__generated__/locations_query.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import Link from 'next/link';
import { memo, startTransition, useCallback, useMemo, useState } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: locations_query$key;
};

const Locations = ({ rootDataRelay }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<locationsPaginationQuery, locations_query$key>(
    graphql`
      fragment locations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "locationsPaginationQuery") {
        locations(first: $count, after: $cursor, where: { nameContains: $locationNameSearchText }, orderBy: $locationsSortingValues)
          @connection(key: "locations_locations") {
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
  const handleSearchTextChange = (str: string) => {
    setLocationNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

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
            locationsSortingValues: [order],
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

  const connectionIds = useMemo(() => [rootData.locations?.__id], [rootData.locations]);
  const locations = useMemo(() => rootData.locations, [rootData.locations]);
  const slicedEdges = locations.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > locations.edges.length ? locations.edges.length : page * pageSize + pageSize,
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

  return (
    <>
      <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
        <Grid>
          <Link href="/location/add">
            <Button variant="contained" startIcon={<AddIcon />}>
              Add Location
            </Button>
          </Link>
        </Grid>
      </Grid>

      <Grid sx={{ marginTop: 1 }}>
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
        <Grid>
          <TablePagination
            count={locations?.totalCount ? locations.totalCount : 0}
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
        </Grid>
        <Grid>
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
          <Grid key={edge.node.id}>
            <LocationCard rootDataRelay={rootData} locationDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>
    </>
  );
};

export default memo(Locations);
