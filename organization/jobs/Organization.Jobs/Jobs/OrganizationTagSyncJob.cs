using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Jobs.Jobs;

public class OrganizationTagSyncJob(IServiceProvider serviceProvider, IRandomHelper randomHelper, ILogger<OrganizationTagSyncJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(OrganizationTagSyncJob).FullName!;

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
                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Car Park Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace,
                                Color = "#87CEEB",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeEventSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Event Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeEventSpace,
                                Color = "#FFD700",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Meeting Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace,
                                Color = "#FF6347",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Office Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace,
                                Color = "#32CD32",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Retail Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace,
                                Color = "#98FB98",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Storage Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace,
                                Color = "#B0E0E6",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Studio Space",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace,
                                Color = "#F5DEB3",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Commercial Kitchen",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen,
                                Color = "#20B2AA",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeShootLocation))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Shoot Location",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeShootLocation,
                                Color = "#4682B4",
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.LocationSpaceTypeOthers))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Others",
                                Type = OrganizationTagTypeConstants.LocationSpaceTypeOthers,
                                Color = "#DAA520",
                                Organization = organization
                            });
                    }
                }

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
            }
        } while (true);
    }
}
