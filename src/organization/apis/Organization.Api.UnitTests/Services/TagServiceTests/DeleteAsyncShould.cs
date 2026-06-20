using Api.Shared.Services.Models;
using Organization.Api.Services;

namespace Organization.Api.UnitTests.Services.TagServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_A_System_Host_Location_Tag(TagService sut, CancellationToken cancellationToken) =>
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.DeleteAsync(HostLocationSystemIds.ProductTag("location-1"), cancellationToken));

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_System_Host_Location_Tags_In_Bulk(TagService sut, CancellationToken cancellationToken) =>
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.DeleteAsync(["normal-tag", HostLocationSystemIds.ProductTag("location-1")], cancellationToken));
}
