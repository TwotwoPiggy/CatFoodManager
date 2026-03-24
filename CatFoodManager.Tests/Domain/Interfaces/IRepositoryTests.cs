using CatFoodManager.Domain.Interfaces;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Domain.Interfaces;

public class IRepositoryTests
{
    private readonly Mock<IRepository<TestEntity>> _repositoryMock;

    public IRepositoryTests()
    {
        _repositoryMock = new Mock<IRepository<TestEntity>>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var expectedEntity = new TestEntity { Id = 1, Name = "Test" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntity);

        var result = await _repositoryMock.Object.GetByIdAsync(1);

        Assert.Equal(expectedEntity, result);
        _repositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestEntity?)null);

        var result = await _repositoryMock.Object.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var entities = new List<TestEntity>
        {
            new() { Id = 1, Name = "Test1" },
            new() { Id = 2, Name = "Test2" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _repositoryMock.Object.GetAllAsync();

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredEntities()
    {
        var entities = new List<TestEntity>
        {
            new() { Id = 1, Name = "Test1" }
        };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _repositoryMock.Object.FindAsync(e => e.Name == "Test1");

        Assert.Single(result);
        Assert.Equal("Test1", result[0].Name);
    }

    [Fact]
    public async Task AddAsync_ShouldCallAdd()
    {
        var entity = new TestEntity { Id = 1, Name = "Test" };
        _repositoryMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repositoryMock.Object.AddAsync(entity);

        _repositoryMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldCallAddRange()
    {
        var entities = new List<TestEntity>
        {
            new() { Id = 1, Name = "Test1" },
            new() { Id = 2, Name = "Test2" }
        };
        _repositoryMock.Setup(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repositoryMock.Object.AddRangeAsync(entities);

        _repositoryMock.Verify(r => r.AddRangeAsync(entities, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallUpdate()
    {
        var entity = new TestEntity { Id = 1, Name = "Updated" };
        _repositoryMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repositoryMock.Object.UpdateAsync(entity);

        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDelete()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _repositoryMock.Object.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        _repositoryMock.Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _repositoryMock.Object.CountAsync();

        Assert.Equal(5, result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        _repositoryMock.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _repositoryMock.Object.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        _repositoryMock.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _repositoryMock.Object.ExistsAsync(999);

        Assert.False(result);
    }
}

public class TestEntity : IEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
