using CatFoodManager.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace CatfoodManagement.Tests.Services;

using OcrApiClass = global::CatfoodManagement.Services.Bridge.OcrApi;

public class OcrApiTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IGeminiOcrService> _mockOcrService;
    private readonly OcrApiClass _ocrApi;

    public OcrApiTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockOcrService = new Mock<IGeminiOcrService>();

        var mockScope = new Mock<IServiceScope>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();

        mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IGeminiOcrService))).Returns(_mockOcrService.Object);

        _ocrApi = new OcrApiClass(_mockServiceProvider.Object);
    }

    [Fact]
    public async Task GetModelsAsync_ShouldReturnModels_WhenServiceReturnsModels()
    {
        var models = new List<ModelInfo>
        {
            new ModelInfo("gemini-2.5-flash", "Gemini 2.5 Flash"),
            new ModelInfo("gemini-2.0-flash", "Gemini 2.0 Flash")
        };

        _mockOcrService
            .Setup(x => x.GetModelsAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result = await _ocrApi.GetModelsAsync();
        var deserializedResult = JsonConvert.DeserializeObject<dynamic>(result);

        Assert.NotNull(deserializedResult);
        Assert.True((bool)deserializedResult!.Success);
        _mockOcrService.Verify(x => x.GetModelsAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetModelsAsync_ShouldReturnFailure_WhenExceptionThrown()
    {
        _mockOcrService
            .Setup(x => x.GetModelsAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _ocrApi.GetModelsAsync();
        var deserializedResult = JsonConvert.DeserializeObject<dynamic>(result);

        Assert.NotNull(deserializedResult);
        Assert.False((bool)deserializedResult!.Success);
        Assert.Equal("Test exception", (string)deserializedResult.Message);
    }

    [Fact]
    public async Task ValidateModelAsync_ShouldReturnSuccess_WhenModelIsValid()
    {
        _mockOcrService
            .Setup(x => x.ValidateModelAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _ocrApi.ValidateModelAsync();
        var deserializedResult = JsonConvert.DeserializeObject<dynamic>(result);

        Assert.NotNull(deserializedResult);
        Assert.True((bool)deserializedResult!.Success);
        Assert.True((bool)deserializedResult.IsValid);
    }

    [Fact]
    public async Task ValidateModelAsync_ShouldReturnFailure_WhenExceptionThrown()
    {
        _mockOcrService
            .Setup(x => x.ValidateModelAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Validation failed"));

        var result = await _ocrApi.ValidateModelAsync();
        var deserializedResult = JsonConvert.DeserializeObject<dynamic>(result);

        Assert.NotNull(deserializedResult);
        Assert.False((bool)deserializedResult!.Success);
        Assert.Equal("Validation failed", (string)deserializedResult.Message);
    }
}
