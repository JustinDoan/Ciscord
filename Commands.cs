using DiscordApp.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DiscordApp
{
    internal class Commands
    {
        public static async Task RegisterCommands(string token)
        {
            string url = "https://discord.com/api/v10/applications/1271971898485641257/guilds/460631433904521216/commands";

            ApplicationCommand slashPing = new ApplicationCommand("ping", 1, "Send a Ping");

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
            string eventJson = JsonSerializer.Serialize(slashPing, options);
            byte[] buffer = Encoding.UTF8.GetBytes(eventJson);

            using (HttpClient client = new HttpClient())
            {

                ByteArrayContent content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bot", token);

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
