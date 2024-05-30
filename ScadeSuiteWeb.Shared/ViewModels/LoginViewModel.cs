using System.ComponentModel.DataAnnotations;

namespace ScadeSuiteWeb.Shared.ViewModels;

public class LoginViewModel
{
    /// <summary>
    /// 用户账号
    /// ie. lh -> 李华
    /// </summary>
    [Required]
    public string UserName { get; set; } = string.Empty;


    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;


    public bool RememberMe { get; set; }
    
}
public class LoginResult
{
    public bool Successful { get; set; }
    public string Error { get; set; } = string.Empty;
    public string? Token { get; set; }
}

