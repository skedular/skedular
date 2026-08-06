using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Api.Shared.Services.Models;
using Customer.Shared.Models;
using Enterprise.Shared;
using Location = Customer.Shared.Models.Location;
using Organization = Customer.Shared.Models.Organization;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using PersonalInformationVisibility = Api.Shared.Services.Models.PersonalInformationVisibility;

namespace Customer.Api.Mappers;

public interface IGrpcMapper
{
    global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer MapToGrpcResponse(Shared.Models.Customer src);
    Shared.Models.Customer MapTo(Admin_AddInput src);
    Identity MapTo(Admin_AddIdentityInput src);
    Identity MapTo(Admin_UpdateIdentityInput src);
}

public class GrpcMapper : IGrpcMapper
{
    public global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer MapToGrpcResponse(Shared.Models.Customer src)
    {
        var customer = new global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer
        {
            Id = src.Id,
            Designation = src.Designation.ToSafeString(),
            Title = src.Title.ToSafeString(),
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Locale = src.Locale.ToSafeString(),
            PhoneNumber = src.PhoneNumber.ToSafeString(),
            IsOnboardingDone = src.IsOnboardingDone,
            DefaultOrganizationId = src.DefaultOrganization is null ? string.Empty : src.DefaultOrganization.Id.ToSafeString(),
            DisplayableName = src.DisplayableName.ToSafeString(),
            PersonalInformationVisibility = src.PersonalInformationVisibility switch
            {
                PersonalInformationVisibility.Visible => global::Api.Shared.Grpc.Skedular.Customer.Core.V1.PersonalInformationVisibility.Visible,
                PersonalInformationVisibility.Redacted =>
                    global::Api.Shared.Grpc.Skedular.Customer.Core.V1.PersonalInformationVisibility.Redacted,
                _ => throw new ArgumentOutOfRangeException(nameof(src.PersonalInformationVisibility), src.PersonalInformationVisibility,
                    $"Unexpected value for {nameof(src.PersonalInformationVisibility)}: {src.PersonalInformationVisibility}. Update enum mapping or caller input."),
            },
            Type = src.Type switch
            {
                CustomerType.Guest => global::Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Guest,
                CustomerType.Registered => global::Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Type), src.Type,
                    $"Unexpected value for {nameof(src.Type)}: {src.Type}. Update enum mapping or caller input."),
            },
        };

        customer.Identities.AddRange(src.Identities.Select(item =>
            new global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Identity
            {
                Id = item.Id,
                Email = item.Email.ToSafeString(),
                EmailVerified = item.EmailVerified ?? false,
            }));

        customer.PreferredLocationIds.AddRange(src.PreferredLocations.Select(item => item.Id));
        customer.PreferredResourceIds.AddRange(src.PreferredResources.Select(item => item.Id));
        customer.PreferredOrganizationTagIds.AddRange(src.PreferredOrganizationTags.Select(item => item.Id));
        customer.FavouriteLocationIds.AddRange(src.FavouriteLocations.Select(item => item.Id));

        return customer;
    }

    public Shared.Models.Customer MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Designation = src.Designation,
            Title = src.Title,
            Name = src.Name,
            GivenName = src.GivenName,
            MiddleName = src.MiddleName,
            FamilyName = src.FamilyName,
            PhotoUrl = src.PhotoUrl,
            PhotoUrl24 = src.PhotoUrl24,
            PhotoUrl32 = src.PhotoUrl32,
            PhotoUrl48 = src.PhotoUrl48,
            PhotoUrl72 = src.PhotoUrl72,
            PhotoUrl192 = src.PhotoUrl192,
            PhotoUrl512 = src.PhotoUrl512,
            Timezone = src.Timezone,
            Locale = src.Locale,
            PhoneNumber = src.PhoneNumber,
            Identities = src.Identities
                .Select(item => new Identity
                {
                    Id = item.Id,
                    Email = item.Email.ToSafeString(),
                    EmailVerified = item.EmailVerified,
                })
                .ToList(),
            IsOnboardingDone = src.IsOnboardingDone,
            DefaultOrganization = string.IsNullOrWhiteSpace(src.DefaultOrganizationId)
                ? null
                : new Organization
                {
                    Id = src.DefaultOrganizationId,
                },
            PreferredLocations = src.PreferredLocations.Select(item =>
                    new Location
                    {
                        Id = item.Id,
                        Organization = new Organization
                        {
                            Id = item.Organization.Id,
                        },
                    })
                .ToList(),
            PreferredResources = src.PreferredResources
                .Select(item => new Resource
                {
                    Id = item.Id,
                    Location = new Location
                    {
                        Id = item.Location.Id,
                    },
                })
                .ToList(),
            PreferredOrganizationTags = src.PreferredOrganizationTags
                .Select(item => new OrganizationTag
                {
                    Id = item.Id,
                    Organization = new Organization
                    {
                        Id = item.Organization.Id,
                    },
                })
                .ToList(),
            FavouriteLocations = [],
            PersonalInformationVisibility = src.PersonalInformationVisibility switch
            {
                global::Api.Shared.Grpc.Skedular.Customer.Core.V1.PersonalInformationVisibility.Visible => PersonalInformationVisibility.Visible,
                global::Api.Shared.Grpc.Skedular.Customer.Core.V1.PersonalInformationVisibility.Redacted =>
                    PersonalInformationVisibility.Redacted,
                _ => throw new ArgumentOutOfRangeException(nameof(src.PersonalInformationVisibility), src.PersonalInformationVisibility,
                    $"Unexpected value for {nameof(src.PersonalInformationVisibility)}: {src.PersonalInformationVisibility}. Update enum mapping or caller input."),
            },
            Type = src.Type switch
            {
                global::Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Guest => CustomerType.Guest,
                global::Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Registered => CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Type), src.Type,
                    $"Unexpected value for {nameof(src.Type)}: {src.Type}. Update enum mapping or caller input."),
            },
        };

    public Identity MapTo(Admin_AddIdentityInput src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified,
            Customer = new Shared.Models.Customer
            {
                Id = src.CustomerId,
            },
        };

    public Identity MapTo(Admin_UpdateIdentityInput src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified,
            Customer = new Shared.Models.Customer
            {
                Id = src.CustomerId,
            },
        };
}
