export const getLocationBaseLink = (id: string, organizationId?: string) =>
  organizationId ? `/organizations/${organizationId}/locations/${id}` : `/locations/${id}`;
export const getLocationAddLink = (organizationId?: string) => (organizationId ? `/organizations/${organizationId}/locations/add` : `/locations/add`);
export const getLocationBookingsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=bookings`;
export const getLocationSettingsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=about`;
export const getLocationMembersLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=members`;
export const getLocationZonesLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=zones`;
export const getLocationDesksLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=desks`;
export const getLocationAnalyticsLink = (id: string, organizationId?: string) => `${getLocationBaseLink(id, organizationId)}?tab=analytics`;
