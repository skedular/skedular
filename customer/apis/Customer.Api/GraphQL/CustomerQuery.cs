using System.Reflection;
using Api.Shared.Services.GraphQL.UnityHub.V1.Customer;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using CustomerOrderInput = Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderInput;
using CustomerOrderField = Customer.Shared.Models.CustomerOrderField;
using OrderDirection = Api.Shared.Services.GraphQL.UnityHub.V1.Customer.OrderDirection;
using Version = Api.Shared.Services.GraphQL.UnityHub.V1.Customer.Version;

namespace Customer.Api.GraphQL;

public class CustomerQuery(IMapper mapper) : Query
{
    public override Task<Version> CustomerVersionAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<CustomerDetails?> MeAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        var customer = await service.GetMeAsync(true, cancellationToken);
        return mapper.MapTo(customer);
    }

    public override async Task<CustomerConnection> CustomersByDefaultLocationAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomerWhereInputV2 where,
        CustomerOrderInput[]? orderBy,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        var (paginatedInfo, edges, totalCount) =
            await service.GetPaginatedCustomersAsync(
                new PaginationInputParam(after, first, before, last),
                new CustomerSearchCriteria(where.NameContains, where.LocationId),
                orderBy is null
                    ? []
                    : orderBy.Select(item =>
                    {
                        var direction = item.Direction == OrderDirection.Ascending
                            ? Enterprise.Shared.Pagination.OrderDirection.Ascending
                            : Enterprise.Shared.Pagination.OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.designation =>
                                CustomerOrderField.Designation,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.title =>
                                CustomerOrderField.Title,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.name =>
                                CustomerOrderField.Name,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.givenName =>
                                CustomerOrderField.GivenName,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.middleName =>
                                CustomerOrderField.MiddleName,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.familyName =>
                                CustomerOrderField.FamilyName,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.timezone =>
                                CustomerOrderField.Timezone,
                            global::Api.Shared.Services.GraphQL.UnityHub.V1.Customer.CustomerOrderField.locale =>
                                CustomerOrderField.Locale,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        return new CustomerOrder(direction, field);
                    }).ToList(),
                cancellationToken);

        return new CustomerConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor
            },
            Edges = edges.Select(mapper.MapTo).ToArray(),
            TotalCount = totalCount
        };
    }
}
