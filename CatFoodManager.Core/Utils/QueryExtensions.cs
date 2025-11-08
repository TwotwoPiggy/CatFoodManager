using CommonTools;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Utils
{
	public static class QueryExtensions
	{
		public static void GetChildren<T>(this List<T> entities, SQLiteConnection db, bool recursive = false)
		{
			foreach (var entity in entities)
			{
				db.GetChildren(entity, recursive);
			}
		}
	}
}
