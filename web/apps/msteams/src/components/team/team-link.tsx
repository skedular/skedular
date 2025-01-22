export const getTeamBaseLink = (id: string, organizationId: string) => `/organizations/${organizationId}/teams/${id}`;
export const getTeamAddLink = (organizationId: string) => `/organizations/${organizationId}/teams/add`;
export const getTeamBookingsLink = (id: string, organizationId: string) => `${getTeamBaseLink(id, organizationId)}?tab=bookings`;
export const getTeamSettingsLink = (id: string, organizationId: string) => `${getTeamBaseLink(id, organizationId)}?tab=about`;
export const getTeamMembersLink = (id: string, organizationId: string) => `${getTeamBaseLink(id, organizationId)}?tab=members`;
