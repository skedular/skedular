import { OrganizationCard } from '@/components/organization';
import type {
  OrganizationOrderField,
  OrganizationOrderInput,
  organizationsPaginationQuery,
} from '@/queries/__generated__/organizationsPaginationQuery.graphql';
import type { organizations_query$key } from '@/queries/__generated__/organizations_query.graphql';
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
import { memo, useCallback, useMemo, useState } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: organizations_query$key;
};

const Organizations = ({ rootDataRelay }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<organizationsPaginationQuery, organizations_query$key>(
    graphql`
      fragment organizations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizationsPaginationQuery") {
        organizations(first: $count, after: $cursor, where: { nameContains: $organizationNameSearchText }, orderBy: $organizationsSortingValues)
          @connection(key: "organizations_organizations") {
          __id
          totalCount
          edges {
            node {
              id
              ...organizationCard_OrganizationDetails
            }
          }
        }
        ...organizationCard_Query
      }
    `,
    rootDataRelay,
  );

  const [sortingOrder, setSortingOrder] = useState<OrganizationOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [pageContextOpen, setPageContextOpen] = useState(false);
  const [organizationNameSearchText, setOrganizationNameSearchText] = useState<string>('');
  const handleSearchTextChange = (str: string) => {
    setOrganizationNameSearchText(str);

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

    handleRefetch(pageSize, sortingOrder, organizationNameSearchText);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: OrganizationOrderInput, organizationNameSearchText: string) => {
      refetch(
        {
          count: pageSize,
          organizationsSortingValues: [order],
          organizationNameSearchText,
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

  const connectionIds = useMemo(() => [rootData.organizations?.__id], [rootData.organizations]);

  const organizations = useMemo(() => rootData.organizations, [rootData.organizations]);

  const slicedEdges = organizations.edges.slice(
    page * pageSize,
    page * pageSize + pageSize > organizations.edges.length ? organizations.edges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as OrganizationOrderField,
    });

    handleRefetch(
      pageSize,
      {
        direction,
        field: value as unknown as OrganizationOrderField,
      },
      organizationNameSearchText,
    );
  };

  return (
    <>
      <Grid container sx={{ justifyContent: 'flex-start', marginTop: 1 }}>
        <Grid>
          <Link href="/organization/add">
            <Button variant="contained" startIcon={<AddIcon />}>
              Add Organization
            </Button>
          </Link>
        </Grid>
      </Grid>

      <Grid sx={{ marginTop: 1 }}>
        <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />} />
          <AccordionDetails>
            <TextField
              defaultValue={organizationNameSearchText}
              helperText="Enter organization name to narrow down the organizations list"
              onChange={(event) => debounceSearchTextChange(event?.target.value)}
            />
          </AccordionDetails>
        </Accordion>
      </Grid>

      <Grid container sx={{ justifyContent: 'flex-end' }}>
        <Grid>
          <TablePagination
            count={organizations?.totalCount ? organizations.totalCount : 0}
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
            <OrganizationCard rootDataRelay={rootData} organizationDetailsRelay={edge.node} connectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>
    </>
  );
};

export default memo(Organizations);
