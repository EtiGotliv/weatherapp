using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WeatherService
{
    private readonly HttpClient client = new HttpClient();
    private readonly DatabaseService databaseService;
    private readonly string apiKey = "a222142f21dd45a7a62221409251702";
    //private readonly string city = "Tel Aviv";
    private static readonly string[] cities = { "New York", "London", "Tokyo", "Paris", "Berlin", "Sydney", "Dubai", "Toronto", "Moscow", "Rio de Janeiro" };
    private static readonly Random random = new Random();

    public WeatherService(DatabaseService dbService)
    {
        databaseService = dbService;
    }

    public async Task FetchAndSaveWeatherData()
    {
        for (int i = 0; i < 30; i++)
        {
            string city = cities[random.Next(cities.Length)];
            string historicalUrl = $"http://api.weatherapi.com/v1/history.json?key={apiKey}&q={city}&dt={DateTime.UtcNow.AddDays(-i):yyyy-MM-dd}";

            HttpResponseMessage historicalResponse = await client.GetAsync(historicalUrl);
            if (historicalResponse.IsSuccessStatusCode)
            {
                string historicalData = await historicalResponse.Content.ReadAsStringAsync();
                dynamic weatherData2 = JsonConvert.DeserializeObject(historicalData);

                float temperature = weatherData2?.forecast?.forecastday[0]?.day?.avgtemp_c;
                int humidity = weatherData2?.forecast?.forecastday[0]?.day?.avghumidity;
                string weatherDescription = weatherData2?.forecast?.forecastday[0]?.day?.condition?.text;
                string dateString = weatherData2?.forecast?.forecastday[0]?.date;

                if (DateTime.TryParse(dateString, out DateTime dateTime))
                {
                    await databaseService.SaveWeatherData(city, temperature, humidity, weatherDescription, dateTime);
                    Console.WriteLine($"Data for {city} on {dateTime:yyyy-MM-dd} saved successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to parse date: {dateString}");
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve data for {city} on {DateTime.UtcNow.AddDays(-i):yyyy-MM-dd}");
            }
        }
    }

    public async Task RunBackgroundTask()
    {
        while (true)
        {
            await FetchAndSaveWeatherData();
            await Task.Delay(TimeSpan.FromMinutes(1440));
        }
    }
}
