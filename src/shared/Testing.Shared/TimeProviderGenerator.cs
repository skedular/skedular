using AutoFixture.Kernel;
using FakeItEasy;

namespace Testing.Shared;

public class TimeProviderGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type)
        {
            return new NoSpecimen();
        }

        if (type != typeof(TimeProvider))
        {
            return new NoSpecimen();
        }

        return A.Fake<TimeProvider>();
    }
}
