using System.Text.Json.Serialization;

namespace DiscordApp.Models
{
    internal class GatewayPayload
    {

        // s and t are null when op is not 0

        [JsonPropertyName("op")]
        public int OPCode { get; set; }

        [JsonPropertyName("d")]
        public GatewayEvent? EventData { get; set; }

        [JsonPropertyName("s")]
        public int? SequenceNumber { get; set; }

        [JsonPropertyName("t")]
        public string? EventName { get; set; }
    }

    internal class GatewayEvent { }

    internal class HelloEvent : GatewayEvent 
    {

        [JsonPropertyName("heartbeat_interval")]
        public int Interval { get; set; }
    }

    internal class ReadyEvent : GatewayEvent
    {
        [JsonPropertyName("v")]
        public int APIVersion { get; set; }
        [JsonPropertyName("user")]
        public DiscordUser User { get; set; }
        [JsonPropertyName("guilds")]
        public List<DiscordGuild> Guilds { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionID { get; set; }
        [JsonPropertyName("resume_gateway_url")]
        public string ResumeGatewayUrl { get; set; }
        [JsonPropertyName("shard")]
        public List<int>? Shard {  get; set; }
        [JsonPropertyName("application")]
        public DiscordApplication Application { get; set; }
    }

    internal class DiscordUser
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("discriminator")]
        public string Discriminator { get; set; }

    }

    internal class DiscordGuild
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }

        [JsonPropertyName("unavilable")]
        public bool Unavailable { get; set; }

    }

    internal class DiscordApplication
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("rpc_origins")]
        public List<string>? RPCOrigins { get; set; }
        [JsonPropertyName("bot_public")]
        public bool BotPublic { get; set; }
        [JsonPropertyName("bot_require_code_grant")]
        public bool BotRequireCodeGrant { get; set; }
        [JsonPropertyName("bot")]
        public DiscordUser? Bot { get; set; }

    }

    internal class InteractionCreateEvent : GatewayEvent
    {
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("application_id")]
        public string ApplicationID { get; set; }
        [JsonPropertyName("type")]
        public int Type { get; set; }
        [JsonPropertyName("token")]
        public string token { get; set; }
    }

}
