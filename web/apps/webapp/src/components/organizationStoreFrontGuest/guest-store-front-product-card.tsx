import { MarketplaceProductCard } from '@/components/marketplaceProductCard';
import type { guestStoreFrontProductCard_product$key } from '@/queries/__generated__/guestStoreFrontProductCard_product.graphql';
import type { guestStoreFrontProductCard_query$key } from '@/queries/__generated__/guestStoreFrontProductCard_query.graphql';
import { memo, useMemo } from 'react';
import { graphql, useFragment } from 'react-relay';

type Props = {
  rootDataRelay: guestStoreFrontProductCard_query$key;
  productRelay: guestStoreFrontProductCard_product$key;
  organizationCustomDomain: string;
};

const GuestStoreFrontProductCard = ({ rootDataRelay, productRelay, organizationCustomDomain }: Props) => {
  const rootData = useFragment<guestStoreFrontProductCard_query$key>(
    graphql`
      fragment guestStoreFrontProductCard_query on Query {
        productPricingCadences {
          type
          name
        }
        currencies {
          type
          name
        }
      }
    `,
    rootDataRelay,
  );

  const product = useFragment(
    graphql`
      fragment guestStoreFrontProductCard_product on ProductDetails {
        id
        listingMetadata {
          title
          subTitle
        }
        featureImages {
          original {
            url
          }
        }
        currency {
          type
          name
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
          isTaxInclusive
          supportsSubscriptionAutoRenewal
        }
      }
    `,
    productRelay,
  );

  const currency = product.currency ? rootData.currencies.find((item) => item.type === product.currency?.type)?.name : null;

  const pricingRows = useMemo(
    () =>
      [...product.pricingOptions]
        .sort((a, b) => a.index - b.index)
        .map((option) => ({
          id: option.id,
          title: option.listingMetadata.title ?? '',
          cadence: option.purchaseCadence,
          cadenceLabel: rootData.productPricingCadences.find((cadence) => cadence.type === option.purchaseCadence)?.name ?? option.purchaseCadence,
          amountLabel: currency ? `${currency} ${option.price}` : `${option.price}`,
          taxLabel: option.isTaxInclusive ? 'incl. tax' : 'excl. tax',
        })),
    [currency, product.pricingOptions, rootData.productPricingCadences],
  );

  return (
    <MarketplaceProductCard
      amenities={product.amenities}
      imageUrl={product.featureImages[0]?.original?.url ?? ''}
      organizationCustomDomain={organizationCustomDomain}
      pricingRows={pricingRows}
      productId={product.id}
      subTitle={product.listingMetadata.subTitle ?? ''}
      title={product.listingMetadata.title ?? ''}
    />
  );
};

export default memo(GuestStoreFrontProductCard);
