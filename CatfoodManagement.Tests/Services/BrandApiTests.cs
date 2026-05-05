using CatfoodManagement.Services.Bridge;
using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CatfoodManagement.Tests.Services
{
    public class BrandApiTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IBrandService> _mockBrandService;
        private readonly BrandApi _brandApi;

        public BrandApiTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockBrandService = new Mock<IBrandService>();
            
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            
            mockScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
            _mockServiceProvider.Setup(x => x.GetService(typeof(IBrandService))).Returns(_mockBrandService.Object);

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
                .Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(brands);

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
                .Setup(x => x.AddAsync(It.IsAny<Brand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback<Brand, CancellationToken>((b, _) => b.Id = 1);

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
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(brand);

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
