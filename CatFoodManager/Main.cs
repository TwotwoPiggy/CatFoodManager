using CatFoodManager.Controls;
using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Models.Dtos;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CatFoodManager.ViewModels;
using CommonTools;
using Lemon.UI.Controls;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Twotwo.Agent.Constants;
using Twotwo.Agent.Services;

namespace CatFoodManager
{
    public partial class Main : Form
    {
        #region services
        private readonly IService<CatFood> _catFoodSerivce;
        private readonly IService<Brand> _brandService;
        private readonly IService<Factory> _factoryService;
        private readonly IService<BestPrice> _lowestPriceService;
        private readonly IPlatformRegExpService _regExpService;
        private readonly PictureContentService _pictureContentService;
        private readonly IGeminiOcrService _geminiOcrService;
        private readonly MainViewModel _viewModel;
        #endregion

        #region view
        private PictureView? _pictureView;
        private BrandManager? _brandManager;
        private LowestPrice? _lowestPrice;
        private readonly OpenFileDialog _openFileDialog = new();
        #endregion

        #region fields
        private Dictionary<string, string[]?>? _pictureFolders;
        private const string BaseCatfoodQueryString = "SELECT DISTINCT a.*\r\nFROM CatFood a \r\nLEFT JOIN Brand b ON a.BrandId = b.Id \r\nWHERE b.Name like";
        private const string BaseBestPriceQueryString = "SELECT DISTINCT a.*\r\nFROM BestPrice a\r\nWHERE a.Name like";

        private bool IsLowestPrice => rbnLowestPrice.Checked;
        #endregion

        public Main(
            IService<CatFood> catFoodSerivce,
            IService<Brand> brandService,
            IService<Factory> factoryService,
            IService<BestPrice> lowestPriceService,
            IPlatformRegExpService regExpService,
            PictureContentService pictureContentService,
            BrandManager brandManager,
            LowestPrice lowestPrice,
            IGeminiOcrService geminiOcrService,
            MainViewModel viewModel)
        {
            InitializeComponent();
            _catFoodSerivce = catFoodSerivce;
            _brandService = brandService;
            _factoryService = factoryService;
            _lowestPriceService = lowestPriceService;
            _regExpService = regExpService;
            _pictureContentService = pictureContentService;
            _pictureFolders = [];
            _brandManager = brandManager;
            _lowestPrice = lowestPrice;
            _geminiOcrService = geminiOcrService;
            _viewModel = viewModel;
            InitializeContext();
            SetupControlBindings();
        }

        #region events
        private void Main_Load(object sender, EventArgs e)
        {
            LoadConfigs();
            InitComponents();
            InitColumns();
            LoadData();
        }

        private async void btnFunction_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsLowestPrice)
                {
                    _lowestPrice = _lowestPrice == null || _lowestPrice.IsDisposed ? new LowestPrice(_lowestPriceService) : _lowestPrice;
                    _lowestPrice.Show();
                }
                else
                {
                    await Sync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                LoadData();
            }
        }

        private void brandManagerBtn_Click(object sender, EventArgs e)
        {
            _brandManager = _brandManager == null || _brandManager.IsDisposed ? new BrandManager(_brandService) : _brandManager;
            _brandManager.Show();
        }

        #region radio buttons
        private void rbnLowestPrice_CheckedChanged(object sender, EventArgs e)
        {
            if (IsLowestPrice)
            {
                btnFunction.Text = ComponentConfigs.CreateButtonName;
                _viewModel.IsLowestPrice = true;
                InitColumns();
                LoadData();
            }
        }

        private void rtbInventory_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLowestPrice)
            {
                btnFunction.Text = ComponentConfigs.SyncButtonName;
                _viewModel.IsLowestPrice = false;
                InitColumns();
                LoadData();
            }
        }
        #endregion

        #region search control
        private void searchUserControl_SearchClicked(object sender, EventArgs e)
        {
            var searchKey = searchUserControl.SearchText?.Trim();
            if (string.IsNullOrEmpty(searchKey))
            {
                LoadData();
                return;
            }
            if (searchKey == "罐头" || searchKey == "冻干")
            {
                searchKey = $"主食{searchKey}";
            }
            var sb = new StringBuilder(IsLowestPrice ? BaseBestPriceQueryString : BaseCatfoodQueryString);
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

            LoadData(sb.ToString(), [.. args]);
        }

        private void searchUserControl_ResetClicked(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Main View
        private void dataGridUserControl_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridUserControl dataGridControl) return;
            var dataGridView = dataGridControl.DataGridView;
            if (dataGridView.EditingControl == null) return;

            var id = dataGridControl.GetIdInRow(e.RowIndex, "Id");
            if (id == null) return;

            var currentCell = dataGridView.CurrentCell;
            var valueToUpdate = currentCell?.EditedFormattedValue;
            if (valueToUpdate == null) return;

            var fieldName = currentCell?.OwningColumn?.Name;
            if (fieldName == "TypeToShow")
            {
                fieldName = "Type";
            }

            if (IsLowestPrice)
            {
                UpdateCell(_lowestPriceService, (long)id, fieldName, valueToUpdate);
            }
            else
            {
                UpdateCell(_catFoodSerivce, (long)id, fieldName, valueToUpdate);
            }
            TimedMessageBox.Show("已更新!", "操作成功", 3, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridUserControl_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (sender is not DataGridUserControl dataGridControl) return;
            var dataGridView = dataGridControl.DataGridView;
            if (!dataGridView.IsCurrentCellDirty) return;

            foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            {
                if (cell.ColumnIndex == dataGridView.Columns["TypeToShow"]?.Index)
                {
                    dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    continue;
                }
                if (cell.ColumnIndex == dataGridView.Columns["HasPurchased"]?.Index ||
                    cell.ColumnIndex == dataGridView.Columns["HasTestReport"]?.Index ||
                    cell.ColumnIndex == dataGridView.Columns["IsWorthRepurchasing"]?.Index)
                {
                    dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    if (!IsLowestPrice) return;

                    var currentCell = dataGridView.CurrentCell;
                    var id = currentCell?.OwningRow?.Cells["Id"].Value;
                    if (id == null) return;

                    var bestPrice = _lowestPriceService.Query((long)id);
                    var valueToUpdate = currentCell?.EditedFormattedValue;
                    var fieldName = currentCell?.OwningColumn?.Name;

                    if (bestPrice != null && valueToUpdate != null && fieldName != null)
                    {
                        var propertyToUpdate = bestPrice.GetType().GetProperty(fieldName);
                        propertyToUpdate?.SetValue(bestPrice, (bool)valueToUpdate);
                        bestPrice.UpdatedAt = DateTime.Now;
                        _lowestPriceService.Update(bestPrice);
                    }
                }
            }
        }

        private void dataGridUserControl_CellClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridUserControl dataGridControl) return;
            var dataGridView = dataGridControl.DataGridView;

            var picturePath = dataGridControl.GetCellValue<string>(e.RowIndex, "PicturePath");
            var columnIndex = e.ColumnIndex;

            if (dataGridView.Columns["PictureButton"]?.Index != columnIndex || picturePath == null) return;

            if (!string.IsNullOrEmpty(picturePath))
            {
                _pictureView = _pictureView == null || _pictureView.IsDisposed ? new PictureView(picturePath) : _pictureView;
                _pictureView.Show();
            }
            else if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var id = dataGridControl.GetIdInRow(e.RowIndex, "Id");
                if (id == null) return;

                var bestPrice = _lowestPriceService.Query((long)id);
                var pictureToUpdate = _openFileDialog.FileName;
                if (bestPrice != null && !string.IsNullOrWhiteSpace(pictureToUpdate))
                {
                    bestPrice.PicturePath = pictureToUpdate;
                    bestPrice.UpdatedAt = DateTime.Now;
                    _lowestPriceService.Update(bestPrice);
                    LoadData();
                }
            }
        }

        private void dataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is not DataGridView dataGridView) return;

            if (dataGridView.Columns["PictureButton"]?.Index != e.ColumnIndex) return;

            var row = dataGridView.Rows[e.RowIndex];
            if (row == null) return;

            var cell = row.Cells["PicturePath"];
            var picturePath = cell?.Value?.ToString();

            e.Value = string.IsNullOrWhiteSpace(picturePath) ? "上传图片" : "查看照片";
            e.FormattingApplied = true;
        }
        #endregion

        #region page control
        private void paginationUserControl_FirstPageClicked(object sender, EventArgs e)
        {
            _viewModel.GoToFirstPage();
            LoadData();
        }

        private void paginationUserControl_PreviousPageClicked(object sender, EventArgs e)
        {
            _viewModel.PreviousPage();
            LoadData();
        }

        private void paginationUserControl_NextPageClicked(object sender, EventArgs e)
        {
            _viewModel.NextPage();
            LoadData();
        }

        private void paginationUserControl_LastPageClicked(object sender, EventArgs e)
        {
            _viewModel.GoToLastPage();
            LoadData();
        }

        private void paginationUserControl_GoToPageClicked(object sender, int page)
        {
            _viewModel.GoToPage(page);
            LoadData();
        }

        private void paginationUserControl_PageSizeChanged(object sender, int pageSize)
        {
            _viewModel.UpdatePageSize(pageSize);
            LoadData();
        }
        #endregion

        #region config control
        private void savePicConfigBtn_Click(object sender, EventArgs e)
        {
            ConfigManager.SetAppConfig(ConfigNames.PictureFolders, picConfigText.Text);
        }
        #endregion

        #endregion

        #region private methods

        private void InitializeContext()
        {
            Context.FillConnectionString(ConfigManager.GetConnectionString("SQLite"));
            Context.FillPlatformRegExps(_regExpService.GetAll());
        }

        private void SetupControlBindings()
        {
            searchUserControl.SearchClicked += searchUserControl_SearchClicked;
            searchUserControl.ResetClicked += searchUserControl_ResetClicked;

            paginationUserControl.FirstPageClicked += paginationUserControl_FirstPageClicked;
            paginationUserControl.PreviousPageClicked += paginationUserControl_PreviousPageClicked;
            paginationUserControl.NextPageClicked += paginationUserControl_NextPageClicked;
            paginationUserControl.LastPageClicked += paginationUserControl_LastPageClicked;
            paginationUserControl.GoToPageClicked += paginationUserControl_GoToPageClicked;
            paginationUserControl.PageSizeChanged += paginationUserControl_PageSizeChanged;

            dataGridUserControl.CellClicked += dataGridUserControl_CellClicked;
            dataGridUserControl.CellValueChanged += dataGridUserControl_CellValueChanged;
            dataGridUserControl.CurrentCellDirtyStateChanged += dataGridUserControl_CurrentCellDirtyStateChanged;
            dataGridUserControl.DataGridView.CellFormatting += dataGridView_CellFormatting;
        }

        private void InitComponents()
        {
            paginationUserControl.Initialize(2);
            dataGridUserControl.Initialize();
            _viewModel.CurrentPage = 1;
            btnFunction.Text = ComponentConfigs.CreateButtonName;
        }

        private void InitColumns()
        {
            dataGridUserControl.ClearColumns();
            var autoSizeColumnsMode = IsLowestPrice;
            var headersToShow = IsLowestPrice ? ColumnHeaders.BestPriceHeaders : ColumnHeaders.CatFoodHeaders;

            foreach (DataGridViewColumn column in headersToShow.Values)
            {
                if (column == null) continue;
                column.Visible = !CustomFilters.ColumnsDisableToShow.Contains(column.Name);

                if (column.Name == "TypeToShow")
                {
                    if (column is DataGridViewComboBoxColumn comboBox)
                    {
                        SetComboBoxWithEnums(comboBox);
                    }
                }
                else if (column.Name == "PictureButton")
                {
                    if (column is DataGridViewButtonColumn button)
                    {
                        SetButtonCellColumn(button);
                    }
                }
                dataGridUserControl.AddColumn(column);
            }
        }

        private void LoadData(string? filter = null, params object[] args)
        {
            var queryArgs = args ?? [];

            if (IsLowestPrice)
            {
                var (results, totalCount) = string.IsNullOrWhiteSpace(filter)
                    ? _lowestPriceService.GetAllWithCount()
                    : _lowestPriceService.FuzzyQueryWithCount(filter, queryArgs);
                dataGridUserControl.DataSource = results.Skip((_viewModel.CurrentPage - 1) * _viewModel.PageSize).Take(_viewModel.PageSize).ToList();
                _viewModel.TotalCount = totalCount;
            }
            else
            {
                var (results, totalCount) = string.IsNullOrWhiteSpace(filter)
                    ? _catFoodSerivce.GetAllWithCount()
                    : _catFoodSerivce.FuzzyQueryWithCount(filter, queryArgs);
                dataGridUserControl.DataSource = results.Skip((_viewModel.CurrentPage - 1) * _viewModel.PageSize).Take(_viewModel.PageSize).ToList();
                _viewModel.TotalCount = totalCount;
            }

            _viewModel.PageCount = _viewModel.TotalCount == 0 ? 1 : (int)Math.Ceiling((double)_viewModel.TotalCount / _viewModel.PageSize);

            paginationUserControl.CurrentPage = _viewModel.CurrentPage;
            paginationUserControl.PageCount = _viewModel.PageCount;
            paginationUserControl.TotalCount = _viewModel.TotalCount;

            Refresh();
        }

        private void LoadConfigs()
        {
            picConfigText.Text = ConfigManager.GetAppConfig(ConfigNames.PictureFolders);
        }

        private void GetPicturesPath()
        {
            if (_pictureFolders != null && _pictureFolders.Any()) return;

            var directories = ConfigManager.GetAppConfig(ConfigNames.PictureFolders);
            if (string.IsNullOrWhiteSpace(directories))
            {
                MessageBox.Show($"照片路径配置:{directories}无效或者不存在, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _pictureFolders = directories.TrimEnd(';').Split(';')
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .ToDictionary(d => Path.GetFileName(d.Trim()), d =>
                {
                    var directory = d.Trim();
                    if (!directory.IsOrExistDirectory())
                    {
                        return null;
                    }
                    return FileManager
                        .GetFiles(directory)
                        .Where(p => CustomFilters.PictureExtensions.Contains(p.GetExtension().TrimStart('.').ToLower()))
                        .ToArray();
                });

            if (_pictureFolders?.Count == 0)
            {
                var message = $"照片路径配置:{directories}无效或者不存在, 请检查!";
                throw new ArgumentException(message);
            }
        }

        private void SetComboBoxWithEnums(DataGridViewComboBoxColumn comboBox)
        {
            comboBox.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            comboBox.ReadOnly = false;
            comboBox.Width = 70;
            comboBox.Resizable = DataGridViewTriState.False;
            comboBox.DropDownWidth = 200;
            comboBox.MaxDropDownItems = 5;
        }

        private void SetButtonCellColumn(DataGridViewButtonColumn buttonColumn)
        {
            buttonColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            buttonColumn.DefaultCellStyle.BackColor = Color.Gray;
            buttonColumn.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            buttonColumn.Width = 100;
            buttonColumn.Resizable = DataGridViewTriState.False;
            buttonColumn.UseColumnTextForButtonValue = false;
        }

        private async Task Sync()
        {
            GetPicturesPath();
            if (_pictureFolders == null)
            {
                MessageBox.Show("图片路径配置为空, 请检查", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PromptLoader.Load("Prompts.json");
            var ocrPrompt = PromptLoader.Get(PromptConstants.OcrAssistantPrompt);
            if (string.IsNullOrEmpty(ocrPrompt))
            {
                MessageBox.Show("OcrAssistantPrompt配置为空, 请检查", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var bestPrices = new List<BestPrice>();

            foreach (var kv in _pictureFolders)
            {
                var platform = kv.Key;
                var paths = kv.Value;
                if (paths == null) continue;

                var folderPath = Path.GetDirectoryName(paths[0]);
                if (string.IsNullOrEmpty(folderPath)) continue;

                await _geminiOcrService.ValidateModelAsync();

                var dtos = await _geminiOcrService.ProcessPicAsync<BestPriceDto>(folderPath, ocrPrompt);
                foreach (var dto in dtos)
                {
                    var bestPrice = new BestPrice
                    {
                        Name = dto.Name,
                        PurchasedAt = dto.PurchasedAt,
                        FinalPrice = dto.FinalPrice,
                        Platform = Enum.TryParse<PlatformType>(platform, out var platformType) ? platformType : PlatformType.None,
                        HasPurchased = dto.FinalPrice > 0,
                        PicturePath = string.Empty
                    };
                    bestPrices.Add(bestPrice);
                }
            }

            if (bestPrices.Count != 0)
            {
                _lowestPriceService.BatchSave(bestPrices);
            }
        }

        private void UpdateCell<T>(IService<T> service, long id, string? fieldName, object? valueToUpdate) where T : BaseEntity
        {
            if (valueToUpdate == null) return;
            var entity = service.Query(id);
            if (entity == null) return;

            if (fieldName == "TypeToShow")
            {
                fieldName = "Type";
            }

            var propertyToUpdate = entity.GetType().GetProperty(fieldName ?? "");
            propertyToUpdate?.SetValue(entity, TypeDescriptor.GetConverter(propertyToUpdate.PropertyType).ConvertFrom(valueToUpdate));
            entity.UpdatedAt = DateTime.Now;
            service.Update(entity);
        }
        #endregion
    }
}
