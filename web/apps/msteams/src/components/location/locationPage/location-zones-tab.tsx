import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import Accordion from '@mui/material/Accordion';
import AccordionDetails from '@mui/material/AccordionDetails';
import AccordionSummary from '@mui/material/AccordionSummary';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import TablePagination from '@mui/material/TablePagination';
import TextField from '@mui/material/TextField';
import { AddIcon } from '@repo/shared/components/icons';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import { NewZoneDialog, ZoneCard } from 'components/zone';
import debounce from 'lodash.debounce';
import { memo, useCallback, useMemo, useState } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';
import type { locationZonesTab_query$key } from './__generated__/locationZonesTab_query.graphql';
import type { LocationTagOrderField, LocationTagOrderInput, zonesPaginationQuery } from './__generated__/zonesPaginationQuery.graphql';

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
  } = usePaginationFragment<zonesPaginationQuery, locationZonesTab_query$key>(
    graphql`
      fragment locationZonesTab_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "zonesPaginationQuery") {
        location(id: $locationId) {
          canModify
        }
        locationZonesTabPaginatedTags: paginatedLocationTags(
          first: $count
          after: $cursor

          where: { locationId: $locationId, tagType: $zoneTagType, nameContains: $zoneNameSearchText }
          orderBy: $zoneSortingValues
        ) @connection(key: "locationZonesTab_locationZonesTabPaginatedTags") {
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
      refetch(
        {
          count: pageSize,
          zoneSortingValues: [order],
          zoneNameSearchText,
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

  if (!rootData.location) {
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
        <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
          <Grid item sx={{ marginRight: 1 }}>
            <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddZoneClick}>
              Add Zone
            </Button>
          </Grid>
        </Grid>
      )}

      <Grid item sx={{ marginTop: 1 }}>
        <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />} />
          <AccordionDetails>
            <TextField
              defaultValue={zoneNameSearchText}
              helperText="Enter zone name to narrow down the zones list"
              onChange={(event) => debounceSearchTextChange(event?.target.value)}
            />
          </AccordionDetails>
        </Accordion>
      </Grid>

      <Grid container sx={{ justifyContent: 'flex-end' }}>
        <Grid item>
          <TablePagination
            count={rootData.locationZonesTabPaginatedTags.totalCount ? rootData.locationZonesTabPaginatedTags.totalCount : 0}
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
