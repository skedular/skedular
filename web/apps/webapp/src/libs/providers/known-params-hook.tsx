import { useParams } from 'next/navigation';

const useKnownParams = () => {
  const {
    organizationUniqueAlphanumericName,
    locationId,
    bookingId,
    productId,
    resourceId,
    organizationBankAccountId,
    customerId,
    floorPlanId,
    teamId,
    organizationStripeConnectAccountId,
  } = useParams();
  const host = typeof window !== 'undefined' ? window.location.hostname : '';
  const isCustomDomain = host !== 'localhost' && host !== '127.0.0.1' && host !== 'skedular.app' && host !== 'staging.skedular.app' && host !== 'www.skedular.app';

  let finalOrganizationUniqueAlphanumericName = '';
  if (isCustomDomain) {
    finalOrganizationUniqueAlphanumericName = host.split('.')[0];
  } else if (typeof organizationUniqueAlphanumericName === 'string') {
    finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName;
  } else if (Array.isArray(organizationUniqueAlphanumericName)) {
    if (typeof organizationUniqueAlphanumericName[0] !== 'undefined') {
      finalOrganizationUniqueAlphanumericName = organizationUniqueAlphanumericName[0];
    }
  }

  return {
    organizationUniqueAlphanumericName: finalOrganizationUniqueAlphanumericName,
    locationId: typeof locationId === 'string' ? locationId : Array.isArray(locationId) && typeof locationId[0] !== 'undefined' ? locationId[0] : '',
    bookingId: typeof bookingId === 'string' ? bookingId : Array.isArray(bookingId) && typeof bookingId[0] !== 'undefined' ? bookingId[0] : '',
    productId: typeof productId === 'string' ? productId : Array.isArray(productId) && typeof productId[0] !== 'undefined' ? productId[0] : '',
    resourceId: typeof resourceId === 'string' ? resourceId : Array.isArray(resourceId) && typeof resourceId[0] !== 'undefined' ? resourceId[0] : '',
    organizationBankAccountId:
      typeof organizationBankAccountId === 'string'
        ? organizationBankAccountId
        : Array.isArray(organizationBankAccountId) && typeof organizationBankAccountId[0] !== 'undefined'
          ? organizationBankAccountId[0]
          : '',
    customerId: typeof customerId === 'string' ? customerId : Array.isArray(customerId) && typeof customerId[0] !== 'undefined' ? customerId[0] : '',
    floorPlanId: typeof floorPlanId === 'string' ? floorPlanId : Array.isArray(floorPlanId) && typeof floorPlanId[0] !== 'undefined' ? floorPlanId[0] : '',
    teamId: typeof teamId === 'string' ? teamId : Array.isArray(teamId) && typeof teamId[0] !== 'undefined' ? teamId[0] : '',
    organizationStripeConnectAccountId:
      typeof organizationStripeConnectAccountId === 'string'
        ? organizationStripeConnectAccountId
        : Array.isArray(organizationStripeConnectAccountId) && typeof organizationStripeConnectAccountId[0] !== 'undefined'
          ? organizationStripeConnectAccountId[0]
          : '',
  };
};

export default useKnownParams;
