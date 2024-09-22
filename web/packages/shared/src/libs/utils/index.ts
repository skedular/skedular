import dayjs, { Dayjs } from 'dayjs';
import advancedFormat from 'dayjs/plugin/advancedFormat';
import isoWeek from 'dayjs/plugin/isoWeek';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import { PayloadError } from 'relay-runtime';

dayjs.extend(isoWeek);
dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(advancedFormat);

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

const now = () => {
  return dayjs().utc();
};

const startOfDay = (date: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : dayjs().utc();

  return finalDate.utc().startOf('day');
};

const endOfDay = (date: Dayjs | string | null | undefined) => {
  return startOfDay(date).add(1, 'day').add(-1, 'milliseconds');
};

const startOfWeek = (date: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : dayjs().utc();

  return finalDate.utc().startOf('isoWeek');
};

const endOfWeek = (date: Dayjs | string | null | undefined) => {
  return startOfWeek(date).add(1, 'week');
};

const startOfMonth = (date: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : dayjs().utc();

  return finalDate.utc().startOf('month');
};

const endOfMonth = (date: Dayjs | string | null | undefined) => {
  return startOfMonth(date).add(1, 'month');
};

const toShortDate = (date: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('dddd, Do MMM YYYY') : '';
};

const toShortDateTime = (date: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('dddd, Do MMM YYYY, HH:mm') : '';
};

const toShortWeekDay = (date: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('ddd') : '';
};

const toShortDateTimeInUtc = (date: Dayjs | string | null | undefined) => {
  return date ? toShortDateTime(dayjs(date).utc()) : '';
};

const toDayAndMonthDate = (date: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM') : '';
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
      avatarLetters = nameDetails.name[0];
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
  joinErrors,
  now,
  startOfDay,
  startOfMonth,
  startOfWeek,
  toDayAndMonthDate,
  toFixed,
  toShortDate,
  toShortDateTime,
  toShortDateTimeInUtc,
  toShortWeekDay,
};
