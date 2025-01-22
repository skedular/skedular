export const getTeamBaseLink = (id: string, organizationId?: string) =>
  organizationId ? `/organizations/${organizationId}/teams/${id}` : `/teams/${id}`;
export const getTeamAddLink = (organizationId?: string) => (organizationId ? `/organizations/${organizationId}/teams/add` : `/teams/add`);
export const getTeamBookingsLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=bookings`;
export const getTeamSettingsLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=about`;
export const getTeamMembersLink = (id: string, organizationId?: string) => `${getTeamBaseLink(id, organizationId)}?tab=members`;
