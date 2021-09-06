using LanguageExt;
using static LanguageExt.Prelude;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriteUtilities.Azure.Blob;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TrashPandaBot.Data.Calendar;

namespace TrashPandaBot
{
    public class ChirpModule : ModuleBase<SocketCommandContext>
    {
        private int _chirpCount;

        public ChirpModule()
        {
            string chirpCountStr = BlobStorageService.ReadBlob("TrashPandaConn", "main", "tideChirpCounter")
                .IfNone(() => { throw new Exception("Unable to retrieve chirp counter. Fix this later, just throw for now."); });
            _chirpCount = int.Parse(chirpCountStr);
        }

        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        [Command("ping")]
        [Summary("Replies to a ping.")]
        public Task PingAsync()
            => ReplyAsync("pong");

        [Command("chirp")]
        [Summary("Increments the Tide Chirp Counter™.")]
        [Alias("chrip", "wtfmox")]
        public Task ChirpAsync()
        {
            _chirpCount++;
            BlobStorageService.WriteBlob("TrashPandaConn", "main", "tideChirpCounter", _chirpCount.ToString());
            return ReplyAsync($"Tide Chirp Counter™: {_chirpCount}");
        }
    }
}
