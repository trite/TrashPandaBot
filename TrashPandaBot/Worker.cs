using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrashPandaBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _handler;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
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
                if (token is null)
                {
                    token = Environment.GetEnvironmentVariable("TrashPandaBot", EnvironmentVariableTarget.User);
                    if (token is null)
                    {
                        token = Environment.GetEnvironmentVariable("TrashPandaBot", EnvironmentVariableTarget.Process);
                        if (token is null)
                        {
                            token = Environment.GetEnvironmentVariable("TrashPandaBot");
                            if (token is null)
                            {
                                throw new Exception("Are you freaking kidding me?!");
                            }
                        }
                    }
                }
                // var token = "Need to store this.";



                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();

                await Task.Delay(-1);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
