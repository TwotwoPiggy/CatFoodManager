using CatFoodManager.Domain.Interfaces;

namespace CatFoodManager.Domain.Entities;

public class PlatformRegExp : IEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string RegularExpression { get; set; } = string.Empty;
    public string FieldInfos { get; set; } = string.Empty;
}
