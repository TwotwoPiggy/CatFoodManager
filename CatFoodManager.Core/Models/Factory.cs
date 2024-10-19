﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Models
{
	public class Factory
	{
		[PrimaryKey, AutoIncrement]
		public long Id { get; set; }

		public string Name { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public IList<CatFood> CatFoods { get; set; }

		//[ManyToMany(typeof(Brand), CascadeOperations = CascadeOperation.All)]
		//public IEnumerable<Brand> Brands { get; set; }
	}
}
