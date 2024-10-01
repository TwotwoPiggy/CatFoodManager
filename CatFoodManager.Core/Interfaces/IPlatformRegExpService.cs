using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatFoodManager.Core.Interfaces
{
	public interface IPlatformRegExpService: IService<PlatformRegExp>
	{
		IEnumerable<PlatformRegExp> GetRegExpByPlatform(PlatformType platformType);
	}
}
