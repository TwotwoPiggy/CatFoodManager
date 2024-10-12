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

		public CatFood GenerateCatFood(string content, string regPattern)
		{
			var catFood = new CatFood();
			return catFood;
		}

		#region private method

		private string Match(string content, string regPattern)
		{
			Regex reg = new Regex(regPattern, RegexOptions.IgnoreCase);
			var result = reg.Match(content).Groups.Values.LastOrDefault();
			if (result != null && !string.IsNullOrWhiteSpace(result.Value))
			{

			}
			return "";
		}

		#endregion
	}
}
