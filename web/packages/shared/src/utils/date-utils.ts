import dayjs, { Dayjs } from 'dayjs';
import advancedFormat from 'dayjs/plugin/advancedFormat';
import isToday from 'dayjs/plugin/isToday';
import isTomorrow from 'dayjs/plugin/isTomorrow';
import isYesterday from 'dayjs/plugin/isYesterday';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';

dayjs.extend(utc);
dayjs.extend(timezone);
dayjs.extend(advancedFormat);
dayjs.extend(isToday);
dayjs.extend(isTomorrow);
dayjs.extend(isYesterday);

export const convertCalendarDayToStartOfDay = (date: Dayjs) => {
  return startOfDay(dayjs().utc().set('year', date.year()).set('month', date.month()).set('date', date.date()));
};

export const localNow = () => {
  return dayjs();
};

export const now = () => {
  const date = new Date();
  return dayjs().utc().set('year', date.getUTCFullYear()).set('month', date.getMonth()).set('date', date.getDate());
};

export const isTodayDate = (date: Dayjs) => {
  return date.isToday();
};

export const isTomorrowDate = (date: Dayjs) => {
  return date.isTomorrow();
};

export const isYesterdayDate = (date: Dayjs) => {
  return date.isYesterday();
};

export const isInSameWeek = (dayA: Dayjs, dayB: Dayjs | null | undefined) => {
  if (dayB == null) {
    return false;
  }

  return dayA.isSame(dayB, 'week');
};

export const isInSameMonth = (dayA: Dayjs, dayB: Dayjs | null | undefined) => {
  if (dayB == null) {
    return false;
  }

  return dayA.isSame(dayB, 'month');
};

export const isInSameYear = (dayA: Dayjs, dayB: Dayjs | null | undefined) => {
  if (dayB == null) {
    return false;
  }

  return dayA.isSame(dayB, 'year');
};

export const startOfDay = (date?: Dayjs | Date | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('day');
};

export const endOfDay = (date: Dayjs | Date | string) => {
  return dayjs(date).add(1, 'day').add(-1, 'milliseconds');
};

export const startOfWeek = (date?: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('week');
};

export const endOfWeek = (date?: Dayjs | string | null | undefined) => {
  return startOfWeek(date).add(1, 'week');
};

export const startOfMonth = (date?: Dayjs | string | null | undefined) => {
  const finalDate = date ? dayjs(date) : now();

  return finalDate.startOf('month');
};

export const endOfMonth = (date?: Dayjs | string | null | undefined) => {
  return startOfMonth(date).add(1, 'month');
};

export const toShortDateWithDayAndMonthOnly = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM') : '';
};

export const toShortDate = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('dddd, Do MMM YYYY') : '';
};

export const toShortTime = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('hh:mm a') : '';
};

export const toShortDateWithoutWeekDay = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM YYYY') : '';
};

export const toShortDateTime = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMMM YYYY, HH:mm:ss') : '';
};

export const toLongDateTime = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('dddd, Do MMMM YYYY, HH:mm') : '';
};

export const toShortWeekDay = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('ddd') : '';
};

export const toShortDateTimeInUtc = (date?: Dayjs | string | null | undefined) => {
  return date ? toLongDateTime(dayjs(date).utc()) : '';
};

export const toDayAndMonthDate = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).format('Do MMM') : '';
};

export const toHourAndMinute = (date?: Dayjs | string | null | undefined) => {
  return date ? dayjs(date).utc().format('h:mma') : '';
};

export const isStoredFullDayRange = (from?: Dayjs | string | null, until?: Dayjs | string | null) => {
  if (!from || !until) {
    return false;
  }

  const utcFrom = dayjs.utc(from);
  const utcUntil = dayjs.utc(until);

  return utcFrom.isValid() && utcUntil.isValid() && utcFrom.hour() === 0 && utcFrom.minute() === 0 && utcUntil.hour() === 0 && utcUntil.minute() === 0;
};

export const toStoredBookingTimeRange = (from?: Dayjs | string | null, until?: Dayjs | string | null) => {
  if (!from || !until || isStoredFullDayRange(from, until)) {
    return '';
  }

  return `${dayjs.utc(from).format('hh:mm a')} - ${dayjs.utc(until).format('hh:mm a')}`;
};

export const getOpeningHoursFromDateTime = (datetime: Dayjs | string | null) => {
  if (!datetime) {
    return '00:00';
  }

  const date = typeof datetime === 'string' ? dayjs(datetime).utc() : datetime;

  return `${date.format('HH')}:${date.format('mm')}`;
};

export const isMidnight = (datetime: Dayjs | null) => getOpeningHoursFromDateTime(datetime) === '00:00';

export const toOpeningHoursFromTime = (time?: string | null | undefined) => {
  if (!time) {
    return null;
  }

  const splittedTime = time.split(':');
  if (splittedTime.length < 2) {
    return null;
  }

  return dayjs().utc().startOf('day').set('hour', parseInt(splittedTime[0]!)).set('minute', parseInt(splittedTime[1]!));
};

export const toShortDateWithAdditionalDayInfo = (date: Dayjs): string => {
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

type DateRangeWithAdditionalDayInfo = {
  primaryLine: string;
  secondaryLine: string;
};

export const dateRangeToShortDateWithAdditionalDayInfo = (from: Dayjs, until: Dayjs): DateRangeWithAdditionalDayInfo => {
  const utcFrom = from.utc();
  const utcUntil = until.utc();

  if (isMidnight(utcFrom) && isMidnight(utcUntil)) {
    if (utcFrom.add(1, 'day').isSame(utcUntil)) {
      return {
        primaryLine: toShortDateWithAdditionalDayInfo(utcFrom),
        secondaryLine: '',
      };
    } else {
      return {
        primaryLine: toShortDateWithAdditionalDayInfo(utcFrom),
        secondaryLine: toShortDateWithAdditionalDayInfo(utcUntil),
      };
    }
  } else {
    if (utcFrom.isSame(utcUntil, 'day')) {
      return {
        primaryLine: `${toShortDateWithAdditionalDayInfo(utcFrom)}`,
        secondaryLine: `${utcFrom.format('hh:mm a')} - ${utcUntil.format('hh:mm a')}`,
      };
    } else {
      return {
        primaryLine: `${toShortDateWithAdditionalDayInfo(utcFrom)} ${utcFrom.format('hh:mm a')}`,
        secondaryLine: `${toShortDateWithAdditionalDayInfo(utcUntil)} ${utcUntil.format('hh:mm a')}`,
      };
    }
  }
};
