using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetEnv;

public class DatabaseService
{
    private readonly string connectionString;
    public DatabaseService(){
    Env.Load();
    connectionString =  $"Server=tcp:{Env.GetString("DB_SERVER")},{Env.GetInt("DB_PORT")};" +
                            $"Initial Catalog={Env.GetString("DB_NAME")};" +
                            $"Persist Security Info=False;" +
                            $"User ID={Env.GetString("DB_USER")};" +
                            $"Password={Env.GetString("DB_PASSWORD")};" +
                            $"MultipleActiveResultSets=False;" +
                            $"Encrypt={Env.GetString("DB_ENCRYPT")};" +
                            $"TrustServerCertificate=False;" +
                            $"Connection Timeout={Env.GetInt("DB_TIMEOUT")};";
    // await connection.OpenAsync();
    }

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
            var dateTime = (DateTime)reader["DateTime"];
            results.Add(new
            {
                City = reader["City"],
                Temperature = reader["Temperature"],
                Humidity = reader["Humidity"],
                WeatherDescription = reader["WeatherDescription"],
                DateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss")
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
            // var dateTime = (DateTime)reader["DateTime"];
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
    
public async Task<List<object>> GetWeatherStatsCP(string city)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
    SELECT City, 
           MIN(Temperature) AS MinTemperature, 
           MAX(Temperature) AS MaxTemperature, 
           AVG(Temperature) AS AverageTemperature
    FROM WeatherData2
    WHERE city = @city
    GROUP BY City";


        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@city", city);
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

        // string formattedDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
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
