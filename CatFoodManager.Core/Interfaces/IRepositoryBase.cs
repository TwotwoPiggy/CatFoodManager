using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Interfaces
{
	public interface IRepositoryBase
	{
		void Migrate<T>();

		void Dispose();
	}
}
