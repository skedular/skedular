export type AddressComponent = {
  long_name?: string;
  short_name?: string;
  types: string[];
  // Allow extra fields from Google payloads
  [key: string]: unknown;
};

export type PlacePrediction = {
  description: string;
  place_id: string;
  types?: string[];
};

export type PlaceDetailsResult = {
  placeId: string;
  formattedAddress: string;
  latitude: number;
  longitude: number;
  addressComponents: AddressComponent[];
  name?: string;
};
