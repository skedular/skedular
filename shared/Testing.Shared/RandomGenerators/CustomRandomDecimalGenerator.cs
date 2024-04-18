namespace Testing.Shared.RandomGenerators;

public class CustomRandomDecimalGenerator
{
    private readonly Random _random = new();

    public decimal Value => decimal.Parse((_random.NextDouble() * 999999999).ToString("f2"));
}
