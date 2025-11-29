import { AddressComponent, PlaceDetailsResult, PlacePrediction } from './google-places-types';

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

const PLACES_AUTOCOMPLETE_URL = 'https://places.googleapis.com/v1/places:autocomplete';
const PLACES_BASE_URL = 'https://places.googleapis.com/v1';

const getApiKey = () => {
  const apiKey = process.env.GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    throw new Error('Missing Google Maps API key. Set GOOGLE_MAPS_API_KEY in your environment.');
  }

  return apiKey;
};

const normalisePlaceResourceName = (placeId: string) => (placeId.startsWith('places/') ? placeId : `places/${placeId}`);

export const fetchPlacePredictionsFromGoogle = async (input: string, languageCode: string, sessionToken?: string): Promise<PlacePrediction[]> => {
  const apiKey = getApiKey();

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

export const fetchPlaceDetailsFromGoogle = async (placeId: string, languageCode: string, sessionToken?: string): Promise<PlaceDetailsResult> => {
  const apiKey = getApiKey();
  const resourceName = normalisePlaceResourceName(placeId);

  const url = new URL(`${PLACES_BASE_URL}/${resourceName}`);
  url.searchParams.set('languageCode', languageCode);
  if (sessionToken) {
    url.searchParams.set('sessionToken', sessionToken);
  }

  const response = await fetch(url.toString(), {
    headers: {
      'X-Goog-Api-Key': apiKey,
      'X-Goog-FieldMask': 'id,formattedAddress,location,addressComponents,displayName',
    },
  });

  if (!response.ok) {
    const errorPayload = await response.json().catch(() => ({}));
    const message = errorPayload?.error?.message ?? response.statusText;
    throw new Error(`Failed to fetch place details: ${message}`);
  }

  const details = (await response.json()) as PlaceDetailsResponse;

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
