using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class MixGroupRoleOpeningWorkflow : BaseWorkflow<MixGroupRoleOpeningWorkflow>, IMixGroupRoleOpeningWorkflow
    {
        private readonly IMixGroupRoleOpeningManager _mixGroupRoleOpeningManager;

        public MixGroupRoleOpeningWorkflow(IMixGroupRoleOpeningManager mixGroupRoleOpeningManager,
            IMemoryCache cache, IMapper mapper, ILogger<MixGroupRoleOpeningWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(cache, mapper, logger, errorMessages, generalMessages)
        {
            _mixGroupRoleOpeningManager = mixGroupRoleOpeningManager;
        }
        public async Task<List<MixGroupRoleOpening>> GetMixGroupRoleOpeningsFromCacheOrDbAsync()
        {
            if (!MemoryCache.TryGetValue(Cache.ActiveMixGroupsRoleOpenings.ToUpperInvariant(), out List<MixGroupRoleOpening> cacheEntry))
            {
                cacheEntry = await _mixGroupRoleOpeningManager.GetActiveMixGroupRoleOpenings();
                if (cacheEntry != null)
                {
                    MemoryCache.Set(Cache.ActiveMixGroupsRoleOpenings.ToUpperInvariant(), cacheEntry, TimeSpan.FromHours(121));
                }
            }

            return cacheEntry;
        }
    }
}
