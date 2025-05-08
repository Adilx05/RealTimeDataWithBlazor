using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using RealTimeData.API.Hubs;
using RealTimeData.API.Models;

namespace RealTimeData.API.Services
{
    public class PostgresListenerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PostgresListenerService> _logger;
        private readonly IHubContext<DataNotificationHub> _hubContext;
        private string _connectionString;

        public PostgresListenerService(
            IConfiguration configuration,
            ILogger<PostgresListenerService> logger,
            IHubContext<DataNotificationHub> hubContext)
        {
            _configuration = configuration;
            _logger = logger;
            _hubContext = hubContext;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ListenForNotifications(stoppingToken);
        }

        private async Task ListenForNotifications(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting PostgreSQL notification listener...");

                    await using var connection = new NpgsqlConnection(_connectionString);
                    await connection.OpenAsync(stoppingToken);

                    _logger.LogInformation("Connected to PostgreSQL. Setting up notification listener...");

                    await using var cmd = new NpgsqlCommand("LISTEN new_data_notification;", connection);
                    await cmd.ExecuteNonQueryAsync(stoppingToken);

                    _logger.LogInformation("Listening for 'new_data_notification' events...");

                    connection.Notification += async (sender, args) =>
                    {
                        try
                        {
                            _logger.LogInformation($"Received notification: {args.Payload}");
                            var dataPoint = JsonSerializer.Deserialize<DataPoint>(args.Payload);

                            if (dataPoint != null)
                            {
                                var json = JsonSerializer.Serialize(dataPoint);
                                await _hubContext.Clients.All.SendAsync("ReceiveDataPoint", json, cancellationToken: stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing PostgreSQL notification");
                        }
                    };

                    while (!stoppingToken.IsCancellationRequested)
                    {

                        await connection.WaitAsync(stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("PostgreSQL listener shutting down");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PostgreSQL notification listener");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
    }
}