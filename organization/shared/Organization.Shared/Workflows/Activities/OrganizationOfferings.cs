using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Temporalio.Activities;

namespace Organization.Shared.Workflows.Activities;

public record RenewOrganizationOfferingInput(string OrganizationId);

public class OrganizationOfferings(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IDbTransactionBuilder transactionBuilder,
    IRandomHelper randomHelper,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher)
{
    [Activity]
    public async Task RenewOrganizationOfferingAsync(RenewOrganizationOfferingInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(args.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var expiredOfferingsRequireAutoRenew = await repositoryFactory.OrganizationOfferingRepository.Query(new Specification<OrganizationOffering>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.Organization.Id == args.OrganizationId && query.End <= now && query.AutoRenew
            }.ApplyOrderByDescending(query => query.End))
            .ToListAsync(cancellationToken);

        if (expiredOfferingsRequireAutoRenew.Count == 0)
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var expiredOfferingRequireAutoRenew = expiredOfferingsRequireAutoRenew.First();
        var offering = expiredOfferingRequireAutoRenew.Code.GetOffering();
        var start = expiredOfferingRequireAutoRenew.End.GetNextOfferingPeriodStart();
        var organizationOffering = new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            Code = expiredOfferingRequireAutoRenew.Code,
            Start = start,
            End = start.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
            AutoRenew = expiredOfferingRequireAutoRenew.AutoRenew,
            UnitPrice = offering.UnitPrice,
            Organization = organization
        };
        repositoryFactory.OrganizationOfferingRepository.Add(organizationOffering);
        repositoryFactory.OrganizationOfferingRepository.RemoveRange(expiredOfferingsRequireAutoRenew);

        var mappedOrganization = mapper.MapTo(organization);
        mappedOrganization.OrganizationOfferings =
        [
            mappedOrganization.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue).OrderByDescending(item => item.End).First()
        ];

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);
        organizationOutboxPublisher.ExecuteWorkflowScheduleRenewOrganizationOffering(
            new ScheduleRenewOrganizationOfferingInput(
                organization.Id,
                organizationOffering.Id,
                organizationOffering.End.GetNextOfferingPeriodStart()),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
