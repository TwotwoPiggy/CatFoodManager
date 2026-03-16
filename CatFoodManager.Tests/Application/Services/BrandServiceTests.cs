using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class BrandServiceTests
{
    private readonly Mock<IRepository<Brand>> _repositoryMock;
    private readonly Mock<ILogger<BrandService>> _loggerMock;
    private readonly BrandService _service;

    public BrandServiceTests()
    {
        _repositoryMock = new Mock<IRepository<Brand>>();
        _loggerMock = new Mock<ILogger<BrandService>>();
        _service = new BrandService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var expectedEntity = new Brand { Id = 1, Name = "Test Brand" };
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
            .ReturnsAsync((Brand?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnEntity_WhenEntityExists()
    {
        var expectedEntity = new Brand { Id = 1, Name = "Test Brand" };
        var entities = new List<Brand> { expectedEntity };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetByNameAsync("Test Brand");

        Assert.Equal(expectedEntity, result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Brand, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Brand>());

        var result = await _service.GetByNameAsync("Non-existent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var entities = new List<Brand>
        {
            new() { Id = 1, Name = "Brand1" },
            new() { Id = 2, Name = "Brand2" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldCallRepositoryAdd()
    {
        var entity = new Brand { Id = 1, Name = "New Brand" };
        _repositoryMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.AddAsync(entity);

        _repositoryMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepositoryUpdate()
    {
        var entity = new Brand { Id = 1, Name = "Updated Brand" };
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
