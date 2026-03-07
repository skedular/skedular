using Api.Shared.Services;
using Api.Shared.Services.Models;
using AutoFixture;
using Customer.Shared.Database.Entities;
using Testing.Shared;

namespace Customer.Domain.IntegrationTests.Fixtures;

public class BasicCustomerWithIdentityFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Shared.Database.Entities.Customer>(composer =>
            composer
                .With(item => item.ModifiedAt, (DateTimeOffset?)null)
                .With(item => item.DeletedAt, (DateTimeOffset?)null)
                .With(item => item.Locale, () =>
                {
                    var value = fixture.Create<string>();
                    return value.Length > Constants.MaxLocaleLength ? value[..Constants.MaxLocaleLength] : value;
                })
                .With(item => item.PersonalInformationVisibility, () =>
                {
                    var values = new List<string> { PersonalInformationVisibilityConstants.Visible, PersonalInformationVisibilityConstants.Redacted };
                    return values[Random.Shared.Next(0, values.Count - 1)];
                })
                .With(item => item.Type, () =>
                {
                    var values = new List<string> { CustomerTypeConstants.Guest, CustomerTypeConstants.Registered };
                    return values[Random.Shared.Next(0, values.Count - 1)];
                })
                .With(item => item.CustomerFeedbacks, [])
                .With(item => item.DefaultOrganization, (Organization?)null)
                .With(item => item.PreferredLocations, [])
                .With(item => item.PreferredResources, [])
                .With(item => item.PreferredOrganizationTags, [])
                .With(item => item.OrganizationMembers, [])
                .With(item => item.StripePaymentMethods, [])
                .With(item => item.FavouriteLocations, [])
                .With(item => item.StripeCustomer, (StripeCustomer?)null)
                .With(item => item.BillingDetails, (CustomerBillingDetails?)null)
                .With(item => item.PreferredOrganizationTags, [])
                .With(item => item.PreferredOrganizationTags, []));

        fixture.Customize<Identity>(composer =>
            composer
                .With(item => item.ModifiedAt, (DateTimeOffset?)null)
                .With(item => item.Customer, (Shared.Database.Entities.Customer?)null));
    }
}
