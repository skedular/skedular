import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackRow, SubtitleIconTypography } from '@/components/commons';
import { getMarketplaceLocationLink, getMarketplaceProductBookingLink, getMarketplaceProductSubscribeLink } from '@/components/links';
import MarketplaceCancellationPolicyDetails from '@/components/marketplaceProduct/cancellation-policy-details';
import { isSubscriptionCadence } from '@/components/marketplaceProductSubscription/subscription-utils';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import type { marketplaceProductDetailBookingCard_product$key } from '@/queries/__generated__/marketplaceProductDetailBookingCard_product.graphql';
import type { marketplaceProductDetailBookingCard_query$key } from '@/queries/__generated__/marketplaceProductDetailBookingCard_query.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Link from '@mui/material/Link';
import Box from '@mui/system/Box';
import NextLink from 'next/link';
import { useRouter } from 'next/navigation';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';
import type { MarketplaceProductPricingPlan } from './types';

type Props = {
  rootDataRelay: marketplaceProductDetailBookingCard_query$key;
};

const MarketplaceProductDetailBookingCard = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<marketplaceProductDetailBookingCard_query$key>(
    graphql`
      fragment marketplaceProductDetailBookingCard_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        productPricingCadences {
          type
          name
        }
        currencies {
          type
          name
        }
        product(id: $productId) {
          ...marketplaceProductDetailBookingCard_product
        }
        marketplaceLocations(where: { productIds: [$productId] }) {
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

  const product = useFragment<marketplaceProductDetailBookingCard_product$key>(
    graphql`
      fragment marketplaceProductDetailBookingCard_product on ProductDetails {
        id
        listingMetadata {
          about
          title
          subTitle
          includedFeatures
        }
        amenities {
          id
          name
          color
        }
        currency {
          type
          name
        }
        pricingOptions {
          id
          index
          listingMetadata {
            title
            subTitle
          }
          purchaseCadence
          price
          isTaxInclusive
          supportsSubscriptionAutoRenewal
          acceptedPaymentMethods
          minDurationMinutes
          maxDurationMinutes
          numberOfResourcesToBook
          cancellationPolicyType
          cancellationRefundRules {
            minutesBefore
            refundPercentage
          }
        }
      }
    `,
    rootData.product,
  );
  const router = useRouter();
  const { integratedPlatrform } = useIntegratedPlatrform();
  const pricingPlans = useMemo<MarketplaceProductPricingPlan[]>(() => {
    if (!product) {
      return [];
    }

    const currencyLabel = rootData.currencies.find((item) => item.type === product.currency.type)?.name ?? product.currency.name;

    return [...product.pricingOptions]
      .sort((left, right) => left.index - right.index)
      .map((pricingOption) => ({
        id: pricingOption.id,
        title: pricingOption.listingMetadata.title ?? '',
        subTitle: pricingOption.listingMetadata.subTitle ?? '',
        cadence: pricingOption.purchaseCadence,
        cadenceLabel: rootData.productPricingCadences.find((item) => item.type === pricingOption.purchaseCadence)?.name ?? pricingOption.purchaseCadence,
        amountLabel: `${currencyLabel}${pricingOption.price}`,
        note: pricingOption.isTaxInclusive ? 'incl. tax' : 'excl. tax',
        cancellationPolicyType: pricingOption.cancellationPolicyType,
        cancellationRefundRules: pricingOption.cancellationRefundRules,
      }));
  }, [product, rootData.currencies, rootData.productPricingCadences]);
  const marketplaceLocations = useMemo(
    () => rootData.marketplaceLocations.edges.map((edge) => edge.node).filter((location): location is NonNullable<typeof location> => !!location),
    [rootData.marketplaceLocations.edges],
  );
  const canBookProduct = marketplaceLocations.length > 0;
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();

  if (!product) {
    return null;
  }

  return (
    <Box sx={{ position: { md: 'sticky' }, top: { md: 90 } }}>
      <Card sx={{ borderRadius: 3, border: 1, borderColor: (theme) => theme.palette.divider }}>
        <CardContent sx={{ p: { xs: 2.5, md: 3 }, '&:last-child': { pb: { xs: 2.5, md: 3 } } }}>
          <CaptionIconTypography label="Product" sx={{ letterSpacing: '0.04em', textTransform: 'uppercase', opacity: 0.7 }} />
          <LeadIconTypography label={product.listingMetadata.title} sx={{ mt: 0.4, mb: 0.6 }} />
          <BodyIconTypography label={product.listingMetadata.about ?? ''} sx={{ opacity: 0.85, mb: 2.2 }} />

          <LeadIconTypography label="Select a pricing option" sx={{ mb: 1.2 }} />
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
            {pricingPlans.map((pricingPlan) => (
              <Box
                key={pricingPlan.id}
                sx={{
                  border: 1,
                  borderColor: (theme) => theme.palette.divider,
                  borderRadius: 2,
                  px: 1.35,
                  py: 1.2,
                }}
              >
                <StackRow sx={{ justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'nowrap' }}>
                  <Box sx={{ minWidth: 0, pr: 1 }}>
                    <CaptionIconTypography label={pricingPlan.cadenceLabel} fontWeight={600} />
                    <SubtitleIconTypography label={pricingPlan.title} sx={{ lineHeight: 1.25 }} />
                    {pricingPlan.subTitle && <CaptionIconTypography label={pricingPlan.subTitle} sx={{ mt: 0.5, opacity: 0.78 }} />}
                  </Box>
                  <Box sx={{ textAlign: 'right', flexShrink: 0 }}>
                    <SubtitleIconTypography label={pricingPlan.amountLabel} fontWeight={600} sx={{ lineHeight: 1.2 }} />
                    <CaptionIconTypography label={pricingPlan.note} sx={{ opacity: 0.7 }} />
                  </Box>
                </StackRow>
                <Button
                  fullWidth
                  variant="contained"
                  disabled={!canBookProduct}
                  onClick={() => {
                    if (!canBookProduct) {
                      return;
                    }

                    router.push(
                      isSubscriptionCadence(pricingPlan.cadence)
                        ? getMarketplaceProductSubscribeLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, product.id, pricingPlan.id)
                        : getMarketplaceProductBookingLink(integratedPlatrform, isCustomDomain, organizationCustomDomain, product.id, pricingPlan.id),
                    );
                  }}
                  sx={{ mt: 1.2, textTransform: 'none' }}
                >
                  {canBookProduct ? (isSubscriptionCadence(pricingPlan.cadence) ? 'Choose plan' : 'Book now') : 'Unavailable'}
                </Button>
                <Box sx={{ mt: 1.2 }}>
                  <MarketplaceCancellationPolicyDetails
                    cancellationPolicyType={pricingPlan.cancellationPolicyType}
                    cancellationRefundRules={pricingPlan.cancellationRefundRules}
                    compact
                    eventLabel={isSubscriptionCadence(pricingPlan.cadence) ? 'the next renewal' : 'the booking starts'}
                  />
                </Box>
              </Box>
            ))}
          </Box>

          <Box sx={{ mt: 2 }}>
            <CaptionIconTypography label={marketplaceLocations.length > 0 ? 'Available locations' : 'Availability'} sx={{ opacity: 0.72, mb: 0.8 }} />
            {marketplaceLocations.length > 0 ? (
              <StackRow spacing={0.75}>
                {marketplaceLocations.map((location) => (
                  <Link
                    key={location.id}
                    component={NextLink}
                    href={getMarketplaceLocationLink(integratedPlatrform, location.id)}
                    underline="none"
                    color="inherit"
                    sx={{ display: 'inline-flex' }}
                  >
                    <CaptionIconTypography
                      label={location.name}
                      sx={{
                        px: 1,
                        py: 0.5,
                        borderRadius: 1,
                        bgcolor: (theme) => theme.palette.action.hover,
                        transition: 'background-color 120ms ease',
                        '&:hover': {
                          bgcolor: (theme) => theme.palette.action.selected,
                        },
                      }}
                    />
                  </Link>
                ))}
              </StackRow>
            ) : (
              <BodyIconTypography label="This product can't be booked right now because no resources are currently available at any location." sx={{ opacity: 0.78 }} />
            )}
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default memo(MarketplaceProductDetailBookingCard);
