namespace CatFoodManager.Infrastructure.Configuration;

public class AppSettings
{
    public const string SectionName = "AppSettings";
    public Dictionary<string, string> PlatformFolders { get; set; } = new();
    public string TessdataPath { get; set; } = string.Empty;
}
