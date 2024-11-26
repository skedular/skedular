using System.Reflection;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;
using Version = Enterprise.Shared.GraphQL.Types.Version;

namespace Customer.Api.GraphQL;

public class CustomerQuery(IMapper mapper)
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
        CancellationToken cancellationToken) =>
        mapper.MapTo(await customerService.GetMeAsync(true, cancellationToken));

    [UseServiceScope]
    public async Task<CustomerConnection?> CustomersByDefaultLocationAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        CustomerWhereInput where,
        CustomerOrderInput[]? orderBy,
        [Service] ICustomerService customerService,
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
                            ? OrderDirection.Ascending
                            : OrderDirection.Descending;
                        var field = item.Field switch
                        {
                            CustomerOrderField.Designation =>
                                Shared.Models.CustomerOrderField.Designation,
                            CustomerOrderField.Title =>
                                Shared.Models.CustomerOrderField.Title,
                            CustomerOrderField.Name =>
                                Shared.Models.CustomerOrderField.Name,
                            CustomerOrderField.GivenName =>
                                Shared.Models.CustomerOrderField.GivenName,
                            CustomerOrderField.MiddleName =>
                                Shared.Models.CustomerOrderField.MiddleName,
                            CustomerOrderField.FamilyName =>
                                Shared.Models.CustomerOrderField.FamilyName,
                            CustomerOrderField.Timezone =>
                                Shared.Models.CustomerOrderField.Timezone,
                            CustomerOrderField.Locale =>
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
}
