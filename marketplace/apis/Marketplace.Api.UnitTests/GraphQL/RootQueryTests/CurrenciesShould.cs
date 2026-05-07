using Api.Shared.Services.Models;
using Marketplace.Api.GraphQL;

namespace Marketplace.Api.UnitTests.GraphQL.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CurrenciesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Currencies(RootQuery sut)
    {
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
