using Microsoft.EntityFrameworkCore;
using Npgsql;
using RealTimeData.API.Models;
using System.Text.Json;

namespace RealTimeData.API.Services
{
    public class DataService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DataService> _logger;
        private readonly string _connectionString;

        public DataService(IConfiguration configuration, ILogger<DataService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<DataPoint>> GetLatestDataPointsAsync(int count = 100)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = @"
                    SELECT id, value, category, timestamp
                    FROM data_points
                    ORDER BY timestamp DESC
                    LIMIT @count";

                await using var cmd = new NpgsqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("count", count);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    dataPoints.Add(new DataPoint
                    {
                        Value = reader.GetDecimal(1),
                        Category = reader.GetString(2),
                        Timestamp = reader.GetDateTime(3)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data points");
                throw;
            }

            return dataPoints.OrderBy(dp => dp.Timestamp);
        }

        public async Task InsertRandomDataAsync()
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "SELECT insert_random_data();";
                await using var cmd = new NpgsqlCommand(sql, connection);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting random data");
                throw;
            }
        }
    }
}