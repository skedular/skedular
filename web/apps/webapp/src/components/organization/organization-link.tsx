export const getOrganizationBaseLink = (id: string) => `/organizations/${id}`;
export const getOrganizationAddLink = () => `/organizations/add`;

export const getModernOrganizationBookingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/bookings`;

export const getModernOrganizationUsersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/users`;

export const getModernOrganizationUserProfileBaseLink = (id: string, customerId: string) =>
  `${getOrganizationBaseLink(id)}/users/${customerId}?section=profile`;
export const getModernOrganizationUserManageTeamsBaseLink = (id: string, customerId: string) =>
  `${getOrganizationBaseLink(id)}/users/${customerId}?section=manage-teams`;
export const getModernOrganizationUserManageBaseLink = (id: string, customerId: string) =>
  `${getOrganizationBaseLink(id)}/users/${customerId}?section=manage-user`;

export const getModernOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/teams`;
export const getModernOrganizationTeamSetupBaseLink = (id: string, teamId: string) => `${getOrganizationBaseLink(id)}/teams/${teamId}?section=setup`;
export const getModernOrganizationTeamMembersBaseLink = (id: string, teamId: string) =>
  `${getOrganizationBaseLink(id)}/teams/${teamId}?section=members`;
export const getModernOrganizationTeamLocationBaseLink = (id: string, teamId: string) =>
  `${getOrganizationBaseLink(id)}/teams/${teamId}?section=location`;

export const getModernOrganizationBookingBaseLink = (id: string, bookingId: string) => `${getOrganizationBaseLink(id)}/bookings/${bookingId}`;

export const getModernOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/locations`;
export const getModernOrganizationLocationSetupBaseLink = (id: string, locationId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}?section=setup`;
export const getModernOrganizationLocationManageDesksBaseLink = (id: string, locationId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}?section=manage-desks`;

export const getModernOrganizationLocationDeskBaseLink = (id: string, locationId: string, deskId: string) =>
  `${getOrganizationBaseLink(id)}/locations/${locationId}/desks/${deskId}`;

export const getModernOrganizationAnalyticsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/analytics?section=organization`;
export const getModernOrganizationLocationsAnalyticsLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/analytics?section=locations`;

export const getModernOrganizationAdminSetupBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=setup`;
export const getModernOrganizationAdminBillingAndPaymentBaseLink = (id: string) =>
  `${getOrganizationBaseLink(id)}/admin?section=billing-payment-setup`;
export const getModernOrganizationAdminSSOBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=sso-setup`;
export const getModernOrganizationAdminZonesBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=zones-setup`;
export const getModernOrganizationAdminCustomTagsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=tags-setup`;
export const getModernOrganizationAdminSubscriptionsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}/admin?section=subscriptions`;

export const getOrganizationBookingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=bookings`;
export const getOrganizationSettingsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=about`;
export const getOrganizationMembersBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=members`;
export const getOrganizationLocationsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=locations`;
export const getOrganizationTeamsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=teams`;
export const getOrganizationOfferingBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=offering`;
export const getOrganizationBillingBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=billing`;
export const getOrganizationAnalyticsBaseLink = (id: string) => `${getOrganizationBaseLink(id)}?tab=analytics`;

export const getNotificationsBaseLink = () => `/notifications`;
export const getModernNotificationsBaseLink = () => `/notifications`;
