using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatFoodManager.Core.Statics
{
	public class ColumnHeaders
	{
		private static readonly Dictionary<string, DataGridViewColumn> _catFoodHeaders = new()
		{
			{ "Id", new DataGridViewTextBoxColumn() { DataPropertyName = "Id", HeaderText = "Id", Name = "Id", DisplayIndex = 1 ,ReadOnly = true } },
			{ "OrderId", new DataGridViewTextBoxColumn() { DataPropertyName = "OrderId", HeaderText = "订单号", Name = "OrderId", DisplayIndex = 20 }},
			{ "Name", new DataGridViewTextBoxColumn() { DataPropertyName = "Name", HeaderText = "名称", Name = "Name", DisplayIndex = 30 }},
			{ "Brand", new DataGridViewTextBoxColumn() { DataPropertyName = "BrandName", HeaderText = "品牌", Name = "Brand", DisplayIndex = 40 }},
			{ "FoodType", new DataGridViewTextBoxColumn() { DataPropertyName = "FoodType", HeaderText = "类型", Name = "FoodType" }},
			{ "FoodTypeToShow", new DataGridViewComboBoxColumn { DataSource = Enum.GetValues(typeof(CatFoodType)), DataPropertyName = "FoodType", HeaderText = "类型", Name = "FoodTypeToShow" , DisplayIndex = 41 } },
			{ "Count", new DataGridViewTextBoxColumn(){ DataPropertyName = "Count", HeaderText = "数量", Name = "Count", DisplayIndex = 50 } },
			{ "Price", new DataGridViewTextBoxColumn() { DataPropertyName = "Price", HeaderText = "价格", Name = "Price", DisplayIndex = 60 } },
			{ "Weights", new DataGridViewTextBoxColumn() { DataPropertyName = "Weights", HeaderText = "份量(g)", Name = "Weights", DisplayIndex = 70 }},
			{ "ProductionDate", new DataGridViewTextBoxColumn() { DataPropertyName = "ProductionDate", HeaderText = "生产日期", Name = "ProductionDate", DisplayIndex = 80 } },
			{ "ExpiredDate", new DataGridViewTextBoxColumn() { DataPropertyName = "ExpiredDate", HeaderText = "过期时间", Name = "ExpiredDate", DisplayIndex = 90 } },
			{ "FeededCount", new DataGridViewTextBoxColumn() { DataPropertyName = "FeededCount", HeaderText = "已投喂数量", Name = "FeededCount", DisplayIndex = 100 } },
			{ "Feeded", new DataGridViewCheckBoxColumn() { DataPropertyName = "Feeded", HeaderText = "已投喂", Name = "Feeded" , DisplayIndex = 110 } },
			{ "PicturePath", new DataGridViewTextBoxColumn() { DataPropertyName = "PicturePath", HeaderText = "照片", Name = "PicturePath" } },
			{ "PictureButton" , new DataGridViewButtonColumn(){ HeaderText = "照片", Name = "PictureButton", Text = "查看照片", DisplayIndex = 120 } },
			{ "PurchasedAt", new DataGridViewTextBoxColumn() { DataPropertyName = "PurchasedAt", HeaderText = "购买时间", Name = "PurchasedAt", DisplayIndex = 130 } },
			{ "UpdatedAt", new DataGridViewTextBoxColumn() { DataPropertyName = "UpdatedAt", HeaderText = "更新时间", Name = "UpdatedAt", DisplayIndex = 140 } },
		};
		public static readonly Dictionary<string, DataGridViewColumn> CatFoodHeaders = _catFoodHeaders;

		private static readonly Dictionary<string, DataGridViewColumn> _brandHeaders = new()
		{
			{ "Id", new DataGridViewTextBoxColumn() { DataPropertyName = "Id", HeaderText = "Id", Name = "Id", DisplayIndex = 1 ,ReadOnly = true } },
			{ "Name", new DataGridViewTextBoxColumn() { DataPropertyName = "Name", HeaderText = "名称", Name = "Name", DisplayIndex = 2 } }
		};

		public static readonly Dictionary<string, DataGridViewColumn> BrandHeaders = _brandHeaders;
	}
}
