namespace EventManagementUI.Services;

public class GlobalConfig
{
    private readonly ILogger<GlobalConfig> _logger;

    public GlobalConfig(ILogger<GlobalConfig> logger)
    {
        _logger = logger;
    }

    public string LogsPath()
    {
        return Path.Combine(Directory.GetParent(Environment.CurrentDirectory).ToString(), @"logs/");
    }

    public string[] ImagesFilter()
    {
        return new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
    }

    public string[] LogsFilter()
    {
        return new String[] { "log" };
    }

}
