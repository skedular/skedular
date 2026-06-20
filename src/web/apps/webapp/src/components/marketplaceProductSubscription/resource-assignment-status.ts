export const getResourceAssignmentPendingMessage = (hasRecurringInstanceOverrides: boolean) =>
  hasRecurringInstanceOverrides ? 'A space administrator is handling this booking individually.' : 'We will keep trying to assign a resource for this selected date.';
