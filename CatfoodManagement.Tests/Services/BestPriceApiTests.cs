using CatfoodManagement.Services.Bridge;
using CatFoodManager.Application.Common;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CatfoodManagement.Tests.Services
{
    public class BestPriceApiTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IBestPriceService> _mockBestPriceService;
        private readonly BestPriceApi _bestPriceApi;

        public BestPriceApiTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockBestPriceService = new Mock<IBestPriceService>();
            
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            
            mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IBestPriceService))).Returns(_mockBestPriceService.Object);

            _bestPriceApi = new BestPriceApi(_mockServiceProvider.Object);
        }

        [Fact]
        public async Task GetBestPrices_ShouldReturnPagedResult()
        {
            var pagedResult = new PagedResult<BestPrice>
            {
                Items = new List<BestPrice>
                {
                    new BestPrice { Id = 1, Name = "Best Price 1", LowestPrice = 100 },
                    new BestPrice { Id = 2, Name = "Best Price 2", LowestPrice = 200 }
                },
                TotalCount = 2
            };

            _mockBestPriceService
                .Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            var result = await _bestPriceApi.GetBestPrices(1, 10);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.Equal(2, (int)deserializedResult!.Total);
        }

        [Fact]
        public async Task UpdateBestPrice_WithValidId_ShouldReturnSuccess()
        {
            var bestPrice = new BestPrice { Id = 1, Name = "Test Best Price" };
            
            _mockBestPriceService
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bestPrice);

            var result = await _bestPriceApi.UpdateBestPrice(1, "LowestPrice", 150);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }

        [Fact]
        public async Task DeleteBestPrice_ShouldCallServiceDelete()
        {
            var result = await _bestPriceApi.DeleteBestPrice(1);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }

        [Fact]
        public async Task DeleteBestPrices_ShouldReturnSuccessWithCount()
        {
            var ids = new long[] { 1, 2, 3 };
            _mockBestPriceService.Setup(s => s.DeleteRangeAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            var result = await _bestPriceApi.DeleteBestPrices(ids);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
            Assert.Equal(3, (int)deserializedResult.Count);
        }

        [Fact]
        public async Task DeleteBestPrices_WithEmptyArray_ShouldReturnZero()
        {
            var ids = Array.Empty<long>();
            _mockBestPriceService.Setup(s => s.DeleteRangeAsync(It.IsAny<IEnumerable<long>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _bestPriceApi.DeleteBestPrices(ids);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
            Assert.Equal(0, (int)deserializedResult.Count);
        }
    }
}
