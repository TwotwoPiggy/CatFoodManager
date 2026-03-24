namespace CatFoodManager.Domain.ValueObjects;

/// <summary>
/// 值对象基类，通过其属性值而非标识符来定义相等性。
/// Value object base class, defining equality through property values rather than identity.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// 获取用于相等性比较的原子值。
    /// Gets the atomic values used for equality comparison.
    /// </summary>
    /// <returns>原子值集合 / Collection of atomic values</returns>
    protected abstract IEnumerable<object> GetAtomicValues();

    /// <summary>
    /// 判断是否与另一个对象相等。
    /// Determines whether this instance is equal to another object.
    /// </summary>
    /// <param name="obj">要比较的对象 / The object to compare</param>
    /// <returns>是否相等 / Whether they are equal</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    /// <summary>
    /// 获取哈希码。
    /// Gets the hash code.
    /// </summary>
    /// <returns>哈希码 / Hash code</returns>
    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// 相等运算符。
    /// Equality operator.
    /// </summary>
    /// <param name="left">左操作数 / Left operand</param>
    /// <param name="right">右操作数 / Right operand</param>
    /// <returns>是否相等 / Whether they are equal</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// 不相等运算符。
    /// Inequality operator.
    /// </summary>
    /// <param name="left">左操作数 / Left operand</param>
    /// <param name="right">右操作数 / Right operand</param>
    /// <returns>是否不相等 / Whether they are not equal</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
