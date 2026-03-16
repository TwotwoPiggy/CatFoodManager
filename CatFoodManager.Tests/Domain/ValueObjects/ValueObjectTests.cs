using CatFoodManager.Domain.ValueObjects;
using Xunit;

namespace CatFoodManager.Tests.Domain.ValueObjects;

public class ValueObjectTests
{
    [Fact]
    public void Equals_WithNullObject_ShouldReturnFalse()
    {
        var valueObject = new TestValueObject("test", 1);

        Assert.False(valueObject.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var valueObject = new TestValueObject("test", 1);

        Assert.False(valueObject.Equals("test"));
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var valueObject1 = new TestValueObject("test", 1);
        var valueObject2 = new TestValueObject("test", 1);

        Assert.True(valueObject1.Equals(valueObject2));
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        var valueObject1 = new TestValueObject("test", 1);
        var valueObject2 = new TestValueObject("test", 2);

        Assert.False(valueObject1.Equals(valueObject2));
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        var valueObject1 = new TestValueObject("test", 1);
        var valueObject2 = new TestValueObject("test", 1);

        Assert.Equal(valueObject1.GetHashCode(), valueObject2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCode()
    {
        var valueObject1 = new TestValueObject("test", 1);
        var valueObject2 = new TestValueObject("test", 2);

        Assert.NotEqual(valueObject1.GetHashCode(), valueObject2.GetHashCode());
    }

    [Fact]
    public void EqualityOperator_WithNullBoth_ShouldReturnTrue()
    {
        TestValueObject? valueObject1 = null;
        TestValueObject? valueObject2 = null;

        Assert.True(valueObject1 == valueObject2);
    }

    [Fact]
    public void EqualityOperator_WithNullLeft_ShouldReturnFalse()
    {
        TestValueObject? valueObject1 = null;
        var valueObject2 = new TestValueObject("test", 1);

        Assert.False(valueObject1 == valueObject2);
    }

    [Fact]
    public void EqualityOperator_WithNullRight_ShouldReturnFalse()
    {
        var valueObject1 = new TestValueObject("test", 1);
        TestValueObject? valueObject2 = null;

        Assert.False(valueObject1 == valueObject2);
    }

    [Fact]
    public void EqualityOperator_WithSameValues_ShouldReturnTrue()
    {
        var valueObject1 = new TestValueObject("test", 1);
        var valueObject2 = new TestValueObject("test", 1);

        Assert.True(valueObject1 == valueObject2);
    }

    [Fact]
    public void InequalityOperator_WithDifferentValues_ShouldReturnTrue()
    {
        var valueObject1 = new TestValueObject("test", 1);
        var valueObject2 = new TestValueObject("test", 2);

        Assert.True(valueObject1 != valueObject2);
    }

    private class TestValueObject : ValueObject
    {
        public string Value1 { get; }
        public int Value2 { get; }

        public TestValueObject(string value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value1;
            yield return Value2;
        }
    }
}
