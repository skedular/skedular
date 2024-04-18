using HotChocolate.Types.Descriptors;

namespace Enterprise.Shared.GraphQL;

public class CustomNamingConventions : DefaultNamingConventions
{
    public override string GetEnumValueName(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var valueName = value.ToString();

        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);

        return valueName;
    }
}
