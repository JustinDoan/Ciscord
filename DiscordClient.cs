using DiscordApp.Converters;
using DiscordApp.Models;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;


namespace DiscordApp
{
    class DiscordClient
    {
        private ClientWebSocket wsClient;
        public bool Listening;
        private string? BaseGateway;
        private int? Sequence;
        private string Token;
        private const string Version = "10";
        private const string GatewayEncoding = "json";
        private const int PayloadSize = 4096;
        private const string BaseUrl = "https://discord.com/api";
        private const string GatewayApi = BaseUrl + "/gateway";
        private Task? HeartbeatTask;
        private Task? ListenTask;

        public DiscordClient(string authentication)
        {
            this.Token = authentication;
            // Get the Gateway WS url
            GetGatewayUrl();
            // Create Websocket Client
            wsClient = new ClientWebSocket();
        }

        async public Task Start()
        {
            await ConnectToGateway();
            Listen();
            await Identify();
            Console.WriteLine("Identified");
            while (Listening) 
            { 
                // This keeps the client running forever :)
            }
        }

        private async Task Identify()
        {
            // Send an identification event to discord
            IdentifyProperties identifyProperties = new IdentifyProperties("Windows 10", "CSharp Bot", "CSharp Bot");
            UpdatePresence updatePresence = new UpdatePresence(null, new List<int>(), "online", false);
            // 8 is Administrator
            IdentifyData identifyData = new IdentifyData(this.Token, identifyProperties, updatePresence, 8);

            OutboundEvent identifyEvent = new OutboundEvent(2, identifyData);

            await SendEvent(identifyEvent);

        }



        private void GetGatewayUrl()
        {

            using HttpClient client = new HttpClient();

            Task<HttpResponseMessage> responseTask = client.GetAsync(GatewayApi);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;

            Task<string> responseString = response.Content.ReadAsStringAsync();
            responseString.Wait();
            string body = responseString.Result;

            BaseGateway = JsonSerializer.Deserialize<Gateway>(body).Url;

        }

        async private Task ConnectToGateway()
        {
            string gatewayUrl = BuildGatewayUrl();
            Console.WriteLine($"Connecting To {gatewayUrl}");
            try
            {
                await wsClient.ConnectAsync(new Uri(gatewayUrl), CancellationToken.None);
                Console.WriteLine($"Successfully connected To {gatewayUrl}");
                Listening = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        private void Listen()
        {
            Console.WriteLine("Starting Gateway Listener");
            ListenTask = Task.Run(async () =>
            {
                while (Listening)
                {

                    Byte[] receiveBuffer = new byte[PayloadSize];
                    ArraySegment<byte> receiveSegment = new ArraySegment<byte>(receiveBuffer);
                    WebSocketReceiveResult result = await wsClient.ReceiveAsync(receiveSegment, CancellationToken.None);

                    string rawEventString = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        Converters = { new GatewayPayloadTypeConverter() }
                    };

                    GatewayPayload? gatewayPayload = JsonSerializer.Deserialize<GatewayPayload>(rawEventString, options);


                    HandleGatewayEvent(gatewayPayload);

                }
            });
            Console.WriteLine("Gateway Listener successfully started.");

        }


        private void HandleGatewayEvent(GatewayPayload? gatewayPayload)
        {
            if (gatewayPayload != null)
            {

                // Set the Sequence number if it exists, needed for discord to know the current sequence.
                if (gatewayPayload.SequenceNumber != null)
                {
                    Sequence = gatewayPayload.SequenceNumber;
                }

                switch (gatewayPayload.OPCode)
                {
                    case 0:

                        switch (gatewayPayload.EventName)
                        {
                            case "READY":
                                ReadyEvent readyEvent = gatewayPayload.EventData as ReadyEvent;
                                Console.WriteLine("App is Authenticated");
                                break;
                            case "INTERACTION_CREATE":
                                Console.WriteLine("Got Interaction Create");
                                InteractionCreateEvent interactionCreateEvent = gatewayPayload.EventData as InteractionCreateEvent;
                                // Need to respond
                                HandleInteraction(interactionCreateEvent);
                                break;
                        }

                        break;

                    case 10:
                        HelloEvent heartbeatEvent = gatewayPayload.EventData as HelloEvent;
                        if (HeartbeatTask != null)
                        {
                            // This means discord would like an immediate heartbeat
                            SendHeartbeat(heartbeatEvent.Interval);
                        }
                        else
                        {
                            StartHeartbeat(heartbeatEvent.Interval);
                        }
                        break;
                    case 11:
                        Console.WriteLine("Heartbeat ACK from Discord");
                        break;
                    default:
                        Console.WriteLine("Got event with no Opcode?");
                        break;
                }

            }


        }


        private void StartHeartbeat(int interval)
        {
            Console.WriteLine("Starting Heartbeat");
            HeartbeatTask = Task.Run(async () =>
            {
                while (Listening)
                {
                    await Task.Delay(interval);
                    // build our Heartbeat payload
                    SendHeartbeat(interval);
                    Console.WriteLine("Successfully Sent Heartbeat");
                }
            });
           
        }

        private async Task SendEvent(OutboundEvent outboundEvent)
        {

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            string eventJson = JsonSerializer.Serialize(outboundEvent, options);
            byte[] byteArray = Encoding.UTF8.GetBytes(eventJson);
            ReadOnlyMemory<byte> buffer = new ReadOnlyMemory<byte>(byteArray);
            await wsClient.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
        }

        private async void SendHeartbeat(int interval)
        {
            HeartbeatEvent heartbeatEvent = new HeartbeatEvent(interval);
            string heartbeatJson = JsonSerializer.Serialize(heartbeatEvent);

            byte[] byteArray = Encoding.UTF8.GetBytes(heartbeatJson);
            ReadOnlyMemory<byte> buffer = new ReadOnlyMemory<byte>(byteArray);

            await wsClient.SendAsync(buffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
        }


        private string BuildGatewayUrl()
        {
            string baseGateway = BaseGateway;
            UriBuilder builder = new UriBuilder(baseGateway);
            NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);

            query["v"] = Version;
            query["encoding"] = GatewayEncoding;

            builder.Query = query.ToString();

            string finalUrL = builder.ToString();

            return finalUrL;

        }

        private async void HandleInteraction(InteractionCreateEvent interactionEvent)
        {
            string url = $"https://discord.com/api/v10/interactions/{interactionEvent.ID}/{interactionEvent.token}/callback";
            await Task.Delay(2000);
            InteractionResponseData data = new InteractionResponseData("Pong!");
            InteractionResponse interactionResponse = new InteractionResponse(4, data);

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            string eventJson = JsonSerializer.Serialize(interactionResponse, options);
            byte[] buffer = Encoding.UTF8.GetBytes(eventJson);

            using (HttpClient client = new HttpClient())
            {

                ByteArrayContent content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                HttpResponseMessage response = await client.SendAsync(request);


                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                else
                {
                    Console.WriteLine("Error sending commands");
                    Console.WriteLine(response.StatusCode);
                }
            }

        }




    }
}
