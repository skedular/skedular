export type MarketplaceProductPricingPlan = {
  id: string;
  title: string;
  subTitle: string;
  cadence: string;
  cadenceLabel: string;
  amountLabel: string;
  note: string;
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
