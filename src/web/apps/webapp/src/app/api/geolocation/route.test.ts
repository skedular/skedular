import { NextRequest } from 'next/server';
import { beforeEach, describe, expect, it, vi } from 'vitest';

const mockLookupIp = vi.fn();
vi.mock('node-ipinfo', () => ({
  IPinfoWrapper: vi.fn().mockImplementation(function () {
    return { lookupIp: mockLookupIp };
  }),
}));

describe('GET /api/geolocation', () => {
  beforeEach(() => {
    mockLookupIp.mockReset();
  });

  it('returns city, region, country, lat, lng from node-ipinfo', async () => {
    mockLookupIp.mockResolvedValueOnce({
      city: 'Auckland',
      region: 'Auckland',
      country: 'NZ',
      loc: '-36.8485,174.7633',
    });

    const { GET } = await import('./route');
    const req = new NextRequest('http://localhost/api/geolocation', {
      headers: { 'x-forwarded-for': '203.0.113.1' },
    });
    const response = await GET(req);
    const data = await response.json();

    expect(response.status).toBe(200);
    expect(data).toEqual({
      city: 'Auckland',
      region: 'Auckland',
      country: 'NZ',
      lat: '-36.8485',
      lng: '174.7633',
    });
  });

  it('returns null lat/lng when loc is missing', async () => {
    mockLookupIp.mockResolvedValueOnce({ city: 'Unknown', region: '', country: '', loc: null });

    const { GET } = await import('./route');
    const req = new NextRequest('http://localhost/api/geolocation');
    const response = await GET(req);
    const data = await response.json();

    expect(response.status).toBe(200);
    expect(data.lat).toBeNull();
    expect(data.lng).toBeNull();
  });
});
