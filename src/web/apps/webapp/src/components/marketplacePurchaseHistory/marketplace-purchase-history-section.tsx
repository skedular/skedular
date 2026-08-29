import { getMarketplaceBookingDetailsLink, getMarketplaceEntitlementPurchaseDetailsLink, getMarketplaceSubscriptionDetailsLink } from '@/components/links';
import { PaymentStatusIcon } from '@/components/icons';
import { RefundStatusBadge } from '@/components/refund/RefundStatusBadge';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import { BodyIconTypography, CaptionIconTypography, SmallIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import Box from '@mui/system/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Link from '@mui/material/Link';
import NextLink from 'next/link';

export type MarketplacePurchaseHistoryItem = {
  readonly id: string;
  readonly sourceId: string;
  readonly sourceType: string;
  readonly sourceTypeName: string;
  readonly lifecycleStateName: string;
  readonly renewalStateName: string;
  readonly activityAt: string;
  readonly bookingFrom: string | null;
  readonly bookingUntil: string | null;
  readonly productTitle?: string | null;
  readonly totalAmount: number | null;
  readonly currency?: string | null;
  readonly bookingId?: string | null;
  readonly creditQuantity: number;
  readonly isDeleted: boolean;
  readonly paymentStatus: string;
  readonly refund?: { readonly status: { readonly name: string }; readonly refundAmount: number | null; readonly currencyToDisplay: string } | null;
};

type Props = {
  items: ReadonlyArray<MarketplacePurchaseHistoryItem>;
  integratedPlatform: string | undefined;
  isCustomDomain: boolean;
  organizationCustomDomain: string;
};

const MarketplacePurchaseHistorySection = ({ items, integratedPlatform, isCustomDomain, organizationCustomDomain }: Props) => {
  if (items.length === 0) {
    return null;
  }

  return (
    <Box sx={{ mt: 4 }}>
      <CaptionIconTypography label="History" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
      <SubtitleIconTypography label="Cancelled and past purchases" sx={{ mt: 0.5 }} />
      <BodyIconTypography
        label="Your cancelled bookings, subscriptions, and credit purchases stay here so you can revisit their details and invoices."
        sx={{ mt: 0.5, opacity: 0.8 }}
      />
      <Box sx={{ mt: 2, display: 'grid', gap: 1.5, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(3, minmax(0, 1fr))' } }}>
        {items.map((item) => {
          const href =
            item.sourceType === 'BOOKING' && item.bookingId
              ? getMarketplaceBookingDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, item.bookingId)
              : item.sourceType === 'SUBSCRIPTION'
                ? getMarketplaceSubscriptionDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, item.sourceId)
                : getMarketplaceEntitlementPurchaseDetailsLink(integratedPlatform, isCustomDomain, organizationCustomDomain, item.sourceId);
          const title = item.productTitle ?? item.sourceTypeName;
          const date = item.bookingFrom
            ? new Date(item.bookingFrom).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' })
            : new Date(item.activityAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' });

          return (
            <Link key={item.id} component={NextLink} href={href} underline="none" color="inherit" sx={{ display: 'block' }}>
              <Card sx={{ height: '100%', borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none', '&:hover': { borderColor: 'primary.main' } }}>
                <CardContent sx={{ p: 2.25 }}>
                  <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', gap: 1 }}>
                    <Box>
                      <SmallIconTypography label={item.sourceTypeName} sx={{ textTransform: 'uppercase', letterSpacing: '0.06em', opacity: 0.62 }} />
                      <SubtitleIconTypography label={title} sx={{ mt: 0.4 }} />
                    </Box>
                    <Chip size="small" label={item.lifecycleStateName} color="default" variant="outlined" />
                  </StackRow>
                  <StackRow sx={{ mt: 2, gap: 0.75, flexWrap: 'wrap' }}>
                    <Chip
                      size="small"
                      icon={<PaymentStatusIcon />}
                      label={item.paymentStatus === 'CONFIRMED' ? 'Paid' : item.paymentStatus}
                      color={item.paymentStatus === 'CONFIRMED' ? 'success' : 'default'}
                      variant="outlined"
                    />
                    {item.refund ? <RefundStatusBadge status={item.refund.status.name} /> : null}
                  </StackRow>
                  <StackColumn spacing={0.8} sx={{ mt: 1.5 }}>
                    <SmallIconTypography label={item.sourceType === 'SUBSCRIPTION' ? item.renewalStateName : date} sx={{ opacity: 0.72 }} />
                    {item.sourceType === 'ENTITLEMENT' ? <SmallIconTypography label={`${item.creditQuantity} credits`} sx={{ opacity: 0.72 }} /> : null}
                    {item.totalAmount !== null && item.currency ? <SmallIconTypography label={`${item.totalAmount} ${item.currency}`} sx={{ opacity: 0.72 }} /> : null}
                    {item.refund?.refundAmount !== null && item.refund?.refundAmount !== undefined ? (
                      <SmallIconTypography label={`Refund: ${item.refund.refundAmount} ${item.refund.currencyToDisplay}`} sx={{ opacity: 0.72 }} />
                    ) : null}
                  </StackColumn>
                  <StackRow sx={{ mt: 2, justifyContent: 'space-between' }}>
                    <BodyIconTypography label="Open purchase details" sx={{ color: 'primary.main', fontWeight: 600 }} />
                    <ChevronRightIcon fontSize="small" />
                  </StackRow>
                </CardContent>
              </Card>
            </Link>
          );
        })}
      </Box>
    </Box>
  );
};

export default MarketplacePurchaseHistorySection;
