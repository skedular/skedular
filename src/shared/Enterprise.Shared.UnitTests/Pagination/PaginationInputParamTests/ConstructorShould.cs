using Enterprise.Shared.Pagination;

namespace Enterprise.Shared.UnitTests.Pagination.PaginationInputParamTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ConstructorShould
{
    [Fact]
    public void Allow_forward_pagination_with_after_and_first()
    {
        var param = new PaginationInputParam("cursor", 10, null, null);

        param.After.ShouldBe("cursor");
        param.First.ShouldBe(10);
        param.Before.ShouldBeNull();
        param.Last.ShouldBeNull();
    }

    [Fact]
    public void Allow_backward_pagination_with_before_and_last()
    {
        var param = new PaginationInputParam(null, null, "cursor", 10);

        param.Before.ShouldBe("cursor");
        param.Last.ShouldBe(10);
        param.After.ShouldBeNull();
        param.First.ShouldBeNull();
    }

    [Fact]
    public void Allow_no_cursor_with_first()
    {
        var param = new PaginationInputParam(null, 5, null, null);
        param.First.ShouldBe(5);
    }

    [Fact]
    public void Throw_when_both_after_and_before_provided() =>
        Should.Throw<ArgumentException>(() => new PaginationInputParam("a", null, "b", null));

    [Fact]
    public void Throw_when_both_first_and_last_provided() =>
        Should.Throw<ArgumentException>(() => new PaginationInputParam(null, 5, null, 5));

    [Fact]
    public void Throw_when_first_and_before_mixed() =>
        Should.Throw<ArgumentException>(() => new PaginationInputParam(null, 5, "b", null));

    [Fact]
    public void Throw_when_after_and_last_mixed() =>
        Should.Throw<ArgumentException>(() => new PaginationInputParam("a", null, null, 5));

    [Fact]
    public void Throw_when_first_is_negative() =>
        Should.Throw<ArgumentException>(() => new PaginationInputParam(null, -1, null, null));

    [Fact]
    public void Throw_when_last_is_negative() =>
        Should.Throw<ArgumentException>(() => new PaginationInputParam(null, null, null, -1));
}
