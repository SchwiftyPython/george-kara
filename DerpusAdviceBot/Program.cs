using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DerpusAdviceBot
{
    internal static class Prefix
    {
        public const string Info = @"[INFO]";
        public const string Error = @"[ERROR]";
        public const string Alert = @"[ALERT]";
        public const string Hint = @"[HINT]";
    }

    public class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _commandHandler;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            var token = File.ReadAllText("token.txt");

            await _client.LoginAsync(TokenType.Bot,
                token);

            await _client.StartAsync();

            _client.Ready += ClientOnReady;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private async Task ClientOnReady()
        {
            Console.WriteLine($"{Prefix.Info} Connected to Discord successfully! ({_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator})");

            _commandHandler = new CommandHandler(_client, new CommandService());
            await _commandHandler.InstallCommandsAsync();
            Console.WriteLine($"{Prefix.Info} Registered chat commands.");

            Console.WriteLine($"{Prefix.Info} Now playing Diablo 2: RizzyResurrected");
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }

    
}

