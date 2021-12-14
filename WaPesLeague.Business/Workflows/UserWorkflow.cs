using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.User;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Data.Entities.User;
using WaPesLeague.Data.Managers.Interfaces;
using WaPesLeague.Business.Dto;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Workflows
{
    public class UserWorkflow : BaseWorkflow<UserWorkflow>, IUserWorkflow
    {
        private readonly IUserPlatformManager _userPlatformManager;
        private readonly IPlatformManager _platformManager;
        private readonly IUserMemberManager _userMemberManager;
        private readonly IUserManager _userManager;
        private readonly IServerWorkflow _serverWorkflow;

        public UserWorkflow(IUserPlatformManager userPlatformManager, IPlatformManager platformManager, IUserMemberManager userMemberManager, IUserManager userManager, IServerWorkflow serverWorkflow, IMemoryCache cache, IMapper mapper, ILogger<UserWorkflow> logger
            , ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _userPlatformManager = userPlatformManager;
            _platformManager = platformManager;
            _userMemberManager = userMemberManager;
            _userManager = userManager;
            _serverWorkflow = serverWorkflow;
        }

        public async Task<int> GetOrCreateUserIdByDiscordId(DiscordCommandPropsDto propsDto)
        {
            var userId = await GetUserIdByUserMemberFromCacheOrDbAsync(propsDto);
            
            if (userId == null)
            {
                userId = await _userMemberManager.GetUserIdByDiscordUserIdAsync(propsDto.UserId.ToString());
                if (userId == null)
                {
                    var userToCreate = new User()
                    {
                        UserGuid = Guid.NewGuid(),
                        DiscordName = propsDto.UserName,
                        DiscordDiscriminator = propsDto.DiscordDiscriminator
                    };
                    var createdUser = await _userManager.AddAsync(userToCreate);
                    userId = createdUser.UserId;
                }

                var server = await _serverWorkflow.GetOrCreateServerAsync(propsDto.ServerId, propsDto.ServerName);

                var userMemberToCreate = new UserMember()
                {
                    UserId = userId.Value,
                    ServerId = server.ServerId,
                    DiscordUserName = propsDto.UserName,
                    DiscordNickName = propsDto.NickName,
                    DiscordMention = propsDto.Mention,
                    DiscordUserId = propsDto.UserId.ToString(),
                    ServerJoin = propsDto.ServerJoin,
                    DiscordJoin = propsDto.DiscordJoin
                };
                userId = (await _userMemberManager.AddAsync(userMemberToCreate)).UserId;

                MemoryCache.Set(Cache.GetUserIdByUserAndServerKey(propsDto.UserId, propsDto.ServerId).ToUpperInvariant(), userId.Value, TimeSpan.FromDays(7));
            }
            return userId.Value;
        }

        public async Task<UserPlatformsDto> GetUserPlatformsByUserIdAsync(int userId)
        {
            var userPlatforms = await _userPlatformManager.GetUserPlatformsByUserId(userId);
            return new UserPlatformsDto()
            {
                UserId = userId,
                Platforms = Mapper.Map<IReadOnlyCollection<PlatformUserDto>>(userPlatforms)
            };
        }

        public async Task<string> SetUserPlatformAsync(int userId, string platformName, string userName)
        {
            var platform = await GetPlatformFromCacheOrDbAsync(platformName);
            if (platform == null)
                return string.Format(string.Format(ErrorMessages.NoPlatformForCodeFound2.GetValueForLanguage(), platformName, Bot.Prefix));

            userName = userName?.Trim() ?? string.Empty;
            var userPlatform = await _userPlatformManager.GetUserPlatformByUserId(userId, platform.PlatformId);
            if (userPlatform == null)
            {
                var newUserPlatform = new UserPlatform
                {
                    UserId = userId,
                    PlatformId = platform.PlatformId,
                    UserName = userName
                };
                await _userPlatformManager.AddAsync(newUserPlatform);
                return string.Format(GeneralMessages.AddedUserPlatform.GetValueForLanguage(), userName, platformName);
            }
            else
            {
                var oldUserName = userPlatform.UserName;

                if (!string.Equals(userName, oldUserName))
                {
                    userPlatform.UserName = userName;
                    await _userPlatformManager.UpdateAsync(userPlatform);
                }
                return string.Format(GeneralMessages.UserPlatformUpdated.GetValueForLanguage(), oldUserName, userName, platformName);
            }
        }

        private async Task<Data.Entities.Platform.Platform> GetPlatformFromCacheOrDbAsync(string name)
        {
            if (!MemoryCache.TryGetValue(Cache.GetPlatformId(name).ToUpperInvariant(), out Data.Entities.Platform.Platform cacheEntry))
            {
                cacheEntry = (await _platformManager.GetPlatformsAsync())
                        .SingleOrDefault(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase));
                if (cacheEntry != null)
                    MemoryCache.Set(Cache.GetPlatformId(name).ToUpperInvariant(), cacheEntry, TimeSpan.FromDays(7));
            }

            return cacheEntry;
        }

        private async Task<int?> GetUserIdByUserMemberFromCacheOrDbAsync(DiscordCommandPropsDto propsDto)
        {
            if (!MemoryCache.TryGetValue(Cache.GetUserIdByUserAndServerKey(propsDto.UserId, propsDto.ServerId).ToUpperInvariant(), out int? cacheEntry))
            {
                var userMember = await _userMemberManager.GetUserMemberByDiscordUserIdAndServerIdAsync(propsDto.UserId.ToString(), propsDto.ServerId.ToString());
                if (userMember != null)
                {
                    if (UserMemberChanged(userMember, propsDto))
                    {
                        userMember.DiscordUserName = string.IsNullOrWhiteSpace(propsDto.UserName) ? userMember.DiscordUserName : propsDto.UserName;
                        userMember.DiscordMention = string.IsNullOrWhiteSpace(propsDto.Mention) ? userMember.DiscordMention : propsDto.Mention;
                        userMember.DiscordNickName = string.IsNullOrWhiteSpace(propsDto.NickName) ? userMember.DiscordNickName : propsDto.NickName;
                        userMember.DiscordJoin = propsDto.DiscordJoin;
                        userMember.ServerJoin = propsDto.ServerJoin;
                        userMember = await _userMemberManager.UpdateAsync(userMember);
                    }
                    cacheEntry = userMember.UserId;
                    MemoryCache.Set(Cache.GetUserIdByUserAndServerKey(propsDto.UserId, propsDto.ServerId).ToUpperInvariant(), cacheEntry.Value, TimeSpan.FromDays(1));
                }
            }

            return cacheEntry;
        }

        private static bool UserMemberChanged(UserMember userMember, DiscordCommandPropsDto propsDto)
        {
            return
                (!string.IsNullOrWhiteSpace(propsDto.UserName) && !string.Equals(userMember.DiscordUserName, propsDto.UserName, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(propsDto.NickName) && !string.Equals(userMember.DiscordNickName, propsDto.NickName, StringComparison.InvariantCultureIgnoreCase)) ||
                (userMember.DiscordJoin == null || userMember.DiscordJoin < propsDto.DiscordJoin) ||
                (propsDto.ServerJoin.HasValue && (userMember.ServerJoin == null || userMember.ServerJoin < propsDto.ServerJoin)) ||
                (!string.IsNullOrWhiteSpace(propsDto.Mention) && !string.Equals(userMember.DiscordMention, propsDto.Mention, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
