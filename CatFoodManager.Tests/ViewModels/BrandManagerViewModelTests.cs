using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.ViewModels;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.ViewModels;

public class BrandManagerViewModelTests
{
    private readonly Mock<IService<Brand>> _mockBrandService;
    private readonly BrandManagerViewModel _viewModel;

    public BrandManagerViewModelTests()
    {
        _mockBrandService = new Mock<IService<Brand>>();
        _mockBrandService.Setup(s => s.GetAllWithCount()).Returns((new List<Brand>(), 0));

        _viewModel = new BrandManagerViewModel(_mockBrandService.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        Assert.NotNull(_viewModel.LoadDataCommand);
        Assert.NotNull(_viewModel.SearchCommand);
        Assert.NotNull(_viewModel.ResetSearchCommand);
        Assert.NotNull(_viewModel.AddBrandCommand);
        Assert.NotNull(_viewModel.UpdateBrandCommand);
        Assert.NotNull(_viewModel.DeleteBrandCommand);
    }

    [Fact]
    public void Constructor_WithNullService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new BrandManagerViewModel(null!));
    }

    [Fact]
    public void LoadData_ShouldLoadBrands()
    {
        var brands = new List<Brand>
        {
            new() { Id = 1, Name = "Brand1" },
            new() { Id = 2, Name = "Brand2" }
        };
        _mockBrandService.Setup(s => s.GetAllWithCount()).Returns((brands, 2));

        _viewModel.LoadData();

        Assert.Equal(2, _viewModel.Brands.Count);
    }

    [Fact]
    public void Search_WithEmptyText_ShouldLoadAllData()
    {
        _viewModel.SearchText = "   ";
        _viewModel.Search();

        _mockBrandService.Verify(s => s.GetAllWithCount(), Times.AtLeastOnce);
    }

    [Fact]
    public void Search_WithText_ShouldCallFuzzyQueryWithCount()
    {
        _viewModel.SearchText = "Test";

        _viewModel.Search();

        _mockBrandService.Verify(s => s.FuzzyQueryWithCount(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ResetSearch_ShouldClearSearchTextAndLoadData()
    {
        _viewModel.SearchText = "Test";

        _viewModel.ResetSearch();

        Assert.Equal(string.Empty, _viewModel.SearchText);
    }

    [Fact]
    public void AddBrand_WithValidName_ShouldCallServiceSave()
    {
        _viewModel.AddBrand("NewBrand");

        _mockBrandService.Verify(s => s.Save(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public void AddBrand_WithEmptyName_ShouldNotCallServiceSave()
    {
        _viewModel.AddBrand("");

        _mockBrandService.Verify(s => s.Save(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public void UpdateBrand_WithValidBrand_ShouldCallServiceUpdate()
    {
        var brand = new Brand { Id = 1, Name = "UpdatedBrand" };
        var existingBrand = new Brand { Id = 1, Name = "OldBrand" };
        _mockBrandService.Setup(s => s.Query(1L)).Returns(existingBrand);

        _viewModel.UpdateBrand(brand);

        _mockBrandService.Verify(s => s.Update(existingBrand), Times.Once);
    }

    [Fact]
    public void UpdateBrand_WithNullBrand_ShouldNotCallServiceUpdate()
    {
        _viewModel.UpdateBrand(null);

        _mockBrandService.Verify(s => s.Update(It.IsAny<Brand>()), Times.Never);
    }

    [Fact]
    public void UpdateBrandName_ShouldCallServiceUpdate()
    {
        var existingBrand = new Brand { Id = 1, Name = "OldBrand" };
        _mockBrandService.Setup(s => s.Query(1L)).Returns(existingBrand);

        _viewModel.UpdateBrandName(1, "NewName");

        _mockBrandService.Verify(s => s.Update(existingBrand), Times.Once);
    }

    [Fact]
    public void DeleteBrand_ShouldCallServiceDelete()
    {
        _viewModel.DeleteBrand(1);

        _mockBrandService.Verify(s => s.Delete(1), Times.Once);
    }

    [Fact]
    public void Brands_PropertyChanged_ShouldBeRaised()
    {
        var eventRaised = false;
        _viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(BrandManagerViewModel.Brands))
            {
                eventRaised = true;
            }
        };

        var brands = new List<Brand> { new() { Id = 1, Name = "Test" } };
        _mockBrandService.Setup(s => s.GetAllWithCount()).Returns((brands, 1));
        _viewModel.LoadData();

        Assert.True(eventRaised);
    }
}
