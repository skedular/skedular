import { NewZoneDialog, ZoneCard } from '@/components/zone';
import type { locationZonesTab_query$key } from '@/queries/__generated__/locationZonesTab_query.graphql';
import type { LocationTagOrderField, LocationTagOrderInput, zones_PaginationQuery } from '@/queries/__generated__/zones_PaginationQuery.graphql';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: locationZonesTab_query$key;
  locationId: string;
};

const LocationZonesTab = ({ rootDataRelay, locationId }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<zones_PaginationQuery, locationZonesTab_query$key>(
    graphql`
      fragment locationZonesTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "zones_PaginationQuery") {
        location(id: $locationId) {
          canModify
        }
        locationZonesTabPaginatedTags: paginatedLocationTags(
          first: $count
          after: $cursor
          where: { locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText }
          orderBy: $zoneSortingValues
        ) @connection(key: "locationZonesTab_locationZonesTabPaginatedTags") @include(if: $locationExists) {
          __id
          totalCount
          edges {
            node {
              id
              ...zoneCard_LocationTagDetails
            }
          }
        }
        ...zoneCard_Query
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<LocationTagOrderInput>({
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

    handleRefetch(pageSize, sortingOrder, zoneNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: LocationTagOrderInput, zoneNameSearchText: string) => {
      startTransition(() => {
        refetch(
          {
            count: pageSize,
            zoneSortingValues: [order],
            zoneNameSearchText,
            locationExists: !!locationId,
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
    [refetch, locationId],
  );

  const loadNextPage = useCallback(() => {
    if (isLoadingNext) {
      return;
    }

    loadNext(pageSize);
  }, [loadNext, isLoadingNext, pageSize]);

  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [zoneNameSearchText, setZoneNameSearchText] = useState<string>('');

  const handlePageContextOpenStateChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
    if (isExpanded) {
      setPageContextOpen(true);
    } else {
      setPageContextOpen(false);
    }
  };

  const handleSearchTextChange = (str: string) => {
    setZoneNameSearchText(str);

    handleRefetch(pageSize, sortingOrder, str);
  };

  const debounceSearchTextChange = debounce(handleSearchTextChange, keyboardDebounceTimeout);
  const connectionIds = useMemo(
    () => (rootData.locationZonesTabPaginatedTags ? [rootData.locationZonesTabPaginatedTags.__id] : []),
    [rootData.locationZonesTabPaginatedTags],
  );
  const [isAddZoneDialogOpen, setIsAddZoneDialogOpen] = useState(false);

  if (!rootData.location || !rootData.locationZonesTabPaginatedTags) {
    return <></>;
  }

  const locationTagEdges = rootData.locationZonesTabPaginatedTags.edges;
  const slicedEdges = locationTagEdges.slice(
    page * pageSize,
    page * pageSize + pageSize > locationTagEdges.length ? locationTagEdges.length : page * pageSize + pageSize,
  );

  const handleAddZoneClick = () => {
    setIsAddZoneDialogOpen(true);
  };

  const handleAddZoneDialogAddClick = () => {
    setIsAddZoneDialogOpen(false);

    handleRefetch(pageSize, sortingOrder, zoneNameSearchText);
  };

  const handleAddZoneDialogCancelClick = () => {
    setIsAddZoneDialogOpen(false);
  };

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as LocationTagOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as LocationTagOrderField,
      },
      zoneNameSearchText,
    );
  };

  return (
    <>
      {rootData.location.canModify && (
        <Stack direction="row" sx={{ justifyContent: 'flex-start' }} spacing={1}>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddZoneClick}>
            Add Zone
          </Button>
        </Stack>
      )}

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />} />
        <AccordionDetails>
          <TextField
            defaultValue={zoneNameSearchText}
            helperText="Enter zone name to narrow down the zones list"
            onChange={(event) => debounceSearchTextChange(event?.target.value)}
          />
        </AccordionDetails>
      </Accordion>

      <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
        <TablePagination
          count={rootData.locationZonesTabPaginatedTags.totalCount ? rootData.locationZonesTabPaginatedTags.totalCount : 0}
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
            <ZoneCard rootDataRelay={rootData} locationTagDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>

      <NewZoneDialog
        connectionIds={connectionIds}
        isDialogOpen={isAddZoneDialogOpen}
        onAddClicked={handleAddZoneDialogAddClick}
        onCancelClicked={handleAddZoneDialogCancelClick}
        locationId={locationId}
      />
    </>
  );
};

export default memo(LocationZonesTab);
