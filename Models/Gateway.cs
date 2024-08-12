using System.Text.Json.Serialization;

namespace DiscordApp.Models
{
    internal class Gateway
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
