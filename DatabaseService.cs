using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DatabaseService
{
    private readonly string connectionString = "Server=tcp:weather-server-2.database.windows.net,1433;Initial Catalog=weatherdb;Persist Security Info=False;User ID=adminuser;Password=Admin1234!;MultipleActiveResultSets=False;Encrypt=true;TrustServerCertificate=False;Connection Timeout=30;";

    public async Task<List<object>> GetWeatherData()
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string query = "SELECT * FROM WeatherData2 ORDER BY DateTime DESC";
        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        var results = new List<object>();
        while (await reader.ReadAsync())
        {
            results.Add(new
            {
                City = reader["City"],
                Temperature = reader["Temperature"],
                Humidity = reader["Humidity"],
                WeatherDescription = reader["WeatherDescription"],
                DateTime = reader["DateTime"]
            });
        }

        return results;
    }

    public async Task<List<object>> GetWeatherStats()
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            SELECT City, 
                   MIN(Temperature) AS MinTemperature, 
                   MAX(Temperature) AS MaxTemperature, 
                   AVG(Temperature) AS AverageTemperature
            FROM WeatherData2
            GROUP BY City";

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        var stats = new List<object>();
        while (await reader.ReadAsync())
        {
            stats.Add(new
            {
                City = reader["City"],
                MinTemperature = reader["MinTemperature"],
                MaxTemperature = reader["MaxTemperature"],
                AverageTemperature = reader["AverageTemperature"]
            });
        }

        return stats;
    }

    public async Task SaveWeatherData(string city, float temperature, int humidity, string weatherDescription, DateTime dateTime)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string insertQuery = "INSERT INTO WeatherData2 (City, Temperature, Humidity, WeatherDescription, DateTime) VALUES (@City, @Temperature, @Humidity, @WeatherDescription, @DateTime)";
        using var command = new SqlCommand(insertQuery, connection);

        command.Parameters.AddWithValue("@City", city);
        command.Parameters.AddWithValue("@Temperature", temperature);
        command.Parameters.AddWithValue("@Humidity", humidity);
        command.Parameters.AddWithValue("@WeatherDescription", weatherDescription);
        command.Parameters.AddWithValue("@DateTime", dateTime);

        await command.ExecuteNonQueryAsync();
    }
}
