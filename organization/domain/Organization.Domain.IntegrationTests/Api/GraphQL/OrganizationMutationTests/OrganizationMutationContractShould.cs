using Organization.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Organization.Domain.IntegrationTests.Api.GraphQL.OrganizationMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Organization.Api")]
public class OrganizationMutationContractShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(OrganizationIntegrationServiceFixtureCustomizer)])]
    public async Task Expose_Update_Organization_Field_Mask_Without_Patch_Alias(
        IOrganizationMutationContractQuery organizationMutationContractQuery,
        CancellationToken cancellationToken)
    {
        var result = await organizationMutationContractQuery.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        result.Data.ShouldNotBeNull();

        var mutationNames = result.Data.__schema.MutationType!.Fields!.Select(field => field.Name).ToList();
        mutationNames.ShouldContain("updateOrganization");
        mutationNames.ShouldNotContain("updateOrganizationPatch");
        mutationNames.ShouldNotContain("updateOrganizationBillingDetailsPatch");
        mutationNames.ShouldNotContain("updateOrganizationSsoSettingsPatch");

        ShouldHaveFieldMask(result.Data.UpdateOrganizationInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationBillingDetailsInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationTaxDetailsInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationTagInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationOfferingInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationBankAccountInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationStripeConnectAccountInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationXeroConnectionInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldMask(result.Data.UpdateOrganizationSsoSettingsInput?.InputFields?.Select(field => field.Name));
    }

    private static void ShouldHaveFieldMask(IEnumerable<string>? inputFieldNames)
    {
        inputFieldNames.ShouldNotBeNull();
        inputFieldNames.ShouldContain("fieldsToUpdate");
    }
}
