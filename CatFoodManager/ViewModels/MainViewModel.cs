using CatFoodManager.Commands;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using CommonTools;
using System.Collections.ObjectModel;
using System.Text;

namespace CatFoodManager.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IService<CatFood> _catFoodService;
    private readonly IService<Brand> _brandService;
    private readonly IService<Factory> _factoryService;
    private readonly IService<BestPrice> _bestPriceService;

    private int _currentPage = 1;
    private int _pageSize = 50;
    private int _totalCount;
    private int _pageCount;
    private string _searchText = string.Empty;
    private bool _isLowestPrice;
    private ObservableCollection<CatFood> _catFoods = [];
    private ObservableCollection<BestPrice> _bestPrices = [];

    private const string BaseCatFoodQueryString = "SELECT DISTINCT a.*\r\nFROM CatFood a \r\nLEFT JOIN Brand b ON a.BrandId = b.Id \r\nWHERE b.Name like";
    private const string BaseBestPriceQueryString = "SELECT DISTINCT a.*\r\nFROM BestPrice a\r\nWHERE a.Name like";

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

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool IsLowestPrice
    {
        get => _isLowestPrice;
        set
        {
            if (SetProperty(ref _isLowestPrice, value))
            {
                CurrentPage = 1;
                LoadDataCommand?.Execute(null);
            }
        }
    }

    public ObservableCollection<CatFood> CatFoods
    {
        get => _catFoods;
        set => SetProperty(ref _catFoods, value);
    }

    public ObservableCollection<BestPrice> BestPrices
    {
        get => _bestPrices;
        set => SetProperty(ref _bestPrices, value);
    }

    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < PageCount;

    public RelayCommand LoadDataCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand NextPageCommand { get; }
    public RelayCommand PreviousPageCommand { get; }
    public RelayCommand FirstPageCommand { get; }
    public RelayCommand LastPageCommand { get; }
    public RelayCommand ResetSearchCommand { get; }
    public RelayCommand<int> GoToPageCommand { get; }

    public MainViewModel(
        IService<CatFood> catFoodService,
        IService<Brand> brandService,
        IService<Factory> factoryService,
        IService<BestPrice> bestPriceService)
    {
        _catFoodService = catFoodService ?? throw new ArgumentNullException(nameof(catFoodService));
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        _factoryService = factoryService ?? throw new ArgumentNullException(nameof(factoryService));
        _bestPriceService = bestPriceService ?? throw new ArgumentNullException(nameof(bestPriceService));

        LoadDataCommand = new RelayCommand(_ => LoadData());
        SearchCommand = new RelayCommand(_ => Search());
        NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanGoNext);
        PreviousPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanGoPrevious);
        FirstPageCommand = new RelayCommand(_ => GoToFirstPage(), _ => CanGoPrevious);
        LastPageCommand = new RelayCommand(_ => GoToLastPage(), _ => CanGoNext);
        ResetSearchCommand = new RelayCommand(_ => ResetSearch());
        GoToPageCommand = new RelayCommand<int>(page => GoToPage(page));
    }

    public void LoadData(string? filter = null, params object[] args)
    {
        var queryArgs = args ?? [];

        if (IsLowestPrice)
        {
            var (results, count) = string.IsNullOrWhiteSpace(filter)
                ? _bestPriceService.GetAllWithCount()
                : _bestPriceService.FuzzyQueryWithCount(filter, queryArgs);
            BestPrices = new ObservableCollection<BestPrice>(results.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
            TotalCount = count;
        }
        else
        {
            var (results, count) = string.IsNullOrWhiteSpace(filter)
                ? _catFoodService.GetAllWithCount()
                : _catFoodService.FuzzyQueryWithCount(filter, queryArgs);
            CatFoods = new ObservableCollection<CatFood>(results.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
            TotalCount = count;
        }

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

        if (searchKey == "罐头" || searchKey == "冻干")
        {
            searchKey = $"主食{searchKey}";
        }

        var sb = new StringBuilder(IsLowestPrice ? BaseBestPriceQueryString : BaseCatFoodQueryString);
        var args = new List<object>();

        sb.Append(" ?");
        args.Add($"%{searchKey}%");

        try
        {
            if (searchKey == "猫粮" || searchKey == "零食" || searchKey == "其他" || searchKey.Contains("主食"))
            {
                sb.Append($" OR a.{(IsLowestPrice ? "Type" : "FoodType")} = ?");
                var enumVal = searchKey.GetEnumFromDescription<ProductType>();
                args.Add((int)enumVal);
            }
            else if (searchKey == "罐头")
            {
                sb.Append($" OR a.{(IsLowestPrice ? "Type" : "FoodType")} = 2");
            }
            else if (searchKey == "冻干")
            {
                sb.Append($" OR a.{(IsLowestPrice ? "Type" : "FoodType")} = 3");
            }
        }
        catch
        {
            throw;
        }

        if (!IsLowestPrice)
        {
            sb.Append(" OR a.Id LIKE ?");
            args.Add($"%{searchKey}%");
        }

        CurrentPage = 1;
        LoadData(sb.ToString(), [.. args]);
    }

    public void ResetSearch()
    {
        SearchText = string.Empty;
        CurrentPage = 1;
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

    public void GoToPage(int page)
    {
        if (page < 1 || page > PageCount)
            return;
        CurrentPage = page;
        LoadData();
    }

    public void UpdatePageSize(int pageSize)
    {
        PageSize = pageSize;
        CurrentPage = 1;
        LoadData();
    }

    public void UpdateCatFood(CatFood catFood)
    {
        ArgumentNullException.ThrowIfNull(catFood);
        catFood.UpdatedAt = DateTime.Now;
        _catFoodService.Update(catFood);
    }

    public void UpdateBestPrice(BestPrice bestPrice)
    {
        ArgumentNullException.ThrowIfNull(bestPrice);
        bestPrice.UpdatedAt = DateTime.Now;
        _bestPriceService.Update(bestPrice);
    }

    public void DeleteCatFood(int id)
    {
        _catFoodService.Delete(id);
        LoadData();
    }

    public void DeleteBestPrice(int id)
    {
        _bestPriceService.Delete(id);
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
