using NanoidDotNet;

namespace Enterprise.Shared.Random;

public interface IRandomHelper
{
    string Generate(string alphabet = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", int size = 21);

    IReadOnlyCollection<string> GenerateMany(int count, string alphabet = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
        int size = 21);
}

public class RandomHelper : IRandomHelper
{
    public string Generate(string alphabet = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", int size = 21) =>
        Nanoid.Generate(alphabet, size);

    public IReadOnlyCollection<string> GenerateMany(
        int count,
        string alphabet = "_-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
        int size = 21) =>
        Enumerable.Range(0, count).Select(_ => Nanoid.Generate(alphabet, size)).ToList();
}
