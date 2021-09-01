using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashPandaBot
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        public static int ChirpCount = 504;

        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        [Command("chirp")]
        [Summary("Increments the Tide Chirp Counter™.")]
        [Alias("chrip")]
        public Task ChirpAsync()
        {
            ChirpCount++;
            return ReplyAsync($"Tide Chirp Counter™: {ChirpCount}");
        }
    }
}
