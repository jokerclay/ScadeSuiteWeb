using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ScadeSuiteWeb.Shared.ViewModels.User;

namespace ScadeSuiteWeb.Client.Auth;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly JwtSecurityTokenHandler _securityTokenHandler;
    private readonly NavigationManager _navigationManager;
    
    public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _navigationManager = navigationManager;
        _securityTokenHandler = new JwtSecurityTokenHandler();
    }
    
    private List<Claim> _claims = new();
    public UserVM  UserInfo { get; set; } = new();

    private static readonly ClaimsPrincipal EmptyClaimsPrincipal = new (new ClaimsIdentity());
    private static readonly AuthenticationState EmptyState = new (EmptyClaimsPrincipal);
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // 从 本地的 localStorage 中获取从后端获得的 token
        string? savedToken = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(savedToken))
        {
            NavToLogin();
            return EmptyState;
        }
        var claims = ParseClaimsFromJwt(savedToken);
        SetClaims(claims);
        var claim = claims.FirstOrDefault(v => v.Type == "exp");
        if (claim == null)
        {
            NavToLogin();
            return EmptyState;
        }
        if (long.TryParse(claim.Value, out long exp))
        {
            var expTime = DateTimeOffset.FromUnixTimeSeconds(exp).LocalDateTime;
            if (expTime < DateTime.Now)
            {
                NavToLogin();
                return EmptyState;
            }
        }
        else
        {
            NavToLogin();
            return EmptyState;
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    private List<Claim> ParseClaimsFromJwt(string jwt)
    {
        JwtSecurityToken jwt1 = _securityTokenHandler.ReadJwtToken(jwt);
        return jwt1.Payload.Claims.ToList();
    }

    private void NavToLogin()
    {
        _navigationManager.NavigateTo("./Login");
    }
    
    private void SetClaims(List<Claim> claims)
    {
        _claims = claims;
        UserVM info = UserInfo;
        foreach (var kv in claims)
        {
            switch (kv.Type)
            {
                case ClaimTypes.Name:
                    info.UserName = kv.Value;
                    break;
            }
        }
    }
}

