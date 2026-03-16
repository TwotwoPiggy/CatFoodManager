using CatFoodManager.Domain.Interfaces;
using CatFoodManager.Domain.Specifications;
using Xunit;

namespace CatFoodManager.Tests.Domain.Specifications;

public class SpecificationTests
{
    [Fact]
    public void Specification_ShouldInitializeWithDefaultValues()
    {
        var spec = new TestSpecification();

        Assert.Null(spec.Criteria);
        Assert.Empty(spec.Includes);
        Assert.Null(spec.OrderBy);
        Assert.Null(spec.OrderByDescending);
        Assert.Null(spec.Skip);
        Assert.Null(spec.Take);
        Assert.False(spec.AsNoTracking);
    }

    [Fact]
    public void AddCriteria_ShouldSetCriteria()
    {
        var spec = new TestSpecification()
            .WithCriteria(e => e.Id == 1);

        Assert.NotNull(spec.Criteria);
    }

    [Fact]
    public void AddInclude_ShouldAddInclude()
    {
        var spec = new TestSpecification()
            .WithInclude(e => e.Name);

        Assert.Single(spec.Includes);
    }

    [Fact]
    public void ApplyOrderBy_ShouldSetOrderBy()
    {
        var spec = new TestSpecification()
            .WithOrderBy(e => e.Name);

        Assert.NotNull(spec.OrderBy);
        Assert.Null(spec.OrderByDescending);
    }

    [Fact]
    public void ApplyOrderByDescending_ShouldSetOrderByDescending()
    {
        var spec = new TestSpecification()
            .WithOrderByDescending(e => e.Name);

        Assert.NotNull(spec.OrderByDescending);
        Assert.Null(spec.OrderBy);
    }

    [Fact]
    public void ApplyPaging_ShouldSetSkipAndTake()
    {
        var spec = new TestSpecification()
            .WithPaging(10, 20);

        Assert.Equal(10, spec.Skip);
        Assert.Equal(20, spec.Take);
    }

    [Fact]
    public void ApplyAsNoTracking_ShouldSetAsNoTrackingToTrue()
    {
        var spec = new TestSpecification()
            .WithAsNoTracking();

        Assert.True(spec.AsNoTracking);
    }

    [Fact]
    public void Specification_CanCombineMultipleCriteria()
    {
        var spec = new TestSpecification()
            .WithCriteria(e => e.Id > 0)
            .WithOrderBy(e => e.Name)
            .WithPaging(0, 10)
            .WithAsNoTracking();

        Assert.NotNull(spec.Criteria);
        Assert.NotNull(spec.OrderBy);
        Assert.Equal(0, spec.Skip);
        Assert.Equal(10, spec.Take);
        Assert.True(spec.AsNoTracking);
    }
}

public class TestSpecification : Specification<TestSpecEntity>
{
    public TestSpecification WithCriteria(System.Linq.Expressions.Expression<Func<TestSpecEntity, bool>> criteria)
    {
        AddCriteria(criteria);
        return this;
    }

    public TestSpecification WithInclude(System.Linq.Expressions.Expression<Func<TestSpecEntity, object>> include)
    {
        AddInclude(include);
        return this;
    }

    public TestSpecification WithOrderBy(System.Linq.Expressions.Expression<Func<TestSpecEntity, object>> orderBy)
    {
        ApplyOrderBy(orderBy);
        return this;
    }

    public TestSpecification WithOrderByDescending(System.Linq.Expressions.Expression<Func<TestSpecEntity, object>> orderByDescending)
    {
        ApplyOrderByDescending(orderByDescending);
        return this;
    }

    public TestSpecification WithPaging(int skip, int take)
    {
        ApplyPaging(skip, take);
        return this;
    }

    public TestSpecification WithAsNoTracking()
    {
        ApplyAsNoTracking();
        return this;
    }
}

public class TestSpecEntity : IEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
