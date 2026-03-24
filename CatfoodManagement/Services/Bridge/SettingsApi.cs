using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CatfoodManagement.Services.Bridge
{
    /// <summary>
    /// 设置管理 API，提供应用程序配置的读取和保存功能
    /// 通过 CefSharp 暴露给前端 JavaScript 调用
    /// </summary>
    public class SettingsApi
    {
        /// <summary>
        /// 应用程序配置
        /// </summary>
        private readonly IConfiguration _configuration;
        
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private readonly string _settingsFilePath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">应用程序配置</param>
        public SettingsApi(IConfiguration configuration)
        {
            _configuration = configuration;
            _settingsFilePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        }

        /// <summary>
        /// 获取应用程序设置
        /// </summary>
        /// <returns>JSON 格式的设置数据</returns>
        public string GetSettings()
        {
            try
            {
                // 读取平台文件夹配置
                var platformFolders = new Dictionary<string, string>();
                var foldersSection = _configuration.GetSection("AppSettings:PlatformFolders");
                if (foldersSection.Exists())
                {
                    foreach (var child in foldersSection.GetChildren())
                    {
                        platformFolders[child.Key] = child.Value ?? "";
                    }
                }

                // 构建设置对象
                var settings = new
                {
                    AI = new
                    {
                        ApiKey = _configuration["AppSettings:AI:ApiKey"] ?? "",
                        ModelName = _configuration["AppSettings:AI:ModelName"] ?? "gemini-2.5-flash",
                        RPM = int.TryParse(_configuration["AppSettings:AI:RPM"], out var rpm) ? rpm : 5,
                        TPM = int.TryParse(_configuration["AppSettings:AI:TPM"], out var tpm) ? tpm : 250000,
                        RPD = int.TryParse(_configuration["AppSettings:AI:RPD"], out var rpd) ? rpd : 20,
                        Proxy = new
                        {
                            Enabled = bool.TryParse(_configuration["AppSettings:AI:Proxy:Enabled"], out var enabled) && enabled,
                            Address = _configuration["AppSettings:AI:Proxy:Address"] ?? "http://127.0.0.1:7890"
                        }
                    },
                    Database = new
                    {
                        ConnectionString = _configuration["DatabaseSettings:ConnectionString"] ?? "./data/catfood.db"
                    },
                    App = new
                    {
                        TessdataPath = _configuration["AppSettings:TessdataPath"] ?? "tessdata",
                        PlatformFolders = platformFolders
                    }
                };

                return JsonConvert.SerializeObject(new { Success = true, Data = settings });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 保存应用程序设置
        /// </summary>
        /// <param name="settingsJson">JSON 格式的设置数据</param>
        /// <returns>JSON 格式的操作结果</returns>
        public string SaveSettings(string settingsJson)
        {
            try
            {
                // 反序列化设置数据
                var settings = JsonConvert.DeserializeObject<dynamic>(settingsJson);
                if (settings == null)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "Invalid settings format" });
                }

                // 读取现有配置文件
                var jsonContent = File.ReadAllText(_settingsFilePath);
                dynamic? appSettings = JsonConvert.DeserializeObject(jsonContent);

                if (appSettings == null)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "Cannot read appsettings.json" });
                }

                // 更新 AI 设置
                if (settings.AI != null)
                {
                    if (appSettings.AppSettings == null)
                    {
                        appSettings.AppSettings = new JObject();
                    }
                    if (appSettings.AppSettings.AI == null)
                    {
                        appSettings.AppSettings.AI = new JObject();
                    }

                    appSettings.AppSettings.AI.ApiKey = settings.AI.ApiKey?.ToString() ?? "";
                    appSettings.AppSettings.AI.ModelName = settings.AI.ModelName?.ToString() ?? "gemini-2.5-flash";
                    appSettings.AppSettings.AI.RPM = settings.AI.RPM?.ToString() ?? "5";
                    appSettings.AppSettings.AI.TPM = settings.AI.TPM?.ToString() ?? "250000";
                    appSettings.AppSettings.AI.RPD = settings.AI.RPD?.ToString() ?? "20";

                    // 更新代理设置
                    if (settings.AI.Proxy != null)
                    {
                        if (appSettings.AppSettings.AI.Proxy == null)
                        {
                            appSettings.AppSettings.AI.Proxy = new JObject();
                        }
                        appSettings.AppSettings.AI.Proxy.Enabled = settings.AI.Proxy.Enabled ?? false;
                        appSettings.AppSettings.AI.Proxy.Address = settings.AI.Proxy.Address?.ToString() ?? "http://127.0.0.1:7890";
                    }
                }

                // 更新数据库设置
                if (settings.Database != null)
                {
                    if (appSettings.DatabaseSettings == null)
                    {
                        appSettings.DatabaseSettings = new JObject();
                    }
                    appSettings.DatabaseSettings.ConnectionString = settings.Database.ConnectionString?.ToString() ?? "./data/catfood.db";
                }

                // 更新应用程序设置
                if (settings.App != null)
                {
                    if (appSettings.AppSettings == null)
                    {
                        appSettings.AppSettings = new JObject();
                    }
                    appSettings.AppSettings.TessdataPath = settings.App.TessdataPath?.ToString() ?? "tessdata";

                    // 更新平台文件夹设置
                    var platformFolders = settings.App.PlatformFolders;
                    if (platformFolders != null)
                    {
                        var foldersObj = new JObject();
                        foreach (JProperty prop in platformFolders)
                        {
                            var key = prop.Name;
                            var value = prop.Value?.ToString();
                            if (!string.IsNullOrEmpty(key))
                            {
                                foldersObj[key] = value ?? "";
                            }
                        }
                        appSettings.AppSettings.PlatformFolders = foldersObj;
                    }
                }

                // 保存配置文件
                var newJson = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
                File.WriteAllText(_settingsFilePath, newJson);

                return JsonConvert.SerializeObject(new { Success = true, Message = "设置已保存，部分设置需要重启应用后生效" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
            }
        }
    }
}
