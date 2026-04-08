import { BodyIconTypography, CaptionIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@/components/commons';
import { Loading } from '@/components/loading';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import type { pageOrganizationRefunds_rootQuery } from '@/queries/__generated__/pageOrganizationRefunds_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Alert from '@mui/material/Alert';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationRefunds_rootQuery($organizationCustomDomain: String!, $statuses: [String!]) {
    organization(customDomain: $organizationCustomDomain) {
      name
    }
    organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {
      canModifyPaymentMethod
    }
    marketplaceRefundStatuses {
      type
      name
    }
    marketplaceRefunds(organizationCustomDomain: $organizationCustomDomain, statuses: $statuses) {
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
      requestedByCustomerName
      refundAmount
      refundPercentage
      currencyToDisplay
      reason
      accountingProvider
      externalRefundNumber
      lastProcessedAt
      lastError
      canProcessInXero
      xeroProcessingBlockedReason
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
`;

const toRefundStatusType = (value?: string | null | undefined) => value?.replace(/([a-z])([A-Z])/g, '$1_$2').toUpperCase() ?? '';

type Props = {
  queryReference: PreloadedQuery<pageOrganizationRefunds_rootQuery, Record<string, unknown>>;
};

const RootPage = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationRefunds_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Refund operations" />
          <BodyIconTypography label={rootData.organization?.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  if (!rootData.organizationBookingPermissions?.canModifyPaymentMethod) {
    return (
      <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
        <Alert severity="warning">You do not have permission to manage refunds for this organization.</Alert>
      </RootShell>
    );
  }

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <StackColumn spacing={2}>
        <StackColumn spacing={0.5}>
          <CaptionIconTypography label="Refund operations" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
          <SubtitleIconTypography label={`${rootData.marketplaceRefunds.length} refund${rootData.marketplaceRefunds.length === 1 ? '' : 's'}`} />
          <BodyIconTypography label="Use this view to review refund state, blocked Xero follow-up, manual actions, and audit history across bookings and subscriptions." />
        </StackColumn>
        {rootData.marketplaceRefunds.map((refund) => {
          const entityLabel = refund.localEntityType === 'MarketplaceBookingSubscription' ? 'subscription' : 'booking';
          const refundStatusType = toRefundStatusType(refund.status.type);
          return (
            <Card key={refund.id} sx={{ borderRadius: 3, border: 1, borderColor: (theme) => theme.palette.divider, boxShadow: 'none' }}>
              <CardContent>
                <StackColumn spacing={1.5}>
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', gap: 2, flexWrap: 'wrap' }}>
                    <StackColumn spacing={0.5}>
                      <SubtitleIconTypography label={`${entityLabel === 'subscription' ? 'Subscription' : 'Booking'} refund`} />
                      <SmallIconTypography label={`Entity id: ${refund.localEntityId}`} sx={{ opacity: 0.72 }} />
                      {refund.requestedByCustomerName ? <SmallIconTypography label={`Requested by ${refund.requestedByCustomerName}`} sx={{ opacity: 0.72 }} /> : null}
                    </StackColumn>
                    <Chip
                      label={refund.status.name}
                      color={
                        refundStatusType === 'FAILED' || refundStatusType === 'MANUAL_REQUIRED'
                          ? 'warning'
                          : refundStatusType === 'COMPLETED' || refundStatusType === 'MANUAL_COMPLETED'
                            ? 'success'
                            : 'info'
                      }
                    />
                  </StackRow>
                  <MarketplaceRefundAdminPanel entityLabel={entityLabel} refund={refund} />
                </StackColumn>
              </CardContent>
            </Card>
          );
        })}
      </StackColumn>
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [selectedStatuses, setSelectedStatuses] = useState<string[]>([]);
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationRefunds_rootQuery>(RootQuery);
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        statuses: selectedStatuses.length > 0 ? selectedStatuses : null,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, organizationCustomDomain, selectedStatuses]);

  const filters = useMemo(
    () =>
      queryReference ? (
        <FilterShell
          queryReference={queryReference}
          onToggleStatus={(status) => setSelectedStatuses((current) => (current.includes(status) ? current.filter((item) => item !== status) : [...current, status]))}
          selectedStatuses={selectedStatuses}
        />
      ) : null,
    [queryReference, selectedStatuses],
  );

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <>
      {filters}
      <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
        <MemoRootPage queryReference={queryReference} />
      </ErrorBoundary>
    </>
  );
};

const FilterShell = ({
  queryReference,
  onToggleStatus,
  selectedStatuses,
}: {
  queryReference: PreloadedQuery<pageOrganizationRefunds_rootQuery, Record<string, unknown>>;
  onToggleStatus: (status: string) => void;
  selectedStatuses: string[];
}) => {
  const rootData = usePreloadedQuery<pageOrganizationRefunds_rootQuery>(RootQuery, queryReference);

  return (
    <StackRow sx={{ mb: 2, gap: 1, flexWrap: 'wrap' }}>
      {rootData.marketplaceRefundStatuses.map((status) => (
        <Chip key={status.type} label={status.name} clickable color={selectedStatuses.includes(status.type) ? 'primary' : 'default'} onClick={() => onToggleStatus(status.type)} />
      ))}
    </StackRow>
  );
};

export default memo(RootPageWithRelay);
