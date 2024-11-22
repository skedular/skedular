import dayjs, { Dayjs } from 'dayjs';
import advancedFormat from 'dayjs/plugin/advancedFormat';
import isToday from 'dayjs/plugin/isToday';
import isTomorrow from 'dayjs/plugin/isTomorrow';
import isYesterday from 'dayjs/plugin/isYesterday';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { PayloadError } from 'relay-runtime';

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(advancedFormat);
dayjs.extend(isToday);
dayjs.extend(isTomorrow);
dayjs.extend(isYesterday);

export type NameDetails = {
  name?: string | null;
  givenName?: string | null;
  middleName?: string | null;
  familyName?: string | null;
};

export const isServer = typeof window === 'undefined';

export const keyboardDebounceTimeout = 500;

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

const startOfDay = (date?: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('day');
};

const endOfDay = (date: Dayjs | string) => {
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

const getPublicSiteUrl = () => {
  return isServer ? process.env.NEXT_PUBLIC_SITE_URL : window.location.origin;
};

const getCurrentCompleteUrl = () => {
  return new URL(window.location.pathname, getPublicSiteUrl()).href;
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

const joinErrors = (errors: PayloadError[]) => errors.map((error) => error.message).join('\n');

const stringToColor = (string: string) => {
  let hash = 0;
  let i;

  for (i = 0; i < string.length; i += 1) {
    hash = string.charCodeAt(i) + ((hash << 5) - hash);
  }

  let color = '#';

  for (i = 0; i < 3; i += 1) {
    const value = (hash >> (i * 8)) & 0xff;
    color += `00${value.toString(16)}`.slice(-2);
  }

  return color;
};

const toShortDateWithAdditionalDayInfo = (date: Dayjs): string => {
  let dateValue = '';

  if (isTodayDate(date)) {
    dateValue = `Today, ${toShortDate(date)}`;
  } else if (isTomorrowDate(date)) {
    dateValue = `Tomorrow, ${toShortDate(date)}`;
  } else {
    dateValue = toShortDate(date);
  }

  return dateValue;
};

export {
  convertCalendarDayToStartOfDay,
  convertStringToLowercaseExceptFirstLetter,
  decodeBase64,
  encodeBase64,
  endOfDay,
  endOfMonth,
  endOfWeek,
  getCurrentCompleteUrl,
  getCustomerAvatarLetters,
  getCustomerFullName,
  getCustomerShortName,
  getPublicSiteUrl,
  isInSameMonth,
  isInSameWeek,
  isInSameYear,
  isTodayDate,
  isTomorrowDate,
  isYesterdayDate,
  joinErrors,
  localNow,
  now,
  startOfDay,
  startOfMonth,
  startOfWeek,
  stringToColor,
  toDayAndMonthDate,
  toFixed,
  toHourAndMinute,
  toLongDateTime,
  toShortDate,
  toShortDateTimeInUtc,
  toShortDateWithAdditionalDayInfo,
  toShortDateWithDayAndMonthOnly,
  toShortWeekDay,
};
