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

export const getInstallMsTeamsLink = () => '/msteams/install-msteams';

export const getOrganizationsBaseLink = (integratedPlatrform: string | undefined) => (integratedPlatrform ? `${integratedPlatrform}/organizations` : '/organizations');

export const getOrganizationSetupLink = (integratedPlatrform: string | undefined) => getOrganizationAddPrivateLink(integratedPlatrform);
export const getOrganizationAddPrivateLink = (integratedPlatrform: string | undefined) => `${getOrganizationsBaseLink(integratedPlatrform)}/add-private`;
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

export const getOrganizationLocationsBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/locations`;
export const getOrganizationAddResourceBaseLink = (integratedPlatrform: string | undefined, id: string) => `${getOrganizationBaseLink(integratedPlatrform, id)}/resources/add`;
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
export const getOrganizationLocationAddResourceBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/resources/add`;
export const getOrganizationLocationBulkAddResourcesBaseLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/resources/bulk-add`;
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
export const getOrganizationAdminZonesBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=zones-setup`;
export const getOrganizationAdminAddZoneBaseLink = (integratedPlatrform: string | undefined, id: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatrform, id)}/admin/zones/add`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminEditZoneBaseLink = (integratedPlatrform: string | undefined, id: string, zoneId: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatrform, id)}/admin/zones/${zoneId}/edit`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminCustomTagsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=tags-setup`;
export const getOrganizationAdminAddCustomTagBaseLink = (integratedPlatrform: string | undefined, id: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatrform, id)}/admin/tags/add`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminEditCustomTagBaseLink = (integratedPlatrform: string | undefined, id: string, customTagId: string, options?: { redirectUrl?: string }) =>
  appendQueryParams(`${getOrganizationBaseLink(integratedPlatrform, id)}/admin/tags/${customTagId}/edit`, { redirectUrl: options?.redirectUrl });
export const getOrganizationAdminSubscriptionsBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=subscriptions`;
export const getOrganizationAdminManageOrganizationBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/admin?section=manage-organization`;

export const getOrganizationSsoSignInBaseLink = (integratedPlatrform: string | undefined, id: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/sso-signin?redirectUrl=${window.location.href}`;

export const getOrganizationLocationFloorPlanAddLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/floorPlans/add`;

export const getOrganizationLocationFloorPlanAdminEditLink = (integratedPlatrform: string | undefined, id: string, locationId: string, floorPlanId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/floorPlans/admin/${floorPlanId}`;

export const getOrganizationLocationFloorPlansLink = (integratedPlatrform: string | undefined, id: string, locationId: string) =>
  `${getOrganizationBaseLink(integratedPlatrform, id)}/locations/${locationId}/floorPlans`;

export const postSignOutReturnToKey = 'postSignOutReturnTo';
export const getSignOutReturnToLink = () => {
  const returnToPath = `${window.location.pathname}${window.location.search}${window.location.hash}`;
  sessionStorage.setItem(postSignOutReturnToKey, returnToPath);
  return window.location.origin;
};
