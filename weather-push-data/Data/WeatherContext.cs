using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Data
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherRecord> WeatherRecords { get; set; }

        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
        }      
    }
}