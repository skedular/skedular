using AutoFixture.Kernel;

namespace Testing.Shared;

/// <summary>
///     At the moment AutoFixture has no out of the box support for
///     <see cref="DateOnly" /> data type added in .Net 6. This generator adds support
///     for <see cref="DateOnly" /> type, using AutoFixture's mechanism for generating
///     random dates.
/// </summary>
public class DateOnlyValueGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type)
        {
            return new NoSpecimen();
        }

        if (type != typeof(DateOnly))
        {
            return new NoSpecimen();
        }

        var dateTime = (DateTime)context.Resolve(typeof(DateTime));

        return DateOnly.FromDateTime(dateTime);
    }
}
