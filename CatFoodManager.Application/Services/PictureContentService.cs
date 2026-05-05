using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Enums;
using CommonTools;
using OcrApi;
using System.Text.RegularExpressions;

namespace CatFoodManager.Application.Services
{
    /// <summary>
    /// 图片内容服务类，提供图片OCR识别和内容提取功能。
    /// Picture content service class, providing image OCR recognition and content extraction functionality.
    /// </summary>
    public class PictureContentService
    {
        private readonly OCRHelper _ocrHelper;
        private readonly IPlatformRegExpService _regExpService;

        /// <summary>
        /// 构造函数。
        /// Constructor.
        /// </summary>
        /// <param name="ocrHelper">OCR帮助类实例 / OCR helper instance</param>
        /// <param name="regExpService">正则表达式服务实例 / Regular expression service instance</param>
        public PictureContentService(OCRHelper ocrHelper, IPlatformRegExpService regExpService)
        {
            _ocrHelper = ocrHelper;
            _regExpService = regExpService;
        }

        private string _originPicturePath = string.Empty;
        private string? _newPicturePath = string.Empty;

        /// <summary>
        /// 处理后的新图片路径。
        /// New picture path after processing.
        /// </summary>
        public string? NewPicturePath => _newPicturePath;

        /// <summary>
        /// 原始图片路径。
        /// Original picture path.
        /// </summary>
        public string OriginPicturePath => _originPicturePath;

        /// <summary>
        /// 设置正则表达式配置。
        /// Sets the regular expression configuration.
        /// </summary>
        /// <param name="regConfig">正则表达式配置字符串 / Regular expression configuration string</param>
        public void SetRegConfig(string regConfig)
        {
            ConfigManager.SetAppConfig("regPattern", regConfig);
        }

        /// <summary>
        /// 获取正则表达式配置。
        /// Gets the regular expression configuration.
        /// </summary>
        /// <returns>正则表达式配置字符串 / Regular expression configuration string</returns>
        public string? GetRegConfig()
        {
            return ConfigManager.GetAppConfig("regPattern");
        }

        /// <summary>
        /// 获取所有正则表达式配置数组。
        /// Gets all regular expression configuration array.
        /// </summary>
        /// <returns>正则表达式配置数组 / Regular expression configuration array</returns>
        public string[]? GetRegConfigs()
        {
            return GetRegConfig()?.Split(";");
        }

        /// <summary>
        /// 设置Tessdata数据路径。
        /// Sets the Tessdata path.
        /// </summary>
        /// <param name="path">Tessdata路径 / Tessdata path</param>
        public void SetTessdataPath(string path)
        {
            _ocrHelper.SetTessdataPath(path);
        }

        /// <summary>
        /// 从图片中获取文本内容。
        /// Gets text content from picture.
        /// </summary>
        /// <param name="picturePath">图片路径 / Picture path</param>
        /// <param name="newPicturePath">处理后保存的新图片路径 / New picture path after processing</param>
        /// <param name="needReduceNoise">是否需要降噪处理 / Whether noise reduction is needed</param>
        /// <returns>识别出的文本内容 / Recognized text content</returns>
        public string GetContentFromPicture(string picturePath, string? newPicturePath = null, bool needReduceNoise = false)
        {
            _originPicturePath = picturePath;
            _newPicturePath = newPicturePath;

            picturePath = needReduceNoise ? _newPicturePath = _ocrHelper.ReduceImageNoise(picturePath, newPicturePath) : _originPicturePath;

            return _ocrHelper.GetTextFromPicture(picturePath).Replace(" ", string.Empty);
        }

        /// <summary>
        /// 根据识别内容生成猫粮实体。
        /// Generates cat food entity from recognized content.
        /// </summary>
        /// <param name="content">识别的文本内容 / Recognized text content</param>
        /// <param name="regPattern">正则表达式模式 / Regular expression pattern</param>
        /// <param name="fieldInfos">字段信息字典 / Field information dictionary</param>
        /// <param name="picturePath">图片路径 / Picture path</param>
        /// <returns>猫粮实体和店铺名称元组 / Tuple of cat food entity and shop name</returns>
        public (CatFood, string) GenerateCatFood(string content, string regPattern, Dictionary<string, int> fieldInfos, string picturePath)
        {
            var catFood = new CatFood() { PicturePath = picturePath, UpdatedAt = DateTime.Now };
            var regex = new Regex(regPattern, RegexOptions.IgnoreCase);
            var groups = regex.Match(content).Groups;
            catFood.OrderId = groups[fieldInfos["orderId"]].Value;
            catFood.Name = groups[fieldInfos["name"]].Value.Replace("猎粑", "猫粮").Replace("店", "");
            catFood.FoodType = catFood.Name.Contains("猫粮") || catFood.Name.Contains("主食") ? ProductType.CatFood : ProductType.CatSnack;
            catFood.Count = Int32.TryParse(groups[fieldInfos["count"]].Value, out int count) ? count : 1;
            catFood.Price = Double.TryParse(groups[fieldInfos["price"]].Value, out double price) ? price : 1D;
            catFood.PurchasedAt = DateTime.TryParse(groups[fieldInfos["purchasedAt"]].Value, out DateTime purchasedAt) ? purchasedAt : DateTime.Now;
            var shopName = groups[fieldInfos["shopName"]].Value
                                                        .Replace("店", string.Empty)
                                                        .Replace("旗舰", string.Empty)
                                                        .Replace("京东", string.Empty)
                                                        .Replace("自营", string.Empty)
                                                        .Replace("官方", string.Empty)
                                                        .Replace("LEGENDSANDY", string.Empty)
                                                        .Replace("宠物", string.Empty)
                                                        .Replace("食品", string.Empty)
                                                        .Replace("海外", string.Empty);
            return (catFood, shopName);
        }
    }
}
