using AutoFixture.Kernel;

namespace Testing.Shared;

public class DateTimeOffsetGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context) => request is not Type type || type != typeof(DateTimeOffset)
        ? new NoSpecimen()
        : DateTimeOffset.UtcNow;
}
