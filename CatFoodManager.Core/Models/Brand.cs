using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CatFoodManager.Core.Models
{
    /// <summary>
    /// 品牌实体，表示猫粮品牌信息。
    /// Brand entity, representing cat food brand information.
    /// </summary>
    public class Brand
    {
        /// <summary>
        /// 品牌唯一标识符。
        /// Unique identifier for the brand.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// 品牌名称。
        /// Name of the brand.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 该品牌下的猫粮产品列表。
        /// List of cat food products under this brand.
        /// </summary>
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public IList<CatFood> CatFoods { get; set; } = [];
    }
}
