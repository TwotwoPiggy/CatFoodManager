using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 实体基类，包含通用的ID、名称和时间戳属性，以及领域事件支持�?/// Base entity class, containing common ID, name, timestamp properties, and domain event support.
/// </summary>
public abstract class BaseEntity : IEntity
{
    /// <summary>
    /// 实体唯一标识符�?    /// Unique identifier for the entity.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }

    /// <summary>
    /// 实体名称�?    /// Name of the entity.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 创建时间�?    /// Creation timestamp.
    /// </summary>
    [Ignore]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 更新时间�?    /// Last update timestamp.
    /// </summary>
    [Ignore]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// 购买时间�?    /// Purchase timestamp.
    /// </summary>
    [Ignore]
    public DateTimeOffset? PurchasedAt { get; set; }

    /// <summary>
    /// 创建时间的字符串表示，用于数据库存储�?    /// String representation of creation time for database storage.
    /// </summary>
    [Column("CreatedAt")]
    public string CreatedAtString
    {
        get => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        set => CreatedAt = ParseDateTimeOffset(value);
    }

    /// <summary>
    /// 更新时间的字符串表示，用于数据库存储�?    /// String representation of update time for database storage.
    /// </summary>
    [Column("UpdatedAt")]
    public string? UpdatedAtString
    {
        get => UpdatedAt.HasValue ? UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
        set => UpdatedAt = string.IsNullOrEmpty(value) ? null : ParseDateTimeOffset(value);
    }

    /// <summary>
    /// 购买时间的字符串表示，用于数据库存储�?    /// String representation of purchase time for database storage.
    /// </summary>
    [Column("PurchasedAt")]
    public string? PurchasedAtString
    {
        get => PurchasedAt.HasValue ? PurchasedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
        set => PurchasedAt = string.IsNullOrEmpty(value) ? null : ParseDateTimeOffset(value);
    }

    /// <summary>
    /// 解析日期时间字符串，支持时间戳和日期格式。
    /// Parses datetime string, supporting both timestamp and date formats.
    /// </summary>
    /// <param name="value">日期时间字符串 / Datetime string</param>
    /// <returns>解析后的日期时间 / Parsed datetime</returns>
    private static DateTimeOffset ParseDateTimeOffset(string value)
    {
        if (long.TryParse(value, out var ticks))
        {
            return new DateTimeOffset(ticks, TimeSpan.Zero);
        }
        
        if (DateTimeOffset.TryParse(value, out var result))
        {
            return result;
        }
        
        return DateTimeOffset.MinValue;
    }

    /// <summary>
    /// 领域事件列表�?    /// List of domain events.
    /// </summary>
    private readonly List<Func<Task>> _domainEvents = new();

    /// <summary>
    /// 获取领域事件的只读集合�?    /// Gets the read-only collection of domain events.
    /// </summary>
    public IReadOnlyCollection<Func<Task>> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// 添加领域事件�?    /// Adds a domain event.
    /// </summary>
    /// <param name="eventHandler">事件处理函数 / Event handler function</param>
    public void AddDomainEvent(Func<Task> eventHandler)
    {
        _domainEvents.Add(eventHandler);
    }

    /// <summary>
    /// 移除领域事件�?    /// Removes a domain event.
    /// </summary>
    /// <param name="eventHandler">事件处理函数 / Event handler function</param>
    public void RemoveDomainEvent(Func<Task> eventHandler)
    {
        _domainEvents.Remove(eventHandler);
    }

    /// <summary>
    /// 清除所有领域事件�?    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
