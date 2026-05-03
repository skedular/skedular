using HotChocolate;
using HotChocolate.Types.Composite;

namespace Api.Shared.Services.Models;

[GraphQLName("CdnFile")]
[Shareable]
public record CdnFile([GraphQLName("url")] string Url, [GraphQLName("height")] int? Height, [GraphQLName("width")] int? Width);

[GraphQLName("CdnImageFile")]
[Shareable]
public record CdnImageFile([GraphQLName("original")] CdnFile? Original, [GraphQLName("thumbnail")] CdnFile? Thumbnail);
