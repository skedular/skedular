using NanoidDotNet;

namespace Enterprise.Shared.Random;

public interface IRandomHelper
{
    Guid GenerateGuid();
    IReadOnlyCollection<Guid> GenerateManyGuids(int count);
    string Generate();
    IReadOnlyCollection<string> GenerateMany(int count);
    string GenerateAlphanumericNumeric(int size = 21);
    IReadOnlyCollection<string> GenerateManyGenerateAlphanumericNumeric(int count, int size = 21);
}

public class RandomHelper : IRandomHelper
{
    public Guid GenerateGuid() => Guid.CreateVersion7();
    public IReadOnlyCollection<Guid> GenerateManyGuids(int count) => Enumerable.Range(0, count).Select(_ => GenerateGuid()).ToList();
    public string Generate() => GenerateGuid().ToString();
    public IReadOnlyCollection<string> GenerateMany(int count) => GenerateManyGuids(count).Select(item => item.ToString()).ToList();

    public string GenerateAlphanumericNumeric(int size = 21) =>
        size < 1 ? throw new ArgumentOutOfRangeException(nameof(size)) :
        size == 1 ? Nanoid.Generate("abcdefghijklmnopqrstuvwxyz", 1) :
        $"{Nanoid.Generate("abcdefghijklmnopqrstuvwxyz", 1)}{Nanoid.Generate("0123456789abcdefghijklmnopqrstuvwxyz", size - 1)}";

    public IReadOnlyCollection<string> GenerateManyGenerateAlphanumericNumeric(int count, int size = 21) =>
        Enumerable.Range(0, count).Select(_ => GenerateAlphanumericNumeric(size)).ToList();
}
