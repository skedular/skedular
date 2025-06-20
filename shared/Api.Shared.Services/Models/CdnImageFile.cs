using HotChocolate;

namespace Api.Shared.Services.Models;

[GraphQLName("CdnFile")]
public record CdnFile([GraphQLName("url")] string Url, [GraphQLName("height")] int? Height, [GraphQLName("width")] int? Width);

[GraphQLName("CdnImageFile")]
public record CdnImageFile([GraphQLName("original")] CdnFile? Original, [GraphQLName("thumbnail")] CdnFile? Thumbnail);
