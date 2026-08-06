using Api.Shared.Services.Models;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Shared.Database.Entities;

namespace Organization.Api.UnitTests.Mappers.OrganizationPatchMapperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ApplyToShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Update_Only_Selected_Name(OrganizationPatchMapper sut)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Name = "Old name",
            Type = OrganizationTypeConstants.Private,
        };
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                OrganizationPatchField.Name,
            },
            "New name");

        var changed = sut.ApplyTo(request, organization, []);

        changed.ShouldBeTrue();
        organization.Name.ShouldBe("New name");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_False_When_Selected_Values_Are_Unchanged(OrganizationPatchMapper sut)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Name = "Existing name",
            Type = OrganizationTypeConstants.Private,
        };
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                OrganizationPatchField.Name,
            },
            organization.Name);

        var changed = sut.ApplyTo(request, organization, []);

        changed.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Update_All_Selected_Organization_Setup_Fields(OrganizationPatchMapper sut)
    {
        var originalIndustrySubCategory = new IndustrySubCategory
        {
            Id = "industry-old",
            Name = "Old",
        };
        var nextIndustrySubCategory = new IndustrySubCategory
        {
            Id = "industry-new",
            Name = "New",
        };
        var organization = new Shared.Database.Entities.Organization
        {
            CustomDomain = "old-domain",
            Name = "Old name",
            Type = OrganizationTypeConstants.Private,
            MarketplaceListingMetadata =
                new ListingMetadata("Old marketplace", "Old marketplace title", "Old marketplace sub title", ["Old marketplace feature"]),
            Website = "https://old.example.com",
            LogoUrl = "https://old.example.com/logo.png",
            CustomerFacingTermsAndConditionsUrl = "https://old.example.com/terms",
            BillingCycle = OrganizationBillingCycleConstants.Weekly,
            InvoiceDueInDays = 7,
            ContactEmail = "old@example.com",
            ContactPhone = "123",
            RefundNotificationEmails = ["refund-old@example.com"],
            IndustrySubCategories = [originalIndustrySubCategory],
            FeatureImages = [new CdnImageFile(new CdnFile("https://old.example.com/image.png", 1, 2), null)],
            PhysicalAddress = new OrganizationPhysicalAddress
            {
                AddressLine1 = "Old address line",
                City = "Old city",
                Zipcode = "1111",
                Country = "Old country",
            },
        };
        var nextFeatureImage = new CdnImageFile(new CdnFile("https://new.example.com/image.png", 3, 4), null);
        var nextMarketplaceListingMetadata = new ListingMetadata("Marketplace", "Marketplace title", "Marketplace sub title", ["Feature"]);
        var nextPhysicalAddress = new Shared.Models.OrganizationPhysicalAddress
        {
            AddressLine1 = "New address line",
            City = "New city",
            Zipcode = "2222",
            Country = "New country",
        };
        var request = new OrganizationPatchRequest(
            "org-1",
            "New-Domain",
            Enum.GetValues<OrganizationPatchField>().ToHashSet(),
            "New name",
            "https://new.example.com",
            "https://new.example.com/logo.png",
            "https://new.example.com/terms",
            OrganizationBillingCycle.Monthly,
            14,
            "new@example.com",
            "456",
            ["refund-new@example.com"],
            [nextIndustrySubCategory.Id],
            [nextFeatureImage],
            nextMarketplaceListingMetadata,
            nextPhysicalAddress);

        var changed = sut.ApplyTo(request, organization, [nextIndustrySubCategory]);

        changed.ShouldBeTrue();
        organization.CustomDomain.ShouldBe("new-domain");
        organization.Name.ShouldBe("New name");
        organization.MarketplaceListingMetadata.ShouldBe(nextMarketplaceListingMetadata);
        organization.Website.ShouldBe("https://new.example.com");
        organization.LogoUrl.ShouldBe("https://new.example.com/logo.png");
        organization.CustomerFacingTermsAndConditionsUrl.ShouldBe("https://new.example.com/terms");
        organization.BillingCycle.ShouldBe(OrganizationBillingCycleConstants.Monthly);
        organization.InvoiceDueInDays.ShouldBe(14);
        organization.ContactEmail.ShouldBe("new@example.com");
        organization.ContactPhone.ShouldBe("456");
        organization.RefundNotificationEmails.ShouldBe(["refund-new@example.com"]);
        organization.IndustrySubCategories.ShouldBe([nextIndustrySubCategory]);
        organization.FeatureImages.ShouldBe([nextFeatureImage]);
        organization.PhysicalAddress.ShouldNotBeNull();
        organization.PhysicalAddress.AddressLine1.ShouldBe(nextPhysicalAddress.AddressLine1);
        organization.PhysicalAddress.City.ShouldBe(nextPhysicalAddress.City);
        organization.PhysicalAddress.Zipcode.ShouldBe(nextPhysicalAddress.Zipcode);
        organization.PhysicalAddress.Country.ShouldBe(nextPhysicalAddress.Country);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Update_Selected_Physical_Address(OrganizationPatchMapper sut)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Name = "Existing name",
            Type = OrganizationTypeConstants.Private,
            PhysicalAddress = new OrganizationPhysicalAddress
            {
                AddressLine1 = "Old address line",
                City = "Old city",
                Zipcode = "1111",
                Country = "Old country",
            },
        };
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField>
            {
                OrganizationPatchField.PhysicalAddress,
            },
            organization.Name,
            PhysicalAddress: new Shared.Models.OrganizationPhysicalAddress
            {
                AddressLine1 = "New address line",
                City = "New city",
                Zipcode = "2222",
                Country = "New country",
            });

        var changed = sut.ApplyTo(request, organization, []);

        changed.ShouldBeTrue();
        organization.PhysicalAddress.AddressLine1.ShouldBe("New address line");
        organization.PhysicalAddress.City.ShouldBe("New city");
        organization.PhysicalAddress.Zipcode.ShouldBe("2222");
        organization.PhysicalAddress.Country.ShouldBe("New country");
    }
}
