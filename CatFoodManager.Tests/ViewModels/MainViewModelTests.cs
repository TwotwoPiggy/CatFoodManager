using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.ViewModels;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly Mock<IService<CatFood>> _mockCatFoodService;
    private readonly Mock<IService<Brand>> _mockBrandService;
    private readonly Mock<IService<Factory>> _mockFactoryService;
    private readonly Mock<IService<BestPrice>> _mockBestPriceService;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _mockCatFoodService = new Mock<IService<CatFood>>();
        _mockBrandService = new Mock<IService<Brand>>();
        _mockFactoryService = new Mock<IService<Factory>>();
        _mockBestPriceService = new Mock<IService<BestPrice>>();

        _mockCatFoodService.Setup(s => s.GetAllWithCount()).Returns((new List<CatFood>(), 0));
        _mockBestPriceService.Setup(s => s.GetAllWithCount()).Returns((new List<BestPrice>(), 0));

        _viewModel = new MainViewModel(
            _mockCatFoodService.Object,
            _mockBrandService.Object,
            _mockFactoryService.Object,
            _mockBestPriceService.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        Assert.NotNull(_viewModel.LoadDataCommand);
        Assert.NotNull(_viewModel.SearchCommand);
        Assert.NotNull(_viewModel.NextPageCommand);
        Assert.NotNull(_viewModel.PreviousPageCommand);
        Assert.NotNull(_viewModel.FirstPageCommand);
        Assert.NotNull(_viewModel.LastPageCommand);
        Assert.NotNull(_viewModel.ResetSearchCommand);
        Assert.NotNull(_viewModel.GoToPageCommand);
    }

    [Fact]
    public void Constructor_WithNullCatFoodService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainViewModel(
            null!,
            _mockBrandService.Object,
            _mockFactoryService.Object,
            _mockBestPriceService.Object));
    }

    [Fact]
    public void Constructor_WithNullBrandService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainViewModel(
            _mockCatFoodService.Object,
            null!,
            _mockFactoryService.Object,
            _mockBestPriceService.Object));
    }

    [Fact]
    public void Constructor_WithNullFactoryService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainViewModel(
            _mockCatFoodService.Object,
            _mockBrandService.Object,
            null!,
            _mockBestPriceService.Object));
    }

    [Fact]
    public void Constructor_WithNullBestPriceService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MainViewModel(
            _mockCatFoodService.Object,
            _mockBrandService.Object,
            _mockFactoryService.Object,
            null!));
    }

    [Fact]
    public void LoadData_WhenNotLowestPrice_ShouldLoadCatFoods()
    {
        var catFoods = new List<CatFood>
        {
            new() { Id = 1, Name = "Test CatFood" }
        };
        _mockCatFoodService.Setup(s => s.GetAllWithCount()).Returns((catFoods, 1));

        _viewModel.IsLowestPrice = false;
        _viewModel.LoadData();

        Assert.Single(_viewModel.CatFoods);
        Assert.Equal(1, _viewModel.TotalCount);
    }

    [Fact]
    public void LoadData_WhenLowestPrice_ShouldLoadBestPrices()
    {
        var bestPrices = new List<BestPrice>
        {
            new() { Id = 1, Name = "Test BestPrice" }
        };
        _mockBestPriceService.Setup(s => s.GetAllWithCount()).Returns((bestPrices, 1));

        _viewModel.IsLowestPrice = true;
        _viewModel.LoadData();

        Assert.Single(_viewModel.BestPrices);
        Assert.Equal(1, _viewModel.TotalCount);
    }

    [Fact]
    public void Search_WithEmptyText_ShouldLoadAllData()
    {
        _viewModel.SearchText = "   ";
        _viewModel.Search();

        _mockCatFoodService.Verify(s => s.GetAllWithCount(), Times.Once);
    }

    [Fact]
    public void ResetSearch_ShouldClearSearchTextAndLoadData()
    {
        _viewModel.SearchText = "Test";
        _viewModel.ResetSearch();

        Assert.Equal(string.Empty, _viewModel.SearchText);
        Assert.Equal(1, _viewModel.CurrentPage);
    }

    [Fact]
    public void NextPage_WhenNotAtLastPage_ShouldIncrementCurrentPage()
    {
        _viewModel.TotalCount = 100;
        _viewModel.PageSize = 10;
        _viewModel.PageCount = 10;
        _viewModel.CurrentPage = 1;

        _viewModel.NextPage();

        Assert.Equal(2, _viewModel.CurrentPage);
    }

    [Fact]
    public void PreviousPage_WhenNotAtFirstPage_ShouldDecrementCurrentPage()
    {
        _viewModel.CurrentPage = 2;

        _viewModel.PreviousPage();

        Assert.Equal(1, _viewModel.CurrentPage);
    }

    [Fact]
    public void GoToFirstPage_ShouldSetCurrentPageToOne()
    {
        _viewModel.CurrentPage = 5;

        _viewModel.GoToFirstPage();

        Assert.Equal(1, _viewModel.CurrentPage);
    }

    [Fact]
    public void GoToLastPage_ShouldSetCurrentPageToPageCount()
    {
        _viewModel.PageCount = 10;

        _viewModel.GoToLastPage();

        Assert.Equal(10, _viewModel.CurrentPage);
    }

    [Fact]
    public void GoToPage_WithValidPage_ShouldSetCurrentPage()
    {
        _viewModel.PageCount = 10;

        _viewModel.GoToPage(5);

        Assert.Equal(5, _viewModel.CurrentPage);
    }

    [Fact]
    public void GoToPage_WithInvalidPage_ShouldNotChangeCurrentPage()
    {
        _viewModel.PageCount = 10;
        _viewModel.CurrentPage = 3;

        _viewModel.GoToPage(0);

        Assert.Equal(3, _viewModel.CurrentPage);

        _viewModel.GoToPage(15);

        Assert.Equal(3, _viewModel.CurrentPage);
    }

    [Fact]
    public void UpdatePageSize_ShouldResetCurrentPageAndLoadData()
    {
        _viewModel.CurrentPage = 5;

        _viewModel.UpdatePageSize(25);

        Assert.Equal(25, _viewModel.PageSize);
        Assert.Equal(1, _viewModel.CurrentPage);
    }

    [Fact]
    public void UpdateCatFood_ShouldCallServiceUpdate()
    {
        var catFood = new CatFood { Id = 1, Name = "Test" };

        _viewModel.UpdateCatFood(catFood);

        _mockCatFoodService.Verify(s => s.Update(catFood), Times.Once);
        Assert.NotNull(catFood.UpdatedAt);
    }

    [Fact]
    public void UpdateBestPrice_ShouldCallServiceUpdate()
    {
        var bestPrice = new BestPrice { Id = 1, Name = "Test" };

        _viewModel.UpdateBestPrice(bestPrice);

        _mockBestPriceService.Verify(s => s.Update(bestPrice), Times.Once);
        Assert.NotNull(bestPrice.UpdatedAt);
    }

    [Fact]
    public void DeleteCatFood_ShouldCallServiceDelete()
    {
        _viewModel.DeleteCatFood(1);

        _mockCatFoodService.Verify(s => s.Delete(1), Times.Once);
    }

    [Fact]
    public void DeleteBestPrice_ShouldCallServiceDelete()
    {
        _viewModel.DeleteBestPrice(1);

        _mockBestPriceService.Verify(s => s.Delete(1), Times.Once);
    }

    [Fact]
    public void CanGoPrevious_WhenAtFirstPage_ShouldReturnFalse()
    {
        _viewModel.CurrentPage = 1;

        Assert.False(_viewModel.CanGoPrevious);
    }

    [Fact]
    public void CanGoPrevious_WhenNotAtFirstPage_ShouldReturnTrue()
    {
        _viewModel.CurrentPage = 2;

        Assert.True(_viewModel.CanGoPrevious);
    }

    [Fact]
    public void CanGoNext_WhenAtLastPage_ShouldReturnFalse()
    {
        _viewModel.CurrentPage = 5;
        _viewModel.PageCount = 5;

        Assert.False(_viewModel.CanGoNext);
    }

    [Fact]
    public void CanGoNext_WhenNotAtLastPage_ShouldReturnTrue()
    {
        _viewModel.CurrentPage = 1;
        _viewModel.PageCount = 5;

        Assert.True(_viewModel.CanGoNext);
    }
}
