using Api.Shared.Services.Models;

namespace Location.Shared.Models;

public record LocationExtraMetadata(ContactDetails? ContactDetails, AreaRange? AreaRange, PeopleCapacity? PeopleCapacity);
