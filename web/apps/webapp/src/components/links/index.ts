export const getRootLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `/${integratedPlatrform}` : '/');
export const getMeLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `/${integratedPlatrform}/me` : '/me');
export const getNotificationsBaseLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `${integratedPlatrform}/notifications` : '/notifications');

export const getOrganizationBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  integratedPlatrform ? `/${integratedPlatrform}/organizations/${id}` : `/organizations/${id}`;
export const getOrganizationAddLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `${integratedPlatrform}/organizations/add` : '/organizations/add');
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
export const getOrganizationUsersBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/users`;
export const getOrganizationUserProfileBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=profile`;
export const getOrganizationUserManageTeamsBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=manage-teams`;
export const getOrganizationUserBillingAndPaymentBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=billing-payment-setup`;
export const getOrganizationUserManageBaseLink = (integratedPlatrform: string | undefined, id: string, customerId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/users/${customerId}?section=manage-user`;

export const getOrganizationTeamsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/teams`;
export const getOrganizationTeamSetupBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=setup`;
export const getOrganizationTeamMembersBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=members`;
export const getOrganizationTeamLocationBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=location`;
export const getOrganizationTeamManageTeamBaseLink = (integratedPlatrform: string | undefined, id: string, teamId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/${teamId}?section=manage-team`;
export const getOrganizationBookingBaseLink = (integratedPlatrform: string | undefined, id: string, bookingId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/bookings/${bookingId}`;

export const getOrganizationLocationsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/locations`;
export const getOrganizationLocationSetupBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=setup`;
export const getOrganizationLocationOpeningHoursBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=opening-hours`;
export const getOrganizationLocationFloorPlansBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=floor-plans`;
export const getOrganizationLocationManageResourcesBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=manage-resources`;
export const getOrganizationLocationManageLocationBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}?section=manage-location`;
export const getOrganizationLocationResourceBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/resources/${resourceId}`;
export const getOrganizationAnalyticsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/analytics?section=organization`;
export const getOrganizationLocationsAnalyticsLocationsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/analytics?section=locations`;

export const getOrganizationAdminSetupBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=setup`;
export const getOrganizationAdminBillingAndPaymentBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=billing-payment-setup`;
export const getOrganizationAdminSsoSettingsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=sso-setup`;
export const getOrganizationAdminTaxDetailsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=tax-setup`;
export const getOrganizationAdminZonesBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=zones-setup`;
export const getOrganizationAdminCustomTagsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=tags-setup`;
export const getOrganizationAdminSubscriptionsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=subscriptions`;
export const getOrganizationAdminManageOrganizationBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=manage-organization`;

export const getOrganizationTeamAddLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/teams/add`;
export const getOrganizationLocationAddLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/add`;

export const getOrganizationMarketplaceSetupBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/setup-marketplace`;
export const getOrganizationMarketplaceSetupProductsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/setup-marketplace?section=products-setup`;
export const getOrganizationMarketplaceSetupProductTagsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/setup-marketplace?section=product-tags-setup`;
export const getOrganizationMarketplaceSetupLocationTagsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/setup-marketplace?section=location-tags-setup`;
export const getOrganizationMarketplaceSetupStripeConnectAccountsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/setup-marketplace?section=stripe-connect-accounts-setup`;
export const getOrganizationMarketplaceSetupBankAccountsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/setup-marketplace?section=bank-accounts-setup`;
export const getOrganizationMarketplaceBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/marketplace`;
export const getOrganizationProductBaseLink = (integratedPlatrform: string | undefined, id: string, productId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/products/${productId}`;
export const getOrganizationProductAddLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/products/add`;
export const getOrganizationBookingProductLink = (integratedPlatrform: string | undefined, id: string, productId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/products/${productId}/book`;

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
