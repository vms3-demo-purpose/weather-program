using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class WeatherRecordContext : DbContext
{
    public DbSet<WeatherRecord> WeatherRecords { get; set; }

    public string DbPath { get; }

    public WeatherRecordContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "weatherrecord.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class WeatherRecord
{
    public int Id { get; set; }
    public string Area { get; set; }
    public string Forecast { get; set; }
    public string SqlStartTime { get; set; }
    public string SqlEndTime { get; set; }

    public override string ToString()
    {
        return 
            //"ID: " + Id + 
            "\nArea: " + Area +
            "\nForecast: " + Forecast + 
            "\nSqlStartTime: " + SqlStartTime + 
            "\nSqlEndTime: " + SqlEndTime;
    }
}