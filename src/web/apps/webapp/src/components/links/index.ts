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

export const getRootLink = (integratedPlatform: string | undefined) => (integratedPlatform ? `/${integratedPlatform}` : '/');
export const getSignInLink = () => '/signin';
export const getSignUpLink = () => '/signup';
export const getWelcomeLink = (integratedPlatform: string | undefined) => (integratedPlatform ? `/${integratedPlatform}/welcome` : '/welcome');
export const getNotificationsLink = (integratedPlatform: string | undefined) => (integratedPlatform ? `${integratedPlatform}/notifications` : '/notifications');
export const getSettingsLink = (integratedPlatform: string | undefined) => (integratedPlatform ? `/${integratedPlatform}/settings` : '/settings');
export const getMarketplaceLocationLink = (integratedPlatform: string | undefined, locationId: string) =>
  integratedPlatform ? `/${integratedPlatform}/marketplace/locations/${locationId}` : `/marketplace/locations/${locationId}`;
export const getMarketplaceLocationFloorPlansLink = (integratedPlatform: string | undefined, locationId: string) =>
  integratedPlatform ? `/${integratedPlatform}/marketplace/locations/${locationId}/floorPlans` : `/marketplace/locations/${locationId}/floorPlans`;

export const getMarketplaceBookingsLink = (integratedPlatform: string | undefined, isCustomDomain: boolean, organizationCustomDomain: string) => {
  const baseLink = isCustomDomain ? 'bookings' : `organizations/${organizationCustomDomain}/bookings`;

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceSubscriptionsLink = (integratedPlatform: string | undefined, isCustomDomain: boolean, organizationCustomDomain: string) => {
  const baseLink = isCustomDomain ? 'subscriptions' : `organizations/${organizationCustomDomain}/subscriptions`;

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceProductLink = (
  integratedPlatform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  resourceIds?: string[],
) => {
  const baseLink = isCustomDomain ? `products/${productId}` : `organizations/${organizationCustomDomain}/products/${productId}`;
  const link = appendQueryParams(baseLink, { resourceIds });

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${link}` : `/marketplace/${link}`;
};

export const getMarketplaceProductBookingLink = (
  integratedPlatform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  pricingOptionId: string,
  resourceIds?: string[],
) => {
  const basePath = isCustomDomain ? `products/${productId}/book` : `organizations/${organizationCustomDomain}/products/${productId}/book`;
  const baseLink = appendQueryParams(basePath, { pricingOptionId, resourceIds });

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceProductBookingDetailsLink = (
  integratedPlatform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  bookingId: string,
) => {
  const baseLink = isCustomDomain ? `products/${productId}/bookings/${bookingId}` : `organizations/${organizationCustomDomain}/products/${productId}/bookings/${bookingId}`;

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceBookingDetailsLink = (integratedPlatform: string | undefined, isCustomDomain: boolean, organizationCustomDomain: string, bookingId: string) => {
  const baseLink = isCustomDomain ? `bookings/${bookingId}` : `organizations/${organizationCustomDomain}/bookings/${bookingId}`;

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceProductSubscribeLink = (
  integratedPlatform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  productId: string,
  pricingOptionId: string,
  resourceIds?: string[],
) => {
  const basePath = isCustomDomain ? `products/${productId}/subscribe` : `organizations/${organizationCustomDomain}/products/${productId}/subscribe`;
  const baseLink = appendQueryParams(basePath, { pricingOptionId, resourceIds });

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getMarketplaceSubscriptionDetailsLink = (
  integratedPlatform: string | undefined,
  isCustomDomain: boolean,
  organizationCustomDomain: string,
  subscriptionId: string,
) => {
  const baseLink = isCustomDomain ? `subscriptions/${subscriptionId}` : `organizations/${organizationCustomDomain}/subscriptions/${subscriptionId}`;

  return integratedPlatform ? `/${integratedPlatform}/marketplace/${baseLink}` : `/marketplace/${baseLink}`;
};

export const getInstallMsTeamsLink = () => '/msteams/install-msteams';

export const getOrganizationsBaseLink = (integratedPlatform: string | undefined) => (integratedPlatform ? `${integratedPlatform}/organizations` : '/organizations');

export const getOrganizationSetupLink = (integratedPlatform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatform)}/setup`;
export const getOrganizationAddPrivateLink = (integratedPlatform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatform)}/add-private`;
export const getOrganizationAddMarketplaceLink = (integratedPlatform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatform)}/add-marketplace`;
export const getOrganizationBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationsBaseLink(integratedPlatform)}/${id}`;

export const getOrganizationBookingsBaseLink = (integratedPlatform: string | undefined, id: string, options?: { customerId?: string; locationId?: string; teamId?: string }) => {
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

  return params ? `${getOrganizationBaseLink(integratedPlatform, id)}/bookings?${params}` : `${getOrganizationBaseLink(integratedPlatform, id)}/bookings`;
};
export const getOrganizationBookingAddLink = (
  integratedPlatform: string | undefined,
  id: string,
  options?: { locationId?: string; date?: string; resourceIds?: string[]; redirectUrl?: string },
) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/bookings/add`, {
    locationId: options?.locationId,
    date: options?.date,
    resourceIds: options?.resourceIds,
    redirectUrl: options?.redirectUrl,
  });
export const getOrganizationSubscriptionsBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/subscriptions`;
export const getOrganizationSubscriptionBaseLink = (integratedPlatform: string | undefined, id: string, subscriptionId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/subscriptions/${subscriptionId}`;
export const getOrganizationUsersBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/users`;
export const getOrganizationUserProfileBaseLink = (integratedPlatform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/users/${customerId}?section=profile`;
export const getOrganizationUserManageTeamsBaseLink = (integratedPlatform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/users/${customerId}?section=manage-teams`;
export const getOrganizationUserBillingAndPaymentBaseLink = (integratedPlatform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/users/${customerId}?section=billing-payment-setup`;
export const getOrganizationUserManageBaseLink = (integratedPlatform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/users/${customerId}?section=manage-user`;

export const getOrganizationTeamAddLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/teams/add`;
export const getOrganizationTeamsBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/teams`;
export const getOrganizationTeamSetupBaseLink = (integratedPlatform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/teams/${teamId}?section=setup`;
export const getOrganizationTeamMembersBaseLink = (integratedPlatform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/teams/${teamId}?section=members`;
export const getOrganizationTeamManageTeamBaseLink = (integratedPlatform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/teams/${teamId}?section=manage-team`;
export const getOrganizationBookingBaseLink = (integratedPlatform: string | undefined, id: string, bookingId: string, options?: { editMode?: 'occurrence' | 'recurring' }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/bookings/${bookingId}`, {
    editMode: options?.editMode,
  });

export const getOrganizationLocationAddPrivateLink = (integratedPlatform: string | undefined, id: string, options?: { redirectUrl?: string }) => {
  let params = '';

  if (options?.redirectUrl) {
    params += `redirectUrl=${options.redirectUrl}`;
  }

  return params ? `${getOrganizationBaseLink(integratedPlatform, id)}/locations/add-private?${params}` : `${getOrganizationBaseLink(integratedPlatform, id)}/locations/add-private`;
};

export const getOrganizationLocationAddMarketplaceLink = (integratedPlatform: string | undefined, id: string, options?: { redirectUrl?: string }) => {
  let params = '';

  if (options?.redirectUrl) {
    params += `redirectUrl=${options.redirectUrl}`;
  }

  return params
    ? `${getOrganizationBaseLink(integratedPlatform, id)}/locations/add-marketplace?${params}`
    : `${getOrganizationBaseLink(integratedPlatform, id)}/locations/add-marketplace`;
};

export const getOrganizationLocationsBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/locations`;
export const getOrganizationAddResourceBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/resources/add`;
export const getOrganizationLocationBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}`;
export const getOrganizationLocationSetupBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=setup`;
export const getOrganizationLocationPhysicalAddressSetupBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=physical-address-setup`;
export const getOrganizationLocationOpeningHoursBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=opening-hours`;
export const getOrganizationLocationFloorPlansBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=floor-plans`;
export const getOrganizationLocationManageResourcesBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=manage-resources`;
export const getOrganizationLocationRestrictedInformationBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=restricted-information`;
export const getOrganizationLocationManageLocationBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}?section=manage-location`;
export const getOrganizationLocationResourceBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/resources/${resourceId}`;
export const getOrganizationLocationAddResourceBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/resources/add`;
export const getOrganizationLocationBulkAddResourcesBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/resources/bulk-add`;
export const getOrganizationLocationResourceSetupBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/resources/${resourceId}?section=setup`;
export const getOrganizationLocationResourceOpeningHoursBaseLink = (integratedPlatform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/resources/${resourceId}?section=opening-hours`;
export const getOrganizationAnalyticsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/analytics?section=organization`;
export const getOrganizationLocationsAnalyticsLocationsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/analytics?section=locations`;
export const getOrganizationAvailabilityDashboardBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/availability-dashboard`;

export const getOrganizationAdminBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/admin`;
export const getOrganizationAdminSetupBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationAdminBaseLink(integratedPlatform, id)}?section=setup`;
export const getOrganizationAdminPhysicalAddressBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=physical-address-setup`;
export const getOrganizationAdminBillingAndPaymentBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=billing-payment-setup`;
export const getOrganizationAdminSsoSettingsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=sso-setup`;
export const getOrganizationAdminTaxDetailsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=tax-details-setup`;
export const getOrganizationAdminZonesBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=zones-setup`;
export const getOrganizationAdminAddZoneBaseLink = (integratedPlatform: string | undefined, id: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/admin/zones/add`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminEditZoneBaseLink = (integratedPlatform: string | undefined, id: string, zoneId: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/admin/zones/${zoneId}/edit`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminCustomTagsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=tags-setup`;
export const getOrganizationAdminAddCustomTagBaseLink = (integratedPlatform: string | undefined, id: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/admin/tags/add`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminEditCustomTagBaseLink = (integratedPlatform: string | undefined, id: string, customTagId: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/admin/tags/${customTagId}/edit`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminSubscriptionsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=subscriptions`;
export const getOrganizationAdminManageOrganizationBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=manage-organization`;

export const getOrganizationMarketplaceSetupBaseLink = (integratedPlatform: string | undefined, id: string) =>
  getOrganizationMarketplaceSetupMarketplaceListingBaseLink(integratedPlatform, id);
export const getOrganizationMarketplaceSetupProductTagsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=product-tags-setup`;
export const getOrganizationAdminAddProductTagBaseLink = (integratedPlatform: string | undefined, id: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/admin/product-tags/add`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminEditProductTagBaseLink = (integratedPlatform: string | undefined, id: string, productTagId: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatform, id)}/admin/product-tags/${productTagId}/edit`, { redirectUrl: options?.redirectUrl });
export const getOrganizationMarketplaceSetupMarketplaceListingBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=marketplace-listing`;
export const getOrganizationMarketplaceSetupBillingCycleBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=billing-cycle`;
export const getOrganizationMarketplaceSetupXeroBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=xero-setup`;
export const getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=stripe-connect-accounts-setup`;
export const getOrganizationMarketplaceSetupBankAccountsBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/admin?section=bank-accounts-setup`;
export const getOrganizationProductsBaseLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/products`;
export const getOrganizationProductBaseLink = (integratedPlatform: string | undefined, id: string, productId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/products/${productId}`;
export const getOrganizationProductAddLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/products/add`;

export const getOrganizationSsoSignInBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/sso-signin?redirectUrl=${window.location.href}`;

export const getOrganizationStripeConnectAccountBaseLink = (integratedPlatform: string | undefined, id: string, stripeConnectAccountId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/stripe-connect-accounts/${stripeConnectAccountId}`;
export const getOrganizationStripeConnectAccountAddLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/stripe-connect-accounts/add`;

export const getOrganizationLocationFloorPlanAddLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/floorPlans/add`;

export const getOrganizationLocationFloorPlanAdminEditLink = (integratedPlatform: string | undefined, id: string, locationId: string, floorPlanId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/floorPlans/admin/${floorPlanId}`;

export const getOrganizationLocationFloorPlansLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/floorPlans`;

export const getOrganizationBankAccountBaseLink = (integratedPlatform: string | undefined, id: string, bankAccountId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/bank-accounts/${bankAccountId}`;
export const getOrganizationBankAccountAddLink = (integratedPlatform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatform, id)}/bank-accounts/add`;

export const postSignOutReturnToKey = 'postSignOutReturnTo';
export const getSignOutReturnToLink = () => {
  const returnToPath = `${window.location.pathname}${window.location.search}${window.location.hash}`;
  sessionStorage.setItem(postSignOutReturnToKey, returnToPath);
  return window.location.origin;
};
