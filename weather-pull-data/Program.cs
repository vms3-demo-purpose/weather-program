using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebApiClient
{
    class Program
    {
        const string api_url = "https://api.data.gov.sg/v1/environment/2-hour-weather-forecast?date=";

        static async Task Main(String[] args)
        {
            try
            {
                await CallWeatherAPI();  
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }

        static async Task CallWeatherAPI()
        {
            // API takes in a date in yyyy-MM-dd format as part of the query
            var singaporeTime = TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            string queryDate = singaporeTime.ToString("yyyy-MM-dd");

            // SQL's DATETIME data type uses a different format dd-MM-yyyy HH:mm:ss
            string sqlDate = singaporeTime.ToString("dd-MM-yyyy");
            // Pull data from API, extract relevant bits and write to new JSON file
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(api_url + queryDate);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(api_url + queryDate);
                if (response.IsSuccessStatusCode)
                {
                    // If API is down, the response is still success but body basically says "Internal Server Error".
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    bool internalServerError = jsonResponse.Contains("Internal Server Error");
                    // If API is down, read from offline backup, else read from response
                    APIData apiData = JsonConvert.DeserializeObject<APIData>(internalServerError ? 
                        File.ReadAllText("offline_response.json") : 
                        await response.Content.ReadAsStringAsync()
                    );

                    // APIData contains EVERYTHING. WeatherRecord contains only the bits that we want
                    List<WeatherRecord> weatherRecord = new List<WeatherRecord>();
                    
                    foreach (Item i in apiData.items)
                    {
                        DateTime start = i.valid_period.start;
                        DateTime end = i.valid_period.end;
                        foreach (Forecast f in i.forecasts)
                        {
                            weatherRecord.Add(new WeatherRecord()
                            {
                                Area = f.area,
                                Forecast = f.forecast,
                                SqlStartTime = start.ToString("yyyy-MM-dd HH:mm:ss"),
                                SqlEndTime = end.ToString("yyyy-MM-dd HH:mm:ss")
                            });

                            string newJSON = JsonConvert.SerializeObject(weatherRecord.ToArray(), Formatting.Indented);
                            System.IO.File.WriteAllText("/data/pull/" + sqlDate + ".json", newJSON);
                        }
                    }
                    Console.Write("Pulled {0} weather records for date: {1} from ", weatherRecord.Count, sqlDate);
                    Console.WriteLine(internalServerError ? "offline backup." : "data.gov.sg.");
                    return;
                }
                Console.WriteLine("Failed response code: {0}", response.StatusCode);
            }
        }
    }
}