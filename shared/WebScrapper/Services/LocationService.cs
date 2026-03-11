using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using CommandLine;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Flurl.Http;
using WebScrapper.Models;
using Admin_AddInput = Api.Shared.Services.Grpc.Skedular.Location.V1.Admin_AddInput;
using Admin_GetInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.Admin_GetInput;
using AreaRange = Api.Shared.Services.Grpc.Skedular.Location.V1.AreaRange;
using Constants = Api.Shared.Services.Constants;
using ContactDetails = Api.Shared.Services.Grpc.Skedular.Location.V1.ContactDetails;
using ListingMetadata = Api.Shared.Services.Grpc.Skedular.Location.V1.ListingMetadata;
using Location = WebScrapper.Models.Location;
using LocationType = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType;
using PeopleCapacity = Api.Shared.Services.Grpc.Skedular.Location.V1.PeopleCapacity;
using PhysicalAddress = Api.Shared.Services.Grpc.Skedular.Location.V1.PhysicalAddress;

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
            new Admin_GetInput { UniqueAlphanumericName = Constants.SkedularPublicLocationsUniqueAlphanumericName },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var tagConnection = await organizationServiceClient.Admin_GetPaginatedTagsAsync(
            new Admin_GetPaginatedTagsInput
            {
                First = ((int?)null).ToNullInt(),
                After = string.Empty,
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new TagWhereInput { OrganizationId = organization.Id }
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var tags = tagConnection.Edges.Select(item => item.Node).ToList();
        var carParkSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace);
        var eventSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeEventSpace);
        var meetingSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace);
        var officeSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace);
        var retailSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace);
        var storageSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace);
        var studioSpace = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace);
        var commercialKitchen = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen);
        var shootLocation = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeShootLocation);
        var other = tags.Single(item => item.TagType == OrganizationTagTypeConstants.LocationSpaceTypeOthers);

        var locations = (await locationServiceClient.Admin_GetPaginatedLocationsAsync(
            new Admin_GetPaginatedLocationsInput
            {
                First = ((int?)null).ToNullInt(),
                After = string.Empty,
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new LocationWhereInput { OrganizationUniqueAlphanumericName = organization.UniqueAlphanumericName }
            },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken)).Edges.Select(item => item.Node).Where(item => item.ExtraMetadata is not null).ToList();

        var importedCount = 0;
        foreach (var rawLocation in rawLocations)
        {
            var tagId = rawLocation.Type switch
            {
                "carpark-space" => carParkSpace.Id,
                "event-space" => eventSpace.Id,
                "meeting-space" => meetingSpace.Id,
                "office-space" => officeSpace.Id,
                "retail-space" => retailSpace.Id,
                "storage-space" => storageSpace.Id,
                "studio-space" => studioSpace.Id,
                "commercial-kitchen" => commercialKitchen.Id,
                "shoot-location" => shootLocation.Id,
                _ => other.Id
            };

            var matchingLocation = locations.FirstOrDefault(item => item.ExtraMetadata.OtherLinks.Contains(rawLocation.Url));
            if (matchingLocation is null)
            {
                var name = await GetSuggestedNameAsync(rawLocation, cancellationToken);
                var description = await GetSuggestedDescriptionAsync(rawLocation, cancellationToken);

                var adminAddInput = new Admin_AddInput
                {
                    Id = randomHelper.Generate(),
                    Name = name.ToSafeString(),
                    OrganizationId = organization.Id,
                    Type = LocationType.Marketplace,
                    Timezone = "Pacific/Auckland",
                    ListingMetadata = new ListingMetadata { About = description.ToSafeString(), Title = string.Empty, SubTitle = string.Empty },
                    ExtraMetadata = new ExtraMetadata
                    {
                        Website = rawLocation.Websites.ToSafeString(),
                        ContactDetails = new ContactDetails(),
                        AreaRange = new AreaRange { FromInSqm = rawLocation.Area.ToSafeString(), ToInSqm = rawLocation.Area.ToSafeString() },
                        PeopleCapacity =
                            new PeopleCapacity { From = rawLocation.People.ToSafeString(), To = rawLocation.People.ToSafeString() }
                    },
                    PhysicalAddress = new PhysicalAddress { AddressLine1 = rawLocation.Address.ToSafeString() },
                    UniqueClaimCode = randomHelper.GenerateAlphanumericNumeric(10).ToUpperInvariant()
                };

                adminAddInput.TagIds.Add(tagId);

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

                Console.WriteLine($"Added location {++importedCount} - {rawLocation.Title}");
            }
            else
            {
                if (matchingLocation.Name != rawLocation.Title || matchingLocation.ListingMetadata.About != rawLocation.Description)
                {
                    continue;
                }

                var name = await GetSuggestedNameAsync(rawLocation, cancellationToken);
                var description = await GetSuggestedDescriptionAsync(rawLocation, cancellationToken);

                var adminUpdateInput = new Admin_UpdateInput
                {
                    Id = matchingLocation.Id,
                    Name = name.ToSafeString(),
                    OrganizationId = organization.Id,
                    Type = LocationType.Marketplace,
                    Timezone = "Pacific/Auckland",
                    ListingMetadata = new ListingMetadata { About = description.ToSafeString(), Title = string.Empty, SubTitle = string.Empty },
                    ExtraMetadata = new ExtraMetadata
                    {
                        Website = rawLocation.Websites.ToSafeString(),
                        ContactDetails = new ContactDetails(),
                        AreaRange = new AreaRange { FromInSqm = rawLocation.Area.ToSafeString(), ToInSqm = rawLocation.Area.ToSafeString() },
                        PeopleCapacity =
                            new PeopleCapacity { From = rawLocation.People.ToSafeString(), To = rawLocation.People.ToSafeString() }
                    },
                    PhysicalAddress = new PhysicalAddress { AddressLine1 = rawLocation.Address.ToSafeString() },
                    UniqueClaimCode = matchingLocation.UniqueClaimCode.ToSafeString()
                };

                adminUpdateInput.TagIds.Add(tagId);

                adminUpdateInput.ExtraMetadata.ContactDetails.ContactEmails.AddRange(
                    rawLocation.Emails.Split(Environment.NewLine)
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item)));
                adminUpdateInput.ExtraMetadata.ContactDetails.ContactPeople.AddRange(
                    rawLocation.ContactPerson.Split(Environment.NewLine)
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item)));
                adminUpdateInput.ExtraMetadata.ContactDetails.ContactPhones.AddRange(
                    rawLocation.ContactPhone.Split(Environment.NewLine)
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item)));
                adminUpdateInput.ExtraMetadata.OtherLinks.Add(rawLocation.Url.ToSafeString());

                await locationServiceClient.Admin_UpdateAsync(
                    adminUpdateInput,
                    locationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                Console.WriteLine($"Updated location {++importedCount} - {rawLocation.Title}");
            }
        }
    }

    private static async Task<string> GetSuggestedNameAsync(Location rawLocation, CancellationToken cancellationToken)
    {
        var response = await "http://localhost:11434/api/generate"
            .PostJsonAsync(
                new
                {
                    model = "llama3:70b-instruct-q4_K_M",
                    prompt = $@"
                        Come up with a name for this location and make sure it is not trackable to its original content. You sentence response should contains maximum 7 words in total max.
                        In your response, only include the new description. Something I can straight away copy into my description field. I mean do not include anything else like her eis the rewritten version of what yo asked,
                        only give me the new description.

                        ---------------------
                        {rawLocation.Title}
                        



                        ---------------------
                        {rawLocation.Subtitle}
                        


                        ---------------------
                        {rawLocation.Description}
                        ",
                    stream = false
                },
                cancellationToken: cancellationToken)
            .ReceiveJson<OllamaResponse>();

        return response.Response;
    }

    private static async Task<string> GetSuggestedDescriptionAsync(Location rawLocation, CancellationToken cancellationToken)
    {
        var response = await "http://localhost:11434/api/generate"
            .PostJsonAsync(
                new
                {
                    model = "llama3:70b-instruct-q4_K_M",
                    prompt = $@"
                        Rewrite the location description to be more human readable. Make sure it is not trackable to its original content.
                        It does not have to be long. Also anything related to their cost needs to be removed, things like this cost this much for this number of hours or day. 
                        I am including its current title, subtitle and description separated by dashes here for your information.
                        In your response, only include the new description. Something I can straight away copy into my description field. I mean do not include anything else like her eis the rewritten version of what yo asked,
                        only give me the new description.

                        ---------------------
                        {rawLocation.Title}
                        



                        ---------------------
                        {rawLocation.Subtitle}
                        


                        ---------------------
                        {rawLocation.Description}
                        ",
                    stream = false
                },
                cancellationToken: cancellationToken)
            .ReceiveJson<OllamaResponse>();

        return response.Response;
    }
}
