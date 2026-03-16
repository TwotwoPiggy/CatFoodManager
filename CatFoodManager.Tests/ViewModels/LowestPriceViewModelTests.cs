using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.ViewModels;
using Moq;
using Xunit;

namespace CatFoodManager.Tests.ViewModels;

public class LowestPriceViewModelTests
{
    private readonly Mock<IService<BestPrice>> _mockBestPriceService;
    private readonly LowestPriceViewModel _viewModel;

    public LowestPriceViewModelTests()
    {
        _mockBestPriceService = new Mock<IService<BestPrice>>();
        _mockBestPriceService.Setup(s => s.GetAllWithCount()).Returns((new List<BestPrice>(), 0));

        _viewModel = new LowestPriceViewModel(_mockBestPriceService.Object);
    }

    [Fact]
    public void Constructor_ShouldInitializeCommands()
    {
        Assert.NotNull(_viewModel.LoadDataCommand);
        Assert.NotNull(_viewModel.SearchCommand);
        Assert.NotNull(_viewModel.ResetSearchCommand);
        Assert.NotNull(_viewModel.AddBestPriceCommand);
        Assert.NotNull(_viewModel.UpdateBestPriceCommand);
        Assert.NotNull(_viewModel.DeleteBestPriceCommand);
        Assert.NotNull(_viewModel.NextPageCommand);
        Assert.NotNull(_viewModel.PreviousPageCommand);
        Assert.NotNull(_viewModel.FirstPageCommand);
        Assert.NotNull(_viewModel.LastPageCommand);
    }

    [Fact]
    public void Constructor_WithNullService_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LowestPriceViewModel(null!));
    }

    [Fact]
    public void LoadData_ShouldLoadBestPrices()
    {
        var bestPrices = new List<BestPrice>
        {
            new() { Id = 1, Name = "BestPrice1" },
            new() { Id = 2, Name = "BestPrice2" }
        };
        _mockBestPriceService.Setup(s => s.GetAllWithCount()).Returns((bestPrices, 2));

        _viewModel.LoadData();

        Assert.Equal(2, _viewModel.TotalCount);
    }

    [Fact]
    public void LoadData_WithPaging_ShouldReturnCorrectPage()
    {
        var bestPrices = Enumerable.Range(1, 100)
            .Select(i => new BestPrice { Id = i, Name = $"BestPrice{i}" })
            .ToList();
        _mockBestPriceService.Setup(s => s.GetAllWithCount()).Returns((bestPrices, 100));

        _viewModel.PageSize = 10;
        _viewModel.CurrentPage = 2;
        _viewModel.LoadData();

        Assert.Equal(10, _viewModel.BestPrices.Count);
    }

    [Fact]
    public void Search_WithEmptyText_ShouldLoadAllData()
    {
        _viewModel.SearchText = "   ";
        _viewModel.Search();

        _mockBestPriceService.Verify(s => s.GetAllWithCount(), Times.AtLeastOnce);
    }

    [Fact]
    public void Search_WithText_ShouldCallFuzzyQueryWithCount()
    {
        _viewModel.SearchText = "Test";

        _viewModel.Search();

        _mockBestPriceService.Verify(s => s.FuzzyQueryWithCount(It.IsAny<string>()), Times.Once);
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
    public void AddBestPrice_WithValidBestPrice_ShouldCallServiceSave()
    {
        var bestPrice = new BestPrice { Name = "NewBestPrice" };

        _viewModel.AddBestPrice(bestPrice);

        _mockBestPriceService.Verify(s => s.Save(bestPrice), Times.Once);
    }

    [Fact]
    public void AddBestPrice_WithNullBestPrice_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _viewModel.AddBestPrice(null!));
    }

    [Fact]
    public void UpdateBestPrice_WithValidBestPrice_ShouldCallServiceUpdate()
    {
        var bestPrice = new BestPrice { Id = 1, Name = "UpdatedBestPrice" };
        var existingBestPrice = new BestPrice { Id = 1, Name = "OldBestPrice" };
        _mockBestPriceService.Setup(s => s.Query(1L)).Returns(existingBestPrice);

        _viewModel.UpdateBestPrice(bestPrice);

        _mockBestPriceService.Verify(s => s.Update(existingBestPrice), Times.Once);
    }

    [Fact]
    public void UpdateBestPrice_WithNullBestPrice_ShouldNotCallServiceUpdate()
    {
        _viewModel.UpdateBestPrice(null);

        _mockBestPriceService.Verify(s => s.Update(It.IsAny<BestPrice>()), Times.Never);
    }

    [Fact]
    public void UpdateBestPriceProperty_ShouldUpdateSpecificProperty()
    {
        var existingBestPrice = new BestPrice { Id = 1, Name = "Test", LowestPrice = 100 };
        _mockBestPriceService.Setup(s => s.Query(1L)).Returns(existingBestPrice);

        _viewModel.UpdateBestPriceProperty(1, nameof(BestPrice.LowestPrice), 200m);

        Assert.Equal(200m, existingBestPrice.LowestPrice);
        _mockBestPriceService.Verify(s => s.Update(existingBestPrice), Times.Once);
    }

    [Fact]
    public void DeleteBestPrice_ShouldCallServiceDelete()
    {
        _viewModel.DeleteBestPrice(1);

        _mockBestPriceService.Verify(s => s.Delete(1), Times.Once);
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

    [Fact]
    public void ProductTypes_ShouldReturnAllProductTypes()
    {
        var types = _viewModel.ProductTypes.ToList();

        Assert.NotEmpty(types);
        Assert.Contains(CatFoodManager.Core.Statics.ProductType.CatFood, types);
    }

    [Fact]
    public void PlatformTypes_ShouldReturnAllPlatformTypes()
    {
        var types = _viewModel.PlatformTypes.ToList();

        Assert.NotEmpty(types);
        Assert.Contains(CatFoodManager.Core.Statics.PlatformType.JD, types);
    }
}
