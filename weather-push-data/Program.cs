using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;

// 14 Nov: Fix program not reading appsettings.json's connection string

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Add services to the container.

    builder.Services.AddControllers();

    // Try connecting
    const int retryIntervalSeconds = 6;
    const int retryAttempts = 10;
    bool connected = false;
    int retriesAttempted = 0;
    while (++retriesAttempted < retryAttempts && !connected)
    {
        Console.WriteLine("Attempting connection {0}/{1} in {2} seconds...", retriesAttempted, retryAttempts, retryIntervalSeconds);
        Thread.Sleep(retriesAttempted > 1 ? retryIntervalSeconds * 1000 : 0);
        try
        {
            builder.Services.AddDbContext<WeatherContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherDb"))
            );
            connected = true;
        } 
        catch (Exception e)
        {
            Console.WriteLine("Failed to connect.\n" + e);
        }
    }
    if (!connected)
    {
        Console.WriteLine("Failed to connect to DB. Aborting.");
        return;
    }
        
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
} catch (Exception e)
{
    Console.WriteLine(e);
}
Console.Read();
