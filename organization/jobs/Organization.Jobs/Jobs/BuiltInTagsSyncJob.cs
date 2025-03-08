using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Jobs.Jobs;

public class BuiltInTagsSyncJob(IServiceProvider serviceProvider, ILogger<BuiltInTagsSyncJob> logger, IRandomHelper randomHelper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.Desk))
                    {
                        repositoryFactory.TagRepository.Add(new Tag
                        {
                            Id = randomHelper.Generate(),
                            Name = "Desk",
                            Type = OrganizationTagTypeConstants.Desk,
                            Color = null,
                            Organization = organization
                        });

                        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.Room))
                    {
                        repositoryFactory.TagRepository.Add(new Tag
                        {
                            Id = randomHelper.Generate(),
                            Name = "Room",
                            Type = OrganizationTagTypeConstants.Room,
                            Color = null,
                            Organization = organization
                        });

                        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(BuiltInTagsSyncJob));
            }
        } while (true);
    }
}
