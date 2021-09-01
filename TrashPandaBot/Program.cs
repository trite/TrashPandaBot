using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace TrashPandaBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _handler;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false
            });

            _client.Log += Log;
            _commands.Log += Log;

            _handler = new CommandHandler(_client, _commands);
            await _handler.InstallCommandsAsync();
            

            var token = Environment.GetEnvironmentVariable("TrashPandaBot", EnvironmentVariableTarget.Machine);
            // var token = "Need to store this.";



            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
