using AutoFixture.Xunit3;

namespace Testing.Shared;

public class InlineAutoFakeItEasyDataAttribute(
    Type[]? fixtureCustomizers = null,
    params object[] objects) : InlineAutoDataAttribute(new AutoFakeItEasyDataAttribute(fixtureCustomizers), objects);
