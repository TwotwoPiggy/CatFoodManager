using CatFoodManager.Core.Statics;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace CatFoodManager.Core.Models
{
	public class CatFood
	{
		[PrimaryKey, AutoIncrement]
		public long Id { get; set; }

        public string OrderId { get; set; }

        public string Name { get; set; }

		public CatFoodType FoodType => Name.Contains("猫粮") || Name.Contains("主食") ? CatFoodType.CatFood : CatFoodType.CatSnack;

		public int Count { get; set; }

		public double Price { get; set; }

		public int Weights { get; set; }

        public string PicturePath { get; set; }

        public DateTime PurchasedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

		public int FeededCount { get; set; }

		public bool Feeded => Count == FeededCount;


		[ForeignKey(typeof(Brand))]
		public int BrandId { get; set; }
		[ManyToOne]
		public Brand Brand { get; set; }

		[ForeignKey(typeof(Factory))]
		public int FactoryId { get; set; }
		[ManyToOne]
		public Factory Factory { get; set; }
	}
}
