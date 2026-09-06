using AutoFixture.Kernel;
using Xunit;

namespace Enterprise.Shared.UnitTesting;

public class CancellationTokenGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context) => request is not Type type || type != typeof(CancellationToken)
        ? NoSpecimen.Instance
        : TestContext.Current.CancellationToken;
}
