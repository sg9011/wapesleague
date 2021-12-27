using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WaPesLeague.Constants;

namespace WaPesLeague.Data.Entities.Metadata.Seeders
{
    internal static class MetadataSeeder
    {
        internal static void SeedMetadatas(this ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Metadata>().HasData(GetMetadatas());
        }

        internal static List<Metadata> GetMetadatas()
        {
            var idCounter = 0;
            return new List<Metadata>()
            {
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.DiscordId,
                    Description = "The Discord Id of the entity",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.WaPesDiscordName,
                    Description = "The Discord name used to register onto WaPes",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.WaPesPSNName,
                    Description = "The PSN name used to register onto WaPes",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.SpeakEnglish,
                    Description = "can speak english prop on registration",
                    PropertyType = Enums.PropertyType.Bool
                },

                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.FavouritePosition1,
                    Description = "Favourite Position 1 prop on registration",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.FavouritePosition2,
                    Description = "Favourite Position 2 prop on registration",
                    PropertyType = Enums.PropertyType.String
                },

                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.Motto,
                    Description = "Football Motto",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.FootballStyle,
                    Description = "Your FootballStyle",
                    PropertyType = Enums.PropertyType.String
                },

                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.Quality1,
                    Description = "Football Quality 1",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.Quality2,
                    Description = "Football Quality 2",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.Quality3,
                    Description = "Football Quality 3",
                    PropertyType = Enums.PropertyType.String
                },
                new Metadata
                {
                    MetadataId = ++idCounter,
                    Code = MetadataConstants.WaPesJoinDate,
                    Description = "WaPes Joining Date",
                    PropertyType = Enums.PropertyType.DateTime
                }
            };
        }
    }
}
