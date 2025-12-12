using MISA.Core.Interfaces.Repository;
using MISA.Core.Interfaces.Service;
using MISA.Core.Middlewares;
using MISA.Core.Services;
using MISA.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình database connection
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Đăng ký service, repo
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

builder.Services.AddScoped<IWorkShiftService, WorkShiftService>();
builder.Services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();

// config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
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
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
