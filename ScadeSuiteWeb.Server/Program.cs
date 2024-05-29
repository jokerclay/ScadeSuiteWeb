using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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