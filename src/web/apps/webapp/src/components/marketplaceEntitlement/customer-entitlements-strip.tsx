import { getMarketplaceEntitlementBookingLink } from '@/components/links';
import type { customerEntitlementsStrip_query$key } from '@/queries/__generated__/customerEntitlementsStrip_query.graphql';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import NextLink from 'next/link';
import { memo } from 'react';
import { graphql, useFragment } from 'react-relay';
import dayjs from 'dayjs';
import { BodyIconTypography, CaptionIconTypography, SmallIconTypography, StackColumn, SubtitleIconTypography } from '@skedular/ui';

type Props = {
  queryReference: customerEntitlementsStrip_query$key;
  integratedPlatform?: string;
};

const QueryFragment = graphql`
  fragment customerEntitlementsStrip_query on Query {
    myEntitlements {
      id
      pricingId
      availableQuantity
      grantedQuantity
      expiresAt
      status
      restrictions {
        productId
        availableDays
      }
    }
  }
`;

const CustomerEntitlementsStrip = ({ queryReference, integratedPlatform }: Props) => {
  const data = useFragment(QueryFragment, queryReference);
  const entitlements = data.myEntitlements.filter((item) => item.status === 'ACTIVE' && item.availableQuantity > 0);

  if (entitlements.length === 0) return null;

  return (
    <Box sx={{ maxWidth: 1600, mx: 'auto', px: { xs: 2, md: 3 }, pt: 2 }}>
      <StackColumn spacing={2} sx={{ mb: 3 }}>
        <div>
          <CaptionIconTypography label="Your purchased entitlements" sx={{ textTransform: 'uppercase', opacity: 0.66 }} />
          <SubtitleIconTypography label="Ready to use" sx={{ mt: 0.75 }} />
          <BodyIconTypography label="Book directly with any available credits." sx={{ mt: 0.5, opacity: 0.8 }} />
        </div>
        {entitlements.map((entitlement) => {
          const bookingLink = getMarketplaceEntitlementBookingLink(integratedPlatform, entitlement.id);

          return (
            <Card key={entitlement.id} sx={{ borderRadius: 3, border: 1, borderColor: 'divider', boxShadow: 'none' }}>
              <CardContent sx={{ p: { xs: 2, sm: 2.5 } }}>
                <BodyIconTypography label="Booking credits" sx={{ fontWeight: 700 }} />
                <SmallIconTypography
                  label={`${entitlement.availableQuantity} of ${entitlement.grantedQuantity} available · Valid until ${dayjs(entitlement.expiresAt).format('MMM D, YYYY')}`}
                  sx={{ mt: 0.5, opacity: 0.78 }}
                />
                {entitlement.restrictions?.availableDays.length ? (
                  <SmallIconTypography label={`Available on ${entitlement.restrictions.availableDays.join(', ')}`} sx={{ mt: 0.5, opacity: 0.78 }} />
                ) : null}
                {bookingLink ? (
                  <Button component={NextLink} href={bookingLink} variant="contained" sx={{ mt: 1.5, textTransform: 'none' }}>
                    Book with credits
                  </Button>
                ) : null}
              </CardContent>
            </Card>
          );
        })}
      </StackColumn>
    </Box>
  );
};

export default memo(CustomerEntitlementsStrip);
