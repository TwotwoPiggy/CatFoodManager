using CatFoodManager.Core.Utils;
using System.ComponentModel;

namespace CatFoodManager.Core.Statics
{
	[TypeConverter(typeof(CustomEnumConverter))]
	public enum CatFoodType
    {
        [Description("主食")]
        CatFood = 0,

		[Description("零食")]
		CatSnack = 1
    }
}
