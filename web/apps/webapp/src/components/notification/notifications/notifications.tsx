import { NotificationCard } from '@/components/notification';
import type {
  NotificationOrderField,
  NotificationOrderInput,
  notificationsPaginationQuery,
} from '@/queries/__generated__/notificationsPaginationQuery.graphql';
import type { notifications_query$key } from '@/queries/__generated__/notifications_query.graphql';
import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import { memo, useCallback, useMemo, useState } from 'react';
import { graphql, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: notifications_query$key;
};

const Notifications = ({ rootDataRelay }: Props) => {
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<notificationsPaginationQuery, notifications_query$key>(
    graphql`
      fragment notifications_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "notificationsPaginationQuery") {
        myNotifications(first: $count, after: $cursor, orderBy: $myNotificationsSortingValues) @connection(key: "notifications_myNotifications") {
          __id
          totalCount
          edges {
            node {
              id
              ...notificationCard_NotificationDetails
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const [sortingOrder, setSortingOrder] = useState<NotificationOrderInput>({
    direction: 'Descending',
    field: 'eventRaisedAt',
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

    handleRefetch(pageSize, sortingOrder);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: NotificationOrderInput) => {
      refetch(
        {
          count: pageSize,
          myNotificationsSortingValues: [order],
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

  const myNotifications = useMemo(() => rootData.myNotifications, [rootData.myNotifications]);
  const slicedEdges = myNotifications.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > myNotifications.edges.length ? myNotifications.edges.length : page * pageSize + pageSize,
  );

  const handleSortingChanged = (direction: Direction, value: string) => {
    setSortingOrder({
      direction,
      field: value as unknown as NotificationOrderField,
    });

    handleRefetch(pageSize, {
      direction,
      field: value as unknown as NotificationOrderField,
    });
  };

  return (
    <>
      <Grid container sx={{ justifyContent: 'flex-end' }}>
        <Grid>
          <TablePagination
            count={myNotifications?.totalCount ? myNotifications.totalCount : 0}
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={pageSize}
            onRowsPerPageChange={handlePageSizeChange}
          />
        </Grid>
        <Grid>
          <Sorting
            options={[{ id: 'eventRaisedAt', label: 'Date' }]}
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
            <NotificationCard notificationDetailsRelay={edge.node} />
          </Grid>
        ))}
      </Grid>
    </>
  );
};

export default memo(Notifications);
