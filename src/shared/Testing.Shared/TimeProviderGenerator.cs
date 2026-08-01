using AutoFixture.Kernel;
using FakeItEasy;

namespace Testing.Shared;

public class TimeProviderGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(TimeProvider))
        {
            return NoSpecimen.Instance;
        }

        return A.Fake<TimeProvider>();
    }
}
