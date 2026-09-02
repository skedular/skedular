export const hasValidWeeklyRequiredDays = (cadence: string, requiredDaysPerWeek: string, availableDays: readonly string[], fulfillmentType = 'RESERVATION') => {
  if (cadence === 'DAILY' || !requiredDaysPerWeek.trim()) {
    return true;
  }

  const requiredDays = Number(requiredDaysPerWeek);
  const availableDayCount = fulfillmentType === 'ENTITLEMENT' ? 7 : availableDays.length || 7;
  return Number.isInteger(requiredDays) && requiredDays > 0 && requiredDays <= availableDayCount;
};

export const getWeeklyRequiredDaysError = (cadence: string, requiredDaysPerWeek: string, availableDays: readonly string[], fulfillmentType = 'RESERVATION') => {
  if (hasValidWeeklyRequiredDays(cadence, requiredDaysPerWeek, availableDays, fulfillmentType)) {
    return null;
  }

  return `Choose a whole number from 1 to ${fulfillmentType === 'ENTITLEMENT' ? 7 : availableDays.length || 7}.`;
};

export const sanitizeWeeklyRequiredDays = (value: string) => value.replace(/[^0-9]/g, '').slice(0, 1);
