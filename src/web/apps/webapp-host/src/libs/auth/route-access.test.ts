import { describe, expect, it } from 'vitest';
import { isUnauthenticatedPath } from './route-access';

describe('isUnauthenticatedPath', () => {
  it.each(['/', '/callback', '/signin', '/signup', '/auth/signin'])('allows signed-out access to %s', (pathname) => {
    expect(isUnauthenticatedPath(pathname)).toBe(true);
  });

  it.each(['/welcome', '/organizations', '/organizations/acme'])('requires authentication for %s', (pathname) => {
    expect(isUnauthenticatedPath(pathname)).toBe(false);
  });
});
