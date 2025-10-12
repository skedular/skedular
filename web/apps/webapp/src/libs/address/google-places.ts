type AddressComponent = {
  long_name?: string;
  short_name?: string;
  types: string[];
  [key: string]: unknown;
};

type AutocompletePrediction = {
  placeId?: string;
  text?: {
    text?: string;
  };
  types?: string[];
};

type AutocompleteSuggestion = {
  placePrediction?: AutocompletePrediction;
};

type AutocompleteResponse = {
  suggestions?: AutocompleteSuggestion[];
};

type PlacesLocation = {
  latitude?: number;
  longitude?: number;
};

type PlacesAddressComponent = {
  longText?: string;
  shortText?: string;
  types?: string[];
};

type PlacesDisplayName = {
  text?: string;
};

type PlaceDetailsResponse = {
  id?: string;
  formattedAddress?: string;
  location?: PlacesLocation;
  addressComponents?: PlacesAddressComponent[];
  displayName?: PlacesDisplayName;
};

let activeSessionToken: string | undefined;

const PLACES_AUTOCOMPLETE_URL = 'https://places.googleapis.com/v1/places:autocomplete';
const PLACES_BASE_URL = 'https://places.googleapis.com/v1';

const ensureBrowserEnvironment = () => {
  if (typeof window === 'undefined' || typeof document === 'undefined') {
    throw new Error('Google Maps Places API is only available in the browser.');
  }
};

const getApiKey = () => {
  const apiKey = process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    throw new Error('Missing Google Maps API key. Set NEXT_PUBLIC_GOOGLE_MAPS_API_KEY in your environment.');
  }

  return apiKey;
};

const createSessionToken = () => {
  const supportsCrypto = typeof crypto !== 'undefined' && 'randomUUID' in crypto;
  return supportsCrypto ? (crypto as Crypto & { randomUUID: () => string }).randomUUID() : Math.random().toString(36).slice(2, 12);
};

const ensureSessionToken = () => {
  if (!activeSessionToken) {
    activeSessionToken = createSessionToken();
  }

  return activeSessionToken;
};

const getLanguageCode = () => {
  if (typeof navigator !== 'undefined' && navigator.language) {
    return navigator.language;
  }

  return 'en';
};

const normalisePlaceResourceName = (placeId: string) => (placeId.startsWith('places/') ? placeId : `places/${placeId}`);

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

export const fetchPlacePredictions = async (input: string): Promise<PlacePrediction[]> => {
  ensureBrowserEnvironment();

  const apiKey = getApiKey();
  const sessionToken = ensureSessionToken();
  const languageCode = getLanguageCode();

  const response = await fetch(PLACES_AUTOCOMPLETE_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-Goog-Api-Key': apiKey,
      'X-Goog-FieldMask': 'suggestions.placePrediction.placeId,suggestions.placePrediction.text,suggestions.placePrediction.types',
    },
    body: JSON.stringify({
      input,
      sessionToken,
      languageCode,
      includedPrimaryTypes: ['street_address', 'premise', 'plus_code'],
    }),
  });

  if (!response.ok) {
    const errorPayload = await response.json().catch(() => ({}));
    const message = errorPayload?.error?.message ?? response.statusText;
    throw new Error(`Failed to fetch place predictions: ${message}`);
  }

  const payload = (await response.json()) as AutocompleteResponse;
  const suggestions = payload.suggestions ?? [];

  const predictions: PlacePrediction[] = [];

  for (const suggestion of suggestions) {
    const prediction = suggestion.placePrediction;
    if (!prediction?.placeId) {
      continue;
    }

    predictions.push({
      description: prediction.text?.text ?? '',
      place_id: prediction.placeId,
      types: prediction.types,
    });
  }

  return predictions;
};

export const fetchPlaceDetails = async (placeId: string): Promise<PlaceDetailsResult> => {
  ensureBrowserEnvironment();

  const apiKey = getApiKey();
  const languageCode = getLanguageCode();
  const resourceName = normalisePlaceResourceName(placeId);

  const url = new URL(`${PLACES_BASE_URL}/${resourceName}`);
  url.searchParams.set('languageCode', languageCode);
  if (activeSessionToken) {
    url.searchParams.set('sessionToken', activeSessionToken);
  }

  const response = await fetch(url.toString(), {
    headers: {
      'X-Goog-Api-Key': apiKey,
      'X-Goog-FieldMask': 'id,formattedAddress,location,addressComponents,displayName',
    },
  });

  if (!response.ok) {
    activeSessionToken = undefined;
    const errorPayload = await response.json().catch(() => ({}));
    const message = errorPayload?.error?.message ?? response.statusText;
    throw new Error(`Failed to fetch place details: ${message}`);
  }

  const details = (await response.json()) as PlaceDetailsResponse;
  activeSessionToken = undefined;

  const location = details.location;
  if (typeof location?.latitude !== 'number' || typeof location.longitude !== 'number') {
    throw new Error('Place details missing location coordinates.');
  }

  const addressComponents: AddressComponent[] = (details.addressComponents ?? []).map((component) => ({
    long_name: component.longText ?? component.shortText ?? '',
    short_name: component.shortText ?? component.longText ?? '',
    types: component.types ?? [],
  }));

  return {
    placeId: details.id ?? placeId,
    formattedAddress: details.formattedAddress ?? details.displayName?.text ?? '',
    latitude: location.latitude,
    longitude: location.longitude,
    addressComponents,
    name: details.displayName?.text ?? undefined,
  };
};
