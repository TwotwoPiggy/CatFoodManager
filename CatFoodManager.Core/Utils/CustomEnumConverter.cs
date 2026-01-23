using CatFoodManager.Core.Statics;
using CommonTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Utils
{
	public class CustomEnumConverter : EnumConverter
	{
		public CustomEnumConverter() : base(typeof(ProductType))
		{
		}


		public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
		{
			if (value is string)
			{
				return (value as string).GetEnumFromDescription<ProductType>();
			}
			return base.ConvertFrom(context, culture, value);
		}

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
