using CatFoodManager.Application.Services;
using CatFoodManager.Domain.Entities;
using CatFoodManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.Application.Services;

public class OcrPromptServiceTests
{
    private readonly Mock<IRepository<OcrPrompt>> _repositoryMock;
    private readonly Mock<ILogger<OcrPromptService>> _loggerMock;
    private readonly OcrPromptService _service;

    public OcrPromptServiceTests()
    {
        _repositoryMock = new Mock<IRepository<OcrPrompt>>();
        _loggerMock = new Mock<ILogger<OcrPromptService>>();
        _service = new OcrPromptService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetDefaultAsync_ShouldReturnDefaultPrompt_WhenDefaultExists()
    {
        var expectedPrompt = new OcrPrompt { Id = 1, Name = "Default", Content = "Test Content", IsDefault = true };
        var prompts = new List<OcrPrompt> { expectedPrompt };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OcrPrompt, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(prompts);

        var result = await _service.GetDefaultAsync();

        Assert.Equal(expectedPrompt, result);
    }

    [Fact]
    public async Task GetDefaultAsync_ShouldReturnFirstPrompt_WhenNoDefaultExists()
    {
        var prompts = new List<OcrPrompt>
        {
            new() { Id = 1, Name = "Prompt1", Content = "Content1", IsDefault = false },
            new() { Id = 2, Name = "Prompt2", Content = "Content2", IsDefault = false }
        };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OcrPrompt, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OcrPrompt>());
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(prompts);

        var result = await _service.GetDefaultAsync();

        Assert.Equal(prompts[0], result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPrompt_WhenPromptExists()
    {
        var expectedPrompt = new OcrPrompt { Id = 1, Name = "Test", Content = "Content" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPrompt);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(expectedPrompt, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPromptDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OcrPrompt?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPrompts_OrderedCorrectly()
    {
        var prompts = new List<OcrPrompt>
        {
            new() { Id = 1, Name = "A", Content = "Content", IsDefault = false },
            new() { Id = 2, Name = "B", Content = "Content", IsDefault = true },
            new() { Id = 3, Name = "C", Content = "Content", IsDefault = false }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(prompts);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(2, result[0].Id);
        Assert.True(result[0].IsDefault);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePrompt_AndClearOtherDefaults()
    {
        var existingDefault = new OcrPrompt { Id = 1, Name = "Old Default", Content = "Content", IsDefault = true };
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OcrPrompt, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OcrPrompt> { existingDefault });

        var result = await _service.CreateAsync("New Prompt", "New Content", true, "Description");

        Assert.Equal("New Prompt", result.Name);
        Assert.Equal("New Content", result.Content);
        Assert.True(result.IsDefault);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<OcrPrompt>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(existingDefault, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePrompt_WhenPromptExists()
    {
        var existingPrompt = new OcrPrompt { Id = 1, Name = "Old Name", Content = "Old Content", IsDefault = false };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPrompt);

        await _service.UpdateAsync(1, "New Name", "New Content", true, "New Description");

        Assert.Equal("New Name", existingPrompt.Name);
        Assert.Equal("New Content", existingPrompt.Content);
        Assert.True(existingPrompt.IsDefault);
        _repositoryMock.Verify(r => r.UpdateAsync(existingPrompt, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenPromptDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OcrPrompt?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, "Name", "Content", false));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePrompt_AndSetNewDefault()
    {
        var defaultPrompt = new OcrPrompt { Id = 1, Name = "Default", Content = "Content", IsDefault = true };
        var otherPrompt = new OcrPrompt { Id = 2, Name = "Other", Content = "Content", IsDefault = false };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultPrompt);
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OcrPrompt> { defaultPrompt, otherPrompt });

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<OcrPrompt>(p => p.Id == 2 && p.IsDefault), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetDefaultAsync_ShouldSetPromptAsDefault_AndClearOtherDefaults()
    {
        var existingDefault = new OcrPrompt { Id = 1, Name = "Old Default", Content = "Content", IsDefault = true };
        var newDefault = new OcrPrompt { Id = 2, Name = "New Default", Content = "Content", IsDefault = false };
        _repositoryMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newDefault);
        _repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OcrPrompt, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OcrPrompt> { existingDefault });

        await _service.SetDefaultAsync(2);

        Assert.True(newDefault.IsDefault);
        _repositoryMock.Verify(r => r.UpdateAsync(newDefault, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(existingDefault, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetDefaultAsync_ShouldThrowException_WhenPromptDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OcrPrompt?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SetDefaultAsync(999));
    }

    [Fact]
    public async Task InitializeDefaultPromptAsync_ShouldCreateDefaultPrompt_WhenNoPromptsExist()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OcrPrompt>());

        await _service.InitializeDefaultPromptAsync();

        _repositoryMock.Verify(r => r.AddAsync(It.Is<OcrPrompt>(p => p.IsDefault && p.Name == "默认 OCR 提示词"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeDefaultPromptAsync_ShouldNotCreatePrompt_WhenPromptsExist()
    {
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OcrPrompt> { new OcrPrompt { Id = 1, Name = "Existing" } });

        await _service.InitializeDefaultPromptAsync();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<OcrPrompt>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
