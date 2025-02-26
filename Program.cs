using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CityService>();//add
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<WeatherService>();

var app = builder.Build();

var databaseService = app.Services.GetRequiredService<DatabaseService>();
var weatherService = app.Services.GetRequiredService<WeatherService>();

app.MapGet("/weather", async (HttpContext context) =>
{
    var results = await databaseService.GetWeatherData();
    return Results.Ok(results);
    //await context.Response.WriteAsJsonAsync(results);
});

app.MapGet("/weather/stats", async () =>
{
    var stats = await databaseService.GetWeatherStats();
    return Results.Ok(stats);
});

app.MapGet("/weather/{city}", async (string city) =>
{
    var stats = await databaseService.GetWeatherStatsCP(city);
    return Results.Ok(stats);
});

_ = weatherService.RunBackgroundTask();

// Starting the server
app.Run("http://0.0.0.0:80");
