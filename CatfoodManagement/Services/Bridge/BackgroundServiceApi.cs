using CatFoodManager.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CatfoodManagement.Services.Bridge
{
    public class BackgroundServiceApi
    {
        private readonly IServiceProvider _serviceProvider;

        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"
        };

        public BackgroundServiceApi(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetStatusAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBackgroundServiceControl>();

            var status = await service.GetStatusAsync();

            return JsonConvert.SerializeObject(new { Success = true, Data = status }, _jsonSettings);
        }

        public async Task<string> StartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBackgroundServiceControl>();

            try
            {
                await service.StartServiceAsync();
                return JsonConvert.SerializeObject(new { Success = true, Message = "后台服务已启动" }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> PauseAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBackgroundServiceControl>();

            try
            {
                await service.PauseAsync();
                return JsonConvert.SerializeObject(new { Success = true, Message = "后台服务已暂停" }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> ResumeAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBackgroundServiceControl>();

            try
            {
                await service.ResumeAsync();
                return JsonConvert.SerializeObject(new { Success = true, Message = "后台服务已恢复" }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }

        public async Task<string> RestartAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBackgroundServiceControl>();

            try
            {
                await service.StopServiceAsync();
                await Task.Delay(500);
                await service.StartServiceAsync();
                return JsonConvert.SerializeObject(new { Success = true, Message = "后台服务已重启" }, _jsonSettings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message }, _jsonSettings);
            }
        }
    }
}
