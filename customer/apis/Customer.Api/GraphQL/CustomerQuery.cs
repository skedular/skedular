using System.Reflection;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;

namespace Customer.Api.GraphQL;

public class CustomerQuery(IServiceProvider serviceProvider, IMapper mapper)
{
    public Task<Version> CustomerVersionAsync(CancellationToken cancellationToken)
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

    public async Task<CustomerDetails?> MeAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        var customer = await service.GetMeAsync(true, cancellationToken);
        return mapper.MapTo(customer);
    }

    public async Task<CustomerConnection?> PaginatedCustomersByDefaultLocationAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomerWhereInput where,
        CustomerOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.LocationId);

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
                            CustomerOrderField.designation =>
                                Shared.Models.CustomerOrderField.Designation,
                            CustomerOrderField.title =>
                                Shared.Models.CustomerOrderField.Title,
                            CustomerOrderField.name =>
                                Shared.Models.CustomerOrderField.Name,
                            CustomerOrderField.givenName =>
                                Shared.Models.CustomerOrderField.GivenName,
                            CustomerOrderField.middleName =>
                                Shared.Models.CustomerOrderField.MiddleName,
                            CustomerOrderField.familyName =>
                                Shared.Models.CustomerOrderField.FamilyName,
                            CustomerOrderField.timezone =>
                                Shared.Models.CustomerOrderField.Timezone,
                            CustomerOrderField.locale =>
                                Shared.Models.CustomerOrderField.Locale,
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

    public async Task<CustomerDetails[]?> CustomersByDefaultLocationAsync(
        CustomerWhereInput where,
        CustomerOrderInput[]? orderBy,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedCustomersByDefaultLocationAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }
}
