using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using WaPesLeague.Data.Helpers;

namespace WaPesLeague.Bot.Helpers
{
    public static class DiscordMessageButtonHelper
    {
        public static DiscordMessageBuilder AddDiscordLinkButtonsToMessageIfNeeded(this DiscordMessageBuilder discordMessageBuilder, Data.Entities.Discord.Server server, Random random)
        {
            if (server != null && server.ButtonGroups.Any())
            {
                var dbNow = DateTimeHelper.GetDatabaseNow();
                var buttons = server.ButtonGroups.SelectMany(bg => bg.Buttons.Where(b => (!b.ShowFrom.HasValue || b.ShowFrom.Value < dbNow) && (!b.ShowUntil.HasValue || b.ShowUntil > dbNow))).ToList();
                if (buttons.Any())
                {
                    foreach (var buttonGroupButtons in buttons.GroupBy(x => x.ButtonGroup))
                    {
                        var randomDouble = random.NextDouble() * 100;
                        if (buttonGroupButtons.Key.ButtonGroupType == Data.Entities.Discord.Enums.ButtonGroupType.ShowAllAtTheSameTime && buttonGroupButtons.Key.UseRate > (decimal)randomDouble)
                        {
                            //When there are a lot of buttons it might brake!!!
                            var buttonComponents = new List<DiscordLinkButtonComponent>();
                            foreach (var button in buttonGroupButtons)
                            {
                                buttonComponents.Add(new DiscordLinkButtonComponent(button.URL, button.Message));
                            }
                            discordMessageBuilder.AddComponents(buttonComponents);
                        }
                        else if (buttonGroupButtons.Key.ButtonGroupType == Data.Entities.Discord.Enums.ButtonGroupType.ShowOneOutOfList && buttonGroupButtons.Key.UseRate > (decimal)randomDouble)
                        {
                            var randomValue = random.Next(0, buttonGroupButtons.Count());
                            var buttonToShow = buttonGroupButtons.Skip(randomValue).First();
                            discordMessageBuilder.AddComponents(new DiscordLinkButtonComponent(buttonToShow.URL, buttonToShow.Message));
                        }
                    }
                }
            }

            return discordMessageBuilder;
        }
    }
}
