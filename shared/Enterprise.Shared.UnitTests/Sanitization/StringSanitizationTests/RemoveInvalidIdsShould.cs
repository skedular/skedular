using Enterprise.Shared.Sanitization;

namespace Enterprise.Shared.UnitTests.Sanitization.StringSanitizationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RemoveInvalidIdsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_non_empty_ids(string id1, string id2)
    {
        var result = new[] { id1, "  ", "", id2 }.RemoveInvalidIds().ToList();

        result.ShouldContain(id1);
        result.ShouldContain(id2);
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void Return_empty_collection_when_all_invalid()
    {
        var result = new[] { "  ", "", "   " }.RemoveInvalidIds().ToList();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void Return_empty_collection_when_input_is_null()
    {
        IEnumerable<string>? input = null;

        var result = input.RemoveInvalidIds().ToList();

        result.ShouldBeEmpty();
    }

    [Fact]
    public void Return_empty_collection_when_input_is_empty()
    {
        var result = Enumerable.Empty<string>().RemoveInvalidIds().ToList();

        result.ShouldBeEmpty();
    }
}
