using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class PlatformRegExpServiceTests
{
    private readonly Mock<IRepository<PlatformRegExp>> _repositoryMock;
    private readonly Mock<ILogger<PlatformRegExpService>> _loggerMock;
    private readonly PlatformRegExpService _service;

    public PlatformRegExpServiceTests()
    {
        _repositoryMock = new Mock<IRepository<PlatformRegExp>>();
        _loggerMock = new Mock<ILogger<PlatformRegExpService>>();
        _service = new PlatformRegExpService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        var entities = new List<PlatformRegExp>
        {
            new() { Id = 1, Name = "Platform1", Platform = "Taobao", RegularExpression = "regex1" },
            new() { Id = 2, Name = "Platform2", Platform = "JD", RegularExpression = "regex2" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Platform1", result[0].Name);
        Assert.Equal("Platform2", result[1].Name);
        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEntitiesExist()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlatformRegExp>());

        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }
}
