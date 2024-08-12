using System.Text.Json.Serialization;

namespace DiscordApp.Models
{
    internal class InteractionResponse
    {
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("data")]
        public InteractionResponseData Data { get; set; }

        public InteractionResponse(int type, InteractionResponseData data)
        {
            this.Type = type;
            this.Data = data;
        }
    }

    internal class InteractionResponseData 
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        public InteractionResponseData(string content)
        {
            this.Content = content;
        }
    }
}
