using Microsoft.AspNetCore.Mvc;
using WeatherApi.Data;

namespace WeatherApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherRecordsController : ControllerBase
{
    private readonly WeatherContext _weatherContext;

    public WeatherRecordsController (WeatherContext weatherContext)
    {
        _weatherContext = weatherContext;
    }

    [HttpGet]
    public ActionResult Get(int take = 10, int skip = 0)
    {
        return Ok(_weatherContext.WeatherRecords.OrderBy(wr => wr.Id).Skip(skip).Take(take));
    }
}