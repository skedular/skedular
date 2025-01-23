export const getTeamAddLink = (organizationId: string) => `/organizations/${organizationId}/teams/add`;
export const getLocationAddLink = (organizationId: string) => `/organizations/${organizationId}/locations/add`;
export const getOrganizationBaseLink = (id: string) => `/organizations/${id}`;
export const getOrganizationAddLink = () => `/organizations/add`;
export const getOrganizationBookingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/bookings`;
export const getOrganizationUsersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/users`;
export const getOrganizationUserProfileBaseLink = (id: string, customerId: string) =>
  `${getOrganizationBaseLink(id)}/users/${customerId}?section=profile`;
export const getOrganizationUserManageTeamsBaseLink = (id: string, customerId: string) =>
  `${getOrganizationBaseLink(id)}/users/${customerId}?section=manage-teams`;
export const getOrganizationUserManageBaseLink = (id: string, customerId: string) =>
  `${getOrganizationBaseLink(id)}/users/${customerId}?section=manage-user`;
export const getOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/teams`;
export const getOrganizationTeamSetupBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=setup`;
export const getOrganizationTeamMembersBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=members`;
export const getOrganizationTeamLocationBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=location`;
export const getOrganizationBookingBaseLink = (id: string, bookingId: string) => `${getOrganizationBaseLink(id)}/bookings/${bookingId}`;
export const getOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/locations`;
export const getOrganizationLocationSetupBaseLink = (id: string, locationId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}?section=setup`;
export const getOrganizationLocationManageDesksBaseLink = (id: string, locationId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}?section=manage-desks`;
export const getOrganizationLocationDeskBaseLink = (id: string, locationId: string, deskId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}/desks/${deskId}`;
export const getOrganizationAnalyticsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/analytics?section=organization`;
export const getOrganizationLocationsAnalyticsLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/analytics?section=locations`;
export const getOrganizationAdminSetupBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=setup`;
export const getOrganizationAdminBillingAndPaymentBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=billing-payment-setup`;
export const getOrganizationAdminSSOBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=sso-setup`;
export const getOrganizationAdminZonesBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=zones-setup`;
export const getOrganizationAdminCustomTagsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=tags-setup`;
export const getOrganizationAdminSubscriptionsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=subscriptions`;
export const getNotificationsBaseLink = () => `/notifications`;
