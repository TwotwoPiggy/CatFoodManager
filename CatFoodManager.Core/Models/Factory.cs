using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CatFoodManager.Core.Models
{
    /// <summary>
    /// 代工厂实体，表示猫粮代工厂信息。
    /// Factory entity, representing cat food factory information.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// 代工厂唯一标识符。
        /// Unique identifier for the factory.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// 代工厂名称。
        /// Name of the factory.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 该代工厂生产的猫粮产品列表。
        /// List of cat food products produced by this factory.
        /// </summary>
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public IList<CatFood> CatFoods { get; set; } = [];
    }
}
