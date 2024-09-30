import { OrganizationBookingsCard } from '@/components/organization/organizationBookingCard';
import type {
  OrganizationOrderField,
  OrganizationOrderInput,
  organizations_PaginationQuery,
} from '@/queries/__generated__/organizations_PaginationQuery.graphql';
import type { organizations_query$key } from '@/queries/__generated__/organizations_query.graphql';
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
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { keyboardDebounceTimeout } from '@repo/shared/libs/utils';
import debounce from 'lodash.debounce';
import NextLink from 'next/link';

import { memo, useCallback, useMemo, useState, useTransition } from 'react';
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
  } = usePaginationFragment<organizations_PaginationQuery, organizations_query$key>(
    graphql`
      fragment organizations_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "organizations_PaginationQuery") {
        organizations(first: $count, after: $cursor, where: { nameContains: $organizationNameSearchText }, orderBy: $organizationsSortingValues)
          @connection(key: "organizations_organizations") {
          __id
          totalCount
          edges {
            node {
              id
              name
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [, startTransition] = useTransition();
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
      startTransition(() => {
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
    <Stack direction="column" spacing={1}>
      <Link component={NextLink} href="/organization/add">
        <Button variant="contained" startIcon={<AddIcon />}>
          Add Organization
        </Button>
      </Link>

      <Accordion onChange={handlePageContextOpenStateChange} expanded={pageContextOpen} sx={{ width: '100%' }}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />} />
        <AccordionDetails>
          <TextField
            defaultValue={organizationNameSearchText}
            helperText="Enter organization name to narrow down the organizations list"
            onChange={(event) => debounceSearchTextChange(event?.target.value)}
          />
        </AccordionDetails>
      </Accordion>

      <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
        <TablePagination
          count={organizations?.totalCount ? organizations.totalCount : 0}
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
            <OrganizationBookingsCard organizationId={edge.node.id} organizationName={edge.node.name} organizationsConnectionIds={connectionIds} />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
};

export default memo(Organizations);
