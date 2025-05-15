using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using CommonTools;
using System.ComponentModel;

namespace CatFoodManager
{
	public partial class BrandManager : Form
	{
		#region services
		private readonly IService<Brand> _brandService;
		#endregion

		#region view
		private BindingSource _bindingSource = [];
		#endregion

		public BrandManager(IService<Brand> brandService)
		{
			InitializeComponent();
			_brandService = brandService;
			InitializeContext();
		}

		#region events

		private void BrandManager_Load(object sender, EventArgs e)
		{
			InitComponents();
			InitColumns();
			LoadData();
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
			var queryString = $"SELECT * FROM Brand WHERE Id LIKE '%{(Int64.TryParse(searchKey, out long id) ? id : 0)}%' or Name like '%{searchKey}%'";
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

		private void dataView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			var dataGridView = sender as DataGridView;
			if (dataGridView != null && dataGridView.EditingControl != null)
			{
				var currentCell = dataGridView.CurrentCell;
				var brandName = currentCell.EditedFormattedValue.ToString();
				var id = (long)currentCell.OwningRow.Cells["Id"].Value;
				if (id.Equals(0))//to insert a new brand
				{
					if (string.IsNullOrWhiteSpace(brandName))
					{
						MessageBox.Show("请输入brandName", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}
					var newBrand = new Brand() { Name = brandName };
					_brandService.Save(newBrand);
				}
				else //to update a existing brand
				{
					var brand = _brandService.Query((long)id);
					if (brand != null)
					{
						if (string.IsNullOrWhiteSpace(brandName))
						{
							MessageBox.Show("请输入brandName", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						brand.Name = brandName;
						_brandService.Update(brand);
					}
				}

				MessageBox.Show("已更新!", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
				LoadData();
			}
		}
		
		#endregion

		#region private methods

		private void InitializeContext()
		{
			Context.FillConnectionString(ConfigManager.GetConnectionString("SQLite"));
		}

		private void InitComponents()
		{
			//dataView.EditMode = DataGridViewEditMode.EditOnEnter;
			dataView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataView.AutoGenerateColumns = false;
			//dataView.AllowUserToAddRows = true;
			//dataView.ReadOnly = false;
		}

		private void InitColumns()
		{
			foreach (DataGridViewColumn column in ColumnHeaders.BrandHeaders.Values)
			{
				if (column == null)
				{
					continue;
				}
				column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
				dataView.Columns.Add(column);
			}

		}

		private void LoadData(string? filter = null)
		{
			(var brandResults, var totalCount) = string.IsNullOrWhiteSpace(filter) ? _brandService.GetAllWithCount() : _brandService.FuzzyQueryWithCount(filter);
			_bindingSource!.DataSource = new BindingList<Brand>(brandResults.ToList());
			dataView.DataSource = _bindingSource;
			Refresh();
		}

		//public override void Refresh()
		//{
		//	LoadData();
		//	base.Refresh();
		//}
		#endregion
	}
}
