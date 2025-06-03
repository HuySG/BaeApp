using BaeApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Lấy ConnectionString từ appsetting.json

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký AppDbContext với SQL Server

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("BaeApp.Infrastructure")
    )
);
// Thêm repository/service ở đây 

// các cấu hình khác ví dụ: 
// builder.service.AddScoped<IUserRepository, UserRepository>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
