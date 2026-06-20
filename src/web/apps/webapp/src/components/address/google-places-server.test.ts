import { fetchPlacePredictionsFromGoogle } from '../../../../../packages/shared/src/google-places/google-places-server';
import { afterEach, describe, expect, it, vi } from 'vitest';

describe('Google Places prediction requests', () => {
  afterEach(() => {
    vi.unstubAllEnvs();
    vi.unstubAllGlobals();
  });

  it('sends the Places Autocomplete request without a forced referrer', async () => {
    vi.stubEnv('GOOGLE_MAPS_API_KEY', 'test-api-key');
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(
        JSON.stringify({
          suggestions: [
            {
              placePrediction: {
                placeId: 'ChIJtest',
                text: { text: '123 Main Street' },
                types: ['street_address'],
              },
            },
          ],
        }),
        { status: 200, headers: { 'Content-Type': 'application/json' } },
      ),
    );
    vi.stubGlobal('fetch', fetchMock);

    const result = await fetchPlacePredictionsFromGoogle('123 Main', 'en', 'session-token');

    expect(result).toEqual([
      {
        description: '123 Main Street',
        place_id: 'ChIJtest',
        types: ['street_address'],
      },
    ]);
    const request = fetchMock.mock.calls[0]?.[1] as RequestInit;
    expect(new Headers(request.headers).get('Referer')).toBeNull();
    expect(new Headers(request.headers).get('X-Goog-FieldMask')).toBe(
      'suggestions.placePrediction.placeId,suggestions.placePrediction.text.text,suggestions.placePrediction.types',
    );
    expect(JSON.parse(String(request.body))).toMatchObject({
      input: '123 Main',
      sessionToken: 'session-token',
      languageCode: 'en',
    });
  });
});
