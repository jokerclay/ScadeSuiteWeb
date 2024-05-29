namespace ScadeSuiteWeb.Server.Options;

public class DatabaseOptions
{
    public string? ConnectionString { get; set; } = string.Empty;
    /// <summary>
    /// 超时重连
    /// </summary>
    public int CommandTimeout { get; set; }
    
    public bool EnableDetailedErrors { get; set; }
    
    public bool EnableSensitiveDataLogging { get; set; }
    
}