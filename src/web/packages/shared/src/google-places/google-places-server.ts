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

type PlacesOperation = 'autocomplete' | 'details';

const getRuntimeContext = () => ({
  vercelEnvironment: process.env.VERCEL_ENV ?? 'unknown',
  nodeEnvironment: process.env.NODE_ENV ?? 'unknown',
  region: process.env.VERCEL_REGION ?? 'unknown',
});

const logPlacesEvent = (level: 'info' | 'warn' | 'error', event: string, properties: Record<string, unknown> = {}) => {
  // Keep request values, tokens, and the API key out of logs. Vercel already includes
  // the function/request context; these fields identify which stage failed.
  console[level]({ service: 'google-places', event, ...getRuntimeContext(), ...properties });
};

const getErrorMessage = (error: unknown) => (error instanceof Error ? error.message : String(error));

const readErrorPayload = async (response: Response) => {
  const body = await response.text().catch(() => '');
  if (!body) {
    return { message: response.statusText || 'No response body', hasBody: false, contentType: response.headers.get('content-type') };
  }

  try {
    const payload = JSON.parse(body) as { error?: { message?: string; status?: string; code?: number } };
    return {
      message: payload.error?.message ?? (response.statusText || 'Unknown Google API error'),
      googleStatus: payload.error?.status,
      googleCode: payload.error?.code,
      hasBody: true,
      contentType: response.headers.get('content-type'),
    };
  } catch {
    return {
      message: response.statusText || 'Non-JSON response from Google API',
      hasBody: true,
      contentType: response.headers.get('content-type'),
      bodyPreview: body.slice(0, 300),
    };
  }
};

const fetchPlaces = async (operation: PlacesOperation, url: string, init: RequestInit) => {
  const startedAt = Date.now();
  logPlacesEvent('info', 'request_started', {
    operation,
    method: init.method ?? 'GET',
    endpoint: new URL(url).pathname,
    apiKeyConfigured: Boolean(process.env.GOOGLE_MAPS_API_KEY),
  });

  try {
    const response = await fetch(url, init);
    const durationMs = Date.now() - startedAt;
    logPlacesEvent(response.ok ? 'info' : 'warn', response.ok ? 'request_succeeded' : 'request_failed', {
      operation,
      status: response.status,
      statusText: response.statusText,
      durationMs,
      contentType: response.headers.get('content-type'),
    });
    return response;
  } catch (error) {
    logPlacesEvent('error', 'request_threw', {
      operation,
      durationMs: Date.now() - startedAt,
      errorName: error instanceof Error ? error.name : 'UnknownError',
      errorMessage: getErrorMessage(error),
    });
    throw error;
  }
};

const getApiKey = () => {
  const apiKey = process.env.GOOGLE_MAPS_API_KEY;

  if (!apiKey) {
    logPlacesEvent('error', 'configuration_missing_api_key');
    throw new Error('Missing Google Maps API key. Set GOOGLE_MAPS_API_KEY in your environment.');
  }

  return apiKey;
};

const normalisePlaceResourceName = (placeId: string) => (placeId.startsWith('places/') ? placeId : `places/${placeId}`);

export const fetchPlacePredictionsFromGoogle = async (input: string, languageCode: string, sessionToken?: string): Promise<PlacePrediction[]> => {
  const apiKey = getApiKey();

  const response = await fetchPlaces('autocomplete', PLACES_AUTOCOMPLETE_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-Goog-Api-Key': apiKey,
      'X-Goog-FieldMask': 'suggestions.placePrediction.placeId,suggestions.placePrediction.text.text,suggestions.placePrediction.types',
    },
    body: JSON.stringify({
      input,
      sessionToken,
      languageCode,
    }),
  });

  if (!response.ok) {
    const errorPayload = await readErrorPayload(response);
    logPlacesEvent('error', 'google_error_response', { operation: 'autocomplete', ...errorPayload });
    const message = errorPayload.message;
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

  const response = await fetchPlaces('details', url.toString(), {
    headers: {
      'X-Goog-Api-Key': apiKey,
      'X-Goog-FieldMask': 'id,formattedAddress,location,addressComponents,displayName',
    },
  });

  if (!response.ok) {
    const errorPayload = await readErrorPayload(response);
    logPlacesEvent('error', 'google_error_response', { operation: 'details', ...errorPayload });
    const message = errorPayload.message;
    throw new Error(`Failed to fetch place details: ${message}`);
  }

  const details = (await response.json()) as PlaceDetailsResponse;

  const location = details.location;
  if (typeof location?.latitude !== 'number' || typeof location.longitude !== 'number') {
    logPlacesEvent('error', 'invalid_success_payload', { operation: 'details', missingLocation: true });
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
