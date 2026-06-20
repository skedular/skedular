export const getResourceAssignmentPendingMessage = (hasRecurringInstanceOverrides: boolean) =>
  hasRecurringInstanceOverrides
    ? 'This individual booking was updated by an operator and will not be changed automatically.'
    : 'Skedular will keep trying to assign a compatible resource on this booking date.';
