using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class CityService
{
    private readonly HttpClient client = new HttpClient();
    private readonly string username = "etigotliv2134";

    public async Task<string> GetRandomCity()
    {
        Random random = new Random();
        string cityName = "Unknown";

        for (int i = 0; i < 3; i++)
        {
            double latitude = random.NextDouble() * 90 - 45;
            double longitude = random.NextDouble() * 180 - 90;

            string geoUrl = $"http://api.geonames.org/findNearbyPlaceNameJSON?lat={latitude}&lng={longitude}&username={username}&lang=en";

            HttpResponseMessage response = await client.GetAsync(geoUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                JObject locationData = JObject.Parse(responseData);

                if (locationData["geonames"]?.HasValues == true)
                {
                    cityName = locationData["geonames"][0]?["name"]?.ToString() ?? "Unknown";
                    if (cityName != "Unknown")
                        break;
                }
            }
        }

        return cityName;
    }
}

