using CatFoodManager.Core.Statics;
using SQLite;
using SQLiteNetExtensions.Attributes;


namespace CatFoodManager.Core.Models
{
	public class CatFood
	{
		[PrimaryKey, AutoIncrement]
		public long Id { get; set; }

		public string Name { get; set; }

		public CatFoodType FoodType { get; set; }

		public int Count { get; set; }

		public double Price { get; set; }

		public int Weights { get; set; }
		
        public DateTime PurchasedAt { get; set; }

        public bool Feeded { get; set; }

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
