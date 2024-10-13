using CatFoodManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace CatFoodManager.Core.Interfaces
{
	public interface IService<T>
	{
		void Save(T entity);
		void BatchSave(IEnumerable<T> entities);
		T Query(int id);
		T Query(string name);
		IEnumerable<T> GetAll();
		(IEnumerable<T>, int) GetAllWithCount();
		IEnumerable<T> FuzzyQuery(string queryString);
		void Update(T brand);
		void Delete(int id);
	}
}
