using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Domain.Entities;

public class Brand : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string? Name { get; set; }
}
