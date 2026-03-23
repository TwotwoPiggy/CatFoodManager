using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Domain.Entities;

/// <summary>
/// 平台正则表达式实体，用于存储不同平台订单信息提取的正则表达式。
/// Platform regular expression entity, used to store regex patterns for extracting order information from different platforms.
/// </summary>
public class PlatformRegExp : IEntity
{
    /// <summary>
    /// 正则表达式唯一标识符。
    /// Unique identifier for the regular expression.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }

    /// <summary>
    /// 正则表达式名称。
    /// Name of the regular expression.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 适用平台。
    /// Applicable platform.
    /// </summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>
    /// 正则表达式字符串。
    /// Regular expression string.
    /// </summary>
    public string RegularExpression { get; set; } = string.Empty;

    /// <summary>
    /// 字段信息的JSON格式字符串。
    /// Field information in JSON format string.
    /// </summary>
    public string FieldInfos { get; set; } = string.Empty;
}
