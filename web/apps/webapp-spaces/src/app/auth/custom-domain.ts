import type { NextRequest } from 'next/server';

const firstHeaderValue = (value: string | null) => value?.split(',')[0]?.trim();

const normalizeHost = (value: string | null | undefined) => (value ?? '').split(':')[0]?.toLowerCase() ?? '';

const skedularHosts = new Set(['', 'localhost', '127.0.0.1', 'skedular.app', 'staging.skedular.app', 'www.skedular.app']);

export const getCustomDomainOrganizationName = (request: NextRequest) => {
  const host = normalizeHost(firstHeaderValue(request.headers.get('x-forwarded-host')) ?? firstHeaderValue(request.headers.get('host')) ?? request.nextUrl.host);

  if (skedularHosts.has(host)) {
    return null;
  }

  return host.split('.')[0] || null;
};
