using AutoMapper;
using Base.Bot.Commands;
using DSharpPlus.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Dto.Server;
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
        private readonly IServerRoleManager _serverRoleManager;
        private readonly IUserMemberServerRoleManager _userMemberServerRoleManager;

        private readonly Base.Bot.Bot _bot;
        private const string UserIdMatchSearch = "UserMember Discord Id";
        private const string UserNameMatch = "UserMember Discord UserName";
        private const string UserNameFoldedStringMatch = "UserMember Discord UserName With FoldedString";
        private const string UserNameStartsWithAndDiscriminatorMatch = "UserMember Discord UserName With startsWith FoldedString & Discriminator";


        public DiscordWorkflow(IUserWorkflow userWorkflow, IUserManager userManager, IUserMemberManager userMemberManager, IServerManager serverManager, IServerRoleManager serverRoleManager, 
            IUserMemberServerRoleManager userMemberServerRoleManager, Base.Bot.Bot bot,
            IMemoryCache cache, IMapper mapper, ILogger<DiscordWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _userWorkflow = userWorkflow;
            _userManager = userManager;
            _userMemberManager = userMemberManager;
            _serverManager = serverManager;
            _serverRoleManager = serverRoleManager;
            _userMemberServerRoleManager = userMemberServerRoleManager;
            _bot = bot;
        }
        public async Task HandleScanForMembersAsync()
        {
            var servers = await _serverManager.GetServersAsync();
            var activeServers = servers.Where(x => x.IsActive == true && !string.Equals(x.DiscordServerId, "0", StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (!activeServers.Any())
            {
                Logger.LogWarning("No active servers found in the HandleScanForMembersAsync");
                return;
            }

            var allUsers = (await _userManager.GetAllAsync()).ToList();
            var allServerRoles = await _serverRoleManager.GetAllAsync();
            var discordClient = await _bot.GetConnectedDiscordClientAsync();
            foreach (var activeServer in activeServers)
            {
                try
                {
                    var discordServerId = ulong.Parse(activeServer.DiscordServerId);
                    if (discordServerId == 0)
                        continue;
                    
                    var guild = await discordClient.GetGuildAsync(discordServerId);
                    await HandleMembersAsync(activeServer, guild, discordClient.CurrentUser.Id, allUsers, allServerRoles.Where(r => r.ServerId == activeServer.ServerId).ToList());

                }
                catch(Exception ex)
                {
                    Logger.LogError(ex, $"ScanForMembers gave an error for server: {activeServer.ServerId}, {activeServer.ServerName}");
                }
            }
        }

        private async Task HandleMembersAsync(Server activeServer, DiscordGuild guild, ulong botUserId, List<User> allUsers, List<ServerRole> serverRoles)
        {
            var members = await guild.GetAllMembersAsync();

            var serverUsers = allUsers.Where(u => u.UserMembers.Any(um => um.ServerId == activeServer.ServerId)).ToList();
            var userMembersToAdd = new List<UserMemberAndServerRolesDto>();
            var usersToAdd = new List<User>();
            var userMembersToAddAfterUsers = new List<UserMemberAndServerRolesDto>();
            var allUserMembersAndRoles = new List<UserMemberAndServerRolesDto>();

            foreach (var member in members.Where(m => m.IsBot == false))
            {
                var discordRoles = member.Roles.ToList();
                var mappedRoles = discordRoles?.Select(x => new ServerRoleDto()
                {
                    DiscordRoleId = x.Id.ToString(),
                    RoleName = x.Name
                }).ToList();

                var userThatHasMatchingMember = serverUsers.FirstOrDefault(su => su.UserMembers.Any(um => string.Equals(um.DiscordUserId, member.Id.ToString(), StringComparison.InvariantCultureIgnoreCase)));

                if (userThatHasMatchingMember != null)
                {
                    await UpdateUserIfNeeded(userThatHasMatchingMember, member);
                    var discordCommandProps = new DiscordCommandProperties(member, botUserId);
                    await _userWorkflow.GetOrCreateUserIdByDiscordId(Mapper.Map<DiscordCommandPropsDto>(discordCommandProps));
                    allUserMembersAndRoles.Add(new UserMemberAndServerRolesDto()
                    {
                        UserMember = userThatHasMatchingMember.UserMembers.First(um => um.ServerId == activeServer.ServerId && string.Equals(um.DiscordUserId, member.Id.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                        DiscordServerRoleDtos = mappedRoles
                    });
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

                        userMembersToAdd.Add(new UserMemberAndServerRolesDto()
                        {
                            UserMember = userMemberToCreate,
                            DiscordServerRoleDtos = mappedRoles
                        });

                        await UpdateUserIfNeeded(matchingUser, member);
                        var discordCommandProps = new DiscordCommandProperties(member, botUserId);
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
                        userMembersToAddAfterUsers.Add(new UserMemberAndServerRolesDto()
                        {
                            UserMember = userMemberToCreate,
                            DiscordServerRoleDtos = mappedRoles
                        });
                    }
                }
            }

            await SaveUserAndUserMemberUpdatesAsync(userMembersToAdd, usersToAdd, userMembersToAddAfterUsers, allUsers);

            allUserMembersAndRoles.AddRange(userMembersToAddAfterUsers);
            allUserMembersAndRoles.AddRange(userMembersToAdd);

            await HandleServerRolesAsync(allUserMembersAndRoles, serverRoles, activeServer.ServerId);
            await HandleUserMemberServerRolesAsync(allUserMembersAndRoles, serverRoles, activeServer.ServerId);
        }

        private async Task SaveUserAndUserMemberUpdatesAsync(List<UserMemberAndServerRolesDto> rangeToAdd, List<User> rangeOfUsersToAdd, List<UserMemberAndServerRolesDto> rangeOfUserMembersToAddAfterUsers, List<User> allUsers)
        {
            if (rangeToAdd.Any())
            {
                await _userMemberManager.AddMultipleAsync(rangeToAdd.Select(x => x.UserMember).ToList());
            }

            if (rangeOfUsersToAdd.Any())
            {
                await _userManager.AddMultipleAsync(rangeOfUsersToAdd);
                rangeOfUserMembersToAddAfterUsers.ForEach(x =>
                {
                    x.UserMember.UserId = x.UserMember.User.UserId;
                    x.UserMember.User = null;
                });
                await _userMemberManager.AddMultipleAsync(rangeOfUserMembersToAddAfterUsers.Select(x => x.UserMember).ToList());
                allUsers.AddRange(rangeOfUsersToAdd);
            };
        }

        private async Task HandleServerRolesAsync(List<UserMemberAndServerRolesDto> userMembersAndServerRoles, List<ServerRole> allServerRoles, int serverId)
        {
            var groupedServerRoleDtos = userMembersAndServerRoles.SelectMany(um => um.DiscordServerRoleDtos).GroupBy(x => x.DiscordRoleId);
            var serverRolesToAdd = new List<ServerRole>();
            var serverRolesToUpdate = new List<ServerRole>();
            foreach (var serverRoleDtoGrouping in groupedServerRoleDtos)
            {
                var serverRole = allServerRoles.FirstOrDefault(sr => string.Equals(sr.DiscordRoleId, serverRoleDtoGrouping.Key, StringComparison.InvariantCultureIgnoreCase));
                var roleName = serverRoleDtoGrouping.First().RoleName;
                if (serverRole == null)
                {
                    serverRolesToAdd.Add(new ServerRole()
                    {
                        ServerId = serverId,
                        DiscordRoleId = serverRoleDtoGrouping.Key,
                        Name = roleName
                    });
                }
                else if (!string.Equals(serverRole.Name, roleName, StringComparison.InvariantCultureIgnoreCase))
                {
                    serverRole.Name = roleName;
                    serverRolesToUpdate.Add(serverRole);
                }
            }

            await _serverRoleManager.AddMultipleAsync(serverRolesToAdd);
            await _serverRoleManager.AddMultipleAsync(serverRolesToUpdate);
            allServerRoles.AddRange(serverRolesToAdd);
        }

        private async Task HandleUserMemberServerRolesAsync(List<UserMemberAndServerRolesDto> allUserMembersAndTheirDiscordServerRoles, List<ServerRole> allServerRoles, int serverId)
        {
            var dbUserMembersAndRoles = await _userMemberServerRoleManager.GetAllByServerIdAsync(serverId);
            var userMemberServerRolesToAdd = new List<UserMemberServerRole>();
            var userMemberServerRolesToDelete = new List<UserMemberServerRole>();

            foreach (var userMemberAndRoles in allUserMembersAndTheirDiscordServerRoles)
            {
                var hasDiscordRoles = userMemberAndRoles.DiscordServerRoleDtos?.Any() ?? false;
                
                var currentUserMemberServerRoles = dbUserMembersAndRoles.Where(dbUm => userMemberAndRoles.UserMember.UserMemberId == dbUm.UserMemberId).ToList();
                var hasRolesinDb = currentUserMemberServerRoles?.Any() ?? false;

                if (hasDiscordRoles)
                {
                    var newRolesForUser = new List<ServerRoleDto>();
                        
                    if (hasRolesinDb)
                    {
                        newRolesForUser = userMemberAndRoles.DiscordServerRoleDtos.Where(discordRole => !currentUserMemberServerRoles.Any(dbRole => string.Equals(discordRole.DiscordRoleId, dbRole.ServerRole.DiscordRoleId, StringComparison.InvariantCultureIgnoreCase))).ToList();
                    }
                    else
                    {
                        newRolesForUser = userMemberAndRoles.DiscordServerRoleDtos;
                    }


                    userMemberServerRolesToAdd.AddRange(newRolesForUser.Select(x => new UserMemberServerRole()
                    {
                        UserMemberId = userMemberAndRoles.UserMember.UserMemberId,
                        ServerRoleId = allServerRoles.First(asr => string.Equals(asr.DiscordRoleId, x.DiscordRoleId, StringComparison.InvariantCultureIgnoreCase)).ServerRoleId
                    }));
                }

                if (hasRolesinDb)
                {
                    var deleteRolesForUser = new List<UserMemberServerRole>();
                    deleteRolesForUser = hasDiscordRoles
                        ? currentUserMemberServerRoles.Where(dbRole => !userMemberAndRoles.DiscordServerRoleDtos.Any(discordRole => string.Equals(discordRole.DiscordRoleId, dbRole.ServerRole.DiscordRoleId, StringComparison.InvariantCultureIgnoreCase))).ToList()
                        : currentUserMemberServerRoles;

                    userMemberServerRolesToDelete.AddRange(deleteRolesForUser);
                }
            }
            if (userMemberServerRolesToAdd.Any())
            {
                var addedUserMemberRolesStringBuilder = new StringBuilder();
                var groupedInfo = userMemberServerRolesToAdd.GroupBy(x => x.UserMemberId, y => y.ServerRoleId);
                foreach (var groupInfo in groupedInfo)
                {
                    addedUserMemberRolesStringBuilder.AppendLine($"UserMemberId: {groupInfo.Key}, ServerRolesIds: {string.Join(',', groupInfo)}");
                }
                Logger.LogCritical($"Added userMemberRoles by scanning Discord: \n{addedUserMemberRolesStringBuilder}");
                await _userMemberServerRoleManager.AddMultipleAsync(userMemberServerRolesToAdd);
            }
            if (userMemberServerRolesToDelete.Any())
            {
                var deletedUserMemberRolesStringBuilder = new StringBuilder();
                var groupedInfo = userMemberServerRolesToDelete.GroupBy(x => x.UserMemberId, y => y.ServerRoleId);
                foreach (var groupInfo in groupedInfo)
                {
                    deletedUserMemberRolesStringBuilder.AppendLine($"UserMemberId: {groupInfo.Key}, ServerRolesIds: {string.Join(',', groupInfo)}");
                }
                Logger.LogCritical($"Deleted userMemberRoles by scanning Discord: \n{deletedUserMemberRolesStringBuilder}");
                await _userMemberServerRoleManager.DeleteMultipleAsync(userMemberServerRolesToDelete);
            }
        }

        private User FindMatchingUser(DiscordMember member, List<User> allUsers)
        {
            var appliedSearch = UserIdMatchSearch;

            var matchingUsers = allUsers.Where(u => u.UserMembers.Any(um => string.Equals(um.DiscordUserId, member.Id.ToString(), StringComparison.InvariantCultureIgnoreCase))).ToList();
            if (!matchingUsers.Any())
            {
                var usersWithoutDiscordId = allUsers.Where(x => x.UserMembers == null || x.UserMembers.All(um => string.IsNullOrEmpty(um.DiscordUserId) || string.Equals(um.DiscordUserId, "0", StringComparison.InvariantCultureIgnoreCase))).ToList();
                appliedSearch = UserNameMatch;
                matchingUsers = usersWithoutDiscordId.Where(u => string.Equals(u.DiscordName, member.Username, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (!matchingUsers.Any())
                {
                    var foldedString = member.Username.FoldToASCII();
                    appliedSearch = UserNameFoldedStringMatch;
                    matchingUsers = usersWithoutDiscordId.Where(u => string.Equals(u.DiscordName.FoldToASCII(), foldedString, StringComparison.InvariantCultureIgnoreCase)).ToList();

                    if (!matchingUsers.Any())
                    {
                        var memberDiscriminator = member.Discriminator;
                        var memberDiscordSubstring = member.Username?.Substring(0, (new int[] { 4, member.Username.Length }).Min());
                        appliedSearch = UserNameStartsWithAndDiscriminatorMatch;
                        matchingUsers = usersWithoutDiscordId.Where(u => string.Equals(u.DiscordDiscriminator, memberDiscriminator, StringComparison.InvariantCultureIgnoreCase) && string.Equals(u.DiscordName?.Substring(0, (new int[] { 4, u.DiscordName.Length }).Min()), memberDiscordSubstring, StringComparison.InvariantCultureIgnoreCase)).ToList();
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
