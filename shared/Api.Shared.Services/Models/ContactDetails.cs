namespace Api.Shared.Services.Models;

public record ContactDetails(IReadOnlyList<string>? ContactPeople, IReadOnlyList<string>? ContactEmails, IReadOnlyList<string>? ContactPhones);
