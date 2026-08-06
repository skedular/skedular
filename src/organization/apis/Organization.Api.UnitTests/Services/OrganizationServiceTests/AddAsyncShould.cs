using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Repositories;
using IndustrySubCategory = Organization.Shared.Database.Entities.IndustrySubCategory;

namespace Organization.Api.UnitTests.Services.OrganizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Host_Standard_Offering_For_New_Host_Organization(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        OrganizationService sut,
        string organizationId,
        string customDomain,
        string organizationName,
        string termsOfUseId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Models.Organization
        {
            Id = organizationId,
            CustomDomain = customDomain,
            Name = organizationName,
            Type = OrganizationType.Host,
            AgreedToTermsOfUse = true,
            TermsOfUse = new TermsOfUse
            {
                Id = termsOfUseId,
            },
            IndustrySubCategories = [],
        };
        var termsOfUse = new Shared.Database.Entities.TermsOfUse
        {
            Id = termsOfUseId,
        };
        var organizationEntity = new Shared.Database.Entities.Organization
        {
            Id = organizationId,
            CustomDomain = customDomain,
            Name = organizationName,
            Type = OrganizationTypeConstants.Host,
        };
        Shared.Database.Entities.Organization? addedOrganization = null;

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(organizationId, customDomain, cancellationToken))
            .Returns(Task.FromResult<Shared.Database.Entities.Organization?>(null));
        A.CallTo(() => repositoryFactory.TermsOfUseRepository.GetActiveAsync(cancellationToken)).Returns(termsOfUse);
        A.CallTo(() => repositoryFactory.IndustrySubCategoryRepository.GetByIdsWithMainCategoryAsync(A<IReadOnlyList<string>>._, cancellationToken))
            .Returns([]);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("offering-host", "tag-product", "tag-resource");
        A.CallTo(() => graphQlMapper.MapTo(organization, termsOfUse, A<IReadOnlyList<IndustrySubCategory>>._)).Returns(organizationEntity);
        A.CallTo(() => repositoryFactory.OrganizationRepository.Add(A<Shared.Database.Entities.Organization>._))
            .Invokes(call => addedOrganization = call.GetArgument<Shared.Database.Entities.Organization>(0))
            .ReturnsLazily(call => call.GetArgument<Shared.Database.Entities.Organization>(0)!);
        A.CallTo(() => graphQlMapper.MapTo(A<Shared.Database.Entities.Organization>._, A<Uri>._)).Returns(organization);

        await sut.AddAsync(organization, null, true, cancellationToken);

        var offering = addedOrganization.ShouldNotBeNull().OrganizationOfferings.Single();
        offering.Code.ShouldBe(OfferingCode.HostStandardV1);
        offering.CatalogVersion.ShouldBe(PricingCatalogConstants.CurrentHostCatalogVersion);
        offering.HostCommissionPercentage.ShouldBe(5m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Spaces_Free_Offering_For_New_Marketplace_Organization(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        TimeProvider timeProvider,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        OrganizationService sut,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var termsOfUseId = "terms-1";
        var organization = new Shared.Models.Organization
        {
            Id = "org-1",
            CustomDomain = "marketplace-org",
            Name = "Marketplace Org",
            Type = OrganizationType.Marketplace,
            AgreedToTermsOfUse = true,
            TermsOfUse = new TermsOfUse
            {
                Id = termsOfUseId,
            },
            IndustrySubCategories = [],
        };
        var termsOfUse = new Shared.Database.Entities.TermsOfUse
        {
            Id = termsOfUseId,
        };
        var organizationEntity = new Shared.Database.Entities.Organization
        {
            Id = organization.Id,
            CustomDomain = organization.CustomDomain,
            Name = organization.Name,
            Type = OrganizationTypeConstants.Marketplace,
        };
        Shared.Database.Entities.Organization? addedOrganization = null;

        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organization.Id,
                organization.CustomDomain,
                cancellationToken))
            .Returns(Task.FromResult<Shared.Database.Entities.Organization?>(null));
        A.CallTo(() => repositoryFactory.TermsOfUseRepository.GetActiveAsync(cancellationToken)).Returns(termsOfUse);
        A.CallTo(() => repositoryFactory.IndustrySubCategoryRepository.GetByIdsWithMainCategoryAsync(
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("offering-1", "tag-1", "tag-2");
        A.CallTo(() => graphQlMapper.MapTo(
                organization,
                termsOfUse,
                A<IReadOnlyList<IndustrySubCategory>>._))
            .Returns(organizationEntity);
        A.CallTo(() => repositoryFactory.OrganizationRepository.Add(A<Shared.Database.Entities.Organization>._))
            .Invokes(call => addedOrganization = call.GetArgument<Shared.Database.Entities.Organization>(0))
            .ReturnsLazily(call => call.GetArgument<Shared.Database.Entities.Organization>(0)!);
        A.CallTo(() => graphQlMapper.MapTo(A<Shared.Database.Entities.Organization>._, A<Uri>._))
            .Returns(organization);

        await sut.AddAsync(organization, null, true, cancellationToken);

        addedOrganization.ShouldNotBeNull();
        var offering = addedOrganization.OrganizationOfferings.Single();
        offering.Code.ShouldBe(OfferingCode.SpacesFreeTierV1);
        addedOrganization.SpacesTrialStartedAt.ShouldBe(now);
    }
}
