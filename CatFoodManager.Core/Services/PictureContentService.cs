using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Statics;
using CommonTools;
using OcrApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SQLite.SQLite3;

namespace CatFoodManager.Core.Services
{
	public class PictureContentService : ServiceBase
	{
		private readonly OCRHelper _ocrHelper;
		private readonly IPlatformRegExpService _regExpService;

        public PictureContentService(IRepository repo, OCRHelper ocrHelper, IPlatformRegExpService regExpService):base(repo)
        {
			_ocrHelper = ocrHelper;
			_regExpService = regExpService;
		}

        private string _originPicturePath = string.Empty;
		private string? _newPicturePath = string.Empty;


		public string? NewPicturePath => _newPicturePath;
		public string OriginPicturePath => _originPicturePath;


		public void SetRegConfig(string regConfig)
		{
			ConfigManager.SetAppConfig("regPattern", regConfig);
		}

		#region RegConfigs
		public string? GetRegConfig()
		{
			return ConfigManager.GetAppConfig("regPattern");
		}

		public string[]? GetRegConfigs()
		{
			return GetRegConfig()?.Split(";");
		}

		public void SetTessdataPath(string path)
		{
			try
			{
				_ocrHelper.SetTessdataPath(path);
			}
			catch (Exception)
			{
				throw;
			}
		}
		#endregion

		public string GetContentFromPicture(string picturePath, string? newPicturePath = null, bool needReduceNoise = false)
		{
			try
			{
				_originPicturePath = picturePath;
				_newPicturePath = newPicturePath;

				picturePath = needReduceNoise ? _newPicturePath = _ocrHelper.ReduceImageNoise(picturePath, newPicturePath) : _originPicturePath;

				return _ocrHelper.GetTextFromPicture(picturePath).Replace(" ", string.Empty);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public (CatFood, string) GenerateCatFood(string content, string regPattern, Dictionary<string, int> fieldInfos, string picturePath)
		{
			var catFood = new CatFood() { PicturePath = picturePath, UpdatedAt = DateTime.Now };
			var regex = new Regex(regPattern, RegexOptions.IgnoreCase);
			var groups = regex.Match(content).Groups;
			catFood.OrderId = groups[fieldInfos["orderId"]].Value;
			catFood.Name = groups[fieldInfos["name"]].Value.Replace("猎粑", "猫粮").Replace("内", "肉");
			catFood.FoodType = catFood.Name.Contains("猫粮") || catFood.Name.Contains("主食") ? CatFoodType.CatFood : CatFoodType.CatSnack;
			catFood.Count = Int32.TryParse(groups[fieldInfos["count"]].Value, out int count) ? count : 1;
			catFood.Price = Double.TryParse(groups[fieldInfos["price"]].Value, out double price) ? price : 1D;
			catFood.PurchasedAt = DateTime.TryParse(groups[fieldInfos["purchasedAt"]].Value, out DateTime purchasedAt) ? purchasedAt : DateTime.Now;
			var shopName = groups[fieldInfos["shopName"]].Value;
			return (catFood, shopName);
		}

		#region private method

		#endregion
	}
}
