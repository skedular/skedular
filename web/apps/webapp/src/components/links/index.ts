const appendQueryParams = (path: string, params: Record<string, string | string[] | undefined>) => {
  const searchParams = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (Array.isArray(value)) {
      if (value.length > 0) {
        searchParams.set(key, value.join(','));
      }
      return;
    }

    if (value) {
      searchParams.set(key, value);
    }
  });

  const query = searchParams.toString();
  return query ? `${path}?${query}` : path;
};

export const getRootLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `/${integratedPlatrform}` : '/');
export const getSignInLink = () => '/signin';
export const getSignUpLink = () => '/signup';
export const getWelcomeLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `/${integratedPlatrform}/welcome` : '/welcome');
export const getNotificationsLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `${integratedPlatrform}/notifications` : '/notifications');
export const getSettingsLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `/${integratedPlatrform}/settings` : '/settings');
export const getMarketplaceLocationLink = (integratedPlatrform: string | undefined, locationId: string) =>
  integratedPlatrform ? `/${integratedPlatrform}/marketplace/locations/${locationId}` : `/marketplace/locations/${locationId}`;
export const getMarketplaceLocationFloorPlansLink = (integratedPlatrform: string | undefined, locationId: string) =>
  integratedPlatrform ? `/${integratedPlatrform}/marketplace/locations/${locationId}/floorPlans` : `/marketplace/locations/${locationId}/floorPlans`;

export const getMarketplaceBookingsLink = (integratedPlatrform: string | undefined, isCustomDomain: boolean, organizationCustomDomain: string) => {
  const baseLink = isCustomDomain ? 'bookings' : `organizations/${organizationCustomDomain}/bookings`;

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceSubscriptionsLink = (integratedPlatrform: string | undefined, isCustomDomain: boolean, organizationCustomDomain: string) => {
  const baseLink = isCustomDomain ? 'subscriptions' : `organizations/${organizationCustomDomain}/subscriptions`;

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceProductLink = (
  integratedPlatrform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  resourceIds?: string[],
) => {
  const baseLink = isCustomDomain ? `products/${productId}` : `organizations/${organizationCustomDomain}/products/${productId}`;
  const link = appendQueryParams(baseLink, { resourceIds });

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${link}` : `/marketplace/${link}`;
};

export const getMarketplaceProductBookingLink = (
  integratedPlatrform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  pricingOptionId: string,
  resourceIds?: string[],
) => {
  const basePath = isCustomDomain ? `products/${productId}/book` : `organizations/${organizationCustomDomain}/products/${productId}/book`;
  const baseLink = appendQueryParams(basePath, { pricingOptionId, resourceIds });

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceProductBookingDetailsLink = (
  integratedPlatrform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  bookingId: string,
) => {
  const baseLink = isCustomDomain ? `products/${productId}/bookings/${bookingId}` : `organizations/${organizationCustomDomain}/products/${productId}/bookings/${bookingId}`;

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceBookingDetailsLink = (integratedPlatrform: string | undefined, isCustomDomain: boolean, organizationCustomDomain: string, bookingId: string) => {
  const baseLink = isCustomDomain ? `bookings/${bookingId}` : `organizations/${organizationCustomDomain}/bookings/${bookingId}`;

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceProductSubscribeLink = (
  integratedPlatrform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  pricingOptionId: string,
  resourceIds?: string[],
) => {
  const basePath = isCustomDomain ? `products/${productId}/subscribe` : `organizations/${organizationCustomDomain}/products/${productId}/subscribe`;
  const baseLink = appendQueryParams(basePath, { pricingOptionId, resourceIds });

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceSubscriptionDetailsLink = (
  integratedPlatrform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  subscriptionId: string,
) => {
  const baseLink = isCustomDomain ? `subscriptions/${subscriptionId}` : `organizations/${organizationCustomDomain}/subscriptions/${subscriptionId}`;

  return integratedPlatrform ? `/${integratedPlatrform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getInstallMsTeamsLink = () => '/msteams/install-msteams';

export const getOrganizationsBaseLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `${integratedPlatrform}/organizations` : '/organizations');

export const getOrganizationSetupLink = (integratedPlatrform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatrform)}/setup`;
export const getOrganizationAddPrivateLink = (integratedPlatrform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatrform)}/add-private`;
export const getOrganizationAddMarketplaceLink = (integratedPlatrform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatrform)}/add-marketplace`;
export const getOrganizationBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationsBaseLink(integratedPlatrform)}/${id}`;

export const getOrganizationBookingsBaseLink = (integratedPlatrform: string | undefined, id: string, options?: { customerId?: string; locationId?: string; teamId?: string }) => {
  let params = '';

  if (options?.customerId) {
    params += `customerId=${options.customerId}`;
  }

  if (options?.locationId) {
    params += params ? `&locationId=${options.locationId}` : `locationId=${options.locationId}`;
  }

  if (options?.teamId) {
    params += params ? `&teamId=${options.teamId}` : `teamId=${options.teamId}`;
  }

  return params ? `${getOrganizationBaseLink(integratedPlatrform, id)}/bookings?${params}` : `${getOrganizationBaseLink(integratedPlatrform, id)}/bookings`;
};
export const getOrganizationBookingAddLink = (
  integratedPlatrform: string | undefined,
  id: string,
  options?: { locationId?: string; date?: string; resourceIds?: string[]; redirectUrl?: string },
) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatrform, id)}/bookings/add`, {
    locationId: options?.locationId,
    date: options?.date,
    resourceIds: options?.resourceIds,
    redirectUrl: options?.redirectUrl,
  });
export const getOrganizationSubscriptionsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/subscriptions`;
export const getOrganizationSubscriptionBaseLink = (integratedPlatrform: string | undefined, id: string, subscriptionId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/subscriptions/${subscriptionId}`;
export const getOrganizationUsersBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/users`;
export const getOrganizationUserProfileBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=profile`;
export const getOrganizationUserManageTeamsBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=manage-teams`;
export const getOrganizationUserBillingAndPaymentBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=billing-payment-setup`;
export const getOrganizationUserManageBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=manage-user`;

export const getOrganizationTeamAddLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/add`;
export const getOrganizationTeamsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/teams`;
export const getOrganizationTeamSetupBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=setup`;
export const getOrganizationTeamMembersBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=members`;
export const getOrganizationTeamManageTeamBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=manage-team`;
export const getOrganizationBookingBaseLink = (integratedPlatrform: string | undefined, id: string, bookingId: string, options?: { editMode?: 'occurrence' | 'recurring' }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatrform, id)}/bookings/${bookingId}`, {
    editMode: options?.editMode,
  });

export const getOrganizationLocationAddPrivateLink = (integratedPlatrform: string | undefined, id: string, options?: { redirectUrl?: string }) => {
  let params = '';

  if (options?.redirectUrl) {
    params += `redirectUrl=${options.redirectUrl}`;
  }

  return params
    ? `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/add-private?${params}`
    : `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/add-private`;
};

export const getOrganizationLocationAddMarketplaceLink = (integratedPlatrform: string | undefined, id: string, options?: { redirectUrl?: string }) => {
  let params = '';

  if (options?.redirectUrl) {
    params += `redirectUrl=${options.redirectUrl}`;
  }

  return params
    ? `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/add-marketplace?${params}`
    : `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/add-marketplace`;
};

export const getOrganizationLocationsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/locations`;
export const getOrganizationLocationSetupBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=setup`;
export const getOrganizationLocationPhysicalAddressSetupBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=physical-address-setup`;
export const getOrganizationLocationOpeningHoursBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=opening-hours`;
export const getOrganizationLocationFloorPlansBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=floor-plans`;
export const getOrganizationLocationManageResourcesBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=manage-resources`;
export const getOrganizationLocationRestrictedInformationBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=restricted-information`;
export const getOrganizationLocationManageLocationBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=manage-location`;
export const getOrganizationLocationResourceBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/resources/${resourceId}`;
export const getOrganizationLocationResourceSetupBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/resources/${resourceId}?section=setup`;
export const getOrganizationLocationResourceOpeningHoursBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/resources/${resourceId}?section=opening-hours`;
export const getOrganizationAnalyticsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/analytics?section=organization`;
export const getOrganizationLocationsAnalyticsLocationsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/analytics?section=locations`;
export const getOrganizationAvailabilityDashboardBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/availability-dashboard`;

export const getOrganizationAdminBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/admin`;
export const getOrganizationAdminSetupBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationAdminBaseLink(integratedPlatrform, id)}?section=setup`;
export const getOrganizationAdminPhysicalAddressBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=physical-address-setup`;
export const getOrganizationAdminBillingAndPaymentBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=billing-payment-setup`;
export const getOrganizationAdminSsoSettingsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=sso-setup`;
export const getOrganizationAdminTaxDetailsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=tax-details-setup`;
export const getOrganizationAdminZonesBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=zones-setup`;
export const getOrganizationAdminCustomTagsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=tags-setup`;
export const getOrganizationAdminSubscriptionsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=subscriptions`;
export const getOrganizationAdminManageOrganizationBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=manage-organization`;

export const getOrganizationMarketplaceSetupBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  getOrganizationMarketplaceSetupMarketplaceListingBaseLink(integratedPlatrform, id);
export const getOrganizationMarketplaceSetupProductTagsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=product-tags-setup`;
export const getOrganizationMarketplaceSetupMarketplaceListingBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=marketplace-listing`;
export const getOrganizationMarketplaceSetupBillingCycleBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=billing-cycle`;
export const getOrganizationMarketplaceSetupXeroBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=xero-setup`;
export const getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=stripe-connect-accounts-setup`;
export const getOrganizationMarketplaceSetupBankAccountsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=bank-accounts-setup`;
export const getOrganizationProductsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/products`;
export const getOrganizationProductBaseLink = (integratedPlatrform: string | undefined, id: string, productId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/products/${productId}`;
export const getOrganizationProductAddLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/products/add`;

export const getOrganizationSsoSignInBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/sso-signin?redirectUrl=${window.location.href}`;

export const getOrganizationStripeConnectAccountBaseLink = (integratedPlatrform: string | undefined, id: string, stripeConnectAccountId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/stripe-connect-accounts/${stripeConnectAccountId}`;
export const getOrganizationStripeConnectAccountAddLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/stripe-connect-accounts/add`;

export const getOrganizationLocationFloorPlanAddLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/floorPlans/add`;

export const getOrganizationLocationFloorPlanAdminEditLink = (integratedPlatrform: string | undefined, id: string, locationId: string, floorPlanId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/floorPlans/admin/${floorPlanId}`;

export const getOrganizationLocationFloorPlansLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/floorPlans`;

export const getOrganizationBankAccountBaseLink = (integratedPlatrform: string | undefined, id: string, bankAccountId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/bank-accounts/${bankAccountId}`;
export const getOrganizationBankAccountAddLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/bank-accounts/add`;

export const postSignOutReturnToKey = 'postSignOutReturnTo';
export const getSignOutReturnToLink = () => {
  const returnToPath = `${window.location.pathname}${window.location.search}${window.location.hash}`;
  sessionStorage.setItem(postSignOutReturnToKey, returnToPath);
  return window.location.origin;
};
