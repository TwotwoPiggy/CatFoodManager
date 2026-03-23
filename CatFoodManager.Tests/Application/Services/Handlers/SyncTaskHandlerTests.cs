using System.Text.Json;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Application.Services.Handlers;
using CatFoodManager.Core.Models.Dtos;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services.Handlers;

public class SyncTaskHandlerTests
{
    private readonly Mock<IGeminiOcrService> _ocrServiceMock;
    private readonly Mock<IBestPriceService> _bestPriceServiceMock;
    private readonly Mock<ILogger<SyncTaskHandler>> _loggerMock;
    private readonly SyncTaskHandler _handler;

    public SyncTaskHandlerTests()
    {
        _ocrServiceMock = new Mock<IGeminiOcrService>();
        _bestPriceServiceMock = new Mock<IBestPriceService>();
        _loggerMock = new Mock<ILogger<SyncTaskHandler>>();
        _handler = new SyncTaskHandler(
            _ocrServiceMock.Object,
            _bestPriceServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void TaskType_ShouldReturnImageSync()
    {
        Assert.Equal(TaskType.ImageSync, _handler.TaskType);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailed_WhenParametersIsNull()
    {
        var result = await _handler.HandleAsync("null");

        Assert.False(result.Success);
        Assert.Equal("Invalid parameters: FolderPath is required", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailed_WhenFolderPathIsEmpty()
    {
        var parameters = JsonSerializer.Serialize(new { FolderPath = "", PromptText = "test" });

        var result = await _handler.HandleAsync(parameters);

        Assert.False(result.Success);
        Assert.Equal("Invalid parameters: FolderPath is required", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailed_WhenFolderPathIsMissing()
    {
        var parameters = JsonSerializer.Serialize(new { PromptText = "test" });

        var result = await _handler.HandleAsync(parameters);

        Assert.False(result.Success);
        Assert.Equal("Invalid parameters: FolderPath is required", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_ShouldAcceptCamelCaseParameters()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var parameters = JsonSerializer.Serialize(new
            {
                folderPath = tempPath,
                promptText = "test prompt",
                platform = 2
            });

            _ocrServiceMock.Setup(x => x.ProcessPicturesAsync<BestPriceSyncDto>(
                tempPath, "test prompt", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProcessPicturesResult<BestPriceSyncDto>(new List<BestPriceSyncDto>(), null));

            var result = await _handler.HandleAsync(parameters);

            Assert.True(result.Success);
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSucceeded_WhenNoResultsReturned()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var parameters = JsonSerializer.Serialize(new
            {
                FolderPath = tempPath,
                PromptText = "test prompt",
                Platform = 2
            });

            _ocrServiceMock.Setup(x => x.ProcessPicturesAsync<BestPriceSyncDto>(
                tempPath, "test prompt", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProcessPicturesResult<BestPriceSyncDto>(new List<BestPriceSyncDto>(), "test-response-id"));

            var result = await _handler.HandleAsync(parameters);

            Assert.True(result.Success);
            Assert.NotNull(result.Result);
            Assert.Equal("test-response-id", result.ResponseId);
            _bestPriceServiceMock.Verify(
                x => x.AddRangeAsync(It.IsAny<IEnumerable<CatFoodManager.Domain.Entities.BestPrice>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldSaveResultsToDatabase_WhenResultsReturned()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var parameters = JsonSerializer.Serialize(new
            {
                FolderPath = tempPath,
                PromptText = "test prompt",
                Platform = 2
            });

            var ocrResults = new List<BestPriceSyncDto>
            {
                new()
                {
                    Name = "Test Product 1",
                    Type = 0,
                    Platform = 2,
                    LowestPrice = 99.99m,
                    FinalPrice = 89.99m,
                    PicturePath = "/path/to/image1.jpg",
                    FactoryName = "Factory A",
                    HasTestReport = true,
                    IsWorthRepurchasing = true,
                    PurchasedAt = DateTime.Now
                },
                new()
                {
                    Name = "Test Product 2",
                    Type = 1,
                    Platform = 0,
                    LowestPrice = 199.99m,
                    FinalPrice = null,
                    PicturePath = "/path/to/image2.jpg",
                    FactoryName = "Factory B",
                    HasTestReport = false,
                    IsWorthRepurchasing = false,
                    PurchasedAt = null
                }
            };

            _ocrServiceMock.Setup(x => x.ProcessPicturesAsync<BestPriceSyncDto>(
                tempPath, "test prompt", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProcessPicturesResult<BestPriceSyncDto>(ocrResults, "test-response-id-123"));

            _bestPriceServiceMock.Setup(x => x.AddRangeAsync(
                It.IsAny<IEnumerable<CatFoodManager.Domain.Entities.BestPrice>>(),
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(parameters);

            Assert.True(result.Success);
            Assert.NotNull(result.Result);
            Assert.Equal("test-response-id-123", result.ResponseId);

            _bestPriceServiceMock.Verify(
                x => x.AddRangeAsync(
                    It.Is<IEnumerable<CatFoodManager.Domain.Entities.BestPrice>>(list =>
                        list.Count() == 2),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldUseFallbackPlatform_WhenDtoPlatformIsZero()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var parameters = JsonSerializer.Serialize(new
            {
                FolderPath = tempPath,
                PromptText = "test prompt",
                Platform = 3
            });

            var ocrResults = new List<BestPriceSyncDto>
            {
                new()
                {
                    Name = "Test Product",
                    Type = 0,
                    Platform = 0,
                    LowestPrice = 50m
                }
            };

            _ocrServiceMock.Setup(x => x.ProcessPicturesAsync<BestPriceSyncDto>(
                tempPath, "test prompt", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProcessPicturesResult<BestPriceSyncDto>(ocrResults, null));

            IEnumerable<CatFoodManager.Domain.Entities.BestPrice>? savedEntities = null;
            _bestPriceServiceMock.Setup(x => x.AddRangeAsync(
                It.IsAny<IEnumerable<CatFoodManager.Domain.Entities.BestPrice>>(),
                It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<CatFoodManager.Domain.Entities.BestPrice>, CancellationToken>(
                    (entities, _) => savedEntities = entities.ToList())
                .Returns(Task.CompletedTask);

            var result = await _handler.HandleAsync(parameters);

            Assert.True(result.Success);
            Assert.NotNull(savedEntities);
            var savedList = savedEntities.ToList();
            Assert.Single(savedList);
            Assert.Equal(PlatformType.PDD, savedList[0].Platform);
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailed_WhenExceptionThrown()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var parameters = JsonSerializer.Serialize(new
            {
                FolderPath = tempPath,
                PromptText = "test prompt",
                Platform = 2
            });

            _ocrServiceMock.Setup(x => x.ProcessPicturesAsync<BestPriceSyncDto>(
                tempPath, "test prompt", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("OCR service error"));

            var result = await _handler.HandleAsync(parameters);

            Assert.False(result.Success);
            Assert.Equal("OCR service error", result.ErrorMessage);
        }
        finally
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath);
        }
    }
}
