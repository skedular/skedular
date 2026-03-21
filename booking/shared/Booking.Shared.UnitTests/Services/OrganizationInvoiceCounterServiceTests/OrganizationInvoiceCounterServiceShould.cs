using Api.Shared.Services;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using FakeItEasy;

namespace Booking.Shared.UnitTests.Services.OrganizationInvoiceCounterServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationInvoiceCounterServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_OrganizationNotFound_When_Organization_Does_Not_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        OrganizationInvoiceCounterService sut,
        IOrganizationRepository organizationRepository,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken))
            .Returns(Task.FromResult<Organization?>(null));

        await Should.ThrowAsync<OrganizationNotFound>(async () =>
            await sut.GetNextInvoiceNumberIdAsync("org-1", cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_New_Counter_And_Return_First_Invoice_Number_When_No_Counter_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        OrganizationInvoiceCounterService sut,
        IOrganizationRepository organizationRepository,
        IOrganizationInvoiceCounterRepository organizationInvoiceCounterRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1", Name = "Test Org" };
        var newCounter = new OrganizationInvoiceCounter { InvoiceNumber = 1, Organization = organization };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationInvoiceCounterRepository).Returns(organizationInvoiceCounterRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationInvoiceCounterRepository.GetByOrganizationIdAsync("org-1", cancellationToken))
            .Returns(Task.FromResult<OrganizationInvoiceCounter?>(null));
        A.CallTo(() => organizationInvoiceCounterRepository.Add(A<OrganizationInvoiceCounter>._))
            .Returns(newCounter);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken))
            .Returns(1);

        var result = await sut.GetNextInvoiceNumberIdAsync("org-1", cancellationToken);

        result.ShouldBe("SKD-000001");
        A.CallTo(() => organizationInvoiceCounterRepository.Add(A<OrganizationInvoiceCounter>.That.Matches(c =>
            c.InvoiceNumber == 1 && c.Organization == organization))).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Increment_Existing_Counter_And_Return_Next_Invoice_Number(
        [Frozen] IRepositoryFactory repositoryFactory,
        OrganizationInvoiceCounterService sut,
        IOrganizationRepository organizationRepository,
        IOrganizationInvoiceCounterRepository organizationInvoiceCounterRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1", Name = "Test Org" };
        var existingCounter = new OrganizationInvoiceCounter { InvoiceNumber = 5, Organization = organization };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationInvoiceCounterRepository).Returns(organizationInvoiceCounterRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationInvoiceCounterRepository.GetByOrganizationIdAsync("org-1", cancellationToken))
            .Returns(existingCounter);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken))
            .Returns(1);

        var result = await sut.GetNextInvoiceNumberIdAsync("org-1", cancellationToken);

        result.ShouldBe("SKD-000006");
        existingCounter.InvoiceNumber.ShouldBe(6);
        A.CallTo(() => organizationInvoiceCounterRepository.Update(existingCounter)).MustHaveHappenedOnceExactly();
    }
}
