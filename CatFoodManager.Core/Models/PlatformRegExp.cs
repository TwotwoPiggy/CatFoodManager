using CatFoodManager.Core.Statics;
using Newtonsoft.Json;
using SQLite;

namespace CatFoodManager.Core.Models
{
    /// <summary>
    /// 平台正则表达式实体，用于存储不同平台订单信息提取的正则表达式。
    /// Platform regular expression entity, used to store regex patterns for extracting order information from different platforms.
    /// </summary>
    public class PlatformRegExp
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
        /// 适用平台类型。
        /// Applicable platform type.
        /// </summary>
        public PlatformType Platform { get; set; }

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

        /// <summary>
        /// 字段信息字典，用于存储字段名和对应的正则组索引。
        /// Field information dictionary, storing field names and corresponding regex group indices.
        /// </summary>
        [Ignore]
        public Dictionary<string, int>? FieldInfoList { get; set; }

        /// <summary>
        /// 根据操作类型自动填充字段。
        /// Auto-fills fields based on the operation type.
        /// </summary>
        /// <param name="isReadAction">是否为读取操作 / Whether it's a read operation</param>
        public void AutoFillFields(bool isReadAction)
        {
            SetName();
            if (isReadAction)
            {
                SetFieldInfoList();
            }
            else
            {
                SetFieldInfos();
            }
        }

        /// <summary>
        /// 设置名称为平台类型的字符串表示。
        /// Sets the name to the string representation of the platform type.
        /// </summary>
        private void SetName()
        {
            Name = Platform.ToString();
        }

        /// <summary>
        /// 将字段信息列表序列化为JSON字符串。
        /// Serializes field information list to JSON string.
        /// </summary>
        private void SetFieldInfos()
        {
            FieldInfos = FieldInfoList == null ? string.Empty : JsonConvert.SerializeObject(FieldInfoList);
        }

        /// <summary>
        /// 从JSON字符串反序列化字段信息列表。
        /// Deserializes field information list from JSON string.
        /// </summary>
        private void SetFieldInfoList()
        {
            FieldInfoList = string.IsNullOrWhiteSpace(FieldInfos)
                            ? default
                            : JsonConvert.DeserializeObject<Dictionary<string, int>?>(FieldInfos);
        }
    }
}
