import { RelayError, startOfDay, toRootError, useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import { ExternalRefundQueuePageInfo, RefundQueue } from '@/components/admin/refund/RefundQueue';
import { DayPicker } from '@/components/datePickers';
import { Loading } from '@/components/loading';
import { RootShell } from '@/components/rootShell';
import { DefaultSelect } from '@/components/styled';
import type { pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation } from '@/queries/__generated__/pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation.graphql';
import type { pageOrganizationRefunds_rootQuery } from '@/queries/__generated__/pageOrganizationRefunds_rootQuery.graphql';
import {
  CollectionToolbar,
  defaultGridStyle,
  defaultPadding,
  GridContainer,
  LeadIconTypography,
  PageHeaderPanel,
  PushToRight,
  SmallIconTypography,
  StackColumn,
  StackRow,
  SubtitleIconTypography,
} from '@skedular/ui';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import MenuItem from '@mui/material/MenuItem';
import Box from '@mui/system/Box';
import type { SxProps, Theme } from '@mui/system';
import { DataGrid, type GridColDef } from '@mui/x-data-grid';
import { getOrganizationRefundBaseLink, getOrganizationSubscriptionBaseLink } from '@/components/links';
import { ListGridToggle } from '@/components/listGridToggle';
import Grid from '@mui/material/Grid';
import Link from 'next/link';
import { useRouter, useSearchParams } from 'next/navigation';
import dayjs from 'dayjs';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';

type QueryVariables = {
  refundRequestedAtFrom?: string | null;
  refundRequestedAtTo?: string | null;
  refundStatuses?: string[] | null;
  externalAfter?: string | null;
  externalFirst?: number | null;
  externalBefore?: string | null;
  externalLast?: number | null;
  externalProvider?: string | null;
  externalStatus?: string | null;
  refundAfter?: string | null;
  refundFirst?: number | null;
  refundBefore?: string | null;
  refundLast?: number | null;
};

const getRefundTypeLabel = (localEntityType: string) =>
  localEntityType === 'MarketplaceBookingSubscription' ? 'Subscription' : localEntityType === 'EntitlementPurchase' ? 'Entitlement' : 'Booking';

const RootQuery = graphql`
  query pageOrganizationRefunds_rootQuery(
    $organizationCustomDomain: String!
    $externalAfter: String
    $externalFirst: Int
    $externalBefore: String
    $externalLast: Int
    $externalProvider: String
    $externalStatus: String
    $refundAfter: String
    $refundFirst: Int
    $refundBefore: String
    $refundLast: Int
    $refundRequestedAtFrom: DateTime
    $refundRequestedAtTo: DateTime
    $refundStatuses: [String!]
  ) {
    organization(customDomain: $organizationCustomDomain) {
      id
      name
      customDomain
    }
    marketplaceRefundStatuses {
      type
      name
    }
    marketplaceRefundQueue(
      after: $refundAfter
      first: $refundFirst
      before: $refundBefore
      last: $refundLast
      where: { organizationCustomDomain: $organizationCustomDomain, requestedAtGte: $refundRequestedAtFrom, requestedAtLte: $refundRequestedAtTo, statuses: $refundStatuses }
    ) {
      pageInfo {
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
      edges {
        node {
          id
          localEntityType
          localEntityId
          currency {
            type
            name
          }
          status {
            type
            name
          }
          requestedAt
          refundAmount
          currencyToDisplay
          requestedByCustomerName
          paymentProvider
          reason
          lastError
          externalRefundNumber
          events {
            id
            eventType {
              type
              name
            }
            occurredAt
            refundAmount
            currencyToDisplay
            reason
            lastError
            externalRefundNumber
            actorName
          }
        }
      }
    }
    marketplaceExternalRefundReconciliations(
      organizationCustomDomain: $organizationCustomDomain
      after: $externalAfter
      first: $externalFirst
      before: $externalBefore
      last: $externalLast
      provider: $externalProvider
      status: $externalStatus
    ) {
      pageInfo {
        hasNextPage
        hasPreviousPage
        startCursor
        endCursor
      }
      edges {
        node {
          provider
          externalRefundId
          amount
          currency
          status
          lastSeenAt
          resolutionReason
        }
      }
    }
  }
`;

const surfaceSx: SxProps<Theme> = {
  borderRadius: 4,
  border: 1,
  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(15, 23, 42, 0.08)' : theme.palette.divider),
  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 255, 255, 0.88)' : theme.palette.background.paper),
  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 24px rgba(15, 23, 42, 0.06)' : '0 1px 3px rgba(0, 0, 0, 0.24)'),
};

const toStatusColor = (status: string): 'default' | 'error' | 'info' | 'success' | 'warning' => {
  if (status === 'Completed') return 'success';
  if (status === 'Failed' || status === 'Reconciliation required') return 'error';
  if (status === 'Under review') return 'warning';
  return 'info';
};

const RefundManagement = ({
  queryReference,
  variables,
  setVariables,
}: {
  queryReference: PreloadedQuery<pageOrganizationRefunds_rootQuery, Record<string, unknown>>;
  variables: QueryVariables;
  setVariables: (variables: QueryVariables) => void;
}) => {
  const data = usePreloadedQuery<pageOrganizationRefunds_rootQuery>(RootQuery, queryReference);
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const searchParams = useSearchParams();
  const [commitResolveExternal] = useMutation<pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation>(graphql`
    mutation pageOrganizationRefunds_resolveMarketplaceExternalRefundReconciliationMutation($input: ResolveMarketplaceExternalRefundReconciliationInput!) {
      resolveMarketplaceExternalRefundReconciliation(input: $input) {
        reconciliation {
          provider
          externalRefundId
          status
          resolutionReason
        }
      }
    }
  `);
  const [resolved, setResolved] = useState<string[]>([]);
  const viewMode = searchParams.get('view') === 'grid' ? 'grid' : 'list';
  const statusFilter = searchParams.get('status') ?? 'All';
  const fromDate = useMemo(() => dayjs(searchParams.get('from') ?? startOfDay().subtract(1, 'month').format('YYYY-MM-DD')), [searchParams]);
  const toDate = useMemo(() => dayjs(searchParams.get('to') ?? startOfDay().format('YYYY-MM-DD')), [searchParams]);
  const updateFilterUrl = (updates: Record<string, string | undefined>) => {
    const params = new URLSearchParams(searchParams.toString());
    Object.entries(updates).forEach(([key, value]) => (value ? params.set(key, value) : params.delete(key)));
    router.push(`?${params.toString()}`);
  };
  useEffect(() => {
    setVariables({
      ...variables,
      refundAfter: undefined,
      refundBefore: undefined,
      refundFirst: 50,
      refundLast: undefined,
      refundRequestedAtFrom: fromDate.startOf('day').toISOString(),
      refundRequestedAtTo: toDate.endOf('day').toISOString(),
      refundStatuses: statusFilter === 'All' ? undefined : [statusFilter],
    });
    // Keep the filter effect scoped to filter changes; including query variables would reset pagination after every fetch.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [fromDate, statusFilter, toDate]);
  const externalRefunds = data.marketplaceExternalRefundReconciliations.edges
    .map(({ node }) => ({ ...node, amount: node.amount == null ? null : node.amount.toString() }))
    .filter((item) => !resolved.includes(`${item.provider}:${item.externalRefundId}`));
  const refunds = data.marketplaceRefundQueue.edges.map(({ node }) => node);
  const openRefund = (refundId: string) => getOrganizationRefundBaseLink(integratedPlatform, data.organization?.customDomain ?? '', refundId);
  const rows = refunds.map((refund) => ({
    id: refund.id,
    type: getRefundTypeLabel(refund.localEntityType),
    customer: refund.requestedByCustomerName ?? 'Customer unavailable',
    amount: `${refund.refundAmount ?? 'Amount unavailable'} ${refund.currencyToDisplay}`,
    reason: refund.reason ?? '',
    status: refund.status.name,
    requestedAt: refund.requestedAt,
  }));
  const columns: GridColDef[] = [
    { field: 'type', headerName: 'Type', minWidth: 130, flex: 0.8 },
    { field: 'customer', headerName: 'Customer', minWidth: 200, flex: 1.4 },
    { field: 'amount', headerName: 'Amount', minWidth: 150, flex: 0.9 },
    { field: 'reason', headerName: 'Cancellation reason', minWidth: 220, flex: 1.4 },
    {
      field: 'status',
      headerName: 'Status',
      minWidth: 150,
      flex: 0.9,
      renderCell: ({ value }) => <Chip size="small" label={value} color={toStatusColor(value)} variant="outlined" />,
    },
    {
      field: 'requestedAt',
      headerName: 'Requested',
      minWidth: 180,
      flex: 1,
      valueFormatter: (value) => new Date(value).toLocaleString(),
    },
  ];

  return (
    <RootShell>
      <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', p: 2 }}>
        <StackColumn spacing={2} sx={{ width: '100%', maxWidth: 1200, mx: 'auto', pb: defaultPadding }}>
          <PageHeaderPanel title="Refunds" description="Review refund requests, track their progress, and resolve provider reconciliation work." />
          <CollectionToolbar
            filters={
              <GridContainer spacing={1} sx={{ alignItems: 'center' }}>
                <DefaultSelect
                  displayEmpty
                  size="small"
                  value={statusFilter}
                  onChange={(event) => updateFilterUrl({ status: event.target.value === 'All' ? undefined : (event.target.value as string) })}
                  renderValue={() => (
                    <StackRow>
                      <LeadIconTypography label="Status" />
                      <Divider orientation="vertical" flexItem />
                      <PushToRight />
                      <SmallIconTypography label={statusFilter === 'All' ? 'All statuses' : statusFilter} />
                    </StackRow>
                  )}
                  aria-label="Refund status"
                >
                  <MenuItem value="All">All statuses</MenuItem>
                  {(data.marketplaceRefundStatuses ?? []).map((status) => (
                    <MenuItem key={status.type} value={status.type}>
                      {status.name}
                    </MenuItem>
                  ))}
                </DefaultSelect>
                <DayPicker label="From" value={fromDate} onDateChanged={(date) => updateFilterUrl({ from: date.format('YYYY-MM-DD') })} />
                <DayPicker label="To" value={toDate} onDateChanged={(date) => updateFilterUrl({ to: date.format('YYYY-MM-DD') })} />
              </GridContainer>
            }
            actions={<ListGridToggle defaultValue={viewMode} onChange={(view) => updateFilterUrl({ view })} />}
          />
          {refunds.length === 0 ? (
            <Box sx={{ ...surfaceSx, px: 3, py: 4 }}>
              <SmallIconTypography label="No refunds match the current filters." sx={{ opacity: 0.78 }} />
            </Box>
          ) : viewMode === 'list' ? (
            <Box sx={{ ...surfaceSx, p: 1.5 }}>
              <DataGrid
                rows={rows}
                columns={columns}
                autoHeight
                disableRowSelectionOnClick
                hideFooter
                onRowClick={(params) => router.push(openRefund(params.row.id))}
                sx={{ ...defaultGridStyle, '& .MuiDataGrid-row': { cursor: 'pointer' } }}
                localeText={{ noRowsLabel: 'No refunds found' }}
              />
            </Box>
          ) : (
            <Grid container spacing={2}>
              {refunds.map((refund) => (
                <Grid key={refund.id} size={{ xs: 12, md: 6 }}>
                  <Card sx={{ ...surfaceSx, height: '100%' }}>
                    <CardContent sx={{ p: 2.5, height: '100%' }}>
                      <StackColumn spacing={1.5} sx={{ height: '100%' }}>
                        <StackRow sx={{ alignItems: 'flex-start', gap: 1 }}>
                          <StackColumn spacing={0.5}>
                            <SubtitleIconTypography label={`${getRefundTypeLabel(refund.localEntityType)} refund`} />
                            <SmallIconTypography label={refund.requestedByCustomerName ?? 'Customer unavailable'} sx={{ opacity: 0.72 }} />
                          </StackColumn>
                          <PushToRight />
                          <Chip label={refund.status.name} size="small" color={toStatusColor(refund.status.name)} variant="outlined" />
                        </StackRow>
                        <Divider />
                        <StackColumn spacing={0.5}>
                          <SmallIconTypography label={`${refund.refundAmount ?? 'Amount unavailable'} ${refund.currencyToDisplay}`} />
                          {refund.reason ? <SmallIconTypography label={`Cancellation reason: ${refund.reason}`} /> : null}
                          <SmallIconTypography label={`Requested ${new Date(refund.requestedAt).toLocaleString()}`} sx={{ opacity: 0.72 }} />
                        </StackColumn>
                        <Button component={Link} href={openRefund(refund.id)} sx={{ alignSelf: 'flex-start', mt: 'auto' }}>
                          Review refund
                        </Button>
                        {refund.localEntityType === 'MarketplaceBookingSubscription' ? (
                          <Button
                            component={Link}
                            href={getOrganizationSubscriptionBaseLink(integratedPlatform, data.organization?.customDomain ?? '', refund.localEntityId)}
                            sx={{ alignSelf: 'flex-start' }}
                          >
                            View subscription
                          </Button>
                        ) : null}
                      </StackColumn>
                    </CardContent>
                  </Card>
                </Grid>
              ))}
            </Grid>
          )}
          {externalRefunds.length > 0 && (
            <RefundQueue
              refunds={[]}
              statusOptions={data.marketplaceRefundStatuses ?? []}
              externalRefunds={externalRefunds}
              externalProvider={variables.externalProvider}
              externalStatus={variables.externalStatus}
              externalPageInfo={data.marketplaceExternalRefundReconciliations.pageInfo as ExternalRefundQueuePageInfo}
              onExternalFilterChange={(provider, status) =>
                setVariables({
                  externalFirst: 50,
                  externalProvider: provider,
                  externalStatus: status,
                  refundAfter: variables.refundAfter,
                  refundFirst: variables.refundFirst,
                  refundBefore: variables.refundBefore,
                  refundLast: variables.refundLast,
                })
              }
              onExternalPageChange={(direction) =>
                setVariables(
                  direction === 'next'
                    ? {
                        externalAfter: data.marketplaceExternalRefundReconciliations.pageInfo.endCursor ?? undefined,
                        externalFirst: 50,
                        externalProvider: variables.externalProvider,
                        externalStatus: variables.externalStatus,
                        refundAfter: variables.refundAfter,
                        refundFirst: variables.refundFirst,
                        refundBefore: variables.refundBefore,
                        refundLast: variables.refundLast,
                      }
                    : {
                        externalBefore: data.marketplaceExternalRefundReconciliations.pageInfo.startCursor ?? undefined,
                        externalLast: 50,
                        externalProvider: variables.externalProvider,
                        externalStatus: variables.externalStatus,
                        refundAfter: variables.refundAfter,
                        refundFirst: variables.refundFirst,
                        refundBefore: variables.refundBefore,
                        refundLast: variables.refundLast,
                      },
                )
              }
              onResolveExternal={(provider, externalRefundId, status, reason) => {
                commitResolveExternal({
                  variables: { input: { organizationId: data.organization?.id ?? '', provider, externalRefundId, status, reason } },
                  onCompleted: () => setResolved((current) => [...current, `${provider}:${externalRefundId}`]),
                });
              }}
            />
          )}
        </StackColumn>
      </Box>
    </RootShell>
  );
};

const Page = () => {
  const { organizationCustomDomain } = useKnownParams();
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationRefunds_rootQuery>(RootQuery);
  const [variables, setVariables] = useState<QueryVariables>(() => {
    const to = startOfDay();
    return {
      externalFirst: 50,
      externalStatus: 'Open',
      refundFirst: 50,
      refundRequestedAtFrom: to.subtract(1, 'month').startOf('day').toISOString(),
      refundRequestedAtTo: to.endOf('day').toISOString(),
    };
  });
  if (!organizationCustomDomain) throw new Error('organizationCustomDomain is required');

  useEffect(() => {
    loadQuery({ organizationCustomDomain, ...variables }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationCustomDomain, variables]);

  if (!queryReference) return <Loading />;
  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <RefundManagement queryReference={queryReference} variables={variables} setVariables={setVariables} />
    </ErrorBoundary>
  );
};

export default memo(Page);
