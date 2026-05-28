import type { NextRequest } from 'next/server';

export const isServer = typeof window === 'undefined';

export const keyboardSearchDebounceTimeout = 500;
export const keyboardTextFieldDebounceTimeout = 10;

export const encodeBase64 = (value: string) => {
  return isServer ? Buffer.from(value, 'utf-8').toString('base64') : btoa(value);
};

export const decodeBase64 = (value: string) => {
  return isServer ? Buffer.from(value, 'base64').toString('utf-8') : atob(value);
};

export const convertStringToLowercaseExceptFirstLetter = (str: string | null | undefined) => {
  return str ? `${str.charAt(0).toUpperCase()}${str.slice(1).toLowerCase()}` : '';
};

export const toFixed = (value: number, fractionDigits?: number): number => {
  return Number(value.toFixed(fractionDigits));
};

export const stringToMultiLines = (str?: string | null) => (str ? str.split('\n').map((item) => item.trim()) : []);

export const stringCollectionToString = (str?: readonly string[] | null) => (str ? str.join('\n') : '');

const splitHeaderValues = (value: string | null) =>
  value
    ?.split(',')
    .map((item) => item.trim())
    .filter(Boolean) ?? [];

const firstHeaderValue = (value: string | null) => splitHeaderValues(value)[0];

const headerValues = (value: string | null) =>
  splitHeaderValues(value)
    .map((item) => item.trim().toLowerCase())
    .filter(Boolean);

const hostFromAbsoluteUrl = (value: string | null) => {
  if (!value) {
    return undefined;
  }

  try {
    return new URL(value).host;
  } catch {
    return undefined;
  }
};

const isLocalHost = (host: string) => {
  const normalizedHost = host.toLowerCase();
  return normalizedHost === 'localhost' || normalizedHost.startsWith('localhost:') || normalizedHost.startsWith('127.0.0.1');
};

export const getPublicOrigin = (request: NextRequest) => {
  const forwardedHostCandidates = [
    ...splitHeaderValues(request.headers.get('x-forwarded-host')),
    ...splitHeaderValues(request.headers.get('x-original-host')),
    ...splitHeaderValues(request.headers.get('x-host')),
    ...splitHeaderValues(request.headers.get('host')),
  ];
  const browserOriginHost = hostFromAbsoluteUrl(request.headers.get('origin'));
  const browserRefererHost = hostFromAbsoluteUrl(request.headers.get('referer'));
  const nonLocalBrowserHost = [browserOriginHost, browserRefererHost].find((item) => !!item && !isLocalHost(item));
  const nonLocalForwardedHost = forwardedHostCandidates.find((item) => !isLocalHost(item));
  const nextUrlHost = request.nextUrl.host;
  const host = nonLocalForwardedHost ?? nonLocalBrowserHost ?? (!isLocalHost(nextUrlHost) ? nextUrlHost : undefined) ?? forwardedHostCandidates[0] ?? nextUrlHost;

  const forwardedProtocols = headerValues(
    firstHeaderValue(request.headers.get('x-forwarded-proto')) ??
      firstHeaderValue(request.headers.get('x-forwarded-protocol')) ??
      firstHeaderValue(request.headers.get('x-forwarded-scheme')),
  );
  const forwardedPort = firstHeaderValue(request.headers.get('x-forwarded-port'));
  const forwardedSsl = firstHeaderValue(request.headers.get('x-forwarded-ssl'))?.toLowerCase();
  const frontEndHttps = firstHeaderValue(request.headers.get('front-end-https'))?.toLowerCase();
  const requestProtocol = request.nextUrl.protocol.replace(':', '');

  if (host) {
    const isHttps = forwardedProtocols.includes('https') || forwardedPort === '443' || forwardedSsl === 'on' || frontEndHttps === 'on' || requestProtocol === 'https';
    const protocol = isHttps ? 'https' : isLocalHost(host) ? 'http' : 'https';
    return `${protocol}://${host}`;
  }

  return request.nextUrl.origin;
};

const monthlyPricingCadenceMonthCount: Record<string, number> = {
  TWO_MONTHS: 2,
  QUARTERLY: 3,
  FOUR_MONTHS: 4,
  FIVE_MONTHS: 5,
  SIX_MONTHS: 6,
  YEARLY: 12,
};

export const getMonthlyPricingCadenceMonthCount = (cadence?: string | null) => {
  if (!cadence) {
    return null;
  }

  return monthlyPricingCadenceMonthCount[cadence] ?? null;
};

const formatCompactNumber = (value: number) => {
  return new Intl.NumberFormat('en-NZ', { maximumFractionDigits: 2, minimumFractionDigits: 0 }).format(value);
};

export const formatPriceForDisplay = (currencyLabel: string | null | undefined, amount: number | string, cadence?: string | null) => {
  const numericAmount = Number(amount);
  const months = getMonthlyPricingCadenceMonthCount(cadence);
  const displayAmount = months ? numericAmount / months : numericAmount;
  const formattedAmount = Number.isFinite(displayAmount) ? formatCompactNumber(displayAmount) : `${amount}`;
  const prefix = currencyLabel ? `${currencyLabel} ` : '';

  return months ? `${prefix}${formattedAmount}/month` : `${prefix}${formattedAmount}`;
};
