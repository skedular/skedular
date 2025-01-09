import Grid from '@mui/material/Grid2';
import TablePagination from '@mui/material/TablePagination';
import { GridContainer, PushToRight, StackRow } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { Direction, Sorting } from '@repo/shared/components/sorting';
import graphql from 'babel-plugin-relay/macro';
import { NotificationCard } from 'components/notification';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePaginationFragment, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { notifications_query$key } from './__generated__/notifications_query.graphql';
import type {
  NotificationOrderField,
  NotificationOrderInput,
  notifications_refetchableFragment,
} from './__generated__/notifications_refetchableFragment.graphql';
import type { notifications_rootQuery } from './__generated__/notifications_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<notifications_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query notifications_rootQuery($myNotificationsSortingValues: [NotificationOrderInput!]) {
    ...notifications_query
  }
`;

const Notifications = ({ queryReference }: Props) => {
  const rootDataRelay = usePreloadedQuery<notifications_rootQuery>(RootQuery, queryReference);
  const {
    data: rootData,
    loadNext,
    isLoadingNext,
    refetch,
  } = usePaginationFragment<notifications_refetchableFragment, notifications_query$key>(
    graphql`
      fragment notifications_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 50 })
      @refetchable(queryName: "notifications_refetchableFragment") {
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

  const [, startTransition] = useTransition();
  const [sortingOrder, setSortingOrder] = useState<NotificationOrderInput>({
    direction: 'Descending',
    field: 'Date',
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

    handleRefetch(pageSize, sortingOrder);
  };

  const handleRefetch = useCallback(
    (pageSize: number, order: NotificationOrderInput) => {
      startTransition(() => {
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

  if (!rootData.myNotifications) {
    return <></>;
  }

  const slicedEdges = rootData.myNotifications.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > rootData.myNotifications.edges.length ? rootData.myNotifications.edges.length : page * pageSize + pageSize,
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
      <StackRow>
        <PushToRight />
        <TablePagination
          component="div"
          count={rootData.myNotifications.totalCount ? rootData.myNotifications.totalCount : 0}
          page={page}
          onPageChange={handleChangePage}
          rowsPerPage={pageSize}
          onRowsPerPageChange={handlePageSizeChange}
        />
        <Sorting
          options={[{ id: 'Date', label: 'Type' }]}
          defaultOption={sortingOrder.field}
          defaultSortingDirectionValue={sortingOrder.direction as unknown as Direction}
          onValueChange={handleSortingChanged}
        />
      </StackRow>

      <GridContainer>
        {slicedEdges.map((edge) => (
          <Grid key={edge.node.id}>
            <NotificationCard notificationDetailsRelay={edge.node} />
          </Grid>
        ))}
      </GridContainer>
    </>
  );
};

const MemoNotifications = memo(Notifications);

const NotificationsWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<notifications_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        myNotificationsSortingValues: [
          {
            direction: 'Descending',
            field: 'Date',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNotifications queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsWithRelay);
