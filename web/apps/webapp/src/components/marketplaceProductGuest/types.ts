export type MarketplaceProductPricingPlan = {
  id: string;
  name: string;
  description?: string;
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
