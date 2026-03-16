namespace CatFoodManager.Infrastructure.Configuration;

public class AppSettings
{
    public const string SectionName = "AppSettings";
    public string PictureFolders { get; set; } = string.Empty;
    public string TessdataPath { get; set; } = string.Empty;
}
