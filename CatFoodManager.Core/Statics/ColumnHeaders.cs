using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Statics
{
	public class ColumnHeaders
	{
		public static Dictionary<string, string> CatFoodHeaders = new Dictionary<string, string>
		{
			{ "Id", "Id" },
			{ "OrderId", "订单号" },
			{ "Name", "名称" },
			{ "FoodTypeName", "类型" },
			{ "Count", "数量" },
			{ "Price", "价格" },
			{ "Weights", "份量(g)" },
			{ "PurchasedAt", "购买时间" },
			{ "UpdatedAt", "更新时间" },
			{ "FeededCount", "已投喂数量" },
			{ "Feeded", "是否已全部投喂" },
			{ "PicturePath", "照片" }
		};
	}
}
