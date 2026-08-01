using AutoFixture.Kernel;
using Xunit;

namespace Testing.Shared;

public class CancellationTokenGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context) => request is not Type type || type != typeof(CancellationToken)
        ? NoSpecimen.Instance
        : TestContext.Current.CancellationToken;
}
