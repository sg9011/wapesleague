using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Bot.Commands.Base;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Bot.Commands.Crown
{
    public class CrownCommandsModule : BaseMixBotModule<CrownCommandsModule>
    {

        public CrownCommandsModule(IServerWorkflow serverWorkflow, ILogger<CrownCommandsModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {

        }

        [Command("Crown")]
        [Description("Assign the crown to a user")]
        public async Task SetCrown(CommandContext ctx,
            [Description("The discord member you want to assign the crown to")] DiscordMember member)
        {
            try
            {
                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                var nickName = member.Nickname ?? member.DisplayName;
                if (!nickName.EndsWith(Constants.DiscordEmoji.Owner))
                {
                    var newNickName = $"{nickName} {Constants.DiscordEmoji.Owner}";
                    await member.ModifyAsync(m => m.Nickname = newNickName);
                }
            }
            catch(Exception)
            {
                var a = "Z";
            }
        }

        [Command("UnCrown")]
        [Description("Remove the crowns")]
        public async Task RemoveCrown(CommandContext ctx,
    [Description("The discord member you want to assign the crown to")] DiscordMember member = null)
        {
            try
            {
                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                if (member != null)
                {
                    var nickName = member.Nickname ?? member.DisplayName;
                    if (nickName.EndsWith(Constants.DiscordEmoji.Owner))
                    {
                        var newNickName = nickName.Substring(0, nickName.Length - 2).Trim();
                        await member.ModifyAsync(m => m.Nickname = newNickName);
                    }
                }
                else
                {
                    var allMembers = await ctx.Guild.GetAllMembersAsync();
                    var membersWithCrown = allMembers.Where(m => m.Nickname?.EndsWith(Constants.DiscordEmoji.Owner) == true).ToList();
                    foreach (var memberWithCrown in membersWithCrown)
                    {
                        var newNick = memberWithCrown.Nickname.Substring(0, memberWithCrown.Nickname.Length - 2).Trim();
                        await memberWithCrown.ModifyAsync(m => m.Nickname = newNick);
                    }
                }

            }
            catch (Exception)
            {
                var a = "Z";
            }
        }

        [Command("Result")]
        [Description("Result")]
        public async Task SetResult(CommandContext ctx,
            [Description("User1")] DiscordMember member1,
            [Description("User2")] DiscordMember member2,
            [Description("the score")] [RemainingText] string score = null)
        {
            //Winner Of match vs current crown get/keeps the crown
            //Loser of that match gets the skull
            //When the crown changes all skull are removed
            //When 2 skulls play vs each other, the winner loses his skull
            //a player with a skull cant play vs the current crown holder.
            try
            {
                if (string.IsNullOrEmpty(score))
                {
                    await ctx.RespondAsync("I m not Big Brother, Please provide a score aswell: .result @User1 @User2 0-2 for example");
                    return;
                }

                var splitted = score.Split(' ', '-', '_', 'x').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                if (splitted.Length != 2)
                {
                    await ctx.RespondAsync("Please provide a score that is readable for a bot like me: .result @User1 @User2 0-2 for example");
                    return;
                }
                var parsed1 = int.TryParse(splitted[0], out int scoreUser1);
                var parsed2 = int.TryParse(splitted[1], out int scoreUser2);

                if (parsed1 && parsed2)
                {
                    var memberResult = scoreUser1 > scoreUser2
                        ? (member1, member2)
                        : scoreUser2 > scoreUser1
                            ? (member2, member1)
                            : (null, null);

                    if (memberResult.Item1 != null)
                    { 
                        var nickNameOfWinner = memberResult.Item1.Nickname ?? memberResult.Item1.DisplayName;
                        var nickNameOfLoser = memberResult.Item2.Nickname ?? memberResult.Item2.DisplayName;

                        var isCrownMatch = nickNameOfWinner.EndsWith(Constants.DiscordEmoji.Owner) || nickNameOfLoser.EndsWith(Constants.DiscordEmoji.Owner);
                        var isSkullMatch = !isCrownMatch && (nickNameOfWinner.EndsWith(Constants.DiscordEmoji.Skull) && nickNameOfLoser.EndsWith(Constants.DiscordEmoji.Skull));

                        if (!isCrownMatch && !isSkullMatch)
                        {
                            await ctx.RespondAsync("None of you is owner of the current crown and you don't both own the Skull, You Twats!!!");
                        }
                        else if (isCrownMatch)
                        {
                            var winnerIsCurrentCrownHolder = nickNameOfWinner.EndsWith(Constants.DiscordEmoji.Owner);
                            if (!winnerIsCurrentCrownHolder)
                            {
                                //ToDo Add logic here instead of in the 2 if statements underneath this 1
                            }
                            if (!nickNameOfWinner.EndsWith(Constants.DiscordEmoji.Owner))
                            {
                                var newWinnerNickName = $"{nickNameOfWinner} {Constants.DiscordEmoji.Owner}";
                                await memberResult.Item1.ModifyAsync(m => m.Nickname = newWinnerNickName);
                            }

                            if (nickNameOfLoser.EndsWith(Constants.DiscordEmoji.Owner))
                            {
                                var newLoserNickName = nickNameOfLoser.Substring(0, nickNameOfLoser.Length - 2).Trim();
                                await memberResult.Item2.ModifyAsync(m => m.Nickname = newLoserNickName);
                            }
                        }
                        else if (isSkullMatch)
                        {
                            //ToDo
                        }
                    }
                }
                else
                {
                    await ctx.RespondAsync("Please provide a score with numbers that is readable for a bot like me: .result @User1 @User2 0-2 for example");
                    return;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
