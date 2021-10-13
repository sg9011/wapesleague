using AutoMapper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Mappers.Formation;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class MixChannelWorkflow : BaseWorkflow<MixChannelWorkflow>, IMixChannelWorkflow
    {
        private readonly IMixSessionWorkflow _mixSessionWorkflow;
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IMixGroupManager _mixGroupManager;
        private readonly IServerManager _serverManager;

        public MixChannelWorkflow(IMixSessionWorkflow mixSessionWorkflow, IMixChannelManager mixChannelManager, IMixGroupManager mixGroupManager,
            IServerManager serverManager,
            IMemoryCache memoryCache, IMapper mapper, ILogger<MixChannelWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(memoryCache, mapper, logger, errorMessages, generalMessages)
        {
            _mixSessionWorkflow = mixSessionWorkflow;
            _mixChannelManager = mixChannelManager;
            _mixGroupManager = mixGroupManager;
            _serverManager = serverManager;
        }

        public async Task<DiscordWorkflowResult> CreateChannelForMixGroupAsync(MixGroup mixGroup, CreateMixRoomGroupDto dto)
        {
            var server = await _serverManager.GetServerByDiscordIdAsync(dto.DiscordServerId.ToString());
            var defaultFormation = server.ServerFormations.Single(sf => sf.IsDefault == true);

            var teamAServerFormation = string.IsNullOrEmpty(dto.TeamAFormation)
                    ? defaultFormation
                    : server.ServerFormations.FirstOrDefault(sf => string.Equals(dto.TeamAFormation, sf.Name, StringComparison.InvariantCultureIgnoreCase)
                        || sf.Tags.Any(t => string.Equals(t.Tag, dto.TeamAFormation, StringComparison.InvariantCultureIgnoreCase)));

            var teamAPositions = teamAServerFormation.MapFormationToMixChannelTeamPositions();

            var defaultTeamA = server.DefaultTeams.Single(dt => dt.Tags.Any(dtt => string.Equals(dtt.Tag, "A", StringComparison.InvariantCultureIgnoreCase)));
            var defaultTeamB = server.DefaultTeams.Single(dt => dt.Tags.Any(dtt => string.Equals(dtt.Tag, "B", StringComparison.InvariantCultureIgnoreCase)));
            
            var teamBServerFormation = string.IsNullOrEmpty(dto.TeamBFormation)
                    ? defaultFormation
                    : server.ServerFormations.FirstOrDefault(sf => string.Equals(dto.TeamBFormation, sf.Name, StringComparison.InvariantCultureIgnoreCase)
                        || sf.Tags.Any(t => string.Equals(t.Tag, dto.TeamAFormation, StringComparison.InvariantCultureIgnoreCase)));

            var teamBPositions = teamBServerFormation.MapFormationToMixChannelTeamPositions();


            var teamA = new MixChannelTeam()
            {
                MixChannelTeamName = dto.TeamAName,
                IsOpen = dto.AIsOpen,
                Tags = dto.TeamATags.Where(x => !string.IsNullOrWhiteSpace(x)).Select(t => new MixChannelTeamTag()
                {
                    Tag = t
                }).ToList(),
                DefaultFormation = teamAPositions

            };

            var teamB = new MixChannelTeam()
            {
                MixChannelTeamName = dto.TeamBName,
                IsOpen = dto.BIsOpen,
                Tags = dto.TeamBTags.Where(x => !string.IsNullOrWhiteSpace(x)).Select(t => new MixChannelTeamTag()
                {
                    Tag = t
                }).ToList(),
                DefaultFormation = teamBPositions
            };

            var mixChannel = new MixChannel()
            {
                MixGroupId = mixGroup.MixGroupId,
                ChannelName = mixGroup.Recurring ? $"{mixGroup.BaseChannelName}{Bot.ChannelNameConnector}{MixChannel.StartOrderNumber}" : mixGroup.BaseChannelName,
                Order = MixChannel.StartOrderNumber,
                DiscordChannelId = dto.DiscordChannelId.ToString(),
                MixChannelTeams = new List<MixChannelTeam>() { teamA, teamB },
                Enabled = true,

            };
            mixChannel = await _mixChannelManager.AddAsync(mixChannel);

            var result = await _mixSessionWorkflow.CreateSessionAsync(mixGroup.ServerId, ulong.Parse(mixChannel.DiscordChannelId));
            result.FollowUpParameters.ChannelName = mixChannel.ChannelName;
            result.FollowUpParameters.UpdateChannelName = true;
            return result;
        }

        public async Task<bool> CreateFollowUpForMixSessionIdAsync(DiscordClient client, int mixGroupId)
        {
            //ToDo check if this works correctly to create mix3 when mix1 fills up after mix2
            try
            {
                var group = await _mixGroupManager.GetMixGroupByIdAsync(mixGroupId);
                if (group == null)
                {
                    throw new Exception(ErrorMessages.NoMixGroupFoundForSession.GetValueForLanguage());
                }

                DiscordChannel discordchannel = null;

                var dbNow = DateTimeHelper.GetDatabaseNow();

                var highestOrderedActiveMixChannel = group.MixChannels
                    .Where(mc => mc.Enabled == true
                        && (mc.MixSessions?.Any(ms => ms.DateClosed == null && ms.DateToClose > dbNow) ?? false))
                    .OrderByDescending(mc => mc.Order)
                    .First();

                var currentMixSessionStartDate = highestOrderedActiveMixChannel.MixSessions.First(ms => ms.DateClosed == null && ms.DateToClose > dbNow).DateStart;
                var mixChannelForNextSession = group.MixChannels.FirstOrDefault(mc => mc.Order == (highestOrderedActiveMixChannel.Order + 1) && mc.Enabled == true); //Only use enabled channels To not fail when someone accidently removed a channel!
                if (mixChannelForNextSession == null)
                {
                    //AutoMapper filtered out all data that could cause any harm
                    mixChannelForNextSession = Mapper.Map<MixChannel>(highestOrderedActiveMixChannel);
                    mixChannelForNextSession.Order = highestOrderedActiveMixChannel.Order + 1;
                    var currentChannelName = highestOrderedActiveMixChannel.ChannelName;
                    var currentOrderSuffixLength = highestOrderedActiveMixChannel.Order.ToString().Length + 1;
                    var currentChannelLength = currentChannelName.Length;

                    mixChannelForNextSession.ChannelName = currentChannelLength > currentOrderSuffixLength
                        ? string.Concat(currentChannelName.Substring(0, currentChannelLength - currentOrderSuffixLength), Bot.ChannelNameConnector, mixChannelForNextSession.Order.ToString())
                        : string.Concat(currentChannelName, Bot.ChannelNameConnector, mixChannelForNextSession.Order.ToString());

                    discordchannel = await (await client.GetChannelAsync(ulong.Parse(highestOrderedActiveMixChannel.DiscordChannelId))).CloneAsync(GeneralMessages.OpenNewMixSessionInChannel.GetValueForLanguage());
                    mixChannelForNextSession.DiscordChannelId = discordchannel.Id.ToString();
                    await discordchannel.ModifyAsync(c => c.Name = mixChannelForNextSession.ChannelName);

                    mixChannelForNextSession = await _mixChannelManager.AddAsync(mixChannelForNextSession);
                }

                if (discordchannel == null)
                {
                    var ulongDiscordChannelId = ulong.Parse(mixChannelForNextSession.DiscordChannelId);
                    try
                    {
                        discordchannel = await client.GetChannelAsync(ulongDiscordChannelId);
                        if (discordchannel == null)
                        {
                            //it will probably never reach this
                            await (await client.GetChannelAsync(ulong.Parse(highestOrderedActiveMixChannel.DiscordChannelId))).SendMessageAsync(ErrorMessages.NoFollowUpChannelFound.GetValueForLanguage());
                            throw new Exception("CreateFollowUpForMixSessionIdAsync discord channel for next channel is null");
                        }
                    }
                    catch (NotFoundException ex)
                    {
                        Logger.LogError(ex, "We could not find the discord channel for the follow up channel.");
                        await _mixChannelManager.DisableChannelAsync(mixChannelForNextSession.MixChannelId);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Something went wrong while getting the new discord channel.");
                        await _mixChannelManager.DisableChannelAsync(mixChannelForNextSession.MixChannelId);
                        throw;
                    }
                }

                var result = await _mixSessionWorkflow.CreateSessionAsync(group.ServerId, ulong.Parse(mixChannelForNextSession.DiscordChannelId), currentMixSessionStartDate, true);
                
                if (result.Success)
                {
                    result = await _mixSessionWorkflow.ShowAsync(group.ServerId, ulong.Parse(mixChannelForNextSession.DiscordChannelId));

                    var allPermissions = discordchannel.PermissionOverwrites.ToList();
                    foreach (var rolePermissions in allPermissions.Where(r => r.Type == OverwriteType.Role))
                    {
                        if (rolePermissions.Denied.HasPermission(Permissions.AccessChannels))
                        {
                            var roleAccessPermissions = rolePermissions.Allowed.Grant(Permissions.AccessChannels);
                            var roleDeniedPermissions = rolePermissions.Denied.Revoke(Permissions.AccessChannels);
                            await discordchannel.AddOverwriteAsync(await rolePermissions.GetRoleAsync(), deny: roleDeniedPermissions, allow: roleAccessPermissions);
                        }
                    }
                }

                await discordchannel.SendMessageAsync(result.Message);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to create a follow up session for the mix");
                return false;
            }
        }
    }
}
