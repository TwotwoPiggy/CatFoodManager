using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CommonTools;

namespace CatFoodManager
{
	public partial class Main : Form
	{
		#region fields

		private readonly IService<Brand> _brandService;
		private readonly IService<CatFood> _catFoodSerivce;
		private readonly IService<Factory> _factoryService;
		private readonly IPlatformRegExpService _regExpService;
		private readonly PictureContentService _pictureContentService;

		#endregion

		public Main(IService<CatFood> catFoodSerivce, IService<Brand> brandService,IService<Factory> factoryService, 
					IPlatformRegExpService regExpService, PictureContentService pictureContentService)
		{
			InitializeComponent();
			_catFoodSerivce = catFoodSerivce;
			_brandService = brandService;
			_factoryService = factoryService;
			_regExpService = regExpService;
			_pictureContentService = pictureContentService;
			InitializeContext();
		}
		#region private methods

		private void InitializeContext()
		{
			Context.FillConnectionString(ConfigManager.GetConnectionString("SQLite"));
			Context.FillPlatformRegExps(_regExpService.GetAll());
		}

		#endregion

		#region public methods

		#endregion

		#region events

		#endregion
	}
}
