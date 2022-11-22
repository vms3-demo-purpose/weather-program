using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;

// TODO: Standardise frameworks to ef-core
// TODO: Remove out folder and delete any weatherdata.json files

// Create builder
var builder = WebApplication.CreateBuilder();

// Add configuration files to builder
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("appsettings.Development.json");
    
// Add services to builder    
builder.Services.AddControllers();
builder.Services.AddDbContext<WeatherContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherDb"))
);

// Build based on config files and services
var app = builder.Build();

// Populate database with Seed()
using (var scope = app.Services.CreateScope())
{
    var weatherContext = scope.ServiceProvider.GetRequiredService<WeatherContext>();
    weatherContext.Database.EnsureCreated();
    weatherContext.Seed();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

Console.Read();