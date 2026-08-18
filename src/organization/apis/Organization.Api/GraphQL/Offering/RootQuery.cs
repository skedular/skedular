using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using HotChocolate.Types;
using Organization.Api.Mappers;

namespace Organization.Api.GraphQL.Offering;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public OrganizationOfferingDetails OrganizationOffering(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var matchedOffering = Offerings.AllOfferings.FirstOrDefault(item => item.ToOfferingCode() == code);
        var offering = matchedOffering.GetOffering();

        return new OrganizationOfferingDetails
        {
            Code = matchedOffering.ToOfferingCode(),
            IsEnterprise = matchedOffering.IsEnterpriseOffering(),
            Name = offering.Name,
            UnitPrice = offering.UnitPrice,
            Currency = new CurrencyDetails
            {
                Type = offering.Currency,
                Name = offering.Currency.ToCurrencyName(),
            },
            FixedPrice = offering.FixedPrice,
            FeatureSet = [.. graphQlMapper.MapTo(offering)],
            UnderPriceLines = [.. offering.UnderPriceLines],
            Free = matchedOffering.IsFreeOffering(),
            EarlyBird = matchedOffering.IsEarlyBirdOffering(),
        };
    }
}
