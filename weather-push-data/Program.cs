using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using Newtonsoft.Json.Linq;

// Create builder
var builder = WebApplication.CreateBuilder();
    
// Add services to builder    
builder.Services.AddControllers();
builder.Services.AddDbContext<WeatherContext>(options =>
    {
        string source = System.IO.File.ReadAllText("/run/secrets/my-secret");
        dynamic data = JObject.Parse(source);
        string secretConnectionString = data.CONNECTION_STRING;
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