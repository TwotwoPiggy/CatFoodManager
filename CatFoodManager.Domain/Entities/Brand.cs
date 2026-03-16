using CatFoodManager.Domain.Interfaces;

namespace CatFoodManager.Domain.Entities;

public class Brand : IEntity
{
    public long Id { get; set; }
    public string? Name { get; set; }
}
