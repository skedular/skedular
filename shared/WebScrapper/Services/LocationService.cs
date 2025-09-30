using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using CommandLine;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Admin_AddInput = Api.Shared.Services.Grpc.Skedular.Location.V1.Admin_AddInput;
using Admin_GetInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.Admin_GetInput;

namespace WebScrapper.Services;

[Verb("import", HelpText = "Import locations")]
// ReSharper disable once ClassNeverInstantiated.Global
public class ImportOptions
{
}

public interface ILocationService
{
    Task HandleAsync(ImportOptions options, CancellationToken cancellationToken);
}

public class LocationService(
    ICsvLocationFileReaderService csvLocationFileReaderService,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    LocationConfiguration locationConfiguration,
    Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient locationServiceClient,
    IRandomHelper randomHelper) : ILocationService
{
    public async Task HandleAsync(ImportOptions options, CancellationToken cancellationToken)
    {
        var rawLocations = csvLocationFileReaderService.ReadLocations();

        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { UniqueAlphanumericName = "skedularpubliclocations" },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var locations = (await locationServiceClient.Admin_GetPaginatedLocationsAsync(
            new Admin_GetPaginatedLocationsInput
            {
                First = ((int?)null).ToNullInt(),
                After = string.Empty,
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new LocationWhereInput { OrganizationId = organization.Id }
            },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken)).Edges.Select(item => item.Node).Where(item => item.ExtraMetadata is not null).ToList();

        var importedCount = 0;
        foreach (var rawLocation in rawLocations)
        {
            if (locations.Any(item => item.ExtraMetadata.OtherLinks.Contains(rawLocation.Url)))
            {
                continue;
            }

            var adminAddInput = new Admin_AddInput
            {
                Id = randomHelper.Generate(),
                Name = rawLocation.Title.ToSafeString(),
                OrganizationId = organization.Id,
                Type = LocationType.Marketplace,
                Timezone = "Pacific/Auckland",
                About = rawLocation.Description.ToSafeString(),
                ExtraMetadata = new ExtraMetadata
                {
                    Website = rawLocation.Websites.ToSafeString(),
                    ContactDetails = new ContactDetails(),
                    AreaRange = new AreaRange { FromInSqm = rawLocation.Area.ToSafeString(), ToInSqm = rawLocation.Area.ToSafeString() },
                    PeopleCapacity = new PeopleCapacity { From = rawLocation.People.ToSafeString(), To = rawLocation.People.ToSafeString() }
                }
            };

            adminAddInput.ExtraMetadata.ContactDetails.ContactEmails.AddRange(
                rawLocation.Emails.Split(Environment.NewLine)
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrWhiteSpace(item)));
            adminAddInput.ExtraMetadata.ContactDetails.ContactPeople.AddRange(
                rawLocation.ContactPerson.Split(Environment.NewLine)
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrWhiteSpace(item)));
            adminAddInput.ExtraMetadata.ContactDetails.ContactPhones.AddRange(
                rawLocation.ContactPhone.Split(Environment.NewLine)
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrWhiteSpace(item)));
            adminAddInput.ExtraMetadata.OtherLinks.Add(rawLocation.Url.ToSafeString());

            await locationServiceClient.Admin_AddAsync(
                adminAddInput,
                locationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);

            Console.WriteLine($"Imported location {++importedCount} - {rawLocation.Title}");

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}
