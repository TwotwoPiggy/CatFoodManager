using CatFoodManager.Core.Statics;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Runtime.Serialization;


namespace CatFoodManager.Core.Models
{
	public class CatFood : BaseEntity
    {
		public string OrderId { get; set; }

		public ProductType FoodType { get; set; }

		public int Count { get; set; }

		public double Price { get; set; }

		public int Weights { get; set; }

		public string PicturePath { get; set; }

		public DateTime ProductionDate { get; set; }

		public int FeededCount { get; set; }

		[Ignore]
		public bool Feeded
		{
			get
			{
				return Count == FeededCount;
			}
			set
			{
				FeededCount = value ? Count : FeededCount;
			}
		}

		[ForeignKey(typeof(Brand))]
		public long BrandId { get; set; }
		[ManyToOne]
		public Brand Brand { get; set; }
		
		[Ignore]
		public string BrandName => Brand?.Name;

		[ForeignKey(typeof(Factory))]
		public int FactoryId { get; set; }
		[ManyToOne]
		public Factory Factory { get; set; }
	}

}
