using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Platform;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class PlatformWorkflow : BaseWorkflow<PlatformWorkflow>, IPlatformWorkflow
    {
        private readonly IPlatformManager _platformManager;
        public PlatformWorkflow(IPlatformManager platformManager, IMemoryCache memoryCache, IMapper mapper, ILogger<PlatformWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base (memoryCache, mapper, logger, errorMessages, generalMessages)
        {
            _platformManager = platformManager;
        }

        public async Task<IReadOnlyCollection<SimplePlatformDto>> GetAllPlatformsAsync()
        {
            if (MemoryCache.TryGetValue(Cache.AllSimplePlatforms, out IReadOnlyCollection<SimplePlatformDto> cacheEntry))
                return cacheEntry;

            var platforms = await _platformManager.GetPlatformsAsync();
            var mappedPlatforms = Mapper.Map<IReadOnlyCollection<SimplePlatformDto>>(platforms);

            MemoryCache.Set(Cache.AllSimplePlatforms, mappedPlatforms, TimeSpan.FromDays(6));

            return mappedPlatforms;
        }

        public async Task<PlatformWithUsersDto> GetPlatformWithUsersAsync(string name, ulong discordServerId)
        {
            var platform = await _platformManager.GetPlatformWithPlatformUserIdsByNameAsync(name, discordServerId.ToString());
            var platformDto = Mapper.Map<PlatformWithUsersDto>(platform);
            return platformDto;
        }
    }
}
