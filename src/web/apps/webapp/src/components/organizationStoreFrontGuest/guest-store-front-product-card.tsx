import { MarketplaceProductCard } from '@/components/marketplaceProductCard';
import type { marketplaceProductCard_product$key } from '@/queries/__generated__/marketplaceProductCard_product.graphql';
import { memo } from 'react';

type Props = {
  productRelay: marketplaceProductCard_product$key;
  organizationCustomDomain: string;
};

const GuestStoreFrontProductCard = ({ productRelay, organizationCustomDomain }: Props) => {
  return <MarketplaceProductCard productRelay={productRelay} organizationCustomDomain={organizationCustomDomain} />;
};

export default memo(GuestStoreFrontProductCard);
