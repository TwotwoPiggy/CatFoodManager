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
		public CustomEnumConverter() : base(typeof(CatFoodType))
		{
		}


		public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
		{
			if (value is string)
			{
				return (value as string).GetEnumFromDescription<CatFoodType>();
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
		{
			if (value is CatFoodType)
			{
				return ((CatFoodType)value).GetEnumDescription();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
