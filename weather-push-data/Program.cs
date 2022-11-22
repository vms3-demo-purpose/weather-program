using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;

// 14 Nov: Fix program not reading appsettings.json's connection string

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("appsettings.Development.json");
    
builder.Services.AddControllers();
builder.Services.AddDbContext<WeatherContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherDb"))
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var weatherContext = scope.ServiceProvider.GetRequiredService<WeatherContext>();
    weatherContext.Database.EnsureCreated();
    weatherContext.Seed();
}

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.MapControllers();
app.Run();

Console.Read();
