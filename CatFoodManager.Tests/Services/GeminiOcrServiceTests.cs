using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Services;
using Moq;
using Twotwo.Agent.Interfaces;
using Twotwo.Agent.Types;
using Xunit;

namespace CatFoodManager.Tests.Services
{
    public class GeminiOcrServiceTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly Mock<IGeminiAgentService> _mockAgentService;

        public GeminiOcrServiceTests()
        {
            _mockRepository = new Mock<IRepository>();
            _mockAgentService = new Mock<IGeminiAgentService>();
        }

        [Fact]
        public void Constructor_WithNullAgentService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => 
                new GeminiOcrService(_mockRepository.Object, null!, false));
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
            
            Assert.NotNull(service);
        }

        [Fact]
        public async Task ValidateModelAsync_WithValidModel_ReturnsTrue()
        {
            _mockAgentService
                .Setup(x => x.ValidateModelAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
            var result = await service.ValidateModelAsync();
            
            Assert.True(result);
            _mockAgentService.Verify(x => x.ValidateModelAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateModelAsync_WithInvalidModel_ThrowsArgumentException()
        {
            _mockAgentService
                .Setup(x => x.ValidateModelAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("Invalid model"));
            
            var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
            
            await Assert.ThrowsAsync<ArgumentException>(() => service.ValidateModelAsync());
        }

        [Fact]
        public async Task ProcessPicAsync_WithNullFolderPath_ThrowsArgumentNullException()
        {
            var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                service.ProcessPicAsync<TestDto>(null!, "prompt"));
        }

        [Fact]
        public async Task ProcessPicAsync_WithEmptyFolderPath_ThrowsArgumentNullException()
        {
            var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                service.ProcessPicAsync<TestDto>("", "prompt"));
        }

        [Fact]
        public async Task ProcessPicAsync_WithNonExistentFolder_ThrowsDirectoryNotFoundException()
        {
            var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
            
            await Assert.ThrowsAsync<DirectoryNotFoundException>(() => 
                service.ProcessPicAsync<TestDto>("non_existent_folder", "prompt"));
        }

        [Fact]
        public async Task ProcessPicAsync_WithValidInput_ReturnsDeserializedResult()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            
            try
            {
                var testImagePath = Path.Combine(tempFolder, "test.jpg");
                await File.WriteAllBytesAsync(testImagePath, new byte[] { 0xFF, 0xD8, 0xFF });
                
                var expectedText = "[{\"Name\":\"Test\",\"Value\":123}]";
                var response = new GeminiResponse("")
                {
                    Text = expectedText,
                    ResponseId = "test-id",
                    ModelVersion = "v1",
                    UsageMetadata = new Google.GenAI.Types.GenerateContentResponseUsageMetadata
                    {
                        TotalTokenCount = 100,
                        PromptTokenCount = 50
                    }
                };
                
                _mockAgentService
                    .Setup(x => x.GenerateContentAsync(It.IsAny<AIRequest>()))
                    .ReturnsAsync(response);
                
                _mockRepository
                    .Setup(x => x.Add(It.IsAny<GeminiResponseEntity>()))
                    .Verifiable();
                
                var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
                var result = await service.ProcessPicAsync<TestDto>(tempFolder, "test prompt");
                
                Assert.Single(result);
                Assert.Equal("Test", result[0].Name);
                Assert.Equal(123, result[0].Value);
                
                _mockRepository.Verify(x => x.Add(It.IsAny<GeminiResponseEntity>()), Times.Once);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        [Fact]
        public async Task ProcessPicAsync_WithNullResponse_ReturnsEmptyList()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            
            try
            {
                var testImagePath = Path.Combine(tempFolder, "test.jpg");
                await File.WriteAllBytesAsync(testImagePath, new byte[] { 0xFF, 0xD8, 0xFF });
                
                _mockAgentService
                    .Setup(x => x.GenerateContentAsync(It.IsAny<AIRequest>()))
                    .ReturnsAsync((GeminiResponse)null!);
                
                var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
                var result = await service.ProcessPicAsync<TestDto>(tempFolder, "test prompt");
                
                Assert.Empty(result);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        [Fact]
        public async Task ProcessPicAsync_WithEmptyTextResponse_ReturnsEmptyList()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            
            try
            {
                var testImagePath = Path.Combine(tempFolder, "test.jpg");
                await File.WriteAllBytesAsync(testImagePath, new byte[] { 0xFF, 0xD8, 0xFF });
                
                var response = new GeminiResponse("")
                {
                    Text = "",
                    ResponseId = "test-id"
                };
                
                _mockAgentService
                    .Setup(x => x.GenerateContentAsync(It.IsAny<AIRequest>()))
                    .ReturnsAsync(response);
                
                _mockRepository
                    .Setup(x => x.Add(It.IsAny<GeminiResponseEntity>()))
                    .Verifiable();
                
                var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
                var result = await service.ProcessPicAsync<TestDto>(tempFolder, "test prompt");
                
                Assert.Empty(result);
                _mockRepository.Verify(x => x.Add(It.IsAny<GeminiResponseEntity>()), Times.Once);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        [Fact]
        public async Task ProcessPicAsync_WithException_ReturnsEmptyList()
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            
            try
            {
                var testImagePath = Path.Combine(tempFolder, "test.jpg");
                await File.WriteAllBytesAsync(testImagePath, new byte[] { 0xFF, 0xD8, 0xFF });
                
                _mockAgentService
                    .Setup(x => x.GenerateContentAsync(It.IsAny<AIRequest>()))
                    .ThrowsAsync(new Exception("API Error"));
                
                var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
                var result = await service.ProcessPicAsync<TestDto>(tempFolder, "test prompt");
                
                Assert.Empty(result);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".bmp")]
        [InlineData(".webp")]
        public async Task ProcessPicAsync_WithAllowedExtensions_ProcessesFile(string extension)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            
            try
            {
                var testImagePath = Path.Combine(tempFolder, $"test{extension}");
                await File.WriteAllBytesAsync(testImagePath, new byte[] { 0xFF, 0xD8, 0xFF });
                
                var response = new GeminiResponse("")
                {
                    Text = "[{\"Name\":\"Test\",\"Value\":1}]",
                    ResponseId = "test-id"
                };
                
                _mockAgentService
                    .Setup(x => x.GenerateContentAsync(It.IsAny<AIRequest>()))
                    .ReturnsAsync(response);
                
                var service = new GeminiOcrService(_mockRepository.Object, _mockAgentService.Object, false);
                var result = await service.ProcessPicAsync<TestDto>(tempFolder, "test prompt");
                
                Assert.Single(result);
            }
            finally
            {
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }
    }

    public class TestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
