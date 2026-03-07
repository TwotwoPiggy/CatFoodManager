using CatFoodManager.Core.Utils;
using System.ComponentModel;

namespace CatFoodManager.Core.Statics
{
	[TypeConverter(typeof(CustomEnumConverter))]
	public enum ProductType
    {
        [Description("猫粮")]
        CatFood = 0,

		[Description("零食")]
		CatSnack = 1,

        [Description("主食罐头")]
        CannedFood = 2,

        [Description("主食冻干")]
        FreezeDriedFood = 3,

        [Description("其他")]
        Others = 4


    }
}
