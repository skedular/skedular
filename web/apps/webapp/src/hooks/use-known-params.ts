'use client';

import { useParams } from 'next/navigation';
import { getOrganizationCustomDomainFromHost } from '../app/auth/host-utils';

const getKnownParamValue = (value: string | string[] | undefined): string => {
  if (typeof value === 'string') {
    return value;
  }

  if (Array.isArray(value) && typeof value[0] !== 'undefined') {
    return value[0];
  }

  return '';
};

const useKnownParams = () => {
  const {
    organizationCustomDomain,
    locationId,
    bookingId,
    subscriptionId,
    productId,
    resourceId,
    organizationBankAccountId,
    customerId,
    floorPlanId,
    teamId,
    organizationStripeConnectAccountId,
  } = useParams();
  const host = typeof window !== 'undefined' ? window.location.hostname : '';
  const customDomainOrganizationName = getOrganizationCustomDomainFromHost(host);
  const isCustomDomain = customDomainOrganizationName !== null;

  return {
    isCustomDomain,
    organizationCustomDomain: customDomainOrganizationName ?? getKnownParamValue(organizationCustomDomain),
    locationId: getKnownParamValue(locationId),
    bookingId: getKnownParamValue(bookingId),
    subscriptionId: getKnownParamValue(subscriptionId),
    productId: getKnownParamValue(productId),
    resourceId: getKnownParamValue(resourceId),
    organizationBankAccountId: getKnownParamValue(organizationBankAccountId),
    customerId: getKnownParamValue(customerId),
    floorPlanId: getKnownParamValue(floorPlanId),
    teamId: getKnownParamValue(teamId),
    organizationStripeConnectAccountId: getKnownParamValue(organizationStripeConnectAccountId),
  };
};

export default useKnownParams;
