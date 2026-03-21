using CatFoodManager.Application.Services;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Google.GenAI.Types;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Twotwo.Agent.Interfaces;
using Twotwo.Agent.Types;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class GeminiOcrServiceTests
{
    private readonly Mock<IGeminiAgentService> _agentServiceMock;
    private readonly Mock<IRepository> _repositoryMock;
    private readonly Mock<ILogger<GeminiOcrService>> _loggerMock;
    private readonly MemoryCache _memoryCache;
    private readonly GeminiOcrService _service;

    public GeminiOcrServiceTests()
    {
        _agentServiceMock = new Mock<IGeminiAgentService>();
        _repositoryMock = new Mock<IRepository>();
        _loggerMock = new Mock<ILogger<GeminiOcrService>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _service = new GeminiOcrService(_agentServiceMock.Object, _repositoryMock.Object, _loggerMock.Object, _memoryCache);
    }

    [Fact]
    public async Task ValidateModelAsync_ShouldReturnTrue_WhenModelIsValid()
    {
        _agentServiceMock.Setup(a => a.ValidateModelAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.ValidateModelAsync();

        Assert.True(result);
        _agentServiceMock.Verify(a => a.ValidateModelAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateModelAsync_ShouldReturnFalse_WhenModelIsInvalid()
    {
        _agentServiceMock.Setup(a => a.ValidateModelAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.ValidateModelAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldThrowArgumentNullException_WhenFolderPathIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ProcessPicturesAsync<TestDto>(null!, "prompt"));
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldThrowArgumentNullException_WhenFolderPathIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ProcessPicturesAsync<TestDto>("", "prompt"));
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldThrowDirectoryNotFoundException_WhenDirectoryDoesNotExist()
    {
        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => _service.ProcessPicturesAsync<TestDto>("non-existent-path", "prompt"));
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldReturnEmptyList_WhenResponseIsNull()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync((GeminiResponse)null!);

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldReturnEmptyList_WhenResponseTextIsEmpty()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var response = new GeminiResponse("")
            {
                Text = "",
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldReturnDeserializedData_WhenResponseIsValid()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"Value\":123}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-id",
                ModelVersion = "test-model",
                UsageMetadata = new Google.GenAI.Types.GenerateContentResponseUsageMetadata
                {
                    TotalTokenCount = 20,
                    PromptTokenCount = 10
                }
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            _repositoryMock.Setup(r => r.Add(It.IsAny<GeminiResponseEntity>()))
                .Verifiable();

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Single(result);
            Assert.Equal("Test", result[0].Name);
            Assert.Equal(123, result[0].Value);
            _repositoryMock.Verify(r => r.Add(It.IsAny<GeminiResponseEntity>()), Times.Once);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldHandleJsonWithMarkdown()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "```json\n[{\"Name\":\"Test\",\"Value\":456}]\n```";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Single(result);
            Assert.Equal("Test", result[0].Name);
            Assert.Equal(456, result[0].Value);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldReturnEmptyList_OnException()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task GetModelsAsync_ShouldReturnModels_WhenAgentReturnsModels()
    {
        var models = new List<Model>
        {
            new Model { Name = "gemini-2.5-flash", DisplayName = "Gemini 2.5 Flash" },
            new Model { Name = "gemini-2.0-flash", DisplayName = "Gemini 2.0 Flash" }
        };

        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result = await _service.GetModelsAsync("test-api-key");

        Assert.Equal(2, result.Count);
        Assert.Equal("gemini-2.5-flash", result[0].Name);
        Assert.Equal("Gemini 2.5 Flash", result[0].DisplayName);
        Assert.Equal("gemini-2.0-flash", result[1].Name);
        Assert.Equal("Gemini 2.0 Flash", result[1].DisplayName);
        _agentServiceMock.Verify(a => a.GetModelsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetModelsAsync_ShouldReturnEmptyList_WhenAgentReturnsEmptyList()
    {
        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Model>());

        var result = await _service.GetModelsAsync("test-api-key");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetModelsAsync_ShouldCacheModels_WhenCalledMultipleTimes()
    {
        var models = new List<Model>
        {
            new Model { Name = "gemini-2.5-flash", DisplayName = "Gemini 2.5 Flash" }
        };

        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result1 = await _service.GetModelsAsync("test-api-key");
        var result2 = await _service.GetModelsAsync("test-api-key");

        Assert.Single(result1);
        Assert.Single(result2);
        _agentServiceMock.Verify(a => a.GetModelsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetModelsAsync_ShouldUseDifferentCache_ForDifferentApiKeys()
    {
        var models1 = new List<Model>
        {
            new Model { Name = "model-1", DisplayName = "Model 1" }
        };
        var models2 = new List<Model>
        {
            new Model { Name = "model-2", DisplayName = "Model 2" }
        };

        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(models1);

        var result1 = await _service.GetModelsAsync("api-key-1");

        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(models2);

        var result2 = await _service.GetModelsAsync("api-key-2");

        Assert.Equal("model-1", result1[0].Name);
        Assert.Equal("model-2", result2[0].Name);
        _agentServiceMock.Verify(a => a.GetModelsAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetModelsAsync_ShouldReturnCachedModels_AfterClearCacheCalled()
    {
        var models = new List<Model>
        {
            new Model { Name = "gemini-2.5-flash", DisplayName = "Gemini 2.5 Flash" }
        };

        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        await _service.GetModelsAsync("test-api-key");
        _service.ClearModelsCache("test-api-key");
        await _service.GetModelsAsync("test-api-key");

        _agentServiceMock.Verify(a => a.GetModelsAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public void ClearModelsCache_ShouldNotThrow_WhenCacheIsEmpty()
    {
        var exception = Record.Exception(() => _service.ClearModelsCache("test-api-key"));
        Assert.Null(exception);
    }
}

public class TestDto
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}
