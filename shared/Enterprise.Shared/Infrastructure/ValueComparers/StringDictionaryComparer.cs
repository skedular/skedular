using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Enterprise.Shared.Infrastructure.ValueComparers;

public class StringDictionaryComparer() : ValueComparer<Dictionary<string, string>>(
    (left, right) => left!.OrderBy(item => item.Key).SequenceEqual(right!.OrderBy(item => item.Key)),
    dictionary => dictionary.Select(pair => HashCode.Combine(pair.Key, pair.Value)).Aggregate(HashCode.Combine),
    dictionary => new Dictionary<string, string>(dictionary));
