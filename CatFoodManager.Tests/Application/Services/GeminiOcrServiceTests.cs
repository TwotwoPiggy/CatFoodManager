using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
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
    private readonly Mock<IRepository<GeminiResponseEntity>> _repositoryMock;
    private readonly Mock<ILogger<GeminiOcrService>> _loggerMock;
    private readonly MemoryCache _memoryCache;
    private readonly GeminiOcrService _service;

    public GeminiOcrServiceTests()
    {
        _agentServiceMock = new Mock<IGeminiAgentService>();
        _repositoryMock = new Mock<IRepository<GeminiResponseEntity>>();
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

            Assert.Empty(result.Items);
            Assert.Null(result.ResponseId);
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

            Assert.Empty(result.Items);
            Assert.Equal("test-id", result.ResponseId);
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

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Single(result.Items);
            Assert.Equal("Test", result.Items[0].Name);
            Assert.Equal(123, result.Items[0].Value);
            Assert.Equal("test-id", result.ResponseId);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldIncludeImageIdsInPrompt()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var image1 = Path.Combine(tempPath, "b.jpg");
            var image2 = Path.Combine(tempPath, "a.png");
            await System.IO.File.WriteAllBytesAsync(image1, [1, 2, 3]);
            await System.IO.File.WriteAllBytesAsync(image2, [4, 5, 6]);

            AIRequest? capturedRequest = null;
            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .Callback<AIRequest>(request => capturedRequest = request)
                .ReturnsAsync(new GeminiResponse("")
                {
                    Text = "{\"items\":[]}",
                    ResponseId = "test-id",
                    ModelVersion = "test-model"
                });

            await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.NotNull(capturedRequest);
            Assert.NotNull(capturedRequest!.Text);
            Assert.Contains("prompt", capturedRequest.Text);
            Assert.Contains("IMAGE ID MAPPING TABLE", capturedRequest.Text);
            Assert.Contains("img-001", capturedRequest.Text);
            Assert.Contains("a.png", capturedRequest.Text);
            Assert.Contains("img-002", capturedRequest.Text);
            Assert.Contains("b.jpg", capturedRequest.Text);
            Assert.Contains("\"items\"", capturedRequest.Text);
            Assert.Contains("VERIFICATION CHECKLIST", capturedRequest.Text);
            Assert.Contains("EXAMPLE OUTPUT STRUCTURE", capturedRequest.Text);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldMapImageIdBackToPicturePath()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var image1 = Path.Combine(tempPath, "b.jpg");
            var image2 = Path.Combine(tempPath, "a.png");
            await System.IO.File.WriteAllBytesAsync(image1, [1, 2, 3]);
            await System.IO.File.WriteAllBytesAsync(image2, [4, 5, 6]);

            var response = new GeminiResponse("")
            {
                Text = "{\"items\":[{\"ImageId\":\"img-001\",\"Name\":\"First\"},{\"ImageId\":\"img-002\",\"Name\":\"Second\"}]}",
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithImage>(tempPath, "prompt");

            Assert.Equal(2, result.Items.Count);
            Assert.Equal("img-001", result.Items[0].ImageId);
            Assert.Equal(Path.Combine(tempPath, "a.png"), result.Items[0].PicturePath);
            Assert.Equal("img-002", result.Items[1].ImageId);
            Assert.Equal(Path.Combine(tempPath, "b.jpg"), result.Items[1].PicturePath);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldMatchByImageId_WhenAIReturnsItemsInDifferentOrder()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var image1 = Path.Combine(tempPath, "b.jpg");
            var image2 = Path.Combine(tempPath, "a.png");
            await System.IO.File.WriteAllBytesAsync(image1, [1, 2, 3]);
            await System.IO.File.WriteAllBytesAsync(image2, [4, 5, 6]);

            var response = new GeminiResponse("")
            {
                Text = "{\"items\":[{\"ImageId\":\"img-002\",\"Name\":\"First\"},{\"ImageId\":\"img-001\",\"Name\":\"Second\"}]}",
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithImage>(tempPath, "prompt");

            Assert.Equal(2, result.Items.Count);
            Assert.Equal("img-002", result.Items[0].ImageId);
            Assert.Equal(Path.Combine(tempPath, "b.jpg"), result.Items[0].PicturePath);
            Assert.Equal("img-001", result.Items[1].ImageId);
            Assert.Equal(Path.Combine(tempPath, "a.png"), result.Items[1].PicturePath);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldUseReverseFallback_WhenImageIdsAreInvalid()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var image1 = Path.Combine(tempPath, "b.jpg");
            var image2 = Path.Combine(tempPath, "a.png");
            await System.IO.File.WriteAllBytesAsync(image1, [1, 2, 3]);
            await System.IO.File.WriteAllBytesAsync(image2, [4, 5, 6]);

            var response = new GeminiResponse("")
            {
                Text = "{\"items\":[{\"ImageId\":\"invalid-id\",\"Name\":\"First\"},{\"ImageId\":\"another-invalid\",\"Name\":\"Second\"}]}",
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithImage>(tempPath, "prompt");

            Assert.Equal(2, result.Items.Count);
            Assert.Equal("img-002", result.Items[0].ImageId);
            Assert.Equal(Path.Combine(tempPath, "b.jpg"), result.Items[0].PicturePath);
            Assert.Equal("img-001", result.Items[1].ImageId);
            Assert.Equal(Path.Combine(tempPath, "a.png"), result.Items[1].PicturePath);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldUseReverseFallback_WhenImageIdsAreEmpty()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var image1 = Path.Combine(tempPath, "b.jpg");
            var image2 = Path.Combine(tempPath, "a.png");
            await System.IO.File.WriteAllBytesAsync(image1, [1, 2, 3]);
            await System.IO.File.WriteAllBytesAsync(image2, [4, 5, 6]);

            var response = new GeminiResponse("")
            {
                Text = "{\"items\":[{\"ImageId\":\"\",\"Name\":\"First\"},{\"ImageId\":null,\"Name\":\"Second\"}]}",
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithImage>(tempPath, "prompt");

            Assert.Equal(2, result.Items.Count);
            Assert.Equal("img-002", result.Items[0].ImageId);
            Assert.Equal(Path.Combine(tempPath, "b.jpg"), result.Items[0].PicturePath);
            Assert.Equal("img-001", result.Items[1].ImageId);
            Assert.Equal(Path.Combine(tempPath, "a.png"), result.Items[1].PicturePath);
        }
        finally
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldUseReverseFallback_ForItemsWithInvalidImageId_WhenOthersMatch()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var image1 = Path.Combine(tempPath, "b.jpg");
            var image2 = Path.Combine(tempPath, "a.png");
            var image3 = Path.Combine(tempPath, "c.bmp");
            await System.IO.File.WriteAllBytesAsync(image1, [1, 2, 3]);
            await System.IO.File.WriteAllBytesAsync(image2, [4, 5, 6]);
            await System.IO.File.WriteAllBytesAsync(image3, [7, 8, 9]);

            var response = new GeminiResponse("")
            {
                Text = "{\"items\":[{\"ImageId\":\"img-002\",\"Name\":\"Second\"},{\"ImageId\":\"invalid\",\"Name\":\"Unknown\"},{\"ImageId\":\"img-001\",\"Name\":\"First\"}]}",
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithImage>(tempPath, "prompt");

            Assert.Equal(3, result.Items.Count);
            Assert.Equal("img-002", result.Items[0].ImageId);
            Assert.Equal(Path.Combine(tempPath, "b.jpg"), result.Items[0].PicturePath);
            Assert.Equal("img-003", result.Items[1].ImageId);
            Assert.Equal(Path.Combine(tempPath, "c.bmp"), result.Items[1].PicturePath);
            Assert.Equal("img-001", result.Items[2].ImageId);
            Assert.Equal(Path.Combine(tempPath, "a.png"), result.Items[2].PicturePath);
        }
        finally
        {
            Directory.Delete(tempPath, true);
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

            Assert.Single(result.Items);
            Assert.Equal("Test", result.Items[0].Name);
            Assert.Equal(456, result.Items[0].Value);
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

            Assert.Empty(result.Items);
            Assert.Null(result.ResponseId);
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

    [Fact]
    public async Task GetModelsAsync_ShouldRemoveModelsPrefix_FromModelNames()
    {
        var models = new List<Model>
        {
            new Model { Name = "models/gemini-2.5-flash", DisplayName = "Gemini 2.5 Flash" },
            new Model { Name = "models/gemini-2.5-pro", DisplayName = "Gemini 2.5 Pro" },
            new Model { Name = "gemini-2.0-flash", DisplayName = "Gemini 2.0 Flash" }
        };

        _agentServiceMock.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);

        var result = await _service.GetModelsAsync("test-api-key");

        Assert.Equal(3, result.Count);
        Assert.Equal("gemini-2.5-flash", result[0].Name);
        Assert.Equal("gemini-2.5-pro", result[1].Name);
        Assert.Equal("gemini-2.0-flash", result[2].Name);
        Assert.Equal("Gemini 2.5 Flash", result[0].DisplayName);
        Assert.Equal("Gemini 2.5 Pro", result[1].DisplayName);
        Assert.Equal("Gemini 2.0 Flash", result[2].DisplayName);
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldCacheEntity_WhenRepositoryThrowsException()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"Value\":123}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-response-id-123",
                ModelVersion = "test-model",
                UsageMetadata = new Google.GenAI.Types.GenerateContentResponseUsageMetadata
                {
                    TotalTokenCount = 20,
                    PromptTokenCount = 10
                }
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Empty(result.Items);

            var failedResponses = _service.GetFailedResponses();
            Assert.Single(failedResponses);
            Assert.Equal("test-response-id-123", failedResponses[0].ResponseId);
            Assert.Equal(tempPath, failedResponses[0].FolderPath);
            Assert.Equal("prompt", failedResponses[0].PromptText);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldClearCache_WhenSuccessful()
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
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            Assert.Single(result.Items);
            var failedResponses = _service.GetFailedResponses();
            Assert.Empty(failedResponses);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public void GetFailedResponses_ShouldReturnEmptyList_WhenNoFailedResponses()
    {
        var result = _service.GetFailedResponses();

        Assert.Empty(result);
    }

    [Fact]
    public async Task RetrySaveResponseAsync_ShouldReturnFalse_WhenNoCachedResponse()
    {
        var result = await _service.RetrySaveResponseAsync("non-existent-response-id");

        Assert.False(result);
    }

    [Fact]
    public async Task RetrySaveResponseAsync_ShouldSaveEntityAndClearCache_WhenCachedResponseExists()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"Value\":123}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-response-id-456",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var callCount = 0;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        throw new Exception("Database error");
                    }
                    return Task.CompletedTask;
                });

            await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            var failedResponses = _service.GetFailedResponses();
            Assert.Single(failedResponses);

            var retryResult = await _service.RetrySaveResponseAsync("test-response-id-456");

            Assert.True(retryResult);
            failedResponses = _service.GetFailedResponses();
            Assert.Empty(failedResponses);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task RetrySaveResponseAsync_ShouldReturnFalse_WhenSaveStillFails()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"Value\":123}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-response-id-789",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            var retryResult = await _service.RetrySaveResponseAsync("test-response-id-789");

            Assert.False(retryResult);
            var failedResponses = _service.GetFailedResponses();
            Assert.Single(failedResponses);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task RemoveFailedResponse_ShouldRemoveCachedResponse()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"Value\":123}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-response-id-abc",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            await _service.ProcessPicturesAsync<TestDto>(tempPath, "prompt");

            var failedResponses = _service.GetFailedResponses();
            Assert.Single(failedResponses);

            _service.RemoveFailedResponse("test-response-id-abc");

            failedResponses = _service.GetFailedResponses();
            Assert.Empty(failedResponses);

            var retryResult = await _service.RetrySaveResponseAsync("test-response-id-abc");
            Assert.False(retryResult);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Theory]
    [InlineData("2024-01-15", 2024, 1, 15)]
    [InlineData("2024/01/15", 2024, 1, 15)]
    [InlineData("2024.01.15", 2024, 1, 15)]
    [InlineData("2024-1-5", 2024, 1, 5)]
    [InlineData("2024/1/5", 2024, 1, 5)]
    public async Task ProcessPicturesAsync_ShouldParseVariousDateFormats(string dateString, int expectedYear, int expectedMonth, int expectedDay)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = $"[{{\"Name\":\"Test\",\"PurchasedAt\":\"{dateString}\",\"FinalPrice\":99.99}}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<GeminiResponseEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _service.ProcessPicturesAsync<TestDtoWithDate>(tempPath, "prompt");

            Assert.Single(result.Items);
            Assert.Equal("Test", result.Items[0].Name);
            Assert.True(result.Items[0].PurchasedAt.HasValue);
            Assert.Equal(expectedYear, result.Items[0].PurchasedAt!.Value.Year);
            Assert.Equal(expectedMonth, result.Items[0].PurchasedAt.Value.Month);
            Assert.Equal(expectedDay, result.Items[0].PurchasedAt.Value.Day);
            Assert.Equal(99.99m, result.Items[0].FinalPrice);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldHandleNullPurchasedAt()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"PurchasedAt\":null,\"FinalPrice\":99.99}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithDate>(tempPath, "prompt");

            Assert.Single(result.Items);
            Assert.Null(result.Items[0].PurchasedAt);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldHandleEmptyPurchasedAt()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"PurchasedAt\":\"\",\"FinalPrice\":99.99}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithDate>(tempPath, "prompt");

            Assert.Single(result.Items);
            Assert.Null(result.Items[0].PurchasedAt);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }

    [Fact]
    public async Task ProcessPicturesAsync_ShouldHandleInvalidDateFormat()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);

        try
        {
            var jsonResponse = "[{\"Name\":\"Test\",\"PurchasedAt\":\"invalid-date\",\"FinalPrice\":99.99}]";
            var response = new GeminiResponse("")
            {
                Text = jsonResponse,
                ResponseId = "test-id",
                ModelVersion = "test-model"
            };

            _agentServiceMock.Setup(a => a.GenerateContentAsync(It.IsAny<AIRequest>()))
                .ReturnsAsync(response);

            var result = await _service.ProcessPicturesAsync<TestDtoWithDate>(tempPath, "prompt");

            Assert.Single(result.Items);
            Assert.Null(result.Items[0].PurchasedAt);
        }
        finally
        {
            Directory.Delete(tempPath);
        }
    }
}

public class TestDto
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class TestDtoWithDate
{
    public string Name { get; set; } = string.Empty;
    public DateTime? PurchasedAt { get; set; }
    public decimal FinalPrice { get; set; }
}

public class TestDtoWithImage
{
    public string ImageId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PicturePath { get; set; } = string.Empty;
}
