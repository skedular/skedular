using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Team.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;
using Offering = Api.Shared.Services.Models.Offering;

namespace Team.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Team.Api")]
public class UpdateTeamPatchSaveShould(
    IUpdateTeamPatchSaveMutation updateTeamPatchSaveMutation,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details_For_Single_And_Grouped_Saves(
        string teamId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalName,
        string updatedName,
        string originalAbout,
        string updatedAbout,
        string originalTimezone,
        string updatedTimezone,
        CancellationToken cancellationToken)
    {
        await SeedOwnedTeamAsync(
            teamId,
            organizationId,
            customerId,
            identityId,
            memberId,
            originalName,
            originalAbout,
            originalTimezone,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var nameResult = await updateTeamPatchSaveMutation.ExecuteAsync(
                teamId,
                [TeamPatchField.Name],
                updatedName,
                null,
                null,
                cancellationToken);

            nameResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            nameResult.Data.ShouldNotBeNull();
            nameResult.Data.UpdateTeam.Team.Name.ShouldBe(updatedName);
            nameResult.Data.UpdateTeam.Team.About.ShouldBe(originalAbout);

            var groupedResult = await updateTeamPatchSaveMutation.ExecuteAsync(
                teamId,
                [TeamPatchField.About, TeamPatchField.Timezone],
                updatedName,
                updatedAbout,
                updatedTimezone,
                cancellationToken);

            groupedResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            groupedResult.Data.ShouldNotBeNull();
            groupedResult.Data.UpdateTeam.Team.Name.ShouldBe(updatedName);
            groupedResult.Data.UpdateTeam.Team.About.ShouldBe(updatedAbout);
            groupedResult.Data.UpdateTeam.Team.Timezone.ShouldBe(updatedTimezone);

            var team = await repositoryFactory.TeamRepository.GetByIdUntrackedAsync(teamId, cancellationToken);
            team.ShouldNotBeNull();
            team.Name.ShouldBe(updatedName);
            team.About.ShouldBe(updatedAbout);
            team.Timezone.ShouldBe(updatedTimezone);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    private async Task SeedOwnedTeamAsync(
        string teamId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalName,
        string originalAbout,
        string originalTimezone,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Offering = new Offering
        {
            Id = organizationId, Code = OfferingCode.EnterpriseCustomV1, Start = now.AddDays(-1), End = now.AddDays(1)
        };
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);

        repositoryFactory.IdentityRepository.Add(new Identity { Id = identityId, Customer = customer });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active
        });
        repositoryFactory.TeamRepository.Add(new Shared.Database.Entities.Team
        {
            Id = teamId,
            Organization = organization,
            Name = originalName,
            About = originalAbout,
            Timezone = originalTimezone
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
