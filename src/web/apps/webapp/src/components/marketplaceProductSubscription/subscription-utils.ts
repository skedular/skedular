const recurringCadences = new Set(['DAILY', 'WEEKLY', 'FORTNIGHTLY', 'MONTHLY', 'TWO_MONTHS', 'QUARTERLY', 'FOUR_MONTHS', 'FIVE_MONTHS', 'SIX_MONTHS', 'YEARLY']);

export const isSubscriptionCadence = (cadence?: string | null) => !!cadence && recurringCadences.has(cadence);
