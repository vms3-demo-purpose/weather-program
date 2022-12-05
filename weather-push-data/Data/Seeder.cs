using Newtonsoft.Json;
namespace WeatherApi.Data
{
    public static class Seeder
    {
        public static void Seed(this WeatherContext weatherContext)
        {
            // Wipe previously existing data
            if (weatherContext.WeatherRecords.Any())
            {
                var rows =  from o in weatherContext.WeatherRecords
                            select o;
                
                foreach (var row in rows)
                {
                    weatherContext.WeatherRecords.Remove(row);
                }
                weatherContext.SaveChanges();
            }
            
            // Begin Seeding
            // Read from json
            var singaporeTime = TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            var queryDate = singaporeTime.ToString("dd-MM-yyyy");
            string pathToJson = "/data/push/" + queryDate + ".json";
            var json = File.ReadAllText(pathToJson);

            // Deserialize json for insertion
            List<WeatherRecord> weatherRecords = JsonConvert.DeserializeObject<List<WeatherRecord>>(json);

            // Insert WeatherRecord
            foreach (WeatherRecord wr in weatherRecords)
            {
                weatherContext.Add(wr);
            }
            weatherContext.SaveChanges();

            // Read
            Console.WriteLine("Querying for all Weather Record");
            var allRecords = weatherContext.WeatherRecords.ToList();

            foreach (WeatherRecord wr in allRecords)
            {
                Console.WriteLine(wr);
            }

            // Log
            Console.WriteLine("There are {0} records in weatherRecords.", weatherRecords.Count);
            Console.WriteLine("There are {0} records in weatherContext.WeatherRecords.ToList().", allRecords.Count);
        }
    }
}