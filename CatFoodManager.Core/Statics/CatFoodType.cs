using CatFoodManager.Core.Utils;
using System.ComponentModel;

namespace CatFoodManager.Core.Statics
{
    /// <summary>
    /// 产品类型枚举，定义猫粮产品的分类。
    /// Product type enum, defining categories of cat food products.
    /// </summary>
    [TypeConverter(typeof(CustomEnumConverter))]
    public enum ProductType
    {
        /// <summary>
        /// 猫粮。
        /// Cat food.
        /// </summary>
        [Description("猫粮")]
        CatFood = 0,

        /// <summary>
        /// 零食。
        /// Cat snack.
        /// </summary>
        [Description("零食")]
        CatSnack = 1,

        /// <summary>
        /// 主食罐头。
        /// Canned food.
        /// </summary>
        [Description("主食罐头")]
        CannedFood = 2,

        /// <summary>
        /// 主食冻干。
        /// Freeze-dried food.
        /// </summary>
        [Description("主食冻干")]
        FreezeDriedFood = 3,

        /// <summary>
        /// 其他。
        /// Others.
        /// </summary>
        [Description("其他")]
        Others = 4
    }
}
