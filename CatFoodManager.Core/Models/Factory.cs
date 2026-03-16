using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CatFoodManager.Core.Models
{
    public class Factory
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public IList<CatFood> CatFoods { get; set; } = [];
    }
}
