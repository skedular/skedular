using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Booking.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace Booking.Shared.Services;

/// <summary>
///     Generates deterministic, opaque subscription keys from a canonicalised
///     <see cref="ResourceAvailabilityDayFilter" />.
///     The key is a URL-safe base64 encoding of the SHA-256 hash of the canonical JSON
///     representation of the filter's relevant fields.
///     Clients must treat the key as opaque — they must never construct or interpret it.
/// </summary>
public interface ISubscriptionKeyService
{
    /// <summary>
    ///     Computes the opaque subscription key for the given filter.
    ///     Two filters that differ only in field ordering or unset optional fields
    ///     produce the same key.
    /// </summary>
    /// <param name="filter">The filter that produced the query result.</param>
    /// <returns>A URL-safe base64-encoded SHA-256 hash string.</returns>
    string Compute(ResourceAvailabilityDayFilter filter);

    /// <summary>
    ///     Returns all subscription keys that must be notified when a booking changes.
    ///     Covers every null-dimension permutation of <paramref name="floorId" />,
    ///     <paramref name="zoneId" />, and <paramref name="resourceType" /> (2³ = 8 keys)
    ///     so that subscribers using partial filters still receive the event.
    /// </summary>
    /// <param name="organizationCustomDomain">The custom domain of the organisation that owns the booking.</param>
    /// <param name="locationId">The location of the booking.</param>
    /// <param name="floorId">The floor of the booking (may be null).</param>
    /// <param name="zoneId">The zone of the booking (may be null).</param>
    /// <param name="resourceType">The resource type tag of the booking (may be null).</param>
    /// <param name="date">The date of the booking.</param>
    /// <returns>Up to 8 distinct subscription keys.</returns>
    IEnumerable<string> AffectedKeys(
        string organizationCustomDomain,
        string locationId,
        string? floorId,
        string? zoneId,
        string? resourceType,
        DateOnly date);
}

/// <inheritdoc cref="ISubscriptionKeyService" />
public sealed class SubscriptionKeyService : ISubscriptionKeyService
{
    /// <inheritdoc />
    public string Compute(ResourceAvailabilityDayFilter filter)
    {
        // When exactly one location is selected, use it for precision key matching.
        // When zero or multiple locations are selected, fall back to the org-wide key (loc = "").
        var locKey = filter.LocationIds.Count == 1 ? filter.LocationIds[0] : string.Empty;

        var canonical = JsonSerializer.Serialize(new
        {
            org = filter.OrganizationCustomDomain,
            loc = locKey,
            floor = filter.FloorId ?? string.Empty,
            zone = filter.ZoneId ?? string.Empty,
            type = filter.ResourceType ?? string.Empty,
            date = filter.Date.ToString("yyyy-MM-dd")
        });
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return WebEncoders.Base64UrlEncode(hash);
    }

    /// <inheritdoc />
    public IEnumerable<string> AffectedKeys(
        string organizationCustomDomain,
        string locationId,
        string? floorId,
        string? zoneId,
        string? resourceType,
        DateOnly date)
    {
        // Emit keys for both the specific location and the org-wide (empty location) variant
        // so that subscribers watching a single location AND subscribers watching all locations
        // both receive the notification.
        foreach (var locIds in new[] { new[] { locationId }, Array.Empty<string>() })
        foreach (var floor in Variants(floorId))
        foreach (var zone in Variants(zoneId))
        foreach (var type in Variants(resourceType))
        {
            yield return Compute(new ResourceAvailabilityDayFilter
            {
                OrganizationCustomDomain = organizationCustomDomain,
                LocationIds = locIds,
                FloorId = floor,
                ZoneId = zone,
                ResourceType = type,
                Date = date
            });
        }
    }

    private static string?[] Variants(string? v) => [v, null];
}
