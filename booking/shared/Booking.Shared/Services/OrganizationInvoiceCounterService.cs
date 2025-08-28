using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IOrganizationInvoiceCounterService
{
    Task<string> GetNextInvoiceNumberIdAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationInvoiceCounterService(IRepositoryFactory repositoryFactory) : IOrganizationInvoiceCounterService
{
    public async Task<string> GetNextInvoiceNumberIdAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        var organizationInvoiceCounter =
            await repositoryFactory.OrganizationInvoiceCounterRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);

        if (organizationInvoiceCounter is null)
        {
            organizationInvoiceCounter = repositoryFactory.OrganizationInvoiceCounterRepository.Add(new OrganizationInvoiceCounter
            {
                InvoiceNumber = 1, Organization = organization
            });
        }
        else
        {
            organizationInvoiceCounter.InvoiceNumber++;
            repositoryFactory.OrganizationInvoiceCounterRepository.Update(organizationInvoiceCounter);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return $"SKD-{organizationInvoiceCounter.InvoiceNumber:D6}";
    }
}
