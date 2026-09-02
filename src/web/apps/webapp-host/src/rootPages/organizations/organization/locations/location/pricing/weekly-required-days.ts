export const hasValidWeeklyRequiredDays = (cadence: string, requiredDaysPerWeek: string, availableDays: readonly string[]) => {
  if (cadence === 'DAILY' || !requiredDaysPerWeek.trim()) {
    return true;
  }

  const requiredDays = Number(requiredDaysPerWeek);
  const availableDayCount = availableDays.length || 7;
  return Number.isInteger(requiredDays) && requiredDays > 0 && requiredDays <= availableDayCount;
};

export const getWeeklyRequiredDaysError = (cadence: string, requiredDaysPerWeek: string, availableDays: readonly string[]) => {
  if (hasValidWeeklyRequiredDays(cadence, requiredDaysPerWeek, availableDays)) {
    return null;
  }

  return `Choose a whole number from 1 to ${availableDays.length || 7}.`;
};

export const sanitizeWeeklyRequiredDays = (value: string) => value.replace(/[^0-9]/g, '').slice(0, 1);
