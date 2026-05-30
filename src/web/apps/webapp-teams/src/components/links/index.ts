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

export const getInstallMsTeamsLink = () => '/msteams/install-msteams';

export const getOrganizationsBaseLink = (integratedPlatform: string | undefined) => (integratedPlatform ? `${integratedPlatform}/organizations` : '/organizations');

export const getOrganizationSetupLink = (integratedPlatform: string | undefined) => getOrganizationAddPrivateLink(integratedPlatform);
export const getOrganizationAddPrivateLink = (integratedPlatform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatform)}/add-private`;
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

export const getOrganizationSsoSignInBaseLink = (integratedPlatform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/sso-signin?redirectUrl=${window.location.href}`;

export const getOrganizationLocationFloorPlanAddLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/floorPlans/add`;

export const getOrganizationLocationFloorPlanAdminEditLink = (integratedPlatform: string | undefined, id: string, locationId: string, floorPlanId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/floorPlans/admin/${floorPlanId}`;

export const getOrganizationLocationFloorPlansLink = (integratedPlatform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatform, id)}/locations/${locationId}/floorPlans`;

export const postSignOutReturnToKey = 'postSignOutReturnTo';
export const getSignOutReturnToLink = () => {
  const returnToPath = `${window.location.pathname}${window.location.search}${window.location.hash}`;
  sessionStorage.setItem(postSignOutReturnToKey, returnToPath);
  return window.location.origin;
};
