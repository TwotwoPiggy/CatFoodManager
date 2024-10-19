using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CatFoodManager.Core.Utils;
using CommonTools;
using OpenCvSharp;
using System.ComponentModel;
using System.Data;
using System.Net.WebSockets;
using System.Windows.Forms;
using static ReaLTaiizor.Manager.MaterialSkinManager;
using static SQLite.TableMapping;

namespace CatFoodManager
{
	public partial class Main : Form
	{
		#region services

		private readonly IService<Brand> _brandService;
		private readonly IService<CatFood> _catFoodSerivce;
		private readonly IService<Factory> _factoryService;
		private readonly IPlatformRegExpService _regExpService;
		private readonly PictureContentService _pictureContentService;

		#endregion

		#region Page

		/// <summary>
		/// �ܼ�¼��
		/// </summary>
		private int _totalCount = 0;

		/// <summary>
		/// ��ҳ��
		/// </summary>
		private int _pageCount = 0;

		/// <summary>
		/// ��ǰҳ��
		/// </summary>
		private int _currentPage = 0;

		/// <summary>
		/// ҳ������¼
		/// </summary>
		private int _pageSize => Int32.TryParse(pageSizeComboBox.SelectedItem?.ToString(), out int pageSize) ? pageSize : 10;

		#endregion

		#region view
		private BindingSource _bindingSource = [];
		private PictureView _pictureView;
		#endregion

		#region fields
		private Dictionary<string, string[]?>? _pictureFolders;
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
			LoadData();
			pageSizeComboBox.SelectedIndexChanged += pageSizeComboBox_SelectedIndexChanged;
		}

		//todo: progress bar
		private void syncBtn_Click(object sender, EventArgs e)
		{
			try
			{
				var regExps = _regExpService.GetAll();
				GetPicturesPath();
				var content = string.Empty;
				PlatformRegExp? regPattern;
				var catFoods = new List<CatFood>();
				CatFood catFood;
				Brand brand;
				string shopName;
				if (_pictureFolders == null)
				{
					MessageBox.Show("ͼƬ·������Ϊ��, ����", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
						(catFood, shopName) = _pictureContentService.GenerateCatFood(content, regPattern.RegularExpression, regPattern.FieldInfoList, path);
						brand = _brandService.Query(shopName);
						catFood.Brand = brand;
						catFoods.Add(catFood);
					}
				}
				if (catFoods.Count != 0)
				{
					_catFoodSerivce.BatchSave(catFoods);
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

		#region Main View
		private void dataView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			var dataGridView = sender as DataGridView;
			if (dataGridView != null && dataGridView.EditingControl != null)
			{
				var currentCell = dataGridView.CurrentCell;
				var id = currentCell.OwningRow.Cells["Id"].Value;
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
				MessageBox.Show("�Ѹ���!", "�����ɹ�", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
				dataView.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
				MessageBox.Show("����ת��ҳ�����Ϲ�, ����!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (gotoPage <= 0)
			{
				MessageBox.Show("����ת��ҳ��������ǰ֧�ֵ���Сҳ��, ����!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (gotoPage > _pageCount)
			{
				MessageBox.Show("����ת��ҳ��������ǰ֧�ֵ����ҳ��, ����!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			pageSizeComboBox.SelectedIndex = 0;
			dataView.EditMode = DataGridViewEditMode.EditOnEnter;
			dataView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
			_currentPage = 1;
		}

		private void SetLabels()
		{
			totalLabel.Text = $"�� {_totalCount} ����¼";
			pageInfoLabel.Text = $"��ǰҳ {_currentPage}/{_pageCount}";
		}

		private void LoadData()
		{
			(var catFoodResults, _totalCount) = _catFoodSerivce.GetAllWithCount();
			_bindingSource.DataSource = catFoodResults.Skip((_currentPage - 1) * _pageSize).Take(_pageSize);
			dataView.DataSource = _bindingSource;
			_pageCount = Convert.ToInt32(Math.Ceiling((double)_totalCount / _pageSize));
			SetColumns();
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

		private void SetColumns()
		{
			var comboBoxColumn = CreateComboBoxWithEnums();
			var buttonColumn = CreateButtonCellColumn();
			foreach (DataGridViewColumn column in dataView.Columns)
			{
				column.Visible = !CustomFilters.ColumnsDisableToShow.Contains(column.HeaderText);
				column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				column.HeaderText = ColumnHeaders.CatFoodHeaders.TryGetValue(column.HeaderText, out string? header) ? header : column.HeaderText;
				column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				column.ReadOnly = false;
				if (column.Name == "FoodType")
				{
					comboBoxColumn.DisplayIndex = column.DisplayIndex;
				}
				if (column.Name == "PicturePath")
				{
					buttonColumn.DisplayIndex = column.DisplayIndex;
				}
			}
			dataView.Columns.Add(comboBoxColumn);
			dataView.Columns.Add(buttonColumn);
			dataView.CellClick += new DataGridViewCellEventHandler(DataView_CellClick);

		}

		private void GetPicturesPath()
		{
			var directories = ConfigManager.GetAppConfig(ConfigNames.PictureFolders);
			if (string.IsNullOrWhiteSpace(directories))
			{
				MessageBox.Show($"��Ƭ·������:{directories}��Ч���߲�����, ����!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
																		.Contains(p.GetExtension().TrimStart('.').ToLower()))
														.ToArray();
										});
			if (_pictureFolders?.Count == 0)
			{
				var message = $"��Ƭ·������:{directories}��Ч���߲�����, ����!";
				throw new ArgumentException(message);
			}
		}


		private DataGridViewComboBoxColumn CreateComboBoxWithEnums()
		{
			var comboBox = new DataGridViewComboBoxColumn();
			comboBox.DataSource = Enum.GetValues(typeof(CatFoodType));
			comboBox.DataPropertyName = "FoodType";

			comboBox.Name = "FoodTypeToShow";
			comboBox.Visible = !CustomFilters.ColumnsDisableToShow.Contains("FoodTypeToShow");
			comboBox.HeaderText = ColumnHeaders.CatFoodHeaders.TryGetValue("FoodTypeName", out string? header) ? header : comboBox.HeaderText;
			comboBox.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			comboBox.ReadOnly = false;
			comboBox.Width = 70;
			comboBox.Resizable = DataGridViewTriState.False;
			comboBox.DropDownWidth = 200;
			comboBox.MaxDropDownItems = 5;
			return comboBox;
		}

		private DataGridViewButtonColumn CreateButtonCellColumn()
		{
			var buttonColumn = new DataGridViewButtonColumn();
			buttonColumn.Visible = !CustomFilters.ColumnsDisableToShow.Contains("PictureButton");
			buttonColumn.HeaderText = ColumnHeaders.CatFoodHeaders.TryGetValue("PicturePath", out string? header) ? header : buttonColumn.HeaderText;
			buttonColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			buttonColumn.DefaultCellStyle.BackColor = Color.Gray;
			buttonColumn.DefaultCellStyle.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			buttonColumn.Name = "PictureButton";
			buttonColumn.Text = "�鿴��Ƭ";
			buttonColumn.Width = 100;
			buttonColumn.Resizable = DataGridViewTriState.False;
			buttonColumn.UseColumnTextForButtonValue = true;
			return buttonColumn;
		}
		#endregion


	}
}
