using System.Text.Json.Serialization;

namespace RealTimeData.API.Models
{
  
    public class DataPoint
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
    }

}