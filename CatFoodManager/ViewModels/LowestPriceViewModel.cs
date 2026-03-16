using CatFoodManager.Commands;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using System.Collections.ObjectModel;

namespace CatFoodManager.ViewModels;

public class LowestPriceViewModel : ViewModelBase
{
    private readonly IService<BestPrice> _bestPriceService;
    private ObservableCollection<BestPrice> _bestPrices = [];
    private BestPrice? _selectedBestPrice;
    private string _searchText = string.Empty;
    private int _currentPage = 1;
    private int _pageSize = 50;
    private int _totalCount;
    private int _pageCount;

    public ObservableCollection<BestPrice> BestPrices
    {
        get => _bestPrices;
        set => SetProperty(ref _bestPrices, value);
    }

    public BestPrice? SelectedBestPrice
    {
        get => _selectedBestPrice;
        set => SetProperty(ref _selectedBestPrice, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => SetProperty(ref _pageSize, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    public int PageCount
    {
        get => _pageCount;
        set => SetProperty(ref _pageCount, value);
    }

    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < PageCount;

    public IEnumerable<ProductType> ProductTypes => Enum.GetValues<ProductType>();
    public IEnumerable<PlatformType> PlatformTypes => Enum.GetValues<PlatformType>();

    public RelayCommand LoadDataCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand ResetSearchCommand { get; }
    public RelayCommand AddBestPriceCommand { get; }
    public RelayCommand<BestPrice> UpdateBestPriceCommand { get; }
    public RelayCommand<long> DeleteBestPriceCommand { get; }
    public RelayCommand NextPageCommand { get; }
    public RelayCommand PreviousPageCommand { get; }
    public RelayCommand FirstPageCommand { get; }
    public RelayCommand LastPageCommand { get; }

    public LowestPriceViewModel(IService<BestPrice> bestPriceService)
    {
        _bestPriceService = bestPriceService ?? throw new ArgumentNullException(nameof(bestPriceService));

        LoadDataCommand = new RelayCommand(_ => LoadData());
        SearchCommand = new RelayCommand(_ => Search());
        ResetSearchCommand = new RelayCommand(_ => ResetSearch());
        AddBestPriceCommand = new RelayCommand(_ => AddBestPrice());
        UpdateBestPriceCommand = new RelayCommand<BestPrice>(price => UpdateBestPrice(price));
        DeleteBestPriceCommand = new RelayCommand<long>(id => DeleteBestPrice(id));
        NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoNext);
        PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoPrevious);
        FirstPageCommand = new RelayCommand(_ => GoToFirstPage(), _ => CanGoPrevious);
        LastPageCommand = new RelayCommand(_ => GoToLastPage(), _ => CanGoNext);
    }

    public void LoadData(string? filter = null, params object[] args)
    {
        var queryArgs = args ?? [];
        var (results, count) = string.IsNullOrWhiteSpace(filter)
            ? _bestPriceService.GetAllWithCount()
            : _bestPriceService.FuzzyQueryWithCount(filter, queryArgs);

        BestPrices = new ObservableCollection<BestPrice>(results.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
        TotalCount = count;
        PageCount = TotalCount == 0 ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize);
        UpdatePageCommands();
    }

    public void Search()
    {
        var searchKey = SearchText.Trim();
        if (string.IsNullOrEmpty(searchKey))
        {
            LoadData();
            return;
        }

        var queryString = $"SELECT * FROM BestPrice WHERE Name like '%{searchKey}%'";
        CurrentPage = 1;
        LoadData(queryString);
    }

    public void ResetSearch()
    {
        SearchText = string.Empty;
        CurrentPage = 1;
        LoadData();
    }

    public void AddBestPrice()
    {
        if (SelectedBestPrice == null || string.IsNullOrWhiteSpace(SelectedBestPrice.Name))
            return;

        SelectedBestPrice.CreatedAt = DateTime.Now;
        _bestPriceService.Save(SelectedBestPrice);
        LoadData();
        SelectedBestPrice = null;
    }

    public void AddBestPrice(BestPrice bestPrice)
    {
        ArgumentNullException.ThrowIfNull(bestPrice);
        if (string.IsNullOrWhiteSpace(bestPrice.Name))
            return;

        bestPrice.CreatedAt = DateTime.Now;
        _bestPriceService.Save(bestPrice);
        LoadData();
    }

    public void UpdateBestPrice(BestPrice? bestPrice)
    {
        if (bestPrice == null)
            return;

        var existingPrice = _bestPriceService.Query(bestPrice.Id);
        if (existingPrice != null)
        {
            existingPrice.Name = bestPrice.Name;
            existingPrice.Type = bestPrice.Type;
            existingPrice.Platform = bestPrice.Platform;
            existingPrice.LowestPrice = bestPrice.LowestPrice;
            existingPrice.FinalPrice = bestPrice.FinalPrice;
            existingPrice.HasPurchased = bestPrice.HasPurchased;
            existingPrice.PicturePath = bestPrice.PicturePath;
            existingPrice.FactoryName = bestPrice.FactoryName;
            existingPrice.HasTestReport = bestPrice.HasTestReport;
            existingPrice.IsWorthRepurchasing = bestPrice.IsWorthRepurchasing;
            existingPrice.UpdatedAt = DateTime.Now;
            _bestPriceService.Update(existingPrice);
            LoadData();
        }
    }

    public void UpdateBestPriceProperty(long id, string propertyName, object? value)
    {
        var bestPrice = _bestPriceService.Query(id);
        if (bestPrice == null)
            return;

        var property = bestPrice.GetType().GetProperty(propertyName);
        if (property != null && value != null)
        {
            property.SetValue(bestPrice, value);
            bestPrice.UpdatedAt = DateTime.Now;
            _bestPriceService.Update(bestPrice);
        }
    }

    public void DeleteBestPrice(long id)
    {
        _bestPriceService.Delete((int)id);
        LoadData();
    }

    public void NextPage()
    {
        if (CurrentPage < PageCount)
        {
            CurrentPage++;
            LoadData();
        }
    }

    public void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            LoadData();
        }
    }

    public void GoToFirstPage()
    {
        CurrentPage = 1;
        LoadData();
    }

    public void GoToLastPage()
    {
        CurrentPage = PageCount;
        LoadData();
    }

    private void UpdatePageCommands()
    {
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
        NextPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        FirstPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
    }
}
