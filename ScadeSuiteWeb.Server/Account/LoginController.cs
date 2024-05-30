using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScadeSuiteWeb.Server.Database;
using ScadeSuiteWeb.Server.Models.User;
using ScadeSuiteWeb.Server.Options;
using ScadeSuiteWeb.Shared.ViewModels;

namespace ScadeSuiteWeb.Server.Account;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    /// <summary>
    ///     签名凭据
    /// </summary>
    private readonly SigningCredentials _credentials;

    /// <summary>
    ///     appSetting.json 中的 JWT 的配置
    /// </summary>
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    ///     Jwt 安全令牌处理程序
    /// </summary>
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    /// <summary>
    ///     登录管理服务(由 Microsoft.AspNetCore.Identity 提供)
    /// </summary>
    private readonly SignInManager<XUser> _signInManager;

    private readonly AppDbContext _dbContext;

    public LoginController(
        SignInManager<XUser> signInManager,
        IOptions<JwtOptions> options, 
        AppDbContext dbContext)
    {
        _signInManager = signInManager;
        _dbContext = dbContext;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _jwtOptions = options.Value;
        // 根据配置的Key 分发凭证
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key)
        );
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    ///     登陆控制器
    ///     从前端传进来 [用户名] 和 [密码]
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<LoginResult> Login([FromBody] LoginViewModel login)
    {
        // 根据用户名获得登陆的用户
        
        var user = await _signInManager.UserManager.FindByNameAsync(login.UserName);

        // 如果 找不到 用户
        if (user == null) return new LoginResult { Successful = false, Error = "无效的用户名或密码！" };

        // 使用 signInManager 检查密码是否正确
        var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

        // 如果登陆不成功
        if (!result.Succeeded) return new LoginResult { Successful = false, Error = "无效的用户名或密码！" };

        // 获取这个用户 拥有的 所有 角色
        var roles = await _signInManager.UserManager.GetRolesAsync(user);


        // 将 [用户名] [用户昵称]  [用户邮箱] 放到 [声明] 中 
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Surname, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Anonymous, user.Id.ToString()),
        };

        //创建 JWT Token， 将 之前放到  claims 的 信息加密到 token 中
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            // 过期时间
            expires: DateTime.Now.AddHours(_jwtOptions.ExpiryInHours),
            signingCredentials: _credentials
        );

        return new LoginResult
        {
            Successful = true,
            Token = _jwtSecurityTokenHandler.WriteToken(token)
        };
    }

    [HttpGet]
    [Route("Validate")]
    [Authorize]
    public bool Validate()
    {
        return true;
    }
}

