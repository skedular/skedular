using AutoFixture.Xunit3;

namespace Enterprise.Shared.UnitTesting;

public class InlineAutoFakeItEasyDataAttribute(Type[]? fixtureCustomizers = null, params object[] objects)
    : InlineAutoDataAttribute(() => AutoFakeItEasyDataAttribute.CreateFixture(fixtureCustomizers), objects);
