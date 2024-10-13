using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CommonTools;
using System.Data;
using System.Drawing.Printing;
using System.IO;

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
			InitComponents();
			SetLabels();
		}

		#region public methods

		#endregion

		#region events
		private void Main_Load(object sender, EventArgs e)
		{
			InitComponents();
			LoadData();
			
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
					foreach (var path in paths)
					{
						content = _pictureContentService.GetContentFromPicture(path, needReduceNoise: true);
						(catFood, shopName) = _pictureContentService.GenerateCatFood(content, regPattern.RegularExpression, regPattern.FieldInfos, path);
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

		private void pageSizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadData();
		}

		private void homeBtn_Click(object sender, EventArgs e)
		{
			_currentPage = 1;
			LoadData();
		}
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
			_currentPage = 1;
		}

		private void SetLabels()
		{
			totalLabel.Text = $"共 {_totalCount} 条记录";
			pageInfoLabel.Text = $"当前页 {_currentPage}/{_pageCount}";
		}

		private void LoadData()
		{
			(var catFoodResults, _totalCount) = _catFoodSerivce.GetAllWithCount();
			_bindingSource.DataSource = catFoodResults.Skip((_currentPage - 1) * _pageSize).Take(_pageSize); ;
			dataView.DataSource = _bindingSource;
			_pageCount = Convert.ToInt32(Math.Ceiling((double)_totalCount / _pageSize));
			SetLabels();
			ControlButtons();
			Refresh();
		}

		private void ControlButtons()
		{
			homeBtn.Enabled = prePageBtn.Enabled = _currentPage != 1;
			lastPageBtn.Enabled = nextPageBtn.Enabled = _currentPage != _pageCount;
		}


		private void GetPicturesPath()
		{
			var directories = ConfigManager.GetAppConfig("PictureFolders");
			if (string.IsNullOrWhiteSpace(directories))
			{
				MessageBox.Show($"照片路径配置:{directories}无效或者不存在, 请检查!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			_pictureFolders = directories.Split(';')
											?.ToDictionary(d =>
											{
												return d.Split(':')[0];
											}, v =>
											{
												var directory = v.Split(':')[1]?.ToString();
												if (!directory.IsOrExistDirectory())
												{
													return null;
												}
												return FileManager.GetFiles(directory).ToArray();
											});
			if (_pictureFolders?.Count == 0)
			{
				var message = $"照片路径配置:{directories}无效或者不存在, 请检查!";
				throw new ArgumentException(message);
			}
		}
		#endregion



		private void prePageBtn_Click(object sender, EventArgs e)
		{

		}
	}
}
