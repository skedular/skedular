using System.Reflection;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;

namespace Customer.Api.GraphQL;

public class CustomerQuery
{
    [UseServiceScope]
    public Version CustomerVersion()
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        };
    }

    [UseServiceScope]
    public async Task<CustomerDetails?> MeAsync(
        [Service] ICustomerService customerService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetMeAsync(true, cancellationToken));

    [UseServiceScope]
    public async Task<CustomerConnection?> PaginatedCustomersByDefaultLocationAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomerWhereInput where,
        CustomerOrderInput[]? orderBy,
        [Service] ICustomerService customerService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(where.LocationId);

        var (paginatedInfo, edges, totalCount) =
            await customerService.GetPaginatedCustomersAsync(
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

    [UseServiceScope]
    public async Task<CustomerDetails[]?> CustomersByDefaultLocationAsync(
        CustomerWhereInput where,
        CustomerOrderInput[]? orderBy,
        [Service] ICustomerService customerService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var result = await PaginatedCustomersByDefaultLocationAsync(
            null,
            null,
            null,
            null,
            where,
            orderBy,
            customerService,
            mapper,
            cancellationToken);
        return result?.Edges.Select(item => item.Node).ToArray();
    }
}
