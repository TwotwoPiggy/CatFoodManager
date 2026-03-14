using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Models.Dtos;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CommonTools;
using Lemon.UI.Controls;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;
using SQLitePCL;
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
        #endregion

        #region Page

        /// <summary>
        /// 总记录数
        /// </summary>
        private int _totalCount = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        private int _pageCount = 0;

        /// <summary>
        /// 当前页数
        /// </summary>
        private int _currentPage = 0;

        /// <summary>
        /// 页数最大记录
        /// </summary>
        private int _pageSize => Int32.TryParse(pageSizeComboBox.SelectedItem?.ToString(), out int pageSize) ? pageSize : 50;

        #endregion

        #region view
        private BindingSource _bindingSource = [];
        private PictureView _pictureView;
        private BrandManager _brandManager;
        private LowestPrice _lowestPrice;
        #endregion

        #region fields
        private Dictionary<string, string[]?>? _pictureFolders;
        private IEnumerable<PlatformRegExp> _platformRegExp;
        private const string _baseCatfoodQueryString = "SELECT DISTINCT a.*\r\nFROM CatFood a \r\nLEFT JOIN Brand b ON a.BrandId = b.Id \r\nWHERE b.Name like";
        private const string _baseBestPriceQueryString = "SELECT DISTINCT a.*\r\nFROM BestPrice a\r\nWHERE a.Name like";

        private bool IsLowestPrice => this.rbnLowestPrice.Checked;

        private static readonly IEnumerable<string> _searchableProperties = typeof(CatFood)
            .GetProperties()
            .Where(p => !p.CustomAttributes.Any(a => a.AttributeType.Name == "IgnoreAttribute" || a.AttributeType.BaseType?.Name == "RelationshipAttribute"))
            .Select(p => p.Name)
            .ToList();
        #endregion

        public Main(IService<CatFood> catFoodSerivce, IService<Brand> brandService, IService<Factory> factoryService, IService<BestPrice> lowestPriceService,
                    IPlatformRegExpService regExpService, PictureContentService pictureContentService,
                    BrandManager brandManager, LowestPrice lowestPrice, IGeminiOcrService geminiOcrService)
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
            InitializeContext();
        }

        #region events
        private void Main_Load(object sender, EventArgs e)
        {
            LoadConfigs();
            InitComponents();
            InitColumns();
            LoadData();
            pageSizeComboBox.SelectedIndexChanged += pageSizeComboBox_SelectedIndexChanged;
            dataView.KeyDown += DataView_KeyDown;
            dataView.Leave += DataView_Leave;
            dataView.CellFormatting += DataView_CellFormatting;
        }

        //todo: create BestPrice & progress bar
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
                this.btnFunction.Text = ComponentConfigs.CreateButtonName;
                InitColumns();
                LoadData();
            }
        }

        private void rtbInventory_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLowestPrice)
            {
                this.btnFunction.Text = ComponentConfigs.SyncButtonName;
                InitColumns();
                LoadData();
            }
        }

        #endregion

        #region search control
        private void searchBtn_Click(object sender, EventArgs e)
        {
            var searchKey = searchText.Text?.Trim();
            if (string.IsNullOrEmpty(searchKey))
            {
                LoadData();
                return;
            }
            if (searchKey == "罐头" || searchKey == "冻干")
            {
                searchKey = $"主食{searchKey}";
            }
            var sb = new StringBuilder(IsLowestPrice ? _baseBestPriceQueryString : _baseCatfoodQueryString);
            var args = new List<object>();

            // First condition: WHERE b.Name like ?
            sb.Append(" ?");
            args.Add($"%{searchKey}%");
            // Handle specific keywords for ProductType
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
            catch (Exception)
            {

                throw;
            }

            if (!IsLowestPrice)
            {
                sb.Append(" OR a.Id LIKE ?");
                args.Add($"%{searchKey}%");
            }

            //foreach (var prop in _searchableProperties)
            //{
            //    sb.Append($" OR a.{prop} LIKE ?");
            //    args.Add($"%{searchKey}%");
            //}


            LoadData(sb.ToString(), [.. args]);
        }


        private void searchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchBtn_Click(sender, e);
            }
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            searchBtn_Click(sender, e);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.searchText.Text = string.Empty;
        }
        #endregion

        #region Main View
        private void DataView_Leave(object? sender, EventArgs e)
        {
            dataView.EndEdit();
        }

        private void DataView_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dataView.EndEdit();
                e.Handled = true;
            }
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView dataGridView || dataGridView.EditingControl == null) return;
            var id = GetIdInSpecifiedRow<long>(sender, e.RowIndex, "Id");
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
            else//catfood inventory
            {
                UpdateCell(_catFoodSerivce, (long)id, fieldName, valueToUpdate);
            }
            TimedMessageBox.Show("已更新!", "操作成功", 3, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// enable the comboBox value change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!dataView.IsCurrentCellDirty) return;
            if (sender is not DataGridView) return;
            foreach (DataGridViewCell cell in ((DataGridView)sender).SelectedCells)
            {
                if (cell.ColumnIndex == dataView.Columns["TypeToShow"]?.Index)
                {
                    dataView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    continue;
                }
                if (cell.ColumnIndex == dataView.Columns["HasPurchased"]?.Index ||
                    cell.ColumnIndex == dataView.Columns["HasTestReport"]?.Index ||
                    cell.ColumnIndex == dataView.Columns["IsWorthRepurchasing"]?.Index)
                {
                    dataView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    if (!IsLowestPrice) return;
                    var currentCell = dataView.CurrentCell;
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


        private void DataView_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            var picturePath = GetCellValueInSpecifiedColumn<string>(sender, e.ColumnIndex, "PictureButton", e.RowIndex, "PicturePath");

            if (picturePath == null) return;
            if (!string.IsNullOrEmpty(picturePath))
            {
                _pictureView = _pictureView == null || _pictureView.IsDisposed ? new PictureView(picturePath) : _pictureView;
                _pictureView.Show();
            }
            else if (openFileDialog.ShowDialog() == DialogResult.OK)//picturePath is empty, upload picture
            {
                var id = GetIdInSpecifiedRow<long>(sender, e.RowIndex, "Id");
                if (id == null) return;
                var bestPrice = _lowestPriceService.Query((long)id);
                var pictureToUpdate = this.openFileDialog.FileName;
                if (bestPrice != null && !string.IsNullOrWhiteSpace(pictureToUpdate))
                {
                    bestPrice.PicturePath = pictureToUpdate;
                    bestPrice.UpdatedAt = DateTime.Now;
                    _lowestPriceService.Update(bestPrice);
                    LoadData();
                }
            }
        }

        private void DataView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            var picturePath = GetCellValueInSpecifiedColumn<string>(sender, e.ColumnIndex, "PictureButton", e.RowIndex, "PicturePath");
            if (picturePath == null) return;
            e.Value = string.IsNullOrWhiteSpace(picturePath) ? "上传图片" : "查看照片";
            e.FormattingApplied = true;
        }
        #endregion

        #region page control

        private void pageSizeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void homeBtn_Click(object sender, EventArgs e)
        {
            if (!homeBtn.Enabled) return;
            _currentPage = 1;
            LoadData();
        }

        private void prePageBtn_Click(object sender, EventArgs e)
        {
            if (!prePageBtn.Enabled) return;
            _currentPage--;
            LoadData();
        }

        private void nextPageBtn_Click(object sender, EventArgs e)
        {
            if (!nextPageBtn.Enabled) return;
            _currentPage++;
            LoadData();
        }

        private void lastPageBtn_Click(object sender, EventArgs e)
        {
            if (!lastPageBtn.Enabled) return;
            _currentPage = _pageCount;
            LoadData();
        }

        private void jumpBtn_Click(object sender, EventArgs e)
        {
            #region validation
            if (!Int32.TryParse(jumpPageText.Text, out int gotoPage))
            {
                MessageBox.Show("待跳转的页数不合规, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (gotoPage <= 0)
            {
                MessageBox.Show("待跳转的页数超出当前支持的最小页数, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (gotoPage > _pageCount)
            {
                MessageBox.Show("待跳转的页数超出当前支持的最大页数, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
            _currentPage = gotoPage;
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

        private void InitComponents()
        {
            pageSizeComboBox.SelectedIndex = 2;
            dataView.EditMode = DataGridViewEditMode.EditOnEnter;
            dataView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _currentPage = 1;
            this.btnFunction.Text = ComponentConfigs.CreateButtonName;
        }

        private void InitColumns()
        {
            dataView.Columns.Clear();
            dataView.AutoGenerateColumns = false;
            // When in lowest price mode, make columns fill the available grid width
            dataView.AutoSizeColumnsMode = IsLowestPrice ? DataGridViewAutoSizeColumnsMode.Fill : DataGridViewAutoSizeColumnsMode.None;
            var headersToShow = IsLowestPrice ? ColumnHeaders.BestPriceHeaders : ColumnHeaders.CatFoodHeaders;
            foreach (DataGridViewColumn column in headersToShow.Values)
            {
                if (column == null) continue;
                column.Visible = !CustomFilters.ColumnsDisableToShow.Contains(column.Name);
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // Only set AutoSizeMode to AllCells when it hasn't been specified in the column definition
                if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                if (column.Name == "TypeToShow")
                {
                    if (column is not DataGridViewComboBoxColumn comboBox) continue;
                    SetComboBoxWithEnums(comboBox);
                }
                else if (column.Name == "PictureButton")
                {
                    if (column is not DataGridViewButtonColumn button) continue;
                    SetButtonCellColumn(button);
                }
                dataView.Columns.Add(column);
            }
            dataView.CellClick += new DataGridViewCellEventHandler(DataView_CellClick);

        }

        private void SetLabels()
        {
            totalLabel.Text = $"共 {_totalCount} 条记录";
            pageInfoLabel.Text = $"当前页 {_currentPage}/{_pageCount}";
        }

        private void LoadData(string? filter = null, params object[] args)
        {
            var queryArgs = args ?? [];
            if (IsLowestPrice)
            {
                (var lowestPriceResults, _totalCount) = string.IsNullOrWhiteSpace(filter) ? _lowestPriceService.GetAllWithCount() : _lowestPriceService.FuzzyQueryWithCount(filter, queryArgs);
                _bindingSource!.DataSource = lowestPriceResults.Skip((_currentPage - 1) * _pageSize).Take(_pageSize);
            }
            else
            {
                (var catFoodResults, _totalCount) = string.IsNullOrWhiteSpace(filter) ? _catFoodSerivce.GetAllWithCount() : _catFoodSerivce.FuzzyQueryWithCount(filter, queryArgs);
                _bindingSource!.DataSource = catFoodResults.Skip((_currentPage - 1) * _pageSize).Take(_pageSize);
            }

            dataView.DataSource = _bindingSource;
            _pageCount = Convert.ToInt32(Math.Ceiling((double)_totalCount / _pageSize));
            SetLabels();
            ControlButtons();
            Refresh();
        }

        private void LoadConfigs()
        {
            picConfigText.Text = ConfigManager.GetAppConfig(ConfigNames.PictureFolders);
        }

        private void ControlButtons()
        {
            homeBtn.Enabled = prePageBtn.Enabled = _currentPage != 1;
            lastPageBtn.Enabled = nextPageBtn.Enabled = _currentPage != _pageCount;
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
                                                        .Where(p => CustomFilters
                                                                        .PictureExtensions
                                                                        .Contains(p.GetExtension().TrimStart('.').ToLower()))
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

            //var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts.json");
            PromptLoader.Load("Prompts.json");
            //if (!File.Exists(promptPath))
            //{
            //    MessageBox.Show("Prompts.json配置文件不存在, 请检查", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //var promptsJson = File.ReadAllText(promptPath);
            //var prompts = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(promptsJson);PromptLoader.Get(PromptConstants.OcrAssistantPrompt)
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


        private long? GetIdInSpecifiedRow<T>(object? sender, int rowIndex, string targetColumnName)
        {
            if (sender is not DataGridView dataGridView)
                return null;
            var row = dataGridView.Rows[rowIndex];
            if (row == null) return default;
            var cell = row.Cells[targetColumnName];
            if (cell == null || cell.Value == null) return default;
            return (long)cell.Value;
        }

        private long? GetIdInSpecifiedRow<T>(DataGridView dataGridView, int rowIndex, string targetColumnName)
        {
            if (dataGridView == null)
                return null;
            var row = dataGridView.Rows[rowIndex];
            if (row == null) return default;
            var cell = row.Cells[targetColumnName];
            if (cell == null || cell.Value == null) return default;
            return (long)cell.Value;
        }

        private T? GetCellValueInSpecifiedColumn<T>(object? sender, int columnIndex, string sourceColumnName, int rowIndex, string targetColumnName) where T : class
        {
            if (sender is not DataGridView dataGridView || columnIndex != dataGridView.Columns[sourceColumnName]?.Index)
                return null;
            var row = dataGridView.Rows[rowIndex];
            if (row == null) return default;
            var cell = row.Cells[targetColumnName];
            if (cell == null || cell.Value == null) return default;
            return (T)cell.Value;
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
