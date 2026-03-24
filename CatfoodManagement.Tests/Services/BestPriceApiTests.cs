using CatfoodManagement.Services.Bridge;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CatfoodManagement.Tests.Services
{
    public class BestPriceApiTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IService<BestPrice>> _mockBestPriceService;
        private readonly BestPriceApi _bestPriceApi;

        public BestPriceApiTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockBestPriceService = new Mock<IService<BestPrice>>();
            
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            
            mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IService<BestPrice>))).Returns(_mockBestPriceService.Object);

            _bestPriceApi = new BestPriceApi(_mockServiceProvider.Object);
        }

        [Fact]
        public async Task GetBestPrices_ShouldReturnPagedResult()
        {
            var bestPrices = new List<BestPrice>
            {
                new BestPrice { Id = 1, Name = "Best Price 1", LowestPrice = 100 },
                new BestPrice { Id = 2, Name = "Best Price 2", LowestPrice = 200 }
            };

            _mockBestPriceService
                .Setup(x => x.GetAllWithCount())
                .Returns((bestPrices, 2));

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
                .Setup(x => x.Query(1))
                .Returns(bestPrice);

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
    }
}
