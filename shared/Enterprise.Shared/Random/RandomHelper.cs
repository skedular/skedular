using NanoidDotNet;

namespace Enterprise.Shared.Random;

public interface IRandomHelper
{
    Guid GenerateGuid();
    IReadOnlyCollection<Guid> GenerateManyGuids(int count);
    string Generate();
    IReadOnlyCollection<string> GenerateMany(int count);
    string GenerateAlphanumeric(int size = 21);
    IReadOnlyCollection<string> GenerateManyGenerateAlphanumeric(int count, int size = 21);
}

public class RandomHelper : IRandomHelper
{
    public Guid GenerateGuid() => Guid.CreateVersion7();
    public IReadOnlyCollection<Guid> GenerateManyGuids(int count) => Enumerable.Range(0, count).Select(_ => GenerateGuid()).ToList();
    public string Generate() => GenerateGuid().ToString();
    public IReadOnlyCollection<string> GenerateMany(int count) => GenerateManyGuids(count).Select(item => item.ToString()).ToList();

    public string GenerateAlphanumeric(int size = 21) => Nanoid.Generate("abcdefghijklmnopqrstuvwxyz", size);

    public IReadOnlyCollection<string> GenerateManyGenerateAlphanumeric(int count, int size = 21) =>
        Enumerable.Range(0, count).Select(_ => GenerateAlphanumeric(size)).ToList();
}
