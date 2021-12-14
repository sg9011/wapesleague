using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Server;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class ServerButtonWorkflow : BaseWorkflow<ServerButtonWorkflow>, IServerButtonWorkflow
    {
        private readonly IServerWorkflow _serverWorkflow;
        private readonly IServerManager _serverManager;
        private readonly IServerButtonGroupManager _serverButtonGroupManager;
        private readonly IServerButtonManager _serverButtonManager;

        public ServerButtonWorkflow(IServerWorkflow serverWorkflow, IServerManager serverManager, IServerButtonGroupManager serverButtonGroupManager, IServerButtonManager serverButtonManager,
            IMemoryCache cache, IMapper mapper, ILogger<ServerButtonWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _serverWorkflow = serverWorkflow;
            _serverManager = serverManager;
            _serverButtonGroupManager = serverButtonGroupManager;
            _serverButtonManager = serverButtonManager;
        }

        public async Task<DiscordWorkflowResult> HandleAddServerButtonAsync(DiscordCommandPropsDto props, string options)
        {
            var addButtonDto = new AddServerButtonDto();
            try { addButtonDto.MapOptionsToDto(options, ErrorMessages); }
            catch (Exception ex)
            {
                return new DiscordWorkflowResult(string.Format(ErrorMessages.OptionMappingError.GetValueForLanguage(), ex.Message), false);
            }

            if (string.IsNullOrWhiteSpace(addButtonDto.Message) || string.IsNullOrWhiteSpace(addButtonDto.URL))
            {
                return new DiscordWorkflowResult(string.Format(ErrorMessages.PropRequired.GetValueForLanguage(), $"Message and URL"), false);
            }

            var server = await _serverManager.GetServerByDiscordIdAsync(props.ServerId.ToString());
            var serverButtonGroup = server.ButtonGroups.FirstOrDefault(x => x.ButtonGroupType == Data.Entities.Discord.Enums.ButtonGroupType.ShowOneOutOfList);
            if (serverButtonGroup == null)
            {
                serverButtonGroup = new ServerButtonGroup()
                {
                    ServerId = server.ServerId,
                    ButtonGroupType = Data.Entities.Discord.Enums.ButtonGroupType.ShowOneOutOfList,
                    UseRate = addButtonDto.UseRateOverwrite ?? 100m
                };

                await _serverButtonGroupManager.AddAsync(serverButtonGroup);
            }
            else
            {
                if (addButtonDto.UseRateOverwrite.HasValue && serverButtonGroup.UseRate != addButtonDto.UseRateOverwrite)
                {
                    serverButtonGroup.UseRate = addButtonDto.UseRateOverwrite.Value;
                    await _serverButtonGroupManager.UpdateAsync(serverButtonGroup);
                }
            }

            var serverButton = new ServerButton()
            {
                ServerButtonGroupId = serverButtonGroup.ServerButtonGroupId,
                Message = addButtonDto.Message.Trim(),
                URL = addButtonDto.URL.Trim(),
                ShowFrom = DateTimeHelper.GetDatabaseNow()
            };

            await _serverButtonManager.AddAsync(serverButton);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> HandleDeleteServerButtonAsync(Server server, int buttonId)
        {
            var databaseNow = DateTimeHelper.GetDatabaseNow();
            var dbServer = await _serverManager.GetServerByDiscordIdAsync(server.DiscordServerId);
            var button = dbServer.ButtonGroups.SelectMany(x => x.Buttons).FirstOrDefault(z => z.ServerButtonId == buttonId && (!z.ShowUntil.HasValue || z.ShowUntil.Value > databaseNow));
            if (button == null)
            {
                return new DiscordWorkflowResult(ErrorMessages.NothingToDelete.GetValueForLanguage(), false);
            }

            await _serverButtonManager.EndAsync(button.ServerButtonId);
            var parsedDiscordServerId = ulong.Parse(server.DiscordServerId);
            await _serverWorkflow.UpdateServerCacheValueAsync(parsedDiscordServerId);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> HandleGetServerButtonsAsync(Server server)
        {
            var databaseNow = DateTimeHelper.GetDatabaseNow();
            var dbServer = await _serverManager.GetServerByDiscordIdAsync(server.DiscordServerId);
            var stringBuilder = new StringBuilder();
            var hasValue = false;
            foreach (var buttonGroup in dbServer.ButtonGroups.Where(bg => bg.Buttons.Any(b => (!b.ShowFrom.HasValue || b.ShowFrom.Value < databaseNow) && (!b.ShowUntil.HasValue || b.ShowUntil.Value > databaseNow))))
            {
                hasValue = true;
                var formattedPercantage = Math.Round(buttonGroup.UseRate * 100, 2);
                stringBuilder.AppendLine(string.Format(GeneralMessages.ServerButtonGroupInfoString.GetValueForLanguage(), buttonGroup.ButtonGroupType.ToString(), $"{formattedPercantage}%"));
                
                foreach (var button in buttonGroup.Buttons.Where(b => (b.ShowFrom == null || b.ShowFrom.Value < databaseNow) && (b.ShowUntil == null || b.ShowUntil.Value > databaseNow)))
                {
                    stringBuilder.AppendLine($"/t{string.Format(GeneralMessages.ServerButtonInfoString.GetValueForLanguage(), button.ServerButtonId, button.Message, button.URL)}");
                }
                stringBuilder.AppendLine(Bot.DoubleLine);
                stringBuilder.AppendLine();
            }


            if (!hasValue)
            {
                stringBuilder.AppendLine(GeneralMessages.NoButtonsConfiguredOnServer.GetValueForLanguage());
            }

            return new DiscordWorkflowResult(stringBuilder.ToString(), true);
        }
    }
}
