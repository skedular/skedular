using AutoFixture.Xunit2;

namespace Testing.Shared;

public class InlineAutoFakeItEasyDataAttribute(
    Type[]? fixtureCustomizers = null,
    params object[] objects) : InlineAutoDataAttribute(new AutoFakeItEasyDataAttribute(fixtureCustomizers), objects);
