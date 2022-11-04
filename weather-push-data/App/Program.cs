using Newtonsoft.Json;
using var db = new WeatherRecordContext();

// Start from clean slate
var rows =  from o in db.WeatherRecords
            select o;
foreach (var row in rows)
{
    db.WeatherRecords.Remove(row);
}
db.SaveChanges();

// Read from json
var singaporeTime = TimeZoneInfo.ConvertTime(DateTime.Today, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
var queryDate = singaporeTime.ToString("dd-MM-yyyy");
string pathToJson = "./data/" + queryDate + ".json";
String json = File.ReadAllText(pathToJson);

// Deserialize json for insertion
List<WeatherRecord> weatherRecords = JsonConvert.DeserializeObject<List<WeatherRecord>>(json);

// Insert WeatherRecord
foreach (WeatherRecord wr in weatherRecords)
{
    db.Add(wr);
}
db.SaveChanges();

// Read
Console.WriteLine("Querying for all Weather Record");
using (var context = db)
{
    var allRecords = context.WeatherRecords.ToList();
    foreach (WeatherRecord wr in allRecords)
    {
        Console.WriteLine(wr);
    }
}
