using AutoFixture.Kernel;

namespace Enterprise.Shared.UnitTesting;

public class DateTimeOffsetGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context) => request is not Type type || type != typeof(DateTimeOffset)
        ? NoSpecimen.Instance
        : TimeProvider.System.GetUtcNow();
}
