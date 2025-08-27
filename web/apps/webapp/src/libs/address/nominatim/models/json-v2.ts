export type Address = {
  house_number?: string;
  road?: string;
  neighbourhood?: string;
  suburb?: string;
  town?: string;
  city?: string;
  county?: string;
  state?: string;
  'ISO3166-2-lvl4'?: string;
  postcode?: string;
  country?: string;
  country_code?: string;
  borough?: string;
  shop?: string;
  [key: string]: string | undefined; // flexible to cover all possible address fields
};

export type AddressJsonV2 = {
  place_id: number;
  licence: string;
  osm_type: 'node' | 'way' | 'relation' | string;
  osm_id: number;
  lat: string;
  lon: string;
  category: string;
  type: string;
  place_rank: number;
  importance: number;
  icon: string;
  addresstype: string;
  name?: string;
  display_name: string;
  address: Address;
  boundingbox: [string, string, string, string];
};
