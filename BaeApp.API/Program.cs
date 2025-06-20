using BaeApp.API.BackgroundServices;
using BaeApp.Core.Interfaces;
using BaeApp.Infrastructure.Persistence;
using BaeApp.Infrastructure.Repositories;
using BaeApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// Lấy ConnectionString từ appsetting.json
// ---------------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ---------------------------------------------------------
// Đăng ký AppDbContext với SQL Server
// ---------------------------------------------------------

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("BaeApp.Infrastructure")
    )
);

// ---------------------------------------------------------
// Đọc Jwtsettings từ appsetting.json
// ---------------------------------------------------------

var jwtsections = builder.Configuration.GetSection("JwtSettings");
var keyBytes = Encoding.UTF8.GetBytes(jwtsections["Key"]);

// ---------------------------------------------------------
// Cấu hình Authentication và JWT Bearer
// ---------------------------------------------------------

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // True nếu đã chạy qua HTTPS
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtsections["Issuer"],
        ValidAudience = jwtsections["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ClockSkew = TimeSpan.Zero,
    };
});

// ---------------------------------------------------------
// Đăng kí Service Cho IuserService ( UserService đã implement ) 
// ---------------------------------------------------------

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddHostedService<ReminderBackgroundService>();


builder.Services.AddControllers();
// ---------------------------------------------------------
// Thêm Controller
// ---------------------------------------------------------

// ---------------------------------------------------------
// Đăng Kí SwaggerGen và cấu hình JWT trong Swagger
// ---------------------------------------------------------

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bae API",
        Version = "v1",
        Description = "API cho Bae App"
    });

    // Định nghĩa SecurityScheme cho JWT Bearer

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhập JWT Bearer dưới dạng Token:\" Bearer {Token} \" ",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme, // Bearer
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme,
        }
    };

    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

    //bắt buộc phải có token khi gọi API 
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme, new string[] {}
        }
    });
});

var app = builder.Build();

// ---------------------------------------------------------
// 7. Pipeline: Swagger + Authentication + Authorization
// ---------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaeApp API V1");
        c.RoutePrefix = "swagger";
        // truy cập swagger UI tại gốc (http://localhost:<port>/)

    });
}
app.UseRouting();

// Kích hoạt xác thực JWT
app.UseAuthentication();

// Kích hoạt phân quyền (Authorize)
app.UseAuthorization();

app.MapControllers();

app.Run();




