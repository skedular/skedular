export type CustomerPurchaseHubCounts = {
  activeCount: number;
  historicCount: number;
};

export const toCustomerPurchaseHubCounts = ({ activeCount, historicCount }: CustomerPurchaseHubCounts) => {
  const normalizedActiveCount = Math.max(0, activeCount);
  const normalizedHistoricCount = Math.max(0, historicCount);

  return {
    activeCount: normalizedActiveCount,
    historicCount: normalizedHistoricCount,
    totalCount: normalizedActiveCount + normalizedHistoricCount,
  };
};

export const shouldShowCustomerPurchaseHubSignInPrompt = (isSignedIn: boolean) => !isSignedIn;
