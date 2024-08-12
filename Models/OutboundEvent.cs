using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordApp.Models
{
    internal class OutboundEvent
    {
        [JsonPropertyName("op")]
        public int OPCode { get; set; }

        [JsonPropertyName("d")]
        [JsonConverter(typeof(EventDataConverter))]
        public EventData EventData { get; set; }

        public OutboundEvent(int opCode, EventData eventData)
        {
            this.OPCode = opCode;
            this.EventData = eventData;
        }
    }

    internal class EventData { }
    internal class HeartbeatEvent
    {
        [JsonPropertyName("op")]
        public const int OPCode = 1;

        [JsonPropertyName("d")]
        public int? SequenceNumber { get; set; }

        public HeartbeatEvent(int? sequenceNumber)
        {
            this.SequenceNumber = sequenceNumber;
        }
    }

    internal class IdentifyData : EventData
    {

        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("properties")]
        public IdentifyProperties Properties { get; set; }
        [JsonPropertyName("compress")]
        public bool? Compress { get; set; }
        [JsonPropertyName("large_threshold")]
        public int? LargeThreshold { get; set; }
        [JsonPropertyName("shard")]
        public List<int>? Shard { get; set; }
        [JsonPropertyName("presence")]
        public UpdatePresence? Presence { get; set; }
        [JsonPropertyName("intents")]
        public int Intents { get; set; }

        public IdentifyData(string token, IdentifyProperties properties, UpdatePresence presence, int intents)
        {
            this.Token = token;
            this.Properties = properties;
            this.Presence = presence;
            this.Intents = intents;

        }
    
    }

    internal class IdentifyProperties 
    {
        [JsonPropertyName("os")]
        public string OS { get; set; }
        [JsonPropertyName("browser")]
        public string Browser { get; set; }
        [JsonPropertyName("device")]
        public string Device { get; set; }

        public IdentifyProperties(string os, string browser, string device)
        {
            this.OS = os;
            this.Browser = browser;
            this.Device = device;

        }

    }
    internal class UpdatePresence 
    {
        [JsonPropertyName("since")]
        public int? Since {  get; set; }
        // TODO Implement Activities
        [JsonPropertyName("activities")]
        public List<int> Activities { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("afk")]
        public bool AFK { get; set; }


        public UpdatePresence(int? since, List<int> activities, string status, bool AFK)
        {
            this.Since = since;
            this.Activities = activities;
            this.Status = status;
            this.AFK = AFK;
        }
    }

    internal class EventDataConverter : JsonConverter<EventData>
    {
        public override EventData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Deserialization is not implemented for EventData.");
        }

        public override void Write(Utf8JsonWriter writer, EventData value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                JsonSerializer.Serialize(writer, (EventData)null, options);
                return;
            }

            var type = value.GetType();
            JsonSerializer.Serialize(writer, value, type, options);
        }
    }
}
