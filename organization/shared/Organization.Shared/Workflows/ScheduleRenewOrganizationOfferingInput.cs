namespace Organization.Shared.Workflows;

public record ScheduleRenewOrganizationOfferingInput(string OrganizationId, string OrganizationOfferingId, DateTimeOffset RenewalDate);
