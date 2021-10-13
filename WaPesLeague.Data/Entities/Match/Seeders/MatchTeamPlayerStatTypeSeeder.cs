using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Match.Seeders
{
    internal static class MatchTeamPlayerStatTypeSeeder
    {
        internal static void SeedMatchTeamPlayerStatTypes(this ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<MatchTeamPlayerStatType>().HasData(GetMatchTeamPlayerStatTypes());
        }

        //https://english.stackexchange.com/questions/197690/when-do-you-use-middle-and-when-center
        //In CSS middle is used for vertical alignment and center for horizontal I think.
        internal static List<MatchTeamPlayerStatType> GetMatchTeamPlayerStatTypes()
        {
            var idCounter = 0;
            return new List<MatchTeamPlayerStatType>()
            {
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "InGameRating",
                    Description = "In Game Rating",
                    Order = 1,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Goals",
                    Description = "Goals",
                    Order = 10,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "OwnGoals",
                    Description = "Own goals",
                    Order = 10,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Assists",
                    Description = "Assists",
                    Order = 20,
                },

                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Shots",
                    Description = "Shots",
                    Order = 100,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "ShotsOnTarget",
                    Description = "Shots on target",
                    Order = 101,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "ShotsOnTargetPerc",
                    Description = "Shots on target percentage",
                    Order = 102,
                },

                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Passes",
                    Description = "Passes attempted",
                    Order = 200,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "PassesCompleted",
                    Description = "Passes completed",
                    Order = 201,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "PassPerc",
                    Description = "Passing percentage",
                    Order = 202,
                },

                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Tackles",
                    Description = "Tackles attempted",
                    Order = 300,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "TacklesCompleted",
                    Description = "Tackles completed",
                    Order = 301,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "TacklesPerc",
                    Description = "Tackles percentage",
                    Order = 302,
                },

                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Interceptions",
                    Description = "Interceptions",
                    Order = 900,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Touches",
                    Description = "Touches",
                    Order = 901,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Fouls",
                    Description = "Fouls",
                    Order = 902,
                },
                new MatchTeamPlayerStatType
                {
                    MatchTeamPlayerStatTypeId = ++idCounter,
                    Code = "Offsides",
                    Description = "Offsides",
                    Order = 903,
                }
            };
        }
    }
}
