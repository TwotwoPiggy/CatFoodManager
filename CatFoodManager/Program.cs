using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Repositories;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CommonTools;
using Microsoft.Extensions.DependencyInjection;
using OcrApi;
using System.Diagnostics.Eventing.Reader;

namespace CatFoodManager
{
	internal static class Program
	{
		public static IServiceProvider ServiceProvider { get; set; }

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			ConfigureServices(needMigrate: true);


			Application.Run(ServiceProvider.GetService<Main>());
		}

		private static void ConfigureServices(bool needMigrate)
		{
			var services = new ServiceCollection();
			var tessdataPath = @"D:\Computer\Projects\CatFoodManager\CatFoodManager\bin\Debug\net8.0-windows\tessdata";
			services.AddSingleton(typeof(Main))
					.AddTransient<SQLiteHelper>()
					.AddTransient<OCRHelper>(serviceProvider => new OCRHelper(tessdataPath))
					.AddTransient<IRepository, CommonRepository>()
					.AddTransient<PictureContentService>()
					.AddTransient<IService<Brand>, BrandService>(serviceProvider => new BrandService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
					.AddTransient<IService<CatFood>, CatFoodService>(serviceProvider => new CatFoodService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
					.AddTransient<IService<Factory>, FactoryService>(serviceProvider => new FactoryService(serviceProvider.GetRequiredService<IRepository>(), needMigrate))
					.AddTransient<IPlatformRegExpService, PlatformRegExpService>(serviceProvider =>
																					new PlatformRegExpService(serviceProvider.GetRequiredService<IRepository>(), needMigrate));
			ServiceProvider = services.BuildServiceProvider();
		}

		public static T? GetService<T>() where T : class
		{
			return (T?)ServiceProvider.GetService(typeof(T));
		}

	}
}