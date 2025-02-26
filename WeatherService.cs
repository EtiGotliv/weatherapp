using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WeatherService
{
    private readonly HttpClient client = new HttpClient();
    private readonly DatabaseService databaseService;
    private readonly CityService cityService;
    private readonly string apiKey = "a222142f21dd45a7a62221409251702";
    //private readonly string city = "Tel Aviv";
    private static readonly string[] cities = { "New York", "London", "Tokyo", "Paris", "Berlin", "Sydney", "Dubai", "Toronto", "Moscow", "Rio de Janeiro" };
    private static readonly Random random = new Random();
    // private readonly string apiKeyCity  = "ebc17d7ad4e1a6b7bf092dd8b1a8c683";

    public WeatherService(DatabaseService dbService/*, CityService cityService*/)
    {
        databaseService = dbService;
        // this.cityService = cityService;
    }

    public async Task FetchAndSaveWeatherData()
    {
        int successfulCityCount = 0;
        // bool isValid = true;
        while (successfulCityCount < 30)
        {
            // string city = await cityService.GetRandomCity();

            // if (city == "Unknown")
            // {
            // // Console.WriteLine($"Failed to get a valid city{city}.");
            // continue;
            // }
            string city = cities[random.Next(cities.Length)];
            
            string historicalUrl = $"http://api.weatherapi.com/v1/history.json?key={apiKey}&q={city}&dt={DateTime.UtcNow.AddDays(-successfulCityCount):yyyy-MM-dd}";

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
                    successfulCityCount++;
                }
                else
                {
                    Console.WriteLine($"Failed to parse date: {dateString}");
                }
            
            }
            else
            {
                Console.WriteLine($"Failed to retrieve data for {city} on {DateTime.UtcNow.AddDays(-successfulCityCount):yyyy-MM-dd}");
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
