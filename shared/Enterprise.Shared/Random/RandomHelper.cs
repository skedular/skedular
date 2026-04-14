using Microsoft.Extensions.Logging;
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

public class RandomHelper(ILogger<RandomHelper> logger) : IRandomHelper
{
    public Guid GenerateGuid()
    {
        logger.LogDebug("Generating version 7 GUID");
        return Guid.CreateVersion7();
    }

    public IReadOnlyCollection<Guid> GenerateManyGuids(int count)
    {
        logger.LogDebug("Generating multiple version 7 GUIDs. Count={Count}", count);
        return Enumerable.Range(0, count).Select(_ => GenerateGuid()).ToList();
    }

    public string Generate()
    {
        logger.LogDebug("Generating string identifier from GUID");
        return GenerateGuid().ToString();
    }

    public IReadOnlyCollection<string> GenerateMany(int count)
    {
        logger.LogDebug("Generating multiple string identifiers. Count={Count}", count);
        return GenerateManyGuids(count).Select(item => item.ToString()).ToList();
    }

    public string GenerateAlphanumericNumeric(int size = 21) =>
        size < 1 ? throw new ArgumentOutOfRangeException(nameof(size)) :
        size == 1 ? Nanoid.Generate("abcdefghijklmnopqrstuvwxyz", 1) :
        $"{Nanoid.Generate("abcdefghijklmnopqrstuvwxyz", 1)}{Nanoid.Generate("0123456789abcdefghijklmnopqrstuvwxyz", size - 1)}";

    public IReadOnlyCollection<string> GenerateManyGenerateAlphanumericNumeric(int count, int size = 21) =>
        Enumerable.Range(0, count).Select(_ => GenerateAlphanumericNumeric(size)).ToList();
}
