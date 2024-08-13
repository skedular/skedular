namespace Enterprise.Shared.Context;

public class PropertyBag
{
    public const string Key = "unityhub_context_key";
    private readonly IDictionary<Type, Property> _properties = new Dictionary<Type, Property>();

    public string? VerifiableToken =>
        _properties.TryGetValue(Type.VerifiableToken, out var property) ? property.StrVal : null;

    public string? Title => _properties.TryGetValue(Type.Title, out var property) ? property.StrVal : null;
    public string? Designation => _properties.TryGetValue(Type.Designation, out var property) ? property.StrVal : null;
    public string? Name => _properties.TryGetValue(Type.Name, out var property) ? property.StrVal : null;
    public string? GivenName => _properties.TryGetValue(Type.GivenName, out var property) ? property.StrVal : null;
    public string? MiddleName => _properties.TryGetValue(Type.MiddleName, out var property) ? property.StrVal : null;
    public string? FamilyName => _properties.TryGetValue(Type.FamilyName, out var property) ? property.StrVal : null;
    public string? PhotoUrl => _properties.TryGetValue(Type.PhotoUrl, out var property) ? property.StrVal : null;
    public string? PhotoUrl24 => _properties.TryGetValue(Type.PhotoUrl24, out var property) ? property.StrVal : null;
    public string? PhotoUrl32 => _properties.TryGetValue(Type.PhotoUrl32, out var property) ? property.StrVal : null;
    public string? PhotoUrl48 => _properties.TryGetValue(Type.PhotoUrl48, out var property) ? property.StrVal : null;
    public string? PhotoUrl72 => _properties.TryGetValue(Type.PhotoUrl72, out var property) ? property.StrVal : null;
    public string? PhotoUrl192 => _properties.TryGetValue(Type.PhotoUrl192, out var property) ? property.StrVal : null;
    public string? PhotoUrl512 => _properties.TryGetValue(Type.PhotoUrl512, out var property) ? property.StrVal : null;
    public string? Timezone => _properties.TryGetValue(Type.Timezone, out var property) ? property.StrVal : null;
    public string? Email => _properties.TryGetValue(Type.Email, out var property) ? property.StrVal : null;

    public string CorrelationId =>
        _properties.TryGetValue(Type.CorrelationId, out var property) ? property.StrVal : string.Empty;

    public bool? EmailVerified =>
        _properties.TryGetValue(Type.EmailVerified, out var property) ? property.BoolVal : null;

    public string? Locale => _properties.TryGetValue(Type.Locale, out var property) ? property.StrVal : null;

    public Guid AzureTenantId =>
        _properties.TryGetValue(Type.AzureTenantId, out var property) ? property.GuidVal : Guid.Empty;

    public string? AzureTenantAudience =>
        _properties.TryGetValue(Type.AzureTenantAudience, out var property) ? property.StrVal : null;

    public PropertyBag AddVerifiableToken(string value)
    {
        _properties[Type.VerifiableToken] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddTitle(string value)
    {
        _properties[Type.Title] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddDesignation(string value)
    {
        _properties[Type.Designation] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddName(string value)
    {
        _properties[Type.Name] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddGivenName(string value)
    {
        _properties[Type.GivenName] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddMiddleName(string value)
    {
        _properties[Type.MiddleName] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddFamilyName(string value)
    {
        _properties[Type.FamilyName] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl(string value)
    {
        _properties[Type.PhotoUrl] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl24(string value)
    {
        _properties[Type.PhotoUrl24] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl32(string value)
    {
        _properties[Type.PhotoUrl32] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl48(string value)
    {
        _properties[Type.PhotoUrl48] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl72(string value)
    {
        _properties[Type.PhotoUrl72] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl192(string value)
    {
        _properties[Type.PhotoUrl192] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddPhotoUrl512(string value)
    {
        _properties[Type.PhotoUrl512] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddEmail(string value)
    {
        _properties[Type.Email] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddEmailVerified(bool value)
    {
        _properties[Type.EmailVerified] = new Property { BoolVal = value };

        return this;
    }

    public PropertyBag AddLocale(string value)
    {
        _properties[Type.Locale] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddTimezone(string value)
    {
        _properties[Type.Timezone] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddCorrelationId(string value)
    {
        _properties[Type.CorrelationId] = new Property { StrVal = value };

        return this;
    }

    public PropertyBag AddAzureTenantId(Guid value)
    {
        _properties[Type.AzureTenantId] = new Property { GuidVal = value };

        return this;
    }

    public PropertyBag AddAzureTenantAudience(string value)
    {
        _properties[Type.AzureTenantAudience] = new Property { StrVal = value };

        return this;
    }

    private enum Type
    {
        VerifiableToken,
        Designation,
        Title,
        Name,
        GivenName,
        MiddleName,
        FamilyName,
        PhotoUrl,
        PhotoUrl24,
        PhotoUrl32,
        PhotoUrl48,
        PhotoUrl72,
        PhotoUrl192,
        PhotoUrl512,
        Email,
        EmailVerified,
        Timezone,
        Locale,
        CorrelationId,
        AzureTenantId,
        AzureTenantAudience
    }

    private class Property
    {
        public string StrVal { get; set; } = string.Empty;
        public bool BoolVal { get; set; }
        public Guid GuidVal { get; set; }
    }
}
