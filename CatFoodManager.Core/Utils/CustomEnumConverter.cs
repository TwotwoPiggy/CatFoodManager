using CatFoodManager.Core.Statics;
using CommonTools;
using System.ComponentModel;
using System.Globalization;

namespace CatFoodManager.Core.Utils
{
    /// <summary>
    /// 自定义枚举转换器，用于枚举类型与描述文本之间的转换。
    /// Custom enum converter, used for conversion between enum types and description text.
    /// </summary>
    public class CustomEnumConverter : EnumConverter
    {
        /// <summary>
        /// 构造函数，初始化基础枚举类型为ProductType。
        /// Constructor, initializing base enum type to ProductType.
        /// </summary>
        public CustomEnumConverter() : base(typeof(ProductType))
        {
        }

        /// <summary>
        /// 将字符串转换为枚举值。
        /// Converts a string to an enum value.
        /// </summary>
        /// <param name="context">类型描述符上下文 / Type descriptor context</param>
        /// <param name="culture">区域性信息 / Culture info</param>
        /// <param name="value">要转换的值 / Value to convert</param>
        /// <returns>转换后的枚举值 / Converted enum value</returns>
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string)
            {
                return (value as string).GetEnumFromDescription<ProductType>();
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// 将枚举值转换为字符串。
        /// Converts an enum value to a string.
        /// </summary>
        /// <param name="context">类型描述符上下文 / Type descriptor context</param>
        /// <param name="culture">区域性信息 / Culture info</param>
        /// <param name="value">要转换的枚举值 / Enum value to convert</param>
        /// <param name="destinationType">目标类型 / Destination type</param>
        /// <returns>转换后的字符串 / Converted string</returns>
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is ProductType)
            {
                return ((ProductType)value).GetEnumDescription();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
