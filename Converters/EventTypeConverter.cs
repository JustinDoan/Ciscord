using DiscordApp.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordApp.Converters
{
    internal class GatewayPayloadTypeConverter : JsonConverter<GatewayPayload>
    {
        public override GatewayPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            { 
            
                GatewayPayload gatewayPayload = new GatewayPayload();

                JsonElement root = document.RootElement;

                // Parse non-dynamic properties first
                if (root.TryGetProperty("op", out JsonElement opElement) && opElement.TryGetInt32(out int opCode))
                {
                    gatewayPayload.OPCode = opCode;
                }

                if (root.TryGetProperty("s", out JsonElement seqElement) && seqElement.ValueKind != JsonValueKind.Null && seqElement.TryGetInt32(out int sequenceNumber))
                {
                    gatewayPayload.SequenceNumber = sequenceNumber;
                }

                if (root.TryGetProperty("t", out JsonElement eventElement))
                {
                    gatewayPayload.EventName = eventElement.GetString();
                }


                JsonElement dynamicEventElement = root.GetProperty("d");

                // Determine type to parse into based off OPCode

                switch (gatewayPayload.OPCode)
                {
                    case 0:

                        switch (gatewayPayload.EventName)
                        {
                            case "READY":
                                ReadyEvent? readyEvent = JsonSerializer.Deserialize<ReadyEvent>(dynamicEventElement);
                                gatewayPayload.EventData = readyEvent;
                                break;

                            case "INTERACTION_CREATE":
                                InteractionCreateEvent? interactionCreateEvent = JsonSerializer.Deserialize<InteractionCreateEvent>(dynamicEventElement);
                                gatewayPayload.EventData = interactionCreateEvent;
                                break;
                        }
                        break;

                    case 10:

                        // Hello
                        HelloEvent? heartbeatEvent = JsonSerializer.Deserialize<HelloEvent>(dynamicEventElement);
                        gatewayPayload.EventData = heartbeatEvent;
                        break;


                    default:
                        // none
                        gatewayPayload.EventData = null;
                        break;
                }


                return gatewayPayload;

            }
            
        }

        public override void Write(Utf8JsonWriter reader, GatewayPayload gatewayPayload, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
