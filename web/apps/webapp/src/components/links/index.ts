export const getOrganizationBaseLink = (id: string) => `/organizations/${id}`;
export const getOrganizationAddLink = () => `/organizations/add`;
export const getOrganizationBookingsBaseLink = (id: string, options?: { customerId?: string; locationId?: string; teamId?: string }) => {
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

  return params ? `${getOrganizationBaseLink(id)}/bookings?${params}` : `${getOrganizationBaseLink(id)}/bookings`;
};
export const getOrganizationUsersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/users`;
export const getOrganizationUserProfileBaseLink = (id: string, customerId: string) => `${getOrganizationBaseLink(id)}/users/${customerId}?section=profile`;
export const getOrganizationUserManageTeamsBaseLink = (id: string, customerId: string) => `${getOrganizationBaseLink(id)}/users/${customerId}?section=manage-teams`;
export const getOrganizationUserManageBaseLink = (id: string, customerId: string) => `${getOrganizationBaseLink(id)}/users/${customerId}?section=manage-user`;
export const getOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/teams`;
export const getOrganizationTeamSetupBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=setup`;
export const getOrganizationTeamMembersBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=members`;
export const getOrganizationTeamLocationBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=location`;
export const getOrganizationTeamManageTeamBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=manage-team`;
export const getOrganizationBookingBaseLink = (id: string, bookingId: string) => `${getOrganizationBaseLink(id)}/bookings/${bookingId}`;
export const getOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/locations`;
export const getOrganizationLocationSetupBaseLink = (id: string, locationId: string) => `${getOrganizationBaseLink(id)}/locations/${locationId}?section=setup`;
export const getOrganizationLocationOpeningHoursBaseLink = (id: string, locationId: string) => `${getOrganizationBaseLink(id)}/locations/${locationId}?section=opening-hours`;
export const getOrganizationLocationManageResourcesBaseLink = (id: string, locationId: string) => `${getOrganizationBaseLink(id)}/locations/${locationId}?section=manage-resources`;
export const getOrganizationLocationManageLocationBaseLink = (id: string, locationId: string) => `${getOrganizationBaseLink(id)}/locations/${locationId}?section=manage-location`;
export const getOrganizationLocationResourceBaseLink = (id: string, locationId: string, resourceId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}/resources/${resourceId}`;
export const getOrganizationAnalyticsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/analytics?section=organization`;
export const getOrganizationLocationsAnalyticsLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/analytics?section=locations`;
export const getOrganizationAdminSetupBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=setup`;
export const getOrganizationAdminBillingAndPaymentBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=billing-payment-setup`;
export const getOrganizationAdminSSOBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=sso-setup`;
export const getOrganizationAdminZonesBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=zones-setup`;
export const getOrganizationAdminCustomTagsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=tags-setup`;
export const getOrganizationAdminProductTagsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=product-tags-setup`;
export const getOrganizationAdminLocationTagsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=location-tags-setup`;
export const getOrganizationAdminSubscriptionsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=subscriptions`;
export const getOrganizationAdminManageOrganizationBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=manage-organization`;
export const getOrganizationTeamAddLink = (id: string) => `${getOrganizationBaseLink(id)}/teams/add`;
export const getOrganizationLocationAddLink = (id: string) => `${getOrganizationBaseLink(id)}/locations/add`;
export const getNotificationsBaseLink = () => `/notifications`;

export const getOrganizationMarketplaceBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/marketplace`;
export const getOrganizationProductSetupBaseLink = (id: string, productId: string) => `${getOrganizationBaseLink(id)}/products/${productId}?section=setup`;
