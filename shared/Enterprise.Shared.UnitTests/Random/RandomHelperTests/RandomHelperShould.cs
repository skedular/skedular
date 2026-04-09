using Enterprise.Shared.Random;

namespace Enterprise.Shared.UnitTests.Random.RandomHelperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RandomHelperShould
{
    [Fact]
    public void GenerateGuid_returns_nonempty_guid()
    {
        var sut = new RandomHelper();

        sut.GenerateGuid().ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GenerateManyGuids_returns_requested_count(RandomHelper sut)
    {
        var guids = sut.GenerateManyGuids(5);

        guids.Count.ShouldBe(5);
        guids.Distinct().Count().ShouldBe(5);
    }

    [Fact]
    public void Generate_returns_non_empty_string()
    {
        var sut = new RandomHelper();

        sut.Generate().ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GenerateMany_returns_requested_count(RandomHelper sut)
    {
        var results = sut.GenerateMany(4);

        results.Count.ShouldBe(4);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GenerateAlphanumericNumeric_returns_string_of_correct_length(RandomHelper sut)
    {
        var result = sut.GenerateAlphanumericNumeric(10);

        result.Length.ShouldBe(10);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GenerateAlphanumericNumeric_size_one_returns_single_letter(RandomHelper sut)
    {
        var result = sut.GenerateAlphanumericNumeric(1);

        result.Length.ShouldBe(1);
        char.IsLetter(result[0]).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void GenerateAlphanumericNumeric_throws_for_zero(RandomHelper sut) =>
        Should.Throw<ArgumentOutOfRangeException>(() => sut.GenerateAlphanumericNumeric(0));

    [Theory]
    [AutoFakeItEasyData]
    public void GenerateManyGenerateAlphanumericNumeric_returns_requested_count(RandomHelper sut)
    {
        var results = sut.GenerateManyGenerateAlphanumericNumeric(3, 5);

        results.Count.ShouldBe(3);
        results.ShouldAllBe(r => r.Length == 5);
    }
}
