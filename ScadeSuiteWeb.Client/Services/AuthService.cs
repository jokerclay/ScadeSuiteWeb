using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ScadeSuiteWeb.Client.Auth;
using ScadeSuiteWeb.Shared.Utils;
using ScadeSuiteWeb.Shared.ViewModels;
using ScadeSuiteWeb.Shared.ViewModels.User;

namespace ScadeSuiteWeb.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ApiAuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;
    private string _userName;
    private string _nickName;
    private string _email;
    private List<string> _roles;

    public AuthService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = (ApiAuthenticationStateProvider)authenticationStateProvider;
        _localStorage = localStorage;
        _userName = string.Empty;
        _nickName = string.Empty;
        _email = string.Empty;
        _roles = new List<string>();
    }

    public async Task<RegisterResult> Register(UserVM registerModel)
    {
        UserVM  m = new()
        {
            UserName = registerModel.UserName,
            Password = Encrypt.Sha256EncryptString(registerModel.Password),
        };

        var result = await _httpClient.PostAsJsonAsync("api/accounts", registerModel);
        try
        {
            var resultString = await result.Content.ReadAsStringAsync();
            var registerResult = JsonSerializer.Deserialize<RegisterResult>(
                resultString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            if (registerResult == null)
            {
                return new RegisterResult
                {
                    Successful = false,
                    Errors = new []{"JSON格式不正确"}
                };
            }
            
            return registerResult;

        }
        catch (Exception )
        {
            return new RegisterResult
            {
                Successful = false,
                Errors = new []{"结果字符串不是JSON格式"}
            };
        }
    }

    /// <summary>
    /// 向后端发送登陆请求
    /// </summary>
    /// <param name="loginModel"></param>
    /// <returns></returns>
    public async Task<LoginResult> Login(LoginViewModel loginModel)
    {
        // 将用户输入信息给到 登陆视图模型 LoginViewModel 
        LoginViewModel m = new()
        {
            UserName = loginModel.UserName,
            Password = Encrypt.Sha256EncryptString(loginModel.Password),
            RememberMe = loginModel.RememberMe,
        };

        // json 序列化 
        var loginAsJson = JsonSerializer.Serialize(m);

        // 发请求
        var response = await _httpClient.PostAsync("api/Login",
            new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

        try
        {
            // 得到返回结果
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (loginResult == null)
            {
                return new LoginResult
                {
                    Successful = false,
                    Error = "响应结果格式不正确"
                };
            }
            
            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }
            
            // 将 后端 返回的 token 存到本地

            await _localStorage.SetItemAsync("authToken", loginResult.Token);



            //  用户选择的角色 这个用户本身不包含  loginResult.Token 会为 null
            if (loginResult.Token != null)
            {
                IEnumerable<Claim> tokens =
                    _authenticationStateProvider.MarkUserAsAuthenticated(
                        loginResult.Token);

                LoadToken(tokens);

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", loginResult.Token);
            }

            return loginResult;
            
            
        }
        catch (Exception e)
        {
            return new LoginResult
            {
                Successful = false,
                Error = "响应结果不是有效的JSON格式"
            };
        }
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        ClearInfo();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public List<string> GetRoles()
    {
        return _roles;
    }
    public string GetUserName()
    {
        return _userName;
    }
    public string GetNickName()
    {
        return _nickName;
    }
    public bool HasRole(string role)
    {
        return _roles.Exists(x => x.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasRoles(IEnumerable<string> roles)
    {
        foreach (var item in roles)
        {
            if (_roles.Exists(x => x.Equals(item, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }
        return true;
    }

    public void LoadToken(IEnumerable<Claim> tokens)
    {
        ClearInfo();
        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case ClaimTypes.Role:
                    _roles.Add(token.Value);
                    break;
                case ClaimTypes.Name:
                    _userName = token.Value;
                    break;
                case ClaimTypes.Email:
                    _email = token.Value;
                    break;
                case ClaimTypes.Surname:
                    _nickName = token.Value;
                    break;
            }
        }
    }
    public void ClearInfo()
    {
        _userName = string.Empty;
        _nickName = string.Empty;
        _email = string.Empty;
        _roles = new List<string>();
    }

    public string GetEmial()
    {
        return _email;
    }
}

public interface IAuthService
{
    /// <summary>
    /// 提交 RegisterViewModel 给 accounts controller 并返回 RegisterResult 给调用者。
    /// </summary>
    /// <param name="registerModel"></param>
    /// <returns></returns>
    Task<RegisterResult> Register(UserVM registerModel);

    /// <summary>
    /// 类似于Register 方法，它将LoginModel 发送给login controller。
    /// 当返回一个成功的结果时，它将返回一个授权令牌并持久化到local storge。
    /// </summary>
    /// <param name="loginModel"></param>
    /// <returns></returns>
    Task<LoginResult> Login(LoginViewModel loginModel);

    /// <summary>
    /// 执行与Login 方法相反的操作。
    /// </summary>
    /// <returns></returns>
    Task Logout();
    /// <summary>
    /// 获得当前登录用户的所有权限角色字符串，当未登录时为空列表
    /// </summary>
    /// <returns></returns>
    public List<string> GetRoles();
    /// <summary>
    /// 判断登录的用户是否存在指定的角色
    /// </summary>
    /// <param name="role">角色的名称，不区分大小写</param>
    /// <returns></returns>
    public bool HasRole(string role);
    public bool HasRoles(IEnumerable<string> roles);

    /// <summary>
    /// 获得登录用户的账号名称，当未登录时为空字符串
    /// </summary>
    /// <returns></returns>
    public string GetUserName();
    /// <summary>
    /// 获得登录用户的昵称，当未登录时为空字符串
    /// </summary>
    /// <returns></returns>
    public string GetNickName();
    /// <summary>
    /// 获得登录用户的邮箱，当未登录时为空字符串
    /// </summary>
    /// <returns></returns>
    public string GetEmial();
    /// <summary>
    /// 加载登录返回的Token消息
    /// </summary>
    /// <param name="tokens"></param>
    public void LoadToken(IEnumerable<Claim> tokens);
    /// <summary>
    /// 清除登录的消息
    /// </summary>
    public void ClearInfo();
}

