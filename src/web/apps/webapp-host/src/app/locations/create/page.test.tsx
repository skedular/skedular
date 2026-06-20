import { describe, expect, it } from 'vitest';
import { validateCreateListing } from './validation';

describe('create listing validation', () => {
  it('returns required errors when fields are missing', () => {
    const errors = validateCreateListing({
      locationName: '',
      addressLine1: '',
      city: '',
      country: '',
      timezone: 'UTC',
    });

    expect(errors.length).toBeGreaterThan(0);
    expect(errors).toContain('Location name is required.');
  });

  it('passes when minimal required fields are present', () => {
    const errors = validateCreateListing({
      locationName: 'HQ',
      addressLine1: '1 Test Street',
      city: 'Auckland',
      country: 'New Zealand',
      timezone: 'Pacific/Auckland',
    });

    expect(errors).toEqual([]);
  });
});
