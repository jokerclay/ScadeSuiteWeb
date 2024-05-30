namespace ScadeSuiteWeb.Server.Options;


public class JwtOptions
{
    /// <summary>
    ///     在服务器上配置的加密的 Key
    /// </summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>
    ///     发布者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    /// <summary>
    ///     观众
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    /// <summary>
    ///     过期时间，单位 小时
    /// </summary>
    public int ExpiryInHours { get; set; }
}
