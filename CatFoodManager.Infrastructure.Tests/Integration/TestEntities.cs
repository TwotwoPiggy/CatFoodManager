using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using SQLite;

namespace CatFoodManager.Infrastructure.Tests.Integration;

public class TestBrand : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string? Name { get; set; }
}

public class TestFactory : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? PurchasedAt { get; set; }
}

public class TestCatFood : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? OrderId { get; set; }
    public int FoodType { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
    public int Weights { get; set; }
    public string? PicturePath { get; set; }
    public DateTimeOffset ProductionDate { get; set; }
    public int FeededCount { get; set; }
    public long BrandId { get; set; }
    public int FactoryId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? PurchasedAt { get; set; }
}

public class TestBestPrice : IEntity
{
    [PrimaryKey, AutoIncrement]
    public long Id { get; set; }
    public string? Name { get; set; }
    public int Type { get; set; }
    public int Platform { get; set; }
    public decimal LowestPrice { get; set; }
    public bool HasPurchased { get; set; }
    public decimal? FinalPrice { get; set; }
    public string? PicturePath { get; set; }
    public string? FactoryName { get; set; }
    public bool HasTestReport { get; set; }
    public bool IsWorthRepurchasing { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? PurchasedAt { get; set; }
}

public class TestEntityFromBaseEntity : BaseEntity
{
    public string? Description { get; set; }
}
