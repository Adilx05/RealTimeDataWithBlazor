using Microsoft.AspNetCore.SignalR.Client;
using RealTimeData.Web.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace RealTimeData.Web.Services
{
    public class DataService : IAsyncDisposable
    {
        private readonly HttpClient _httpClient;
        private HubConnection? _hubConnection;
        public event Action<DataPoint>? OnDataPointReceived;
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task InitializeSignalRConnection()
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7191/datahub")
                    .WithAutomaticReconnect()
                    .Build();


                _hubConnection.On<string>("ReceiveDataPoint", json =>
                {
                    var dataPoint = JsonSerializer.Deserialize<DataPoint>(json);
                    if (dataPoint != null)
                    {
                        Console.WriteLine("🔥 SIGNALR JSON VERİSİ:");
                        Console.WriteLine($"  Category: {dataPoint.Category}");
                        Console.WriteLine($"  Value   : {dataPoint.Value}");
                        Console.WriteLine($"  Time    : {dataPoint.Timestamp:yyyy-MM-dd HH:mm:ss.fff}");

                        OnDataPointReceived?.Invoke(dataPoint);
                    }
                    else
                    {
                        Console.WriteLine("❌ JSON parse edilemedi!");
                    }
                });


                await _hubConnection.StartAsync();
            }
        }

        public async Task<List<DataPoint>> GetLatestDataPointsAsync(int count = 100)
        {
            return await _httpClient.GetFromJsonAsync<List<DataPoint>>($"api/data?count={count}") ?? new List<DataPoint>();
        }

        // For testing - triggers the backend to insert a random data point
        public async Task GenerateRandomDataPointAsync()
        {
            await _httpClient.PostAsync("api/data/generate", null);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}