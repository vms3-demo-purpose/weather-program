using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using Newtonsoft.Json.Linq;

// Create builder
var builder = WebApplication.CreateBuilder();
    
// Add services to builder    
builder.Services.AddControllers();
builder.Services.AddDbContext<WeatherContext>(options =>
    {
        string secretConnectionString = 
            "Server=weather-save-data;Database=Master;User Id=SA;Password=" +
            System.Environment.GetEnvironmentVariable("DB_PASSWORD") +
            ";MultipleActiveResultSets=true;";
        options.UseSqlServer(secretConnectionString);
    }
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

/*
app.UseAuthorization();
app.MapControllers();
app.Run();
*/