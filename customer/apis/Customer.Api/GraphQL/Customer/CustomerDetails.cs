using Api.Shared.Services.Models;
using Customer.Api.GraphQL.Payment;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using CustomerBillingDetails = Customer.Api.GraphQL.Billing.CustomerBillingDetails;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerDetails")]
[EntityKey("id")]
public class CustomerDetails : Node
{
    [GraphQLName("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("emails")] public IEnumerable<string> Emails { get; set; } = [];
    [GraphQLName("identities")] public IEnumerable<CustomerIdentity> Identities { get; set; } = [];
    [GraphQLName("designation")] public string? Designation { get; set; }
    [GraphQLName("title")] public string? Title { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("photoUrl")] public string? PhotoUrl { get; set; }
    [GraphQLName("photoUrl24")] public string? PhotoUrl24 { get; set; }
    [GraphQLName("photoUrl32")] public string? PhotoUrl32 { get; set; }
    [GraphQLName("photoUrl48")] public string? PhotoUrl48 { get; set; }
    [GraphQLName("photoUrl72")] public string? PhotoUrl72 { get; set; }
    [GraphQLName("photoUrl192")] public string? PhotoUrl192 { get; set; }
    [GraphQLName("photoUrl512")] public string? PhotoUrl512 { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("locale")] public string? Locale { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
    [GraphQLName("type")] public CustomerType Type { get; set; }
    [GraphQLName("isOnboardingDone")] public bool IsOnboardingDone { get; set; }
    [GraphQLName("defaultOrganizationId")] public string? DefaultOrganizationId { get; set; }

    [GraphQLName("defaultOrganizationCustomDomain")]
    public string? DefaultOrganizationCustomDomain { get; set; }

    [GraphQLName("preferredLocationIds")] public IEnumerable<string> PreferredLocationIds { get; set; } = [];
    [GraphQLName("preferredZones")] public IEnumerable<OrganizationTagDetails> PreferredZones { get; set; } = [];
    [GraphQLName("preferredCustomTags")] public IEnumerable<OrganizationTagDetails> PreferredCustomTags { get; set; } = [];
    [GraphQLName("preferredResourceIds")] public IEnumerable<string> PreferredResourceIds { get; set; } = [];
    [GraphQLName("favouriteLocationIds")] public IEnumerable<string> FavouriteLocationIds { get; set; } = [];

    [GraphQLName("personalInformationVisibility")]
    public PersonalInformationVisibilityDetails PersonalInformationVisibility { get; set; } = new();

    [UseResolverScope]
    public async Task<bool> HasAttachedPaymentMethodAsync(
        [Parent] CustomerDetails customer,
        [Service] IPaymentService paymentService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        await paymentService.HasAttachedPaymentMethodAsync(customer.Id, cancellationToken);

    [UseResolverScope]
    public async Task<IEnumerable<CustomerPaymentMethod>> PaymentMethodsAsync(
        [Parent] CustomerDetails customer,
        [Service] IPaymentService paymentService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await paymentService.GetPaymentMethodsAsync(customer.Id, cancellationToken));

    [UseResolverScope]
    public async Task<CustomerBillingDetails?> BillingDetailsAsync(
        [Parent] CustomerDetails customer,
        [Service] IBillingService billingService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        mapper.MapToGraphQl(await billingService.GetBillingAsync(customer.Id, cancellationToken));
}

[ObjectType<CustomerDetails>]
public static partial class CustomerDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<CustomerDetails> descriptor)
    {
        descriptor.Ignore(item => item.DefaultOrganizationId);
        descriptor.Ignore(item => item.DefaultOrganizationCustomDomain);
        descriptor.Ignore(item => item.PreferredLocationIds);
        descriptor.Ignore(item => item.PreferredResourceIds);
        descriptor.Ignore(item => item.FavouriteLocationIds);
    }

    public static OrganizationDetails? GetOrganization([Parent] CustomerDetails item) => string.IsNullOrWhiteSpace(item.DefaultOrganizationId)
        ? null
        : new OrganizationDetails(item.DefaultOrganizationId, item.DefaultOrganizationCustomDomain.ToSafeString());

    public static IEnumerable<LocationDetails> GetPreferredLocations([Parent] CustomerDetails item) =>
        item.PreferredLocationIds.Select(id => new LocationDetails(id));

    public static IEnumerable<ResourceDetails> GetPreferredResources([Parent] CustomerDetails item) =>
        item.PreferredResourceIds.Select(id => new ResourceDetails(id));

    public static IEnumerable<LocationDetails> GetFavouriteLocations([Parent] CustomerDetails item) =>
        item.FavouriteLocationIds.Select(id => new LocationDetails(id));
}
