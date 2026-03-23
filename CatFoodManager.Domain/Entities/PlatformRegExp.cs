using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Domain.Entities;

public class PlatformRegExp : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string RegularExpression { get; set; } = string.Empty;
    public string FieldInfos { get; set; } = string.Empty;
}
