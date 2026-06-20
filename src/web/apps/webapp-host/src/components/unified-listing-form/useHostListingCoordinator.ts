import { useMemo, useState } from 'react';

export type HostListingCoordinatorState = {
  locationId: string | null;
  productId: string | null;
  productReady: boolean;
  pendingProductDraft: {
    title?: string;
    about?: string;
    price?: number;
    cadence?: string;
    cancellationPolicyType?: string;
    imageUrls?: string[];
    minDurationMinutes?: number;
    maxDurationMinutes?: number;
    isTaxInclusive?: boolean;
    supportsSubscriptionAutoRenewal?: boolean;
  } | null;
};

let sharedState: HostListingCoordinatorState = {
  locationId: null,
  productId: null,
  productReady: false,
  pendingProductDraft: null,
};

export const useHostListingCoordinator = () => {
  const [state, setState] = useState<HostListingCoordinatorState>({
    ...sharedState,
  });

  const syncState = (next: HostListingCoordinatorState) => {
    sharedState = next;
    setState(next);
  };

  const setLocationCreated = (locationId: string) => {
    syncState({ ...sharedState, locationId });
  };

  const setProductReady = (productId: string) => {
    syncState({ ...sharedState, productId, productReady: true });
  };

  const setPendingProductDraft = (draft: NonNullable<HostListingCoordinatorState['pendingProductDraft']>) => {
    syncState({ ...sharedState, pendingProductDraft: draft });
  };

  const clearPendingProductDraft = () => {
    syncState({ ...sharedState, pendingProductDraft: null });
  };

  const canEditProduct = useMemo(() => state.productReady && Boolean(state.productId), [state.productId, state.productReady]);

  return {
    state,
    canEditProduct,
    setLocationCreated,
    setProductReady,
    setPendingProductDraft,
    clearPendingProductDraft,
  };
};
