using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using Newtonsoft.Json.Linq;

// Create builder
var builder = WebApplication.CreateBuilder();
    
// Add services to builder    
builder.Services.AddControllers();
builder.Services.AddDbContext<WeatherContext>(options =>
    {
        string source = File.ReadAllText("/run/secrets/my-secret");
        dynamic data = JObject.Parse(source);
        string password = data.DB_PASSWORD;

        string connectionString = 
            "Server=weather-save-data;Database=Master;User Id=SA;Password=" +
            password +
            ";MultipleActiveResultSets=true;";
        
        options.UseSqlServer(connectionString);
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