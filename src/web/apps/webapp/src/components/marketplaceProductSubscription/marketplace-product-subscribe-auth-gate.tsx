import { formatPriceForDisplay, useIntegratedPlatform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, LeadIconTypography, StackColumn, StackRow, SubtitleIconTypography } from '@skedular/ui';
import { ArrowRightIcon, CheckIcon, ClosedAllDayIcon } from '@/components/icons';
import { getMarketplaceProductLink, getSignInLink, getSignUpLink } from '@/components/links';

import type { marketplaceProductSubscribeAuthGate_query$key } from '@/queries/__generated__/marketplaceProductSubscribeAuthGate_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Chip from '@mui/material/Chip';
import Container from '@mui/material/Container';
import Divider from '@mui/material/Divider';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';
import { isSubscriptionCadence } from './subscription-utils';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  bodyLabel?: string;
  contextLabel?: string;
  mode?: 'booking' | 'subscription';
  trustLabel?: string;
  rootDataRelay: marketplaceProductSubscribeAuthGate_query$key;
};

const subscriptionBenefitLabels = ['Book and manage subscriptions', 'Save your favorite workspaces', 'Access invoices and billing history', 'Change renewal preferences later'];

const bookingBenefitLabels = ['Book workspaces faster', 'Save your favorite workspaces', 'Access billing history', 'Modify future bookings later'];

const MarketplaceProductSubscribeAuthGate = ({ bodyLabel, contextLabel = 'You’re booking', mode = 'subscription', rootDataRelay, trustLabel }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment marketplaceProductSubscribeAuthGate_query on Query @argumentDefinitions(productId: { type: "String!" }) {
        productPricingCadences {
          type
          name
        }
        currencies {
          type
          name
        }
        product(id: $productId) {
          id
          type {
            type
            name
          }
          listingMetadata {
            title
            subTitle
            about
          }
          currency {
            type
            name
          }
          featureImages {
            original {
              url
            }
          }
          amenities {
            id
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
            supportsSubscriptionAutoRenewal
            billingMode
          }
        }
      }
    `,
    rootDataRelay,
  );

  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();
  const { integratedPlatform } = useIntegratedPlatform();
  const { isCustomDomain, organizationCustomDomain } = useKnownParams();
  const pricingOptionId = searchParams.get('pricingOptionId');
  const selectedResourceIds = useMemo(() => {
    const resourceIds = searchParams.get('resourceIds');
    if (resourceIds) {
      return resourceIds.split(',').filter(Boolean);
    }

    const resourceId = searchParams.get('resourceId');
    return resourceId ? [resourceId] : [];
  }, [searchParams]);

  const availablePricingOptions = useMemo(() => {
    const pricingOptions = [...(rootData.product?.pricingOptions ?? [])];

    return pricingOptions
      .filter((option) => (mode === 'subscription' ? isSubscriptionCadence(option.purchaseCadence) : !isSubscriptionCadence(option.purchaseCadence)))
      .sort((left, right) => left.index - right.index);
  }, [mode, rootData.product?.pricingOptions]);

  const selectedPricingOption = useMemo(() => {
    if (pricingOptionId) {
      const selectedOption = availablePricingOptions.find((item) => item.id === pricingOptionId);

      if (selectedOption) {
        return selectedOption;
      }
    }

    return availablePricingOptions[0] ?? null;
  }, [availablePricingOptions, pricingOptionId]);
  const cadenceLabel = useMemo(
    () => rootData.productPricingCadences.find((item) => item.type === selectedPricingOption?.purchaseCadence)?.name ?? selectedPricingOption?.purchaseCadence ?? '',
    [rootData.productPricingCadences, selectedPricingOption?.purchaseCadence],
  );
  const returnTo = useMemo(() => {
    const query = searchParams.toString();
    return query ? `${pathname}?${query}` : pathname;
  }, [pathname, searchParams]);

  if (!rootData.product) {
    return null;
  }
  const product = rootData.product;

  if (product.type.type === 'EVENT' && mode === 'subscription') {
    return (
      <Container maxWidth="sm" sx={{ py: { xs: 4, md: 6 } }}>
        <Card sx={{ borderRadius: 4 }}>
          <CardContent sx={{ p: 4, textAlign: 'center' }}>
            <CaptionIconTypography label="Timed booking only" sx={{ letterSpacing: '0.08em', textTransform: 'uppercase', opacity: 0.68 }} />
            <LeadIconTypography label="This event product does not support recurring plans" sx={{ mt: 1 }} />
            <BodyIconTypography
              label="Event products must be booked with an explicit date and time. Use the booking flow for this product instead of starting a subscription."
              sx={{ mt: 1.5, opacity: 0.82 }}
            />
            <Button
              variant="contained"
              sx={{ mt: 3, textTransform: 'none' }}
              onClick={() => router.push(getMarketplaceProductLink(integratedPlatform, isCustomDomain, organizationCustomDomain, product.id, selectedResourceIds))}
            >
              Back to product
            </Button>
          </CardContent>
        </Card>
      </Container>
    );
  }

  const currencyLabel = rootData.currencies.find((item) => item.type === product.currency.type)?.name ?? product.currency.name ?? '';

  if (!selectedPricingOption) {
    return null;
  }

  const productLink = getMarketplaceProductLink(integratedPlatform, isCustomDomain, organizationCustomDomain, product.id, selectedResourceIds);
  const priceLabel = formatPriceForDisplay(currencyLabel, selectedPricingOption.price, selectedPricingOption.purchaseCadence);
  const pricingTitle = selectedPricingOption.listingMetadata.title ?? selectedPricingOption.listingMetadata.subTitle ?? cadenceLabel;

  return (
    <Box
      sx={{
        minHeight: '100vh',
        pb: 8,
        background:
          'radial-gradient(circle at top left, rgba(23, 93, 175, 0.14), transparent 28%), radial-gradient(circle at top right, rgba(255, 159, 67, 0.12), transparent 22%)',
      }}
    >
      <Container maxWidth="md" sx={{ pt: { xs: 3, md: 5 } }}>
        <Card sx={{ overflow: 'hidden', borderRadius: 5, boxShadow: '0 18px 64px rgba(7, 22, 41, 0.14)' }}>
          <Box
            sx={{
              position: 'relative',
              display: 'grid',
              gap: 3,
              gridTemplateColumns: { xs: '1fr', md: '120px minmax(0, 1fr)' },
              alignItems: 'center',
              p: { xs: 3, md: 4 },
              color: 'common.white',
              backgroundImage: `linear-gradient(140deg, rgba(7,14,28,0.94), rgba(16,71,110,0.88)), url(${rootData.product.featureImages[0]?.original?.url ?? ''})`,
              backgroundSize: 'cover',
              backgroundPosition: 'center',
            }}
          >
            <Box
              sx={{
                position: 'absolute',
                inset: 0,
                background: 'linear-gradient(140deg, rgba(7,14,28,0.76), rgba(16,71,110,0.58) 48%, rgba(255,255,255,0.08) 100%)',
                pointerEvents: 'none',
              }}
            />
            {rootData.product.featureImages[0]?.original?.url ? (
              <Box
                component="img"
                src={rootData.product.featureImages[0].original.url}
                alt={rootData.product.listingMetadata.title ?? ''}
                sx={{
                  width: { xs: '100%', md: 120 },
                  height: { xs: 180, md: 120 },
                  objectFit: 'cover',
                  borderRadius: 3,
                  border: '1px solid rgba(255,255,255,0.14)',
                }}
              />
            ) : null}

            <Box
              sx={{
                position: 'relative',
                zIndex: 1,
                px: { xs: 0, md: 1 },
                py: 1,
                borderRadius: 3,
                bgcolor: 'rgba(7, 14, 28, 0.34)',
                backdropFilter: 'blur(10px)',
                boxShadow: '0 12px 32px rgba(3, 8, 19, 0.18)',
              }}
            >
              <CaptionIconTypography label={contextLabel} sx={{ letterSpacing: '0.14em', textTransform: 'uppercase', opacity: 0.86, color: 'common.white' }} />
              <LeadIconTypography
                label={rootData.product.listingMetadata.title ?? ''}
                sx={{
                  mt: 1,
                  fontSize: { xs: '1.7rem', md: '2.1rem' },
                  lineHeight: 1.05,
                  color: 'common.white',
                  textShadow: '0 2px 18px rgba(0,0,0,0.35)',
                }}
              />
              <SubtitleIconTypography
                label={rootData.product.listingMetadata.subTitle ?? ''}
                sx={{ mt: 0.75, opacity: 0.96, color: 'rgba(255,255,255,0.96)', textShadow: '0 1px 14px rgba(0,0,0,0.3)' }}
              />
              <StackRow sx={{ mt: 2 }}>
                <Chip label={`${pricingTitle} • ${priceLabel}`} sx={{ bgcolor: 'rgba(255,255,255,0.14)', color: 'common.white', borderRadius: 999 }} />
                {rootData.product.amenities.slice(0, 2).map((amenity) => (
                  <Chip key={amenity.id} label={amenity.name} variant="outlined" sx={{ borderColor: 'rgba(255,255,255,0.22)', color: 'common.white', borderRadius: 999 }} />
                ))}
              </StackRow>
            </Box>
          </Box>

          <CardContent sx={{ p: { xs: 3, md: 5 }, textAlign: 'center' }}>
            <Box
              sx={{
                width: 84,
                height: 84,
                borderRadius: '50%',
                bgcolor: 'rgba(16, 118, 191, 0.08)',
                display: 'inline-flex',
                alignItems: 'center',
                justifyContent: 'center',
                mb: 3,
              }}
            >
              <ClosedAllDayIcon sx={{ fontSize: 40, color: 'primary.main' }} />
            </Box>

            <LeadIconTypography label="Sign in to continue" sx={{ fontSize: { xs: '1.9rem', md: '2.35rem' } }} />
            <BodyIconTypography
              label={bodyLabel ?? `You’ll need an account to start this ${cadenceLabel.toLowerCase()} plan, manage renewals, and keep your booking details saved in one place.`}
              sx={{ mt: 1.5, opacity: 0.76, maxWidth: 560, mx: 'auto' }}
            />

            <Box
              sx={{
                mt: 4,
                mx: 'auto',
                p: 2.5,
                maxWidth: 540,
                textAlign: 'left',
                borderRadius: 4,
                bgcolor: (theme) => theme.palette.action.hover,
              }}
            >
              <StackColumn spacing={1.5}>
                {(mode === 'subscription' ? subscriptionBenefitLabels : bookingBenefitLabels).map((benefit) => (
                  <StackRow key={benefit} spacing={1.5} sx={{ flexWrap: 'nowrap' }}>
                    <Box
                      sx={{
                        width: 24,
                        height: 24,
                        borderRadius: '50%',
                        bgcolor: 'rgba(25, 135, 84, 0.14)',
                        display: 'inline-flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        color: 'success.main',
                        flexShrink: 0,
                      }}
                    >
                      <CheckIcon sx={{ fontSize: 15 }} />
                    </Box>
                    <BodyIconTypography label={benefit} />
                  </StackRow>
                ))}
              </StackColumn>
            </Box>

            <StackColumn spacing={1.5} sx={{ mt: 4, maxWidth: 540, mx: 'auto' }}>
              <Button
                variant="contained"
                size="large"
                onClick={() => router.push(`${getSignInLink()}?returnTo=${encodeURIComponent(returnTo)}`)}
                sx={{ textTransform: 'none', py: 1.7, borderRadius: 3 }}
              >
                Log In
                <ArrowRightIcon sx={{ ml: 0.5 }} />
              </Button>
              <Button
                variant="outlined"
                size="large"
                onClick={() => router.push(`${getSignUpLink()}?returnTo=${encodeURIComponent(returnTo)}`)}
                sx={{ textTransform: 'none', py: 1.7, borderRadius: 3, borderWidth: 2 }}
              >
                Create Account
              </Button>
            </StackColumn>

            <Divider sx={{ my: 3.5 }} />

            <Button variant="text" onClick={() => router.push(productLink)} sx={{ textTransform: 'none' }}>
              Back to product details
            </Button>
          </CardContent>

          <Box
            sx={{
              px: { xs: 3, md: 5 },
              py: 2.5,
              bgcolor: (theme) => theme.palette.action.hover,
              display: 'flex',
              justifyContent: 'center',
              gap: 2.5,
              flexWrap: 'wrap',
            }}
          >
            <CaptionIconTypography label="Secure checkout" sx={{ opacity: 0.72 }} />
            <CaptionIconTypography label={trustLabel ?? 'Return to this exact page after auth'} sx={{ opacity: 0.72 }} />
            <CaptionIconTypography
              label={
                mode === 'subscription'
                  ? selectedPricingOption.supportsSubscriptionAutoRenewal
                    ? 'Auto-renew can be managed later'
                    : 'One cycle access only'
                  : 'Booking details stay with your account'
              }
              sx={{ opacity: 0.72 }}
            />
          </Box>
        </Card>
      </Container>
    </Box>
  );
};

export default memo(MarketplaceProductSubscribeAuthGate);
