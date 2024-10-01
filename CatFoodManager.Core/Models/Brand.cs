using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Models
{
	public class Brand
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Name { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public IList<CatFood> CatFoods { get; set; }

		//[ManyToMany(typeof(Factory), CascadeOperations = CascadeOperation.All)]
		//public IList<Factory> Factories { get; set; }
		
    }
}
