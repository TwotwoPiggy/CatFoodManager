using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CommonTools;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace CatFoodManager
{
	public partial class Main : Form
	{
		#region services
		private readonly IService<CatFood> _catFoodSerivce;
		private readonly IService<Brand> _brandService;
		private readonly IService<Factory> _factoryService;
		private readonly IPlatformRegExpService _regExpService;
		private readonly PictureContentService _pictureContentService;

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
		private int _pageSize => Int32.TryParse(pageSizeComboBox.SelectedItem?.ToString(), out int pageSize) ? pageSize : 10;

		#endregion

		#region view
		private BindingSource _bindingSource = [];
		private PictureView? _pictureView;
		private BrandManager? _brandManager;
		#endregion

		#region fields
		private Dictionary<string, string[]?>? _pictureFolders;
		private const string _baseQueryString = "SELECT DISTINCT a.*\r\nFROM CatFood a \r\nLEFT JOIN Brand b ON a.BrandId = b.Id \r\nWHERE b.Name like";
		#endregion

		public Main(IService<CatFood> catFoodSerivce, IService<Brand> brandService, IService<Factory> factoryService,
					IPlatformRegExpService regExpService, PictureContentService pictureContentService)
		{
			InitializeComponent();
			_catFoodSerivce = catFoodSerivce;
			_brandService = brandService;
			_factoryService = factoryService;
			_regExpService = regExpService;
			_pictureContentService = pictureContentService;
			_pictureFolders = [];
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
		}

		//todo: progress bar
		private void syncBtn_Click(object sender, EventArgs e)
		{
			var filesNeedToArchive = new List<string>();
			try
			{
				var regExps = _regExpService.GetAll();
				GetPicturesPath();
				var content = string.Empty;
				PlatformRegExp? regPattern;
				var catFoods = new List<CatFood>();
				CatFood catFood;
				Brand? brand;
				string shopName;
				if (_pictureFolders == null)
				{
					MessageBox.Show("图片路径配置为空, 请检查", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				foreach (var kv in _pictureFolders)
				{
					var platform = kv.Key;
					var paths = kv.Value;
					regPattern = regExps.FirstOrDefault(r => r.Name == platform);
					if (paths == null || regPattern == null)
					{
						continue;
					}
					regPattern.AutoFillFields(true);
					foreach (var path in paths)
					{
						content = _pictureContentService.GetContentFromPicture(path, needReduceNoise: true);
						(catFood, shopName) = _pictureContentService.GenerateCatFood(content, regPattern.RegularExpression!, regPattern.FieldInfoList!, path);
						brand = _brandService.Query(shopName);
						catFood.Brand = brand!;
						catFood.BrandId = brand!.Id;
						catFoods.Add(catFood);
						filesNeedToArchive.Add(path);
					}
				}
				if (catFoods.Count != 0)
				{
					_catFoodSerivce.BatchSave(catFoods);
					_pictureContentService.FileArchive(filesNeedToArchive);

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

		#region search control
		private void searchBtn_Click(object sender, EventArgs e)
		{
			var searchKey = searchText.Text;
			if (string.IsNullOrWhiteSpace(searchKey))
			{
				LoadData();
				return;
			}
			var properties = typeof(CatFood)
								.GetProperties()
								.Where(p =>
										!p.CustomAttributes.Any(a =>
																a.AttributeType.Name == "IgnoreAttribute"
																|| a.AttributeType.BaseType?.Name == "RelationshipAttribute")
										)
								.Select(p => $"\r\nOR a.{p.Name} LIKE '%{searchKey}%'");

			var foodTypeQueryCondition = searchKey == "主食" || searchKey == "零食"
										? $"\r\nOR a.FoodType LIKE '{(int)searchKey.GetEnumFromDescription<CatFoodType>()}'"
										: string.Empty;
			var queryString = $"{_baseQueryString} '%{searchKey}%' {String.Join(' ', properties)} {foodTypeQueryCondition}";
			LoadData(queryString);
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
		#endregion

		#region create control
		private void createBtn_Click(object sender, EventArgs e)
		{

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

		private void dataView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			var dataGridView = sender as DataGridView;
			if (dataGridView != null && dataGridView.EditingControl != null)
			{
				var currentCell = dataGridView.CurrentCell;
				var id = currentCell.OwningRow.Cells["Id"].Value;
				if (id == null)
				{
					return;
				}
				var valueToUpdate = currentCell.EditedFormattedValue;
				var catFood = _catFoodSerivce.Query((long)id);
				if (catFood != null && valueToUpdate != null)
				{
					var fieldName = currentCell.OwningColumn.Name;
					if (fieldName == "FoodTypeToShow")
					{
						fieldName = "FoodType";
					}
					var propertyToUpdate = catFood.GetType().GetProperty(fieldName);
					var typeConverter = new TypeConverter();
					propertyToUpdate?.SetValue(catFood, TypeDescriptor.GetConverter(propertyToUpdate.PropertyType).ConvertFrom(valueToUpdate));
					_catFoodSerivce.Update(catFood);
				}
				MessageBox.Show("已更新!", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		/// <summary>
		/// enable the comboBox value change event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (dataView.IsCurrentCellDirty)
			{
				if (sender is DataGridView)
				{
					foreach (DataGridViewCell cell in ((DataGridView)sender).SelectedCells)
					{
						if (cell.ColumnIndex == dataView.Columns["FoodTypeToShow"].Index)
						{
							dataView.CommitEdit(DataGridViewDataErrorContexts.Commit);
						}
					}
				}
				//dataView.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}


		private void DataView_CellClick(object? sender, DataGridViewCellEventArgs e)
		{
			// Ignore clicks that are not on button cells. 
			if (e.RowIndex < 0 || e.ColumnIndex !=
				dataView.Columns["PictureButton"].Index) return;

			var dataGridView = sender as DataGridView;
			if (dataGridView != null)
			{
				var picturePath = dataGridView.Rows[e.RowIndex].Cells["PicturePath"].Value.ToString();
				_pictureView = _pictureView == null || _pictureView.IsDisposed ? new PictureView(picturePath) : _pictureView;
				_pictureView.Show();
			}
		}
		#endregion

		#region page control

		private void pageSizeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
		{
			LoadData();
		}

		private void pageSizeComboBox_SelectedValueChanged(object sender, EventArgs e)
		{

		}

		private void homeBtn_Click(object sender, EventArgs e)
		{
			if (!homeBtn.Enabled)
			{
				return;
			}
			_currentPage = 1;
			LoadData();
		}

		private void prePageBtn_Click(object sender, EventArgs e)
		{
			if (!prePageBtn.Enabled)
			{
				return;
			}
			_currentPage--;
			LoadData();
		}

		private void nextPageBtn_Click(object sender, EventArgs e)
		{
			if (!nextPageBtn.Enabled)
			{
				return;
			}
			_currentPage++;
			LoadData();
		}

		private void lastPageBtn_Click(object sender, EventArgs e)
		{
			if (!lastPageBtn.Enabled)
			{
				return;
			}
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


		private void jumpPageText_TextChanged(object sender, EventArgs e)
		{
			jumpBtn_Click(sender, e);
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
			pageSizeComboBox.SelectedIndex = 0;
			dataView.EditMode = DataGridViewEditMode.EditOnEnter;
			dataView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			_currentPage = 1;
		}

		private void InitColumns()
		{
			dataView.AutoGenerateColumns = false;

			foreach (DataGridViewColumn column in ColumnHeaders.CatFoodHeaders.Values)
			{
				if (column == null)
				{
					continue;
				}
				column.Visible = !CustomFilters.ColumnsDisableToShow.Contains(column.Name);
				column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				if (column.Name == "FoodTypeToShow")
				{
					var comboBox = column as DataGridViewComboBoxColumn;
					SetComboBoxWithEnums(comboBox);
				}
				if (column.Name == "PictureButton")
				{
					var button = column as DataGridViewButtonColumn;
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

		private void LoadData(string? filter = null)
		{
			//var isFirstLoad = _bindingSource == null;
			(var catFoodResults, _totalCount) = string.IsNullOrWhiteSpace(filter) ? _catFoodSerivce.GetAllWithCount() : _catFoodSerivce.FuzzyQueryWithCount(filter);
			_bindingSource!.DataSource = catFoodResults.Skip((_currentPage - 1) * _pageSize).Take(_pageSize);
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
			var directories = ConfigManager.GetAppConfig(ConfigNames.PictureFolders);
			if (string.IsNullOrWhiteSpace(directories))
			{
				MessageBox.Show($"照片路径配置:{directories}无效或者不存在, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			_pictureFolders = directories.TrimEnd(';')
										.Split(';')
										?.ToDictionary(d =>
										{
											return d.Split('-')[0];
										}, v =>
										{
											var directory = v.Split('-')[1]?.ToString();
											if (!directory.IsOrExistDirectory())
											{
												return null;
											}
											return FileManager
														.GetFiles(directory)
														.Where(p => CustomFilters
																		.PictureExtensions
																		.Contains(p.GetExtension().TrimStart('.').ToLower())
																	&& !FileManager.HasAttribute(p, FileAttributes.Archive)
																)
														.ToArray();
										});
			if (_pictureFolders?.Count == 0)
			{
				var message = $"照片路径配置:{directories}无效或者不存在, 请检查!";
				throw new ArgumentException(message);
			}
		}


		private void SetComboBoxWithEnums(DataGridViewComboBoxColumn? comboBox)
		{
			comboBox!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			comboBox!.ReadOnly = false;
			comboBox!.Width = 70;
			comboBox!.Resizable = DataGridViewTriState.False;
			comboBox!.DropDownWidth = 200;
			comboBox!.MaxDropDownItems = 5;
		}

		private void SetButtonCellColumn(DataGridViewButtonColumn? buttonColumn)
		{
			buttonColumn!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			buttonColumn!.DefaultCellStyle.BackColor = Color.Gray;
			buttonColumn!.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			buttonColumn!.Width = 100;
			buttonColumn!.Resizable = DataGridViewTriState.False;
			buttonColumn!.UseColumnTextForButtonValue = true;
		}

		#endregion
	}
}
