using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing organization invoice number generation.
/// </summary>
public interface IOrganizationInvoiceCounterService
{
    /// <summary>
    ///     Generates the next invoice number ID for the specified organization.
    ///     The format is "SKD-{6-digit number}".
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The next invoice number ID.</returns>
    /// <exception cref="OrganizationNotFound">Thrown when the organization is not found.</exception>
    Task<string> GetNextInvoiceNumberIdAsync(string organizationId, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the organization invoice counter service.
/// </summary>
public class OrganizationInvoiceCounterService(IRepositoryFactory repositoryFactory) : IOrganizationInvoiceCounterService
{
    /// <summary>
    ///     Generates the next invoice number ID for the specified organization.
    ///     Increments the counter and formats as "SKD-{6-digit number}".
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The next invoice number ID.</returns>
    /// <exception cref="OrganizationNotFound">Thrown when the organization is not found.</exception>
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
