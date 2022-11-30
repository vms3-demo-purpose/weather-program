using NUnit.Framework;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Api.UnitTests.Services
{
    [TestFixture]
    public class ApiService_IsOnline
    {
        string api_url; 
        DateTime singaporeTime;
        string queryDate;

        [SetUp]
        public void SetUp()
        {  
            api_url = "https://api.data.gov.sg/v1/environment/2-hour-weather-forecast?date=";
            singaporeTime = TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            queryDate = singaporeTime.ToString("yyyy-MM-dd");
        }

        /*
            Try getting a response from https://data.gov.sg/dataset/weather-forecast based on today's date. 
            If this fails, their API is probably down.
        */ 
        [Test]
        public async Task ApiIsOnline()
        {
            Console.WriteLine("\nTesting if API is online...");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(api_url + queryDate);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(api_url + queryDate);
                
                string jsonResponse = await response.Content.ReadAsStringAsync();
                System.IO.File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, queryDate + "_response.json"), jsonResponse);
                bool isOnline = !jsonResponse.Contains("Internal Server Error");  

                Console.WriteLine(isOnline ? "API is online." : "API is offline.");
                Assert.IsTrue(isOnline);
            }
        }

        /*  
            This tests if JSON retrieved today can be deserialized into WeatherRecords. 
            If this fails, the format of the JSON has changed and WeatherRecord needs an update:
            https://json2csharp.com/
        */ 
        [Test]
        public void JsonDeserializesToWeatherRecord()
        {
            Console.WriteLine("Testing if JSON retrieved can be deserialised...");
            bool success = false;
            try
            {
                var json = File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, queryDate + "_response.json"));
                APIData apiData = JsonConvert.DeserializeObject<APIData>(json);
                List<WeatherRecord> weatherRecords = new List<WeatherRecord>();
                
                foreach (Item i in apiData.items)
                {
                    DateTime start = i.valid_period.start;
                    DateTime end = i.valid_period.end;
                    foreach (Forecast f in i.forecasts)
                    {
                        weatherRecords.Add(new WeatherRecord()
                        {
                            Area = f.area,
                            Forecast = f.forecast,
                            SqlStartTime = start.ToString("yyyy-MM-dd HH:mm:ss"),
                            SqlEndTime = end.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                }
                var newJSON = JsonConvert.SerializeObject(weatherRecords.ToArray(), Formatting.Indented);
                File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, queryDate + "_output.json"), newJSON);
                success = true;
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Write(success ? "JSON can be deserialised to WeatherRecords." : "JSON cannot be deserialised to WeatherRecords.");
            Assert.IsTrue(success);
        }
    }
}