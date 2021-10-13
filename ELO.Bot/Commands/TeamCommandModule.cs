using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WaPesLeague.Constants.Resources;

namespace ELO.Bot.Commands
{
    public class TeamCommandModule : BaseBotModule<TeamCommandModule>
    {
        public TeamCommandModule(ILogger<TeamCommandModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(logger, errorMessages, generalMessages)
        {

        }
        [Command("Team"), Aliases("AddTeam")]
        [Description("Add a Team.")]
        public async Task HelpMixBot(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (textToIgnore != null)
                {
                    var regex = new Regex(@"[0-2]?[0-9][:,h](([0-5][0-9])|\s)");
                    var hasMatch = regex.IsMatch(textToIgnore);
                }
                
                await ctx.RespondAsync("ELO TIME");
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync($"Something unexpected went wrong. please try again and contact the server admins if the error persists\nError: {ex.Message}");
            }
        }
    }
}
