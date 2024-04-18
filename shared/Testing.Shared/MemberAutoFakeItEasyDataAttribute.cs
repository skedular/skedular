using AutoFixture.Xunit2;

namespace Testing.Shared;

public class MemberAutoFakeItEasyDataAttribute(
    string memberName,
    Type[]? fixtureCustomizers = null,
    params object[] objects)
    : MemberAutoDataAttribute(new AutoFakeItEasyDataAttribute(fixtureCustomizers), memberName,
        objects);
