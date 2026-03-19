using CatfoodManagement.Services.Bridge;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CatfoodManagement.Tests.Services
{
    public class BrandApiTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IService<Brand>> _mockBrandService;
        private readonly BrandApi _brandApi;

        public BrandApiTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockBrandService = new Mock<IService<Brand>>();
            
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            
            mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IService<Brand>))).Returns(_mockBrandService.Object);

            _brandApi = new BrandApi(_mockServiceProvider.Object);
        }

        [Fact]
        public async Task GetBrands_ShouldReturnBrandList()
        {
            var brands = new List<Brand>
            {
                new Brand { Id = 1, Name = "Brand 1" },
                new Brand { Id = 2, Name = "Brand 2" }
            };

            _mockBrandService
                .Setup(x => x.GetAllWithCount())
                .Returns((brands, 2));

            var result = await _brandApi.GetBrands();
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.Equal(2, (int)deserializedResult!.Total);
        }

        [Fact]
        public async Task AddBrand_ShouldReturnSuccessWithId()
        {
            var brand = new Brand { Id = 1, Name = "New Brand" };

            _mockBrandService
                .Setup(x => x.Save(It.IsAny<Brand>()))
                .Callback<Brand>(b => b.Id = 1);

            var result = await _brandApi.AddBrand("New Brand");
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }

        [Fact]
        public async Task UpdateBrand_ShouldCallServiceUpdate()
        {
            var brand = new Brand { Id = 1, Name = "Old Name" };

            _mockBrandService
                .Setup(x => x.Query(1))
                .Returns(brand);

            var result = await _brandApi.UpdateBrand(1, "New Name");
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }

        [Fact]
        public async Task DeleteBrand_ShouldCallServiceDelete()
        {
            var result = await _brandApi.DeleteBrand(1);
            var deserializedResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);

            Assert.NotNull(deserializedResult);
            Assert.True((bool)deserializedResult!.Success);
        }
    }
}
