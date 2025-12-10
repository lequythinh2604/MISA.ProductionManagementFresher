using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using MISA.Core.Middlewares;
using MISA.Core.Services;
using MISA.Infrastructure.Repositories;
using MySqlConnector;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddScoped<IDbConnection>(sp => new MySqlConnection(connectionString));
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Đăng ký service
builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();

// Đăng ký repo
builder.Services.AddScoped<IWorkShiftRepository>(sp => new WorkShiftRepository(connectionString));

// config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allowed", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vue chạy cổng này
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

app.UseMiddleware<ErrorExceptionMiddleware>();
app.UseCors("Allowed");
app.UseAuthorization();
app.MapControllers();

app.Run();
