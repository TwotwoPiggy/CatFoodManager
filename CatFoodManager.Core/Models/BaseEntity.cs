using SQLite;
using System.Runtime.Serialization;

namespace CatFoodManager.Core.Models
{
    /// <summary>
    /// 实体基类，包含通用的ID、名称和时间戳属性。
    /// Base entity class, containing common ID, name, and timestamp properties.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 实体唯一标识符。
        /// Unique identifier for the entity.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// 实体名称。
        /// Name of the entity.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间。
        /// Creation timestamp.
        /// </summary>
        [Ignore]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间。
        /// Last update timestamp.
        /// </summary>
        [Ignore]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 购买时间。
        /// Purchase timestamp.
        /// </summary>
        [Ignore]
        public DateTime? PurchasedAt { get; set; }

        /// <summary>
        /// 创建时间的字符串表示，用于数据库存储。
        /// String representation of creation time for database storage.
        /// </summary>
        [Column("CreatedAt")]
        public string CreatedAtString
        {
            get => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            set => CreatedAt = ParseDateTime(value);
        }

        /// <summary>
        /// 更新时间的字符串表示，用于数据库存储。
        /// String representation of update time for database storage.
        /// </summary>
        [Column("UpdatedAt")]
        public string? UpdatedAtString
        {
            get => UpdatedAt.HasValue ? UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
            set => UpdatedAt = string.IsNullOrEmpty(value) ? null : ParseDateTime(value);
        }

        /// <summary>
        /// 购买时间的字符串表示，用于数据库存储。
        /// String representation of purchase time for database storage.
        /// </summary>
        [Column("PurchasedAt")]
        public string? PurchasedAtString
        {
            get => PurchasedAt.HasValue ? PurchasedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;
            set => PurchasedAt = string.IsNullOrEmpty(value) ? null : ParseDateTime(value);
        }

        /// <summary>
        /// 解析日期时间字符串，支持时间戳和日期格式。
        /// Parses datetime string, supporting both timestamp and date formats.
        /// </summary>
        /// <param name="value">日期时间字符串 / Datetime string</param>
        /// <returns>解析后的日期时间 / Parsed datetime</returns>
        private static DateTime ParseDateTime(string value)
        {
            if (long.TryParse(value, out var ticks))
            {
                return new DateTime(ticks);
            }
            return DateTime.Parse(value);
        }
    }
}
