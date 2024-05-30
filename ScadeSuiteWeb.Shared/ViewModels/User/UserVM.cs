using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadeSuiteWeb.Shared.ViewModels.User;

public class UserVM
{
    [Required]
    public string UserName { get; set; } = string.Empty;
    
    
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 注册结果
/// </summary>
public class RegisterResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Successful { get; set; }
    /// <summary>
    /// 错误信息
    /// </summary>
    public IEnumerable<string> Errors { get; set; }
}