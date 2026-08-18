export const getCreditOutcomeMessage = (isCreditFunded: boolean | undefined, creditOutcome: string | null | undefined): string | null =>
  isCreditFunded && creditOutcome ? creditOutcome : null;
