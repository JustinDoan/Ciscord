namespace DiscordApp
{

    class Program
    {
        static async Task Main(string[] args)
        {
            string token = "";
            // await Commands.RegisterCommands(token);
            DiscordClient client = new DiscordClient(token);
            await client.Start();

        }



    }
}