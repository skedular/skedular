import { renderHook } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import useKnownParams from '../use-known-params';

vi.mock('next/navigation', () => ({
  useParams: vi.fn(),
}));

import { useParams } from 'next/navigation';

describe('useKnownParams', () => {
  beforeEach(() => {
    vi.mocked(useParams).mockReturnValue({});
  });

  it('returns empty strings for all params when useParams returns empty', () => {
    const { result } = renderHook(() => useKnownParams());
    expect(result.current.locationId).toBe('');
    expect(result.current.bookingId).toBe('');
    expect(result.current.productId).toBe('');
    expect(result.current.teamId).toBe('');
  });

  it('returns string param value directly', () => {
    vi.mocked(useParams).mockReturnValue({ locationId: 'loc-123' });
    const { result } = renderHook(() => useKnownParams());
    expect(result.current.locationId).toBe('loc-123');
  });

  it('returns first element when param is an array', () => {
    vi.mocked(useParams).mockReturnValue({ productId: ['prod-a', 'prod-b'] });
    const { result } = renderHook(() => useKnownParams());
    expect(result.current.productId).toBe('prod-a');
  });
});
