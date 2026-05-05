using CatFoodManager.Application.Interfaces;
using CatFoodManager.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    /// <summary>
    /// OCR 识别 API，提供基于 Gemini AI 的图片文字识别功能
    /// 通过 CefSharp 暴露给前端 JavaScript 调用
    /// </summary>
    public class OcrApi
    {
        /// <summary>
        /// 服务提供者，用于创建作用域并获取服务
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public OcrApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 验证 AI 模型是否可用
        /// </summary>
        /// <returns>JSON 格式的验证结果</returns>
        public async Task<string> ValidateModelAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var ocrService = scope.ServiceProvider.GetRequiredService<IGeminiOcrService>();

            try
            {
                var isValid = await ocrService.ValidateModelAsync();
                return JsonConvert.SerializeObject(new { Success = true, IsValid = isValid });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取可用的 AI 模型列表
        /// </summary>
        /// <param name="apiKey">API Key，用于生成缓存键</param>
        /// <returns>JSON 格式的模型列表</returns>
        public async Task<string> GetModelsAsync(string? apiKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var ocrService = scope.ServiceProvider.GetRequiredService<IGeminiOcrService>();

            try
            {
                var models = await ocrService.GetModelsAsync(apiKey);
                return JsonConvert.SerializeObject(new { Success = true, Data = models });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 清除模型列表缓存
        /// </summary>
        /// <param name="apiKey">API Key，用于生成缓存键</param>
        /// <returns>JSON 格式的操作结果</returns>
        public string ClearModelsCache(string? apiKey = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var ocrService = scope.ServiceProvider.GetRequiredService<IGeminiOcrService>();

            try
            {
                ocrService.ClearModelsCache(apiKey);
                return JsonConvert.SerializeObject(new { Success = true, Message = "Cache cleared" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 从图片文件夹同步识别猫粮信息
        /// </summary>
        /// <param name="folderPath">图片文件夹路径</param>
        /// <param name="promptText">AI 提示文本</param>
        /// <returns>JSON 格式的识别结果</returns>
        public async Task<string> SyncFromPicturesAsync(string folderPath, string promptText, int platform)
        {
            using var scope = _serviceProvider.CreateScope();
            var ocrService = scope.ServiceProvider.GetRequiredService<IGeminiOcrService>();

            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "Folder path is required" });
                }

                if (!Directory.Exists(folderPath))
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = "Folder not found" });
                }

                var results = await ocrService.ProcessPicturesAsync<CatFoodManager.Core.Models.Dtos.CatFoodDto>(folderPath, promptText);
                foreach (var item in results.Items)
                {
                    item.Platform = (PlatformType)platform;
                }
                return JsonConvert.SerializeObject(new { Success = true, Count = results.Items.Count, Data = results.Items, ResponseId = results.ResponseId });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 打开文件夹选择对话框
        /// </summary>
        /// <returns>选中的文件夹路径，如果取消则返回空字符串</returns>
        public async Task<string> SelectFolderAsync()
        {
            return await Task.Run(() =>
            {
                string? result = null;
                var thread = new Thread(() =>
                {
                    using var dialog = new FolderBrowserDialog
                    {
                        Description = "选择文件夹",
                        ShowNewFolderButton = true
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        result = dialog.SelectedPath;
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                return result ?? string.Empty;
            });
        }

        /// <summary>
        /// 打开文件选择对话框
        /// </summary>
        /// <returns>选中的文件路径，如果取消则返回空字符串</returns>
        public async Task<string> SelectFileAsync()
        {
            return await Task.Run(() =>
            {
                string? result = null;
                var thread = new Thread(() =>
                {
                    using var dialog = new OpenFileDialog
                    {
                        Title = "选择图片",
                        Filter = "图片文件|*.jpg;*.jpeg;*.png;*.gif;*.bmp|所有文件|*.*",
                        Multiselect = false
                    };

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        result = dialog.FileName;
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                return result ?? string.Empty;
            });
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="folderPath">要打开的文件夹路径</param>
        /// <returns>JSON 格式的操作结果</returns>
        public async Task<string> OpenFolderAsync(string folderPath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(folderPath))
                    {
                        return JsonConvert.SerializeObject(new { Success = false, Message = "文件夹路径不能为空" });
                    }

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = folderPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });

                    return JsonConvert.SerializeObject(new { Success = true });
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
                }
            });
        }
    }
}
