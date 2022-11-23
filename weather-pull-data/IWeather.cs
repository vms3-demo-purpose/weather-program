namespace WebApiClient
{
    interface IWeather
    {
        Task CallWeatherApi(String url);
        String api_url { get; set; }
        
    }
}
