using AutoFixture.Kernel;

namespace Testing.Shared;

public class CancellationTokenGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context) => request is not Type type || type != typeof(CancellationToken)
        ? new NoSpecimen()
        : new CancellationTokenSource().Token;
}
