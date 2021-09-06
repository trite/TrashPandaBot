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
using TriteUtilities.Azure.Blob;

namespace TrashPandaBot
{
    public class Worker : CriticalBackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly BlobStorageService _storageService;

        private DiscordSocketClient _client;
        private CommandService _commands;
        private CommandHandler _handler;

        public Worker(ILogger<Worker> logger,
                      BlobStorageService storageService,
                      IHostApplicationLifetime lifetime,
                      ILogger<CriticalBackgroundService> baseLogger)
            :base(lifetime, baseLogger)
        {
            _logger = logger;
            _storageService = storageService;
        }

        public override async Task DoWork(CancellationToken cancellationToken)
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

                string token = BlobStorageService.ReadBlob("TrashPandaConn", "main", "trashPandaProdToken")
                    .IfNone(() =>
                    {
                        string msg = "Failed to retrieve discord bot token, exiting.";
                        _logger.LogCritical(msg);
                        throw new Exception(msg);
                    });

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
