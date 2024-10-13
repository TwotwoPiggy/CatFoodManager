using CatFoodManager.Core.Statics;
using Newtonsoft.Json;
using SQLite;

namespace CatFoodManager.Core.Models
{
	public class PlatformRegExp
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Name { get; set; }

		public PlatformType Platform { get; set; }

        public string RegularExpression { get; set; }

		public string FieldInfos { get; set; }
		[Ignore]
		public Dictionary<string,int>? FieldInfoList { get; set; }



		public void AutoFillFields(bool isReadAction)
		{
			SetName();
			if (isReadAction)
			{
				SetFieldInfoList();

			}
			else
			{
				SetFieldInfos();
			}
		}

		private void SetName()
		{
			Name = Platform.ToString();
		}

		private void SetFieldInfos()
		{
			FieldInfos = FieldInfoList == null ? string.Empty : JsonConvert.SerializeObject(FieldInfoList);
		}

		private void SetFieldInfoList()
		{
			FieldInfoList = string.IsNullOrWhiteSpace(FieldInfos) 
							? default 
							: JsonConvert.DeserializeObject<Dictionary<string, int>?>(FieldInfos);
		}

	}
}
