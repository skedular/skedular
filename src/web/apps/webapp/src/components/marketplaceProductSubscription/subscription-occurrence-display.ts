export const getSubscriptionOccurrenceModificationLabel = (hasRecurringInstanceOverrides?: boolean | null) =>
  hasRecurringInstanceOverrides === true ? 'Individually updated' : null;
