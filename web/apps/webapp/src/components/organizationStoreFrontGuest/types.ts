export type GuestStoreFrontPricingOption = {
  id: string;
  name: string;
  periodLabel: string;
  description: string;
  price: number;
};

export type GuestStoreFrontProduct = {
  id: string;
  name: string;
  type: string;
  description: string;
  imageUrl: string;
  amenities: string[];
  availableCount: number;
  pricingOptions: GuestStoreFrontPricingOption[];
};

export type GuestStoreFrontData = {
  products: GuestStoreFrontProduct[];
};
