import { describe, expect, it } from 'vitest';
import { getHostAppLink } from './index';

describe('getHostAppLink', () => {
  it('uses the configured host application URL', () => {
    process.env.NEXT_PUBLIC_SKEDULAR_HOST_APP_URL = 'https://hoststaging.skedular.app/';

    expect(getHostAppLink()).toBe('https://hoststaging.skedular.app/');
  });

  it('supports the production host application URL', () => {
    process.env.NEXT_PUBLIC_SKEDULAR_HOST_APP_URL = 'https://host.skedular.app';

    expect(getHostAppLink()).toBe('https://host.skedular.app/');
  });
});
