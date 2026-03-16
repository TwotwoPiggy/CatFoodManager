using CatFoodManager.Commands;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using System.Collections.ObjectModel;

namespace CatFoodManager.ViewModels;

public class BrandManagerViewModel : ViewModelBase
{
    private readonly IService<Brand> _brandService;
    private ObservableCollection<Brand> _brands = [];
    private string _searchText = string.Empty;
    private Brand? _selectedBrand;

    public ObservableCollection<Brand> Brands
    {
        get => _brands;
        set => SetProperty(ref _brands, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public Brand? SelectedBrand
    {
        get => _selectedBrand;
        set => SetProperty(ref _selectedBrand, value);
    }

    public RelayCommand LoadDataCommand { get; }
    public RelayCommand SearchCommand { get; }
    public RelayCommand ResetSearchCommand { get; }
    public RelayCommand AddBrandCommand { get; }
    public RelayCommand<Brand> UpdateBrandCommand { get; }
    public RelayCommand<long> DeleteBrandCommand { get; }

    public BrandManagerViewModel(IService<Brand> brandService)
    {
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));

        LoadDataCommand = new RelayCommand(_ => LoadData());
        SearchCommand = new RelayCommand(_ => Search());
        ResetSearchCommand = new RelayCommand(_ => ResetSearch());
        AddBrandCommand = new RelayCommand(_ => AddBrand());
        UpdateBrandCommand = new RelayCommand<Brand>(brand => UpdateBrand(brand));
        DeleteBrandCommand = new RelayCommand<long>(id => DeleteBrand(id));
    }

    public void LoadData(string? filter = null)
    {
        var (results, _) = string.IsNullOrWhiteSpace(filter)
            ? _brandService.GetAllWithCount()
            : _brandService.FuzzyQueryWithCount(filter);
        Brands = new ObservableCollection<Brand>(results);
    }

    public void Search()
    {
        var searchKey = SearchText.Trim();
        if (string.IsNullOrWhiteSpace(searchKey))
        {
            LoadData();
            return;
        }

        var queryString = $"SELECT * FROM Brand WHERE Id LIKE '%{(long.TryParse(searchKey, out long id) ? id : 0)}%' or Name like '%{searchKey}%'";
        LoadData(queryString);
    }

    public void ResetSearch()
    {
        SearchText = string.Empty;
        LoadData();
    }

    public void AddBrand()
    {
        if (SelectedBrand == null || string.IsNullOrWhiteSpace(SelectedBrand.Name))
            return;

        var newBrand = new Brand { Name = SelectedBrand.Name };
        _brandService.Save(newBrand);
        LoadData();
        SelectedBrand = null;
    }

    public void AddBrand(string brandName)
    {
        if (string.IsNullOrWhiteSpace(brandName))
            return;

        var newBrand = new Brand { Name = brandName };
        _brandService.Save(newBrand);
        LoadData();
    }

    public void UpdateBrand(Brand? brand)
    {
        if (brand == null || string.IsNullOrWhiteSpace(brand.Name))
            return;

        var existingBrand = _brandService.Query(brand.Id);
        if (existingBrand != null)
        {
            existingBrand.Name = brand.Name;
            _brandService.Update(existingBrand);
            LoadData();
        }
    }

    public void UpdateBrandName(long id, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return;

        var brand = _brandService.Query(id);
        if (brand != null)
        {
            brand.Name = newName;
            _brandService.Update(brand);
            LoadData();
        }
    }

    public void DeleteBrand(long id)
    {
        _brandService.Delete((int)id);
        LoadData();
    }
}
