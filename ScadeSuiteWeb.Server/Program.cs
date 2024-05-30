using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScadeSuiteWeb.Server.Database;
using ScadeSuiteWeb.Server.Database.Data;
using ScadeSuiteWeb.Server.Models.User;
using ScadeSuiteWeb.Server.Options;
using ScadeSuiteWeb.Server.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookRepository, BookRepository>();

// 还是要做角色，权限管理，功能维度，数据维度
// cookine
// localDB
// input browser property
// localstoge


// 打开文件
// 加载项目
// XML 自表达 --> json
// etp -> 
//      roots
//         folder
//         
// 项目的数据库表
// 文件目录，
// 打开文件

#region 添加数据库服务

// 将数据库的选项转到可配置的 appsettings.json 中
builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
{
    var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;
    optionsBuilder.UseSqlite(databaseOptions.ConnectionString, sqliteOptionsAction =>
    {
        sqliteOptionsAction.CommandTimeout(databaseOptions.CommandTimeout);
    });
    
#if DEBUG
    // Note: This should be only  used in Debug mode, In production, it may leak sensitive information
    optionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
    optionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
#endif

});
#endregion


#region 添加 Identity 服务
// DI ASP.NET Core Identity
builder.Services.AddIdentity<XUser, XRole>(opt =>
{
    // 密码的参数 
    //      RequiredLength              -   需要的长度        -  int
    //      RequiredUniqueChars         -   所需的唯一字符    -  int
    //      RequireNonAlphanumeric      -   要求非字母数字    -  bool
    //      RequireLowercase            -   需要小写          -  bool
    //      RequireUppercase            -   需要大写          -  bool
    //      RequireDigit                -   需要数字          -  bool
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();
#endregion



#region Jwt 服务
var configuration = builder.Configuration;
var jwtSection = configuration.GetSection("JWT");
var jwtOptions = jwtSection.Get<JwtOptions>();
if (jwtOptions == null)
{
    throw new Exception("'appSetting.json' 中无效的 JWT配置");
}

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // 保证JwtSecurityKey 的安全是非常重要的，因为这是用来对API产生的令牌签名的，如果泄露那么你的应用程序将不在安全。
        // 由于我们在本地运行所有内容，所以我将Issuer和Audience设置为localhost。
        // 如果在生产环境使用它，我们需要将Issuer 设置为API运行的域名，将Audience设置为客户端应用程序运行的域名。
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true, // 是否必须存在失效时间
            ValidateIssuer = true, // 是否验证 Issuer
            ValidateAudience = true, // 是否验证 Audience
            ValidateIssuerSigningKey = true, // 是否验证 签发者签名密钥
            ValidateLifetime = true, // 是否验证失效时间
            ValidIssuer = jwtOptions.Issuer, //  有效的 Issuer
            ValidAudience = jwtOptions.Audience, //  有效的 Audience
            IssuerSigningKey =  new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)), // 签发者签名密钥
        };
    });

// 将配置添加的DI中方便依赖注入
builder.Services.Configure<JwtOptions>(jwtSection);


#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

#region 创建数据库并初始化
app.CreateDbIfNotExists();
#endregion

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();