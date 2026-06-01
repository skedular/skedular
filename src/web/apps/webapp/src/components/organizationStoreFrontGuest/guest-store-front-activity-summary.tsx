import { getMarketplaceBookingsLink, getMarketplaceSubscriptionsLink } from '@/components/links';
import useKnownParams from '@/hooks/use-known-params';
import type { guestStoreFrontActivitySummary_query$key } from '@/queries/__generated__/guestStoreFrontActivitySummary_query.graphql';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import Button from '@mui/material/Button';
import Chip from '@mui/material/Chip';
import Paper from '@mui/material/Paper';
import Box from '@mui/system/Box';
import { useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, StackRow, SubtitleIconTypography } from '@skedular/ui';
import NextLink from 'next/link';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontActivitySummary_query$key;
};

const GuestStoreFrontActivitySummary = ({ rootDataRelay }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment guestStoreFrontActivitySummary_query on Query
      @argumentDefinitions(
        bookingsSearchCriteriaFrom: { type: "DateTime!" }
        bookingsSearchCriteriaTo: { type: "DateTime!" }
        includeUpcomingBookings: { type: "Boolean!", defaultValue: false }
        includeActiveSubscriptions: { type: "Boolean!", defaultValue: false }
        organizationCustomDomain: { type: "String!" }
      ) {
        bookings(
          first: 0
          where: {
            organizationCustomDomain: $organizationCustomDomain
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
            includeMineOnly: true
            channel: MARKETPLACE
          }
        ) @include(if: $includeUpcomingBookings) {
          totalCount
        }
        marketplaceBookingSubscriptions(first: 0, where: { includeMineOnly: true, organizationCustomDomain: $organizationCustomDomain, status: ACTIVE })
          @include(if: $includeActiveSubscriptions) {
          totalCount
        }
      }
    `,
    rootDataRelay,
  );
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();

  if (!rootData.bookings && !rootData.marketplaceBookingSubscriptions) {
    return null;
  }

  const bookingCount = rootData.bookings?.totalCount ?? 0;
  const subscriptionCount = rootData.marketplaceBookingSubscriptions?.totalCount ?? 0;
  const activityLabel = `${bookingCount} ${bookingCount === 1 ? 'booking' : 'bookings'} this week · ${subscriptionCount} active ${
    subscriptionCount === 1 ? 'subscription' : 'subscriptions'
  }`;

  return (
    <Paper
      variant="outlined"
      sx={{
        borderRadius: 2,
        px: { xs: 1.5, md: 2 },
        py: 1.25,
        bgcolor: (theme) => theme.palette.background.paper,
      }}
    >
      <StackRow sx={{ justifyContent: 'space-between', alignItems: 'center', gap: 1.5 }}>
        <Box sx={{ minWidth: 0 }}>
          <CaptionIconTypography label="Your activity" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.66 }} />
          <StackRow sx={{ mt: 0.5, alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
            <SubtitleIconTypography label={activityLabel} />
            {bookingCount + subscriptionCount === 0 ? <Chip size="small" variant="outlined" label="Nothing active" /> : <Chip size="small" color="primary" label="Signed in" />}
          </StackRow>
          <BodyIconTypography label="Quick links to your bookings and subscriptions for this marketplace." sx={{ mt: 0.5, opacity: 0.72 }} />
        </Box>

        <StackRow spacing={1} sx={{ flexWrap: 'nowrap' }}>
          <Button
            component={NextLink}
            href={getMarketplaceBookingsLink(integratedPlatform, isCustomDomain, organizationCustomDomain)}
            variant="text"
            endIcon={<ChevronRightIcon fontSize="small" />}
            sx={{ textTransform: 'none', whiteSpace: 'nowrap' }}
          >
            Bookings
          </Button>
          <Button
            component={NextLink}
            href={getMarketplaceSubscriptionsLink(integratedPlatform, isCustomDomain, organizationCustomDomain)}
            variant="text"
            endIcon={<ChevronRightIcon fontSize="small" />}
            sx={{ textTransform: 'none', whiteSpace: 'nowrap' }}
          >
            Subscriptions
          </Button>
        </StackRow>
      </StackRow>
    </Paper>
  );
};

export default memo(GuestStoreFrontActivitySummary);
