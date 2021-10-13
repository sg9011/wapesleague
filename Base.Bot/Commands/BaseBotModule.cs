using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace Base.Bot.Commands
{
    public abstract class BaseBotModule<T> : BaseCommandModule where T : BaseBotModule<T>
    {
        protected readonly ILogger<T> Logger;
        protected readonly ErrorMessages ErrorMessages;
        protected readonly GeneralMessages GeneralMessages;

        public BaseBotModule(ILogger<T> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
        {
            Logger = logger;
            ErrorMessages = errorMessages;
            GeneralMessages = generalMessages;
        }

        protected async Task<bool> ValidateRolesAsync(CommandContext ctx, List<string> allowedRoles)
        {
            var roles = ctx.Member.Roles.Select(r => r.Name).ToList();
            if (allowedRoles.Any(x => roles.Any(r => string.Equals(r, x, StringComparison.InvariantCultureIgnoreCase))))
                return true;

            await ReplyToFailedRequest(ctx.Message, string.Format(ErrorMessages.RoleNotAllowed.GetValueForLanguage(), string.Join(", ", allowedRoles)));
            return false;
        }

        protected async Task<DiscordMember> GetDiscordMemberByMentionAsync(CommandContext ctx, string mention)
        {
            var numberChars = Array.FindAll(mention.ToCharArray(), c => char.IsDigit(c));
            var longMemberId = ulong.Parse(new string(numberChars));
            var aMember = await ctx.Guild.GetMemberAsync(longMemberId);
            return aMember;

        }

        protected void TriggerTypingFireAndForget(CommandContext ctx)
        {
            Task.Run(async () => await ctx.TriggerTypingAsync()).ConfigureAwait(false);
        }

        protected async Task<DiscordMessage> ReplyToFailedRequest(DiscordMessage message, string failedMessage)
        {
            return await new DiscordMessageBuilder()
                .WithContent($"{WaPesLeague.Constants.DiscordEmoji.ThumbsDownString} {failedMessage}")
                .WithReply(message.Id, true)
                .SendAsync(message.Channel);
        }

        protected async Task<DiscordMessage> ReplyToRequest(DiscordMessage message, string messageString)
        {
            return await new DiscordMessageBuilder()
                .WithContent(messageString)
                .WithReply(message.Id, true)
                .SendAsync(message.Channel);
        }
    }
}
