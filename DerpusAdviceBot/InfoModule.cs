using System.Threading.Tasks;
using Discord.Commands;

namespace DerpusAdviceBot
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        //todo configure for help and list of commands

        // ~say hello world -> hello world
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);

        // ReplyAsync is a method on ModuleBase
	}
}
