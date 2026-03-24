using CatFoodManager.Application.Common;
using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class CatFoodServiceTests
{
    private readonly Mock<IRepository<CatFood>> _repositoryMock;
    private readonly Mock<ILogger<CatFoodService>> _loggerMock;
    private readonly CatFoodService _service;

    public CatFoodServiceTests()
    {
        _repositoryMock = new Mock<IRepository<CatFood>>();
        _loggerMock = new Mock<ILogger<CatFoodService>>();
        _service = new CatFoodService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var expectedEntity = new CatFood { Id = 1, Name = "Test Food" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntity);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(expectedEntity, result);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CatFood?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var expectedEntity = new CatFood { Id = 1, Name = "Test Food" };
        var entities = new List<CatFood> { expectedEntity };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CatFood, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetByNameAsync("Test Food");

        Assert.Equal(expectedEntity, result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CatFood, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CatFood>());

        var result = await _service.GetByNameAsync("Non-existent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var entities = new List<CatFood>
        {
            new() { Id = 1, Name = "Food1" },
            new() { Id = 2, Name = "Food2" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResult()
    {
        var entities = new List<CatFood>
        {
            new() { Id = 1, Name = "Food1" },
            new() { Id = 2, Name = "Food2" },
            new() { Id = 3, Name = "Food3" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetPagedAsync(1, 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage_WhenPageIsTwo()
    {
        var entities = new List<CatFood>
        {
            new() { Id = 1, Name = "Food1" },
            new() { Id = 2, Name = "Food2" },
            new() { Id = 3, Name = "Food3" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetPagedAsync(2, 2);

        Assert.Single(result.Items);
        Assert.Equal("Food3", result.Items[0].Name);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingEntities()
    {
        var entities = new List<CatFood>
        {
            new() { Id = 1, Name = "Test Food" }
        };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CatFood, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.SearchAsync("Test");

        Assert.Single(result);
        Assert.Equal("Test Food", result[0].Name);
    }

    [Fact]
    public async Task AddAsync_ShouldCallRepositoryAdd()
    {
        var entity = new CatFood { Id = 1, Name = "New Food" };
        _repositoryMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.AddAsync(entity);

        _repositoryMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldCallRepositoryAddRange()
    {
        var entities = new List<CatFood>
        {
            new() { Id = 1, Name = "Food1" },
            new() { Id = 2, Name = "Food2" }
        };
        _repositoryMock.Setup(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.AddRangeAsync(entities);

        _repositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<CatFood>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepositoryUpdate()
    {
        var entity = new CatFood { Id = 1, Name = "Updated Food" };
        _repositoryMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.UpdateAsync(entity);

        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDelete()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }
}
