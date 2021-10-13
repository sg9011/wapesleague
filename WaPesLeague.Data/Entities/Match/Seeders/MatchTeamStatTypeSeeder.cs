using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Match.Seeders
{
    internal static class MatchTeamStatTypeSeeder
    {
        internal static void SeedMatchTeamStatTypes(this ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<MatchTeamStatType>().HasData(GetMatchTeamStatTypes());
        }

        //https://english.stackexchange.com/questions/197690/when-do-you-use-middle-and-when-center
        //In CSS middle is used for vertical alignment and center for horizontal I think.
        internal static List<MatchTeamStatType> GetMatchTeamStatTypes()
        {
            var idCounter = 0;
            return new List<MatchTeamStatType>()
            {
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Pos",
                    Description = "Possession",
                    Order = 100,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "DefPos",
                    Description = "Possession in defensive third",
                    Order = 110,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "MidPos",
                    Description = "Possession in midfield third",
                    Order = 111,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "AttPos",
                    Description = "Possession in final third",
                    Order = 112,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "LeftPos",
                    Description = "Possession on the left side",
                    Order = 120,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "CenterPos",
                    Description = "Possession in the center of the pitch",
                    Order = 121,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "RightPos",
                    Description = "Possession on the right side",
                    Order = 122,
                },

                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Shots",
                    Description = "Shots",
                    Order = 200,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "ShotsOnTarget",
                    Description = "Shots on target",
                    Order = 201,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "ShotsOnTargetPerc",
                    Description = "Shots on target percentage",
                    Order = 202,
                },

                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Passes",
                    Description = "Passes attempted",
                    Order = 300,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "PassesCompleted",
                    Description = "Passes completed",
                    Order = 301,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "PassPerc",
                    Description = "Passing percentage",
                    Order = 302,
                },

                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Tackles",
                    Description = "Tackles attempted",
                    Order = 400,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "TacklesCompleted",
                    Description = "Tackles completed",
                    Order = 401,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "TacklesPerc",
                    Description = "Tackles percentage",
                    Order = 402,
                },

                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Interceptions",
                    Description = "Interceptions",
                    Order = 500,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Fouls",
                    Description = "Fouls committed",
                    Order = 600,
                },
                new MatchTeamStatType
                {
                    MatchTeamStatTypeId = ++idCounter,
                    Code = "Offides",
                    Description = "Offsides",
                    Order = 601,
                }
            };
        }
    }
}
