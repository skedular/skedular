using NanoidDotNet;

namespace Enterprise.Shared.Random;

public interface IRandomHelper
{
    string Generate();
    IReadOnlyCollection<string> GenerateMany(int count);
}

public class RandomHelper : IRandomHelper
{
    public string Generate() => Nanoid.Generate();

    public IReadOnlyCollection<string> GenerateMany(int count) =>
        Enumerable.Range(0, count).Select(_ => Nanoid.Generate()).ToList();
}
