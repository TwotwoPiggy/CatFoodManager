using CatFoodManager.Core.Statics;
using SQLite;

namespace CatFoodManager.Core.Models
{
	public class PlatformRegExp
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Name => Platform.ToString();

        public PlatformType Platform { get; set; }

        public string RegularExpression { get; set; }

		public Dictionary<string,int> FieldInfos { get; set; }
	}
}
