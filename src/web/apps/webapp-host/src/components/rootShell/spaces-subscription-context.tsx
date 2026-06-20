'use client';

import { createContext, type PropsWithChildren, useContext } from 'react';

export type SpacesSubscriptionState = {
  readonly subscriptionStatus: string;
  readonly accessReason: string;
  readonly trialStartedAt: string | null | undefined;
  readonly trialEndsAt: string | null | undefined;
  readonly remainingTrialDays: number;
  readonly canUseProduct: boolean;
  readonly canAcceptBookings: boolean;
  readonly canProtectExistingCommitments: boolean;
  readonly upgradeRequired: boolean;
  readonly isComplimentaryBridge: boolean;
  readonly nextBillingAt: string | null | undefined;
};

const SpacesSubscriptionContext = createContext<SpacesSubscriptionState | null>(null);

export const SpacesSubscriptionProvider = ({ value, children }: PropsWithChildren<{ value: SpacesSubscriptionState | null }>) => (
  <SpacesSubscriptionContext value={value}>{children}</SpacesSubscriptionContext>
);

export const useSpacesSubscription = () => useContext(SpacesSubscriptionContext);
