using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Interfaces
{
	public interface IServiceBase
	{
		void Migrate<T>();
	}
}
