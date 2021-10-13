using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Platform.Constants;

namespace WaPesLeague.Data.Entities.Platform.Seeders
{
    internal static class PlatformSeeder
    {
        internal static void SeedPlatforms(this ModelBuilder modelbuilder)
        {
            var platforms = GetPlatforms();
            modelbuilder.Entity<Platform>().HasData(platforms);
        }

        private static List<Platform> GetPlatforms()
        {
            var platformIdCounter = 0;
            return new List<Platform>
            {
                new Platform
                {
                    PlatformId = ++platformIdCounter,
                    Name = PlatformConstants.PlayStationNetwork,
                    Description = "The Playstation Network Platform"
                }
            };
        }
    }
}
