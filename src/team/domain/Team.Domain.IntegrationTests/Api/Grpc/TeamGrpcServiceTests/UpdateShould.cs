using Api.Shared.Grpc.Skedular.Team.Core.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Grpc;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;
using Offering = Api.Shared.Services.Models.Offering;
using TeamGrpcConfig = Api.Shared.Services.Configurations.Grpc.TeamConfiguration;
using OrganizationMemberEntity = Team.Shared.Database.Entities.OrganizationMember;

namespace Team.Domain.IntegrationTests.Api.Grpc.TeamGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Team.Api")]
public class UpdateShould(
    TeamService.TeamServiceClient teamServiceClient,
    IRepositoryFactory repositoryFactory,
    TeamGrpcConfig teamConfiguration,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details(
        string teamId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalName,
        string updatedName,
        string originalAbout,
        string originalTimezone,
        CancellationToken cancellationToken)
    {
        await SeedTeamAsync(
            teamId, organizationId, customerId, identityId, memberId, originalName, originalAbout, originalTimezone, cancellationToken);

        var result = await teamServiceClient.UpdateAsync(
            new UpdateInput
            {
                Id = teamId,
                OrganizationId = organizationId,
                Name = updatedName,
                FieldsToUpdate =
                {
                    TeamPatchField.Name,
                },
            },
            teamConfiguration.ApiKey.CreateMetadata(identityId),
            cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(updatedName);

        var team = await repositoryFactory.TeamRepository.GetByIdUntrackedAsync(teamId, cancellationToken);
        team.ShouldNotBeNull();
        team.Name.ShouldBe(updatedName);
        team.About.ShouldBe(originalAbout);
        team.Timezone.ShouldBe(originalTimezone);
    }

    private async Task SeedTeamAsync(
        string teamId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string name,
        string about,
        string timezone,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Offering = new Offering
        {
            Id = organizationId,
            Code = OfferingCode.EnterpriseCustomV1,
            Start = now.AddDays(-1),
            End = now.AddDays(1),
        };

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);

        repositoryFactory.IdentityRepository.Add(new Identity
        {
            Id = identityId,
            Customer = customer,
        });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMemberEntity
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active,
        });
        repositoryFactory.TeamRepository.Add(new Shared.Database.Entities.Team
        {
            Id = teamId,
            Organization = organization,
            Name = name,
            About = about,
            Timezone = timezone,
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
