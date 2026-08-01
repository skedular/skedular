using AutoFixture.Kernel;
using NetTopologySuite.Geometries;

namespace Testing.Shared;

public class CoordinateGenerator : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context) => request is not Type type || type != typeof(Coordinate)
        ? NoSpecimen.Instance
        : new Coordinate(0, 0);
}
