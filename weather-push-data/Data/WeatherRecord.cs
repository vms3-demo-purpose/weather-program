namespace WeatherApi.Data
{
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
                "\nID: " + Id + 
                "\nArea: " + Area +
                "\nForecast: " + Forecast + 
                "\nSqlStartTime: " + SqlStartTime + 
                "\nSqlEndTime: " + SqlEndTime;
        }
    }
}