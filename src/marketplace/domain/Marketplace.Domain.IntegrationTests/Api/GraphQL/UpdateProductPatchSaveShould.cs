using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Marketplace.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Currency = Marketplace.Domain.IntegrationTests.Skedular.GraphQL.V1.Currency;
using OfferingModel = Api.Shared.Services.Models.Offering;

namespace Marketplace.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Marketplace.Api")]
public class UpdateProductPatchSaveShould(
    IUpdateProductPatchSaveMutation updateProductPatchSaveMutation,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details_For_Single_And_Grouped_Saves(
        string productId,
        string productVersionId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalTitle,
        string updatedTitle,
        string updatedSubTitle,
        CancellationToken cancellationToken)
    {
        await SeedOwnedProductAsync(
            productId,
            productVersionId,
            organizationId,
            customerId,
            identityId,
            memberId,
            originalTitle,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var titleResult = await updateProductPatchSaveMutation.ExecuteAsync(
                productId,
                [ProductPatchField.ListingMetadata],
                new ListingMetadataInput
                {
                    Title = updatedTitle,
                },
                cancellationToken);

            titleResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            titleResult.Data.ShouldNotBeNull();
            titleResult.Data.UpdateProduct.Product.ListingMetadata.Title.ShouldBe(updatedTitle);
            titleResult.Data.UpdateProduct.Product.Currency.Type.ShouldBe(
                Currency.Nzd);

            var groupedResult = await updateProductPatchSaveMutation.ExecuteAsync(
                productId,
                [ProductPatchField.ListingMetadata],
                new ListingMetadataInput
                {
                    Title = updatedTitle,
                    SubTitle = updatedSubTitle,
                },
                cancellationToken);

            groupedResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            groupedResult.Data.ShouldNotBeNull();
            groupedResult.Data.UpdateProduct.Product.ListingMetadata.Title.ShouldBe(updatedTitle);
            groupedResult.Data.UpdateProduct.Product.ListingMetadata.SubTitle.ShouldBe(updatedSubTitle);

            var product = await repositoryFactory.ProductRepository.GetByIdUntrackedAsync(productId, cancellationToken);
            product.ShouldNotBeNull();
            var version = product.ProductVersions.ShouldHaveSingleItem();
            version.Currency.ShouldBe(CurrencyConstants.Nzd);
            version.ListingMetadata.ShouldNotBeNull();
            version.ListingMetadata.Title.ShouldBe(updatedTitle);
            version.ListingMetadata.SubTitle.ShouldBe(updatedSubTitle);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    private async Task SeedOwnedProductAsync(
        string productId,
        string productVersionId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalTitle,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Type = OrganizationTypeConstants.Marketplace;
        var trialStartedAt = timeProvider.GetUtcNow();
        organization.Offering = new OfferingModel
        {
            Code = OfferingCode.SpacesFreeTierV1,
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = trialStartedAt,
            SpacesTrialEndsAt = trialStartedAt.AddDays(14),
        };
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);
        var product = repositoryFactory.ProductRepository.Add(new Product
        {
            Id = productId,
            Organization = organization,
        });

        repositoryFactory.IdentityRepository.Add(new Identity
        {
            Id = identityId,
            Customer = customer,
        });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active,
        });
        repositoryFactory.ProductVersionRepository.Add(new ProductVersion
        {
            Id = productVersionId,
            Product = product,
            Type = ProductTypeConstants.Resource,
            Currency = CurrencyConstants.Nzd,
            ListingMetadata = new ListingMetadata(null, originalTitle, null, []),
            PricingOptions =
            [
                new ProductPricing(
                    productVersionId,
                    0,
                    ListingMetadata.Empty,
                    ProductPricingCadence.PerHour,
                    ProductPricingCadence.PerHour,
                    10m,
                    true,
                    false,
                    [PaymentMethod.Card],
                    ProductPricingBillingMode.Upfront,
                    null,
                    null,
                    30,
                    30,
                    1,
                    ProductPricingCancellationPolicyType.NoCancellation,
                    []),
            ],
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
