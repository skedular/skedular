using AutoFixture.Kernel;

namespace Testing.Shared;

public class CancellationTokenGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type)
        {
            return new NoSpecimen();
        }

        if (type != typeof(CancellationToken))
        {
            return new NoSpecimen();
        }

        return new CancellationTokenSource().Token;
    }
}
