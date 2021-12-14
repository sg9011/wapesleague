using AutoMapper;
using Base.Bot.Commands;
using DSharpPlus.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class DiscordWorkflow : BaseWorkflow<DiscordWorkflow>, IDiscordWorkflow
    {
        private readonly IUserWorkflow _userWorkflow;
        private readonly IUserManager _userManager;
        private readonly IUserMemberManager _userMemberManager;
        private readonly IServerManager _serverManager;

        private readonly Base.Bot.Bot _bot;
        private const string UserIdMatch = "UserMember Discord Id";
        private const string UserNameMatch = "UserMember Discord UserName";
        private const string UserNameFoldedStringMatch = "UserMember Discord UserName With FoldedString";
        private const string UserNameStartsWithAndDiscriminatorMatch = "UserMember Discord UserName With startsWith FoldedString & Discriminator";

        public DiscordWorkflow(IUserWorkflow userWorkflow, IUserManager userManager, IUserMemberManager userMemberManager, IServerManager serverManager, Base.Bot.Bot bot,
            IMemoryCache cache, IMapper mapper, ILogger<DiscordWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _userWorkflow = userWorkflow;
            _userManager = userManager;
            _userMemberManager = userMemberManager;
            _serverManager = serverManager;
            _bot = bot;
        }
        public async Task HandleScanForMembersAsync()
        {
            var servers = await _serverManager.GetServersAsync();
            var activeServers = servers.Where(x => x.IsActive == true).ToList();
            if (!activeServers.Any())
            {
                Logger.LogWarning("No active servers found in the HandleScanForMembersAsync");
                return;
            }

            var allUsers = (await _userManager.GetAllAsync()).ToList();
            var discordClient = await _bot.GetConnectedDiscordClientAsync();
            foreach (var activeServer in activeServers)
            {
                try
                {
                    var discordServerId = ulong.Parse(activeServer.DiscordServerId);
                    if (discordServerId == 0)
                        continue;
                    
                    var guild = await discordClient.GetGuildAsync(discordServerId);
                    await HandleMembersAsync(activeServer, guild, discordClient.CurrentUser.Id, allUsers);

                }
                catch(Exception ex)
                {
                    Logger.LogError(ex, $"ScanForMembers gave an error for server: {activeServer.ServerId}, {activeServer.ServerName}");
                }
            }
        }

        private async Task HandleMembersAsync(Server activeServer, DiscordGuild guild, ulong botUserId, List<User> allUsers)
        {
            var members = await guild.GetAllMembersAsync();

            var serverUsers = allUsers.Where(u => u.UserMembers.Any(um => um.ServerId == activeServer.ServerId)).ToList();
            var userMembersToAdd = new List<UserMember>();
            var usersToAdd = new List<User>();
            var userMembersToAddAfterUsers = new List<UserMember>();

            foreach (var member in members.Where(m => m.IsBot == false))
            {
                var matchingMember = serverUsers.FirstOrDefault(su => su.UserMembers.Any(um => string.Equals(um.DiscordUserId, member.Id.ToString(), StringComparison.InvariantCultureIgnoreCase)));

                if (matchingMember != null)
                {
                    await UpdateUserIfNeeded(matchingMember, member);
                    var discordCommandProps = new DiscordCommandProperties(member, botUserId);
                    await _userWorkflow.GetOrCreateUserIdByDiscordId(Mapper.Map<DiscordCommandPropsDto>(discordCommandProps));
                }
                else
                {
                    var matchingUser = FindMatchingUser(member, allUsers);

                    if (matchingUser != null)
                    {
                        var userMemberToCreate = new UserMember()
                        {
                            UserId = matchingUser.UserId,
                            ServerId = activeServer.ServerId,
                            DiscordUserName = member.Username,
                            DiscordNickName = member.Nickname ?? member.Username,
                            DiscordMention = member.Mention,
                            DiscordUserId = member.Id.ToString(),
                            ServerJoin = member.JoinedAt.UtcDateTime,
                            DiscordJoin = member.CreationTimestamp.UtcDateTime
                        };

                        userMembersToAdd.Add(userMemberToCreate);

                        await UpdateUserIfNeeded(matchingUser, member);
                        var discordCommandProps = new DiscordCommandProperties(member, botUserId);
                        await _userWorkflow.GetOrCreateUserIdByDiscordId(Mapper.Map<DiscordCommandPropsDto>(discordCommandProps));
                    }
                    else
                    {
                        var userToCreate = new User()
                        {
                            UserGuid = Guid.NewGuid(),
                            DiscordName = member.Username,
                            DiscordDiscriminator = member.Discriminator,
                            ExtraInfo = "Created By ScanMembers",
                        };
                        var userMemberToCreate = new UserMember()
                        {
                            UserId = 0,
                            ServerId = activeServer.ServerId,
                            DiscordUserName = member.Username,
                            DiscordNickName = member.Nickname ?? member.Username,
                            DiscordMention = member.Mention,
                            DiscordUserId = member.Id.ToString(),
                            ServerJoin = member.JoinedAt.UtcDateTime,
                            DiscordJoin = member.CreationTimestamp.UtcDateTime,
                            User = userToCreate
                        };

                        usersToAdd.Add(userToCreate);
                        userMembersToAddAfterUsers.Add(userMemberToCreate);
                    }
                }
            }


            if (userMembersToAdd.Any())
            {
                await _userMemberManager.AddMultipleAsync(userMembersToAdd);
            }

            if (usersToAdd.Any())
            {
                await _userManager.AddMultipleAsync(usersToAdd);
                userMembersToAddAfterUsers.ForEach(x =>
                {
                    x.UserId = x.User.UserId;
                    x.User = null;
                });
                await _userMemberManager.AddMultipleAsync(userMembersToAddAfterUsers);
                allUsers.AddRange(usersToAdd);
            };
        }

        private User FindMatchingUser(DiscordMember member, List<User> allUsers)
        {
            var appliedSearch = UserIdMatch;
            
            var matchingUsers = allUsers.Where(u => u.UserMembers.Any(um => string.Equals(um.DiscordUserId, member.Id.ToString(), StringComparison.InvariantCultureIgnoreCase))).ToList();
            if (!matchingUsers.Any())
            {
                appliedSearch = UserNameMatch;
                matchingUsers = allUsers.Where(u => string.Equals(u.DiscordName, member.Username, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (!matchingUsers.Any())
                {
                    var foldedString = member.Username.FoldToASCII();
                    appliedSearch = UserNameFoldedStringMatch;
                    matchingUsers = allUsers.Where(u => string.Equals(u.DiscordName.FoldToASCII(), foldedString, StringComparison.InvariantCultureIgnoreCase)).ToList();

                    if (!matchingUsers.Any())
                    {
                        var memberDiscriminator = member.Discriminator;
                        var memberDiscordSubstring = member.Username?.Substring(0, (new int[] { 4, member.Username.Length }).Min());
                        appliedSearch = UserNameStartsWithAndDiscriminatorMatch;
                        matchingUsers = allUsers.Where(u => string.Equals(u.DiscordDiscriminator, memberDiscriminator, StringComparison.InvariantCultureIgnoreCase) && string.Equals(u.DiscordName?.Substring(0, (new int[] { 4, u.DiscordName.Length }).Min()), memberDiscordSubstring, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    }
                }
            }

            var returnUser = matchingUsers.FirstOrDefault();

            if (matchingUsers.Count > 1)
            {
                Logger.LogWarning($"Member {member.Username}, {member.Id} on Server {member.Guild.Name}, {member.Guild.Id} had more then 1 user by applying search: {appliedSearch}. Internal userIds: {string.Join(',', matchingUsers.Select(x => x.UserId))}+ We matched the member to userId: {returnUser.UserId}");
            }

            if (string.Equals(appliedSearch, UserNameStartsWithAndDiscriminatorMatch, StringComparison.InvariantCultureIgnoreCase) && matchingUsers.Count > 0)
            {
                Logger.LogWarning($"Member {member.Username}, {member.Id} on Server {member.Guild.Name}, {member.Guild.Id} matched with user: {returnUser.UserId} by {UserNameStartsWithAndDiscriminatorMatch}");
            }

            return returnUser;
        }

        private async Task UpdateUserIfNeeded(User user, DiscordMember discordMember)
        {
            if (!string.Equals(user.DiscordName, discordMember.Username, StringComparison.InvariantCultureIgnoreCase)
                || !string.Equals(user.DiscordDiscriminator, discordMember.Discriminator, StringComparison.InvariantCultureIgnoreCase))
            {
                user.DiscordDiscriminator = discordMember.Discriminator;
                user.DiscordName = discordMember.Username;

                await _userManager.UpdateAsync(Mapper.Map<User>(user));
            }
        }
    }
}
