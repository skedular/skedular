export type MarketplaceProductPricingPlan = {
  id: string;
  name: string;
  cadenceLabel: string;
  amountLabel: string;
  note: string;
  highlighted?: boolean;
};

export type MarketplaceProductResource = {
  id: string;
  name: string;
  details: string[];
};

export type MarketplaceProductLocation = {
  id: string;
  name: string;
  address: string;
  availableLabel: string;
  resources: MarketplaceProductResource[];
};

export type MarketplaceProductDetail = {
  id: string;
  title: string;
  typeLabel: string;
  shortDescription: string;
  longDescription: string;
  imageUrls: string[];
  features: string[];
  amenities: string[];
  pricingPlans: MarketplaceProductPricingPlan[];
  locations: MarketplaceProductLocation[];
};
