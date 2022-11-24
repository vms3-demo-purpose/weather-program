using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebApiClient
{
    class Program
    {
        const string api_url = "https://api.data.gov.sg/v1/environment/2-hour-weather-forecast?date=";

        static async Task Main(String[] args)
        {
            // API takes in a date in yyyy-MM-dd format as part of the query
            var singaporeTime = TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            string queryDate = singaporeTime.ToString("yyyy-MM-dd");

            // SQL's DATETIME data type uses a different format dd-MM-yyyy HH:mm:ss
            string sqlDate = singaporeTime.ToString("dd-MM-yyyy");

            try
            {
                // If API is up, use this
                //await CallWeatherAPI(queryDate, sqlDate);  

                // If API is down, use this
                CallWeatherAPIOffline("2022-11-24", "24-11-2022"); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }

        static async Task CallWeatherAPI(string queryDate, string sqlDate)
        {
            // Pull data from API, extract relevant bits and write to new JSON file
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(api_url + queryDate);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync(api_url + queryDate);
                if (response.IsSuccessStatusCode)
                {
                    // This contains the entire JSON, of which we only want a subset of
                    APIData APIData = JsonConvert.DeserializeObject<APIData>(await response.Content.ReadAsStringAsync());
                    // APIData contains EVERYTHING. Data contains only the bits that we want
                    List<WeatherRecord> data = new List<WeatherRecord>();

                    if (APIData is not null)
                    {
                        foreach (Item i in APIData.items)
                        {
                            DateTime start = i.valid_period.start;
                            DateTime end = i.valid_period.end;
                            foreach (Forecast f in i.forecasts)
                            {
                                data.Add(new WeatherRecord()
                                {
                                    Area = f.area,
                                    Forecast = f.forecast,
                                    SqlStartTime = start.ToString("yyyy-MM-dd HH:mm:ss"),
                                    SqlEndTime = end.ToString("yyyy-MM-dd HH:mm:ss")
                                });

                                string newJSON = JsonConvert.SerializeObject(data.ToArray(), Formatting.Indented);
                                System.IO.File.WriteAllText("/data/pull/" + sqlDate + ".json", newJSON);
                            }
                        }
                        Console.WriteLine("Pulled {0} weather records for date: {1}.", data.Count, sqlDate);
                    } 
                    else 
                    {
                        Console.WriteLine("Failed response code: {0}", response.StatusCode);
                    }
                }
            }
        }

        static void CallWeatherAPIOffline(string queryDate, string sqlDate)
        {
            try
            {
                // This uses a local copy of 24th November 2022's full JSON
                var offlineJSON = File.ReadAllText("response_1669254678978.json");
                APIData APIData = JsonConvert.DeserializeObject<APIData>(offlineJSON);

                List<WeatherRecord> data = new List<WeatherRecord>();

                foreach (Item i in APIData.items)
                {
                    DateTime start = i.valid_period.start;
                    DateTime end = i.valid_period.end;
                    foreach (Forecast f in i.forecasts)
                    {
                        data.Add(new WeatherRecord()
                        {
                            Area = f.area,
                            Forecast = f.forecast,
                            SqlStartTime = start.ToString("yyyy-MM-dd HH:mm:ss"),
                            SqlEndTime = end.ToString("yyyy-MM-dd HH:mm:ss")
                        });

                        string newJSON = JsonConvert.SerializeObject(data.ToArray(), Formatting.Indented);
                        System.IO.File.WriteAllText("/data/pull/" + sqlDate + ".json", newJSON);
                    }
                }
                Console.WriteLine("Pulled {0} weather records for date: {1}.", data.Count, sqlDate);
            } 
            catch (Exception e )
            {
                Console.WriteLine(e);
            }
        }
    }
}