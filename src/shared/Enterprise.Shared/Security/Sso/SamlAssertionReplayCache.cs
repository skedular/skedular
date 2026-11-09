using System.Collections.Concurrent;

namespace Enterprise.Shared.Security.Sso;

public interface ISamlAssertionReplayCache
{
    bool TryRegister(string issuer, string assertionId, DateTimeOffset now);
}

// This cache protects one API process; multi-replica deployments need a shared atomic store.
public class SamlAssertionReplayCache : ISamlAssertionReplayCache
{
    private const int MaxEntries = 10_000;
    private static readonly TimeSpan s_retention = TimeSpan.FromHours(24);
    private readonly ConcurrentDictionary<(string Issuer, string AssertionId), DateTimeOffset> _entries = new();

    public bool TryRegister(string issuer, string assertionId, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(assertionId);

        foreach (var entry in _entries)
        {
            if (now - entry.Value > s_retention)
            {
                _entries.TryRemove(entry.Key, out _);
            }
        }

        if (_entries.Count >= MaxEntries)
        {
            return false;
        }

        return _entries.TryAdd((issuer, assertionId), now);
    }
}
