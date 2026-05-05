using CatFoodManager.Domain.Enums;

namespace CatFoodManager.Core.Models.Dtos
{
    /// <summary>
    /// 猫粮数据传输对象，用于数据展示和传输�?    /// Cat food data transfer object, used for data display and transmission.
    /// </summary>
    public class CatFoodDto
    {
        /// <summary>
        /// 图片唯一标识，用于将 OCR 结果与输入图片关联�?        /// </summary>
        public string ImageId { get; set; } = string.Empty;

        /// <summary>
        /// 产品名称�?        /// Product name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 订单编号�?        /// Order ID.
        /// </summary>
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// 食品类型（整数表示）�?        /// Food type (represented as integer).
        /// </summary>
        public int FoodType { get; set; }

        /// <summary>
        /// 购买数量�?        /// Purchase quantity.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 购买价格�?        /// Purchase price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 重量（克）�?        /// Weight in grams.
        /// </summary>
        public int Weights { get; set; }

        /// <summary>
        /// 产品图片路径�?        /// Product image path.
        /// </summary>
        public string PicturePath { get; set; } = string.Empty;

        /// <summary>
        /// 生产日期�?        /// Production date.
        /// </summary>
        public DateTime? ProductionDate { get; set; }

        /// <summary>
        /// 品牌名称�?        /// Brand name.
        /// </summary>
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// 购买平台�?        /// Purchase platform.
        /// </summary>
        public PlatformType Platform { get; set; }
    }
}
