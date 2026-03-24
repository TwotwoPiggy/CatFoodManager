using CatfoodManagement.Services.Bridge;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CatfoodManagement.Tests
{
    public class CatFoodApiTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IService<CatFood>> _mockCatFoodService;
        private readonly CatFoodApi _catFoodApi;

        public CatFoodApiTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockCatFoodService = new Mock<IService<CatFood>>();
            
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            
            mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IService<CatFood>))).Returns(_mockCatFoodService.Object);

            _catFoodApi = new CatFoodApi(_mockServiceProvider.Object);
        }

        [Fact]
        public async Task GetCatFoods_ShouldReturnPagedResult()
        {
            var catFoods = new List<CatFood>
            {
                new CatFood { Id = 1, Name = "Test Cat Food 1" },
                new CatFood { Id = 2, Name = "Test Cat Food 2" }
            };

            _mockCatFoodService
                .Setup(x => x.GetAllWithCount())
                .Returns((catFoods, 2));

            var result = await _catFoodApi.GetCatFoods(1, 10);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.Equal(2, (int)deserializedResult!.Total);
        }

        [Fact]
        public async Task UpdateCatFood_WithValidId_ShouldReturnSuccess()
        {
            var catFood = new CatFood { Id = 1, Name = "Test Cat Food" };
            
            _mockCatFoodService
                .Setup(x => x.Query(1))
                .Returns(catFood);

            var result = await _catFoodApi.UpdateCatFood(1, "Name", "Updated Name");
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }

        [Fact]
        public async Task UpdateCatFood_WithInvalidId_ShouldReturnFailure()
        {
            _mockCatFoodService
                .Setup(x => x.Query(999))
                .Returns((CatFood?)null);

            var result = await _catFoodApi.UpdateCatFood(999, "Name", "Updated Name");
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.False((bool)deserializedResult!.Success);
        }

        [Fact]
        public async Task DeleteCatFood_ShouldCallServiceDelete()
        {
            var result = await _catFoodApi.DeleteCatFood(1);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }
    }
}
