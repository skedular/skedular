import { RelayError, toRootError, useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import { getOrganizationRefundsBaseLink } from '@/components/links';
import { Loading } from '@/components/loading';
import MarketplaceRefundAdminPanel from '@/components/marketplaceRefund/marketplace-refund-admin-panel';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationRefundDetail_rootQuery } from '@/queries/__generated__/pageOrganizationRefundDetail_rootQuery.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { PageHeaderPanel, StackColumn } from '@skedular/ui';
import Link from 'next/link';
import { useParams } from 'next/navigation';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationRefundDetail_rootQuery($refundId: String!) {
    marketplaceRefund(id: $refundId) {
      id
      localEntityType
      currency {
        type
        name
      }
      status {
        type
        name
      }
      requestedByCustomerName
      refundAmount
      currencyToDisplay
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
`;
const Content = ({
  queryReference,
  organizationCustomDomain,
}: {
  queryReference: PreloadedQuery<pageOrganizationRefundDetail_rootQuery, Record<string, unknown>>;
  organizationCustomDomain: string;
}) => {
  const data = usePreloadedQuery<pageOrganizationRefundDetail_rootQuery>(RootQuery, queryReference);
  const { integratedPlatform } = useIntegratedPlatform();
  if (!data.marketplaceRefund)
    return (
      <RootShell>
        <PageHeaderPanel title="Refund not found" description="This refund may no longer be available." />
      </RootShell>
    );
  return (
    <RootShell>
      <StackColumn spacing={2} sx={{ maxWidth: 1200, mx: 'auto', width: '100%' }}>
        <Button component={Link} href={getOrganizationRefundsBaseLink(integratedPlatform, organizationCustomDomain)} sx={{ alignSelf: 'flex-start' }}>
          Back to refunds
        </Button>
        <PageHeaderPanel title="Refund details" description="Review the full timeline and take the next permitted action." />
        <Card variant="outlined" sx={{ borderRadius: 3 }}>
          <CardContent>
            <MarketplaceRefundAdminPanel
              entityLabel={data.marketplaceRefund.localEntityType === 'MarketplaceBookingSubscription' ? 'subscription' : 'booking'}
              refund={data.marketplaceRefund}
            />
          </CardContent>
        </Card>
      </StackColumn>
    </RootShell>
  );
};
const Page = () => {
  const { organizationCustomDomain } = useKnownParams();
  const { refundId } = useParams<{ refundId: string }>();
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationRefundDetail_rootQuery>(RootQuery);
  if (!organizationCustomDomain || !refundId) throw new Error('organizationCustomDomain and refundId are required');
  useEffect(() => {
    loadQuery({ refundId }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, refundId]);
  return !queryReference ? (
    <Loading />
  ) : (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <Content queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};
export default memo(Page);
