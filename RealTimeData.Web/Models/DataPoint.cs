using System.Text.Json.Serialization;

namespace RealTimeData.Web.Models
{
    public class DataPoint
    {
        public int Id { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
    }

}