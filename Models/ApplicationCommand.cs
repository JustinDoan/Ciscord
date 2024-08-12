using System.Text.Json.Serialization;

namespace DiscordApp.Models
{
    internal class ApplicationCommand
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }


        public ApplicationCommand(string name, int type, string description)
        {
            this.Name = name;
            this.Type = type;
            Description = description;
        }

    }
}
