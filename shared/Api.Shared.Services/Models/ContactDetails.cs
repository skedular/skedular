namespace Api.Shared.Services.Models;

public record ContactDetails(ICollection<string>? ContactPeople, ICollection<string>? ContactEmails, ICollection<string>? ContactPhones);
