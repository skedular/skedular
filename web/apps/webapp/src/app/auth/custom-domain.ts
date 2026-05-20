import type { NextRequest } from 'next/server';
import { getOrganizationCustomDomainFromHost, normalizeHost } from './host-utils';

const firstHeaderValue = (value: string | null) => value?.split(',')[0]?.trim();

export const getCustomDomainOrganizationName = (request: NextRequest) => {
  const host = normalizeHost(firstHeaderValue(request.headers.get('x-forwarded-host')) ?? firstHeaderValue(request.headers.get('host')) ?? request.nextUrl.host);

  return getOrganizationCustomDomainFromHost(host);
};
