using Microsoft.AspNetCore.SignalR;
using RealTimeData.API.Models;

namespace RealTimeData.API.Hubs
{
    public class DataNotificationHub : Hub
    {
        private readonly ILogger<DataNotificationHub> _logger;

        public DataNotificationHub(ILogger<DataNotificationHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
    }
}