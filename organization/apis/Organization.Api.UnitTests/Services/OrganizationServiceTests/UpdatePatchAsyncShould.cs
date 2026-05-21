using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using Testing.Shared.Assertions;
using IndustrySubCategory = Organization.Shared.Database.Entities.IndustrySubCategory;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;

namespace Organization.Api.UnitTests.Services.OrganizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Name_And_Return_Latest_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Old name",
            Type = OrganizationTypeConstants.Private,
            ListingMetadata = new ListingMetadata("Old description", "Title", "Sub title", ["Wifi"])
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedOrganization =
            new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = "New name" };
        var stripeAuthorizeUrl = Constants.EmptyUri;
        var request = new OrganizationPatchRequest(
            organization.Id,
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.Name },
            "New name",
            "Ignored description");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, organization, A<IReadOnlyList<IndustrySubCategory>>._))
            .Invokes(() => organization.Name = "New name")
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => organizationRepository.Update(organization)).Returns(organization);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.Organization.ShouldBeSameAs(updatedOrganization);
        organization.Name.ShouldBe("New name");
        organization.ListingMetadata.ShouldNotBeNull();
        organization.ListingMetadata.About.ShouldBe("Old description");
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(customerIds => customerIds.SequenceEqual(new[] { customer.Id })),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Organization patch update started")
            .MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Organization patch update completed with applied changes")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Evict_Previous_Custom_Domain_And_All_Member_Organization_Lists_When_Custom_Domain_Changes(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "old-domain",
            Name = "Existing name",
            Type = OrganizationTypeConstants.Private,
            OrganizationMembers =
            [
                new OrganizationMember { CustomerId = "customer-1" },
                new OrganizationMember { CustomerId = "customer-2" }
            ]
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedOrganization =
            new Shared.Models.Organization { Id = organization.Id, CustomDomain = "new-domain", Name = organization.Name };
        var stripeAuthorizeUrl = Constants.EmptyUri;
        var request = new OrganizationPatchRequest(
            organization.Id,
            updatedOrganization.CustomDomain,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.CustomDomain },
            organization.Name,
            null);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, updatedOrganization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, organization, A<IReadOnlyList<IndustrySubCategory>>._))
            .Invokes(() => organization.CustomDomain = updatedOrganization.CustomDomain)
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => organizationRepository.Update(organization)).Returns(organization);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.Organization.ShouldBeSameAs(updatedOrganization);
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, "old-domain", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, "new-domain", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(customerIds => customerIds.SequenceEqual(new[] { "customer-1", "customer-2" })),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Description_And_Preserve_Listing_Metadata(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Existing name",
            Type = OrganizationTypeConstants.Private,
            ListingMetadata = new ListingMetadata("Old description", "Title", "Sub title", ["Wifi"])
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedOrganization =
            new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name };
        var stripeAuthorizeUrl = Constants.EmptyUri;
        var request = new OrganizationPatchRequest(
            organization.Id,
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.Description },
            "Ignored name",
            "New description");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, organization, A<IReadOnlyList<IndustrySubCategory>>._))
            .Invokes(() => organization.ListingMetadata = organization.ListingMetadata! with { About = "New description" })
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => organizationRepository.Update(organization)).Returns(organization);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        await sut.UpdatePatchAsync(request, cancellationToken);

        organization.Name.ShouldBe("Existing name");
        organization.ListingMetadata.ShouldNotBeNull();
        organization.ListingMetadata.About.ShouldBe("New description");
        organization.ListingMetadata.Title.ShouldBe("Title");
        organization.ListingMetadata.SubTitle.ShouldBe("Sub title");
        organization.ListingMetadata.IncludedFeatures.ShouldBe(["Wifi"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Add_Physical_Address_When_Selected_Organization_Has_None(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationPhysicalAddressRepository organizationPhysicalAddressRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1", CustomDomain = "acme", Name = "Existing name", Type = OrganizationTypeConstants.Private
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var physicalAddress = new OrganizationPhysicalAddress
        {
            AddressLine1 = "1 Example Road", City = "Auckland", Zipcode = "1010", Country = "New Zealand"
        };
        var physicalAddressEntity = new Shared.Database.Entities.OrganizationPhysicalAddress
        {
            Id = "physical-address-1",
            AddressLine1 = physicalAddress.AddressLine1,
            City = physicalAddress.City,
            Zipcode = physicalAddress.Zipcode,
            Country = physicalAddress.Country,
            Organization = organization
        };
        var updatedOrganization =
            new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name };
        var stripeAuthorizeUrl = Constants.EmptyUri;
        var request = new OrganizationPatchRequest(
            organization.Id,
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.PhysicalAddress },
            organization.Name,
            null,
            PhysicalAddress: physicalAddress);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationPhysicalAddressRepository).Returns(organizationPhysicalAddressRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => randomHelper.Generate()).Returns(physicalAddressEntity.Id);
        A.CallTo(() => graphQlMapper.MapTo(physicalAddress, organization)).Returns(physicalAddressEntity);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, organization, A<IReadOnlyList<IndustrySubCategory>>._)).Returns(false);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => organizationRepository.Update(organization)).Returns(organization);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.Organization.ShouldBeSameAs(updatedOrganization);
        physicalAddress.Id.ShouldBe(physicalAddressEntity.Id);
        organization.PhysicalAddress.ShouldBeSameAs(physicalAddressEntity);
        A.CallTo(() => organizationPhysicalAddressRepository.Add(physicalAddressEntity)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Latest_Organization_Without_Saving_When_Selected_Values_Are_Unchanged(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Existing name",
            Type = OrganizationTypeConstants.Private,
            ListingMetadata = new ListingMetadata("Existing description", "Title", "Sub title", [])
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var latestOrganization =
            new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name };
        var stripeAuthorizeUrl = Constants.EmptyUri;
        var request = new OrganizationPatchRequest(
            organization.Id,
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.Name, OrganizationPatchField.Description },
            organization.Name,
            organization.ListingMetadata.About);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, organization, A<IReadOnlyList<IndustrySubCategory>>._)).Returns(false);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(latestOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.Organization.ShouldBeSameAs(latestOrganization);
        A.CallTo(() => organizationRepository.Update(A<Shared.Database.Entities.Organization>._)).MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustNotHaveHappened();
        LogAssertions.ACallToLogInfoContaining(logger, "Organization patch update completed with no changes")
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Retry_Selected_Fields_Against_Latest_Organization_After_Concurrency_Conflict(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        using var dbContext = new OrganizationDbContext(
            new DbContextOptionsBuilder<OrganizationDbContext>()
                .UseNpgsql(
                    "Host=localhost;Database=local.test;Username=test;Password=test",
                    builder => builder.UseNetTopologySuite())
                .Options,
            new CustomDbContextOptions<OrganizationDbContext> { IsPostgisEnabled = true });
        var staleOrganization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Old name",
            Type = OrganizationTypeConstants.Private,
            ListingMetadata = new ListingMetadata("Old description", "Title", "Sub title", [])
        };
        var latestOrganization = new Shared.Database.Entities.Organization
        {
            Id = staleOrganization.Id,
            CustomDomain = staleOrganization.CustomDomain,
            Name = staleOrganization.Name,
            Type = OrganizationTypeConstants.Private,
            ListingMetadata = new ListingMetadata("Latest description", "Title", "Sub title", [])
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var stripeAuthorizeUrl = Constants.EmptyUri;
        var request = new OrganizationPatchRequest(
            staleOrganization.Id,
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.Name },
            "New name",
            null);
        var saveAttempt = 0;

        A.CallTo(() => repositoryFactory.DbContext).Returns(dbContext);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(staleOrganization.Id, null, cancellationToken))
            .ReturnsNextFromSequence(staleOrganization, latestOrganization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(A<Shared.Database.Entities.Organization>._, customer.Id, cancellationToken))
            .Returns(true);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, A<Shared.Database.Entities.Organization>._, A<IReadOnlyList<IndustrySubCategory>>._))
            .Invokes((OrganizationPatchRequest _, Shared.Database.Entities.Organization organization, IReadOnlyList<IndustrySubCategory> _) =>
                organization.Name = request.Name!)
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(staleOrganization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => organizationRepository.Update(A<Shared.Database.Entities.Organization>._))
            .ReturnsLazily((Shared.Database.Entities.Organization organization) => organization);
        A.CallTo(() => graphQlMapper.MapTo(A<Shared.Database.Entities.Organization>._, stripeAuthorizeUrl))
            .ReturnsLazily((Shared.Database.Entities.Organization organization, Uri _) =>
                new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name });
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken))
            .ReturnsLazily(() =>
            {
                if (saveAttempt++ == 0)
                {
                    throw new DbUpdateConcurrencyException();
                }

                return Task.FromResult(1);
            });

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.Organization.Name.ShouldBe(request.Name);
        latestOrganization.Name.ShouldBe(request.Name);
        latestOrganization.ListingMetadata.ShouldNotBeNull();
        latestOrganization.ListingMetadata.About.ShouldBe("Latest description");
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(staleOrganization.Id, null, cancellationToken))
            .MustHaveHappenedTwiceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedTwiceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(A<IEnumerable<Shared.Models.Organization>>._, unitOfWork))
            .MustHaveHappenedTwiceExactly();
        A.CallTo(() => cachedOrganizationService.UpdateByIdOrCustomDomainAsync(
                latestOrganization.Id,
                latestOrganization.CustomDomain,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Organization patch update hit a concurrency conflict"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Invalid_Field_Selection(
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField> { (OrganizationPatchField)999 },
            "Name",
            null);

        A.CallTo(() => organizationPatchMapper.Validate(request))
            .Throws(new ArgumentOutOfRangeException(nameof(request), request.FieldsToUpdate.Single(),
                "This organisation patch field is not supported."));

        await Should.ThrowAsync<ArgumentOutOfRangeException>(() => sut.UpdatePatchAsync(request, cancellationToken));
        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("field selection is not supported"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Blank_Selected_Name(
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        CancellationToken cancellationToken)
    {
        var request = new OrganizationPatchRequest(
            "org-1",
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.Name },
            " ",
            null);

        A.CallTo(() => organizationPatchMapper.Validate(request))
            .Throws(new ArgumentException("Organisation name is required.", nameof(request)));

        await Should.ThrowAsync<ArgumentException>(() => sut.UpdatePatchAsync(request, cancellationToken));
        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("validation failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        string organizationId,
        string customDomain,
        string organizationName,
        string customerId,
        string updatedName,
        CancellationToken cancellationToken)
    {
        var organization = OrganizationPatchTestHelpers.CreateOrganization(organizationId, customDomain, organizationName);
        var customer = OrganizationPatchTestHelpers.CreateCustomer(customerId);
        var request = OrganizationPatchTestHelpers.CreateNameRequest(organization, updatedName);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken))
            .Returns((customer, new Shared.Database.Entities.Customer { Id = customer.Id }));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdatePatchAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("customer is not authorized"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Persistence_Failure(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationPatchMapper organizationPatchMapper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<OrganizationService> logger,
        OrganizationService sut,
        string organizationId,
        string customDomain,
        string organizationName,
        string customerId,
        string updatedName,
        CancellationToken cancellationToken)
    {
        var organization = OrganizationPatchTestHelpers.CreateOrganization(organizationId, customDomain, organizationName);
        var customer = OrganizationPatchTestHelpers.CreateCustomer(customerId);
        var request = OrganizationPatchTestHelpers.CreateNameRequest(organization, updatedName);
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken))
            .Returns((customer, new Shared.Database.Entities.Customer { Id = customer.Id }));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => organizationPatchMapper.ApplyTo(request, organization, A<IReadOnlyList<IndustrySubCategory>>._))
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => organizationRepository.Update(organization)).Returns(organization);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl))
            .Returns(new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name });
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).ThrowsAsync(new InvalidOperationException("save failed"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdatePatchAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("failed during persistence"))
            .MustHaveHappenedOnceExactly();
    }
}
