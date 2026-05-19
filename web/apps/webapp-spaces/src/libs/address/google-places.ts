import { PlaceDetailsResult, PlacePrediction } from './google-places-types';

const createSessionToken = () => {
  const supportsCrypto = typeof crypto !== 'undefined' && 'randomUUID' in crypto;
  return supportsCrypto ? (crypto as Crypto & { randomUUID: () => string }).randomUUID() : Math.random().toString(36).slice(2, 12);
};

const getLanguageCode = () => {
  if (typeof navigator !== 'undefined' && navigator.language) {
    return navigator.language;
  }

  return 'en';
};

export const fetchPlacePredictions = async (input: string, sessionToken?: string, languageCode = getLanguageCode()): Promise<PlacePrediction[]> => {
  const response = await fetch('/api/places/predictions', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ input, sessionToken, languageCode }),
  });

  if (!response.ok) {
    const errorPayload = await response.json().catch(() => ({}));
    const message = errorPayload?.error ?? response.statusText;
    throw new Error(`Failed to fetch place predictions: ${message}`);
  }

  const payload = (await response.json()) as { predictions: PlacePrediction[] };
  return payload.predictions;
};

export const fetchPlaceDetails = async (placeId: string, sessionToken?: string, languageCode = getLanguageCode()): Promise<PlaceDetailsResult> => {
  const response = await fetch('/api/places/details', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ placeId, sessionToken, languageCode }),
  });

  if (!response.ok) {
    const errorPayload = await response.json().catch(() => ({}));
    const message = errorPayload?.error ?? response.statusText;
    throw new Error(`Failed to fetch place details: ${message}`);
  }

  const payload = (await response.json()) as { details: PlaceDetailsResult };
  return payload.details;
};

export const createPlacesSessionToken = createSessionToken;
