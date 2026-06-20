import { beforeEach, describe, expect, it, vi } from 'vitest';

const { redirectMock } = vi.hoisted(() => ({ redirectMock: vi.fn() }));

vi.mock('next/navigation', () => ({
  redirect: redirectMock,
}));

import EditLocationPage from './page';

describe('EditLocationPage', () => {
  beforeEach(() => {
    redirectMock.mockReset();
  });

  it('redirects to the location overview page', () => {
    EditLocationPage({ params: { id: 'location-123' } });

    expect(redirectMock).toHaveBeenCalledTimes(1);
    expect(redirectMock).toHaveBeenCalledWith('/locations/location-123');
  });
});
