import type { Dayjs } from 'dayjs';

const calendarDays = [
  { code: 'SUNDAY', name: 'Sunday', day: 0 },
  { code: 'MONDAY', name: 'Monday', day: 1 },
  { code: 'TUESDAY', name: 'Tuesday', day: 2 },
  { code: 'WEDNESDAY', name: 'Wednesday', day: 3 },
  { code: 'THURSDAY', name: 'Thursday', day: 4 },
  { code: 'FRIDAY', name: 'Friday', day: 5 },
  { code: 'SATURDAY', name: 'Saturday', day: 6 },
] as const;

export const getAvailableDaysLabel = (availableDays: readonly string[] | null | undefined) => {
  const selectedDays = new Set(availableDays ?? []);

  if (selectedDays.size === 0) {
    return 'Available every calendar day';
  }

  return `Available on ${calendarDays
    .filter((day) => selectedDays.has(day.code))
    .map((day) => day.name)
    .join(', ')}`;
};

export const isDateAvailableForPrice = (date: Dayjs, availableDays: readonly string[] | null | undefined) => {
  const selectedDays = availableDays ?? [];

  return selectedDays.length === 0 || selectedDays.includes(calendarDays.find((day) => day.day === date.day())?.code ?? '');
};

export const getAvailableDaysGuidance = (availableDays: readonly string[] | null | undefined) =>
  `${getAvailableDaysLabel(availableDays)}. Opening hours and resource availability still apply; the booking service confirms the selected date.`;
