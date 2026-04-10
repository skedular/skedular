using Api.Shared.Services.Models;
using Enterprise.Shared.Version;
using Marketplace.Api.GraphQL;

namespace Marketplace.Api.UnitTests.GraphQL.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CurrenciesShould
{
    [Fact]
    public void Return_All_Currencies()
    {
        var sut = new RootQuery(A.Fake<IVersionService>());

        var result = sut.Currencies().ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(item =>
            item.Type == Currency.Nzd &&
            item.Name == Currency.Nzd.ToCurrencyName());
        result.ShouldContain(item =>
            item.Type == Currency.Usd &&
            item.Name == Currency.Usd.ToCurrencyName());
    }
}
