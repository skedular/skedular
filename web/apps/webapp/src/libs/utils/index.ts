import { aqua, flame, sunbeam, violet } from '@/libs/theme/theme-primitives';
import dayjs, { Dayjs } from 'dayjs';
import advancedFormat from 'dayjs/plugin/advancedFormat';
import isToday from 'dayjs/plugin/isToday';
import isTomorrow from 'dayjs/plugin/isTomorrow';
import isYesterday from 'dayjs/plugin/isYesterday';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import type { NextRequest } from 'next/server';
import type { PayloadError } from 'relay-runtime';

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(advancedFormat);
dayjs.extend(isToday);
dayjs.extend(isTomorrow);
dayjs.extend(isYesterday);

type ErrorWithGraphQlSource = {
  message?: string | null;
  source?: {
    errors?: PayloadError[] | null;
  } | null;
};

type RelayErrorLike = ErrorWithGraphQlSource | PayloadError[];

const secondaryColors = [violet, aqua, sunbeam, flame];

export type NameDetails = {
  name?: string | null;
  givenName?: string | null;
  middleName?: string | null;
  familyName?: string | null;
};

export const isServer = typeof window === 'undefined';

export const keyboardSearchDebounceTimeout = 500;
export const keyboardTextFieldDebounceTimeout = 10;

const convertCalendarDayToStartOfDay = (date: Dayjs) => {
  return startOfDay(dayjs().utc().set('year', date.year()).set('month', date.month()).set('date', date.date()));
};

const localNow = () => {
  return dayjs();
};

const now = () => {
  const date = new Date();
  return dayjs().utc().set('year', date.getUTCFullYear()).set('month', date.getMonth()).set('date', date.getDate());
};

const isTodayDate = (date: Dayjs) => {
  return date.isToday();
};

const isTomorrowDate = (date: Dayjs) => {
  return date.isTomorrow();
};

const isYesterdayDate = (date: Dayjs) => {
  return date.isYesterday();
};

const isInSameWeek = (dayA: Dayjs, dayB: Dayjs | null | undefined) => {
  if (dayB == null) {
    return false;
  }

  return dayA.isSame(dayB, 'week');
};

const isInSameMonth = (dayA: Dayjs, dayB: Dayjs | null | undefined) => {
  if (dayB == null) {
    return false;
  }

  return dayA.isSame(dayB, 'month');
};

const isInSameYear = (dayA: Dayjs, dayB: Dayjs | null | undefined) => {
  if (dayB == null) {
    return false;
  }

  return dayA.isSame(dayB, 'year');
};

const startOfDay = (date?: Dayjs | Date | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('day');
};

const endOfDay = (date: Dayjs | Date | string) => {
  return dayjs(date).add(1, 'day').add(-1, 'milliseconds');
};

const startOfWeek = (date?: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('week');
};

const endOfWeek = (date?: Dayjs | string | null | undefined) => {
  return startOfWeek(date).add(1, 'week');
};

const startOfMonth = (date?: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('month');
};

const endOfMonth = (date?: Dayjs | string | null | undefined) => {
  return startOfMonth(date).add(1, 'month');
};

const toShortDateWithDayAndMonthOnly = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM') : '';
};

const toShortDate = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('dddd, Do MMM YYYY') : '';
};

const toShortTime = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('hh:mm a') : '';
};

const toShortDateWithoutWeekDay = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM YYYY') : '';
};

const toShortDateTime = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMMM YYYY, HH:mm:ss') : '';
};

const toLongDateTime = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('dddd, Do MMMM YYYY, HH:mm') : '';
};

const toShortWeekDay = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('ddd') : '';
};

const toShortDateTimeInUtc = (date?: Dayjs | string | null | undefined) => {
  return date ? toLongDateTime(dayjs(date).utc()) : '';
};

const toDayAndMonthDate = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM') : '';
};

const toHourAndMinute = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).utc().format('h:mma') : '';
};

const isStoredFullDayRange = (from?: Dayjs | string | null, until?: Dayjs | string | null) => {
  if (!from || !until) {
    return false;
  }

  const utcFrom = dayjs.utc(from);
  const utcUntil = dayjs.utc(until);

  return utcFrom.isValid() && utcUntil.isValid() && utcFrom.hour() === 0 && utcFrom.minute() === 0 && utcUntil.hour() === 0 && utcUntil.minute() === 0;
};

const toStoredBookingTimeRange = (from?: Dayjs | string | null, until?: Dayjs | string | null) => {
  if (!from || !until || isStoredFullDayRange(from, until)) {
    return '';
  }

  return `${dayjs.utc(from).format('hh:mm a')} - ${dayjs.utc(until).format('hh:mm a')}`;
};

const encodeBase64 = (value: string) => {
  return isServer ? Buffer.from(value, 'utf-8').toString('base64') : btoa(value);
};

const decodeBase64 = (value: string) => {
  return isServer ? Buffer.from(value, 'base64').toString('utf-8') : atob(value);
};

const getCustomerShortName = (nameDetails?: NameDetails | null) => {
  if (!nameDetails) {
    return '';
  }

  if (nameDetails.givenName) {
    return nameDetails.givenName;
  }

  if (nameDetails.middleName) {
    return nameDetails.middleName;
  }

  if (nameDetails.familyName) {
    return nameDetails.middleName;
  }

  if (nameDetails.name) {
    return nameDetails.name;
  }

  return '';
};

const getCustomerFullName = (nameDetails?: NameDetails | null) => {
  if (!nameDetails) {
    return '';
  }

  if (nameDetails.name) {
    return nameDetails.name;
  }

  if (nameDetails.givenName && nameDetails.familyName) {
    return `${nameDetails.givenName} ${nameDetails.familyName}`;
  }

  if (nameDetails.givenName && !nameDetails.familyName) {
    return nameDetails.givenName;
  }

  if (!nameDetails.givenName && nameDetails.familyName) {
    return nameDetails.familyName;
  }

  if (nameDetails.middleName) {
    return nameDetails.middleName;
  }

  return '';
};

const getCustomerAvatarLetters = (nameDetails?: NameDetails | null) => {
  if (!nameDetails) {
    return '';
  }

  let avatarLetters = '';

  if (nameDetails) {
    if (nameDetails.givenName && nameDetails.familyName) {
      avatarLetters = `${nameDetails.givenName[0]}${nameDetails.familyName[0]}`;
    } else if (nameDetails.name && typeof nameDetails.name[0] !== 'undefined') {
      avatarLetters = nameDetails.name.split(' ').reduce((acc, val) => acc + val[0], '');
    } else {
      avatarLetters = '';
    }
  } else {
    avatarLetters = '';
  }

  return avatarLetters;
};

const convertStringToLowercaseExceptFirstLetter = (str: string | null | undefined) => {
  return str ? `${str.charAt(0).toUpperCase()}${str.slice(1).toLowerCase()}` : '';
};

const toFixed = (value: number, fractionDigits?: number): number => {
  return Number(value.toFixed(fractionDigits));
};

const normalizeRelayErrorMessage = (message: string | null | undefined) => {
  const trimmedMessage = message?.trim();
  if (!trimmedMessage) {
    return null;
  }

  if (trimmedMessage === 'See the error `source` property for more information..') {
    return null;
  }

  return trimmedMessage;
};

const getRelayMessagesFromSingleError = (error: ErrorWithGraphQlSource) => {
  const graphQlMessages = (error.source?.errors ?? []).map((item) => normalizeRelayErrorMessage(item.message)).filter((item): item is string => !!item);

  if (graphQlMessages.length > 0) {
    return graphQlMessages;
  }

  const fallbackMessage = normalizeRelayErrorMessage(error.message);
  return fallbackMessage ? [fallbackMessage] : [];
};

const getRelayErrorMessage = (error: RelayErrorLike) => {
  if (Array.isArray(error)) {
    const messages = error.flatMap((item) => getRelayMessagesFromSingleError(item));
    return Array.from(new Set(messages)).join('\n');
  }

  const messages = getRelayMessagesFromSingleError(error);
  if (messages.length > 0) {
    return Array.from(new Set(messages)).join('\n');
  }

  return 'Unknown error';
};

const stringToColor = (string: string) => {
  let hash = 0;

  for (let i = 0; i < string.length; i++) {
    hash = string.charCodeAt(i) + ((hash << 5) - hash);
  }

  const index = Math.abs(hash) % secondaryColors.length;
  return secondaryColors[index];
};

const toShortDateWithAdditionalDayInfo = (date: Dayjs): string => {
  let dateValue = '';

  if (isTodayDate(date)) {
    dateValue = `Today, ${toShortDateWithoutWeekDay(date)}`;
  } else if (isTomorrowDate(date)) {
    dateValue = `Tomorrow, ${toShortDateWithoutWeekDay(date)}`;
  } else {
    dateValue = toShortDate(date);
  }

  return dateValue;
};

const dateRangeToShortDateWithAdditionalDayInfo = (from: Dayjs, until: Dayjs): string => {
  const utcFrom = from.utc();
  const utcUntil = until.utc();

  if (isMidnight(utcFrom) && isMidnight(utcUntil)) {
    if (utcFrom.add(1, 'day').isSame(utcUntil)) {
      return `${toShortDateWithAdditionalDayInfo(utcFrom)}`;
    } else {
      return `${toShortDateWithAdditionalDayInfo(utcFrom)} - ${toShortDateWithAdditionalDayInfo(utcUntil)}`;
    }
  } else {
    if (utcFrom.isSame(utcUntil, 'day')) {
      return `${toShortDateWithAdditionalDayInfo(utcFrom)} ${utcFrom.format('hh:mm a')} - ${utcUntil.format('hh:mm a')}`;
    } else {
      return `${toShortDateWithAdditionalDayInfo(utcFrom)} ${utcFrom.format('hh:mm a')} - ${toShortDateWithAdditionalDayInfo(utcUntil)} ${utcUntil.format('hh:mm a')}`;
    }
  }
};

const toOpeningHoursFromTime = (time?: string | null | undefined) => {
  if (!time) {
    return null;
  }

  const splittedTime = time.split(':');
  if (splittedTime.length < 2) {
    return null;
  }

  return dayjs().utc().startOf('day').set('hour', parseInt(splittedTime[0])).set('minute', parseInt(splittedTime[1]));
};

const getOpeningHoursFromDateTime = (datetime: Dayjs | string | null) => {
  if (!datetime) {
    return '00:00';
  }

  const date = typeof datetime === 'string' ? dayjs(datetime).utc() : datetime;

  return `${date.format('HH')}:${date.format('mm')}`;
};

const isMidnight = (datetime: Dayjs | null) => getOpeningHoursFromDateTime(datetime) === '00:00';

const stringToMultiLines = (str?: string | null) => (str ? str.split('\n').map((item) => item.trim()) : []);

const stringCollectionToString = (str?: readonly string[] | null) => (str ? str.join('\n') : '');

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

const getPublicOrigin = (request: NextRequest) => {
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

const getMonthlyPricingCadenceMonthCount = (cadence?: string | null) => {
  if (!cadence) {
    return null;
  }

  return monthlyPricingCadenceMonthCount[cadence] ?? null;
};

const formatCompactNumber = (value: number) => {
  return new Intl.NumberFormat('en-NZ', { maximumFractionDigits: 2, minimumFractionDigits: 0 }).format(value);
};

const formatPriceForDisplay = (currencyLabel: string | null | undefined, amount: number | string, cadence?: string | null) => {
  const numericAmount = Number(amount);
  const months = getMonthlyPricingCadenceMonthCount(cadence);
  const displayAmount = months ? numericAmount / months : numericAmount;
  const formattedAmount = Number.isFinite(displayAmount) ? formatCompactNumber(displayAmount) : `${amount}`;
  const prefix = currencyLabel ? `${currencyLabel} ` : '';

  return months ? `${prefix}${formattedAmount}/month` : `${prefix}${formattedAmount}`;
};

export {
  convertCalendarDayToStartOfDay,
  convertStringToLowercaseExceptFirstLetter,
  dateRangeToShortDateWithAdditionalDayInfo,
  decodeBase64,
  encodeBase64,
  endOfDay,
  endOfMonth,
  endOfWeek,
  formatPriceForDisplay,
  getCustomerAvatarLetters,
  getCustomerFullName,
  getCustomerShortName,
  getOpeningHoursFromDateTime,
  getPublicOrigin,
  getRelayErrorMessage,
  isInSameMonth,
  isInSameWeek,
  isInSameYear,
  isMidnight,
  isStoredFullDayRange,
  isTodayDate,
  isTomorrowDate,
  isYesterdayDate,
  localNow,
  now,
  startOfDay,
  startOfMonth,
  startOfWeek,
  stringCollectionToString,
  stringToColor,
  stringToMultiLines,
  toDayAndMonthDate,
  toFixed,
  toHourAndMinute,
  toLongDateTime,
  toOpeningHoursFromTime,
  toShortDate,
  toShortDateTime,
  toShortDateTimeInUtc,
  toShortDateWithAdditionalDayInfo,
  toShortDateWithDayAndMonthOnly,
  toShortDateWithoutWeekDay,
  toShortTime,
  toShortWeekDay,
  toStoredBookingTimeRange,
};
