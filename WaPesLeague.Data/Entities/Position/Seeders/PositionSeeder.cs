using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Position.Constants;
using WaPesLeague.Data.Entities.Position.Extensions;

namespace WaPesLeague.Data.Entities.Position.Seeders
{
    internal static class PositionSeeder
    {
        internal static void SeedPositions(this ModelBuilder modelbuilder)
        {
            var positionGroups = GetPositionGroups();
            var positions = GetPositions(positionGroups);

            modelbuilder.Entity<PositionGroup>().HasData(positionGroups);
            modelbuilder.Entity<Position>().HasData(positions);
        }

        internal static List<PositionGroup> GetPositionGroups()
        {
            var positionGroupIdCounter = 0;

            return new List<PositionGroup>
            {
                new PositionGroup
                {
                    PositionGroupId = ++positionGroupIdCounter,
                    Code = PositionConstants.Group.Goalkeeper,
                    Name = "Goalkeeper",
                    Description = "Container for the goalkeeper position",
                    Order = 1
                },
                new PositionGroup
                {
                    PositionGroupId = ++positionGroupIdCounter,
                    Code = PositionConstants.Group.Defenders,
                    Name = "Defenders",
                    Description = "Container for the defensive positions",
                    Order = 2,
                },
                new PositionGroup
                {
                    PositionGroupId = ++positionGroupIdCounter,
                    Code = PositionConstants.Group.Midfielders,
                    Name = "Midfielders",
                    Description = "Container for the midfield positions",
                    Order = 3
                },
                new PositionGroup
                {
                    PositionGroupId = ++positionGroupIdCounter,
                    Code = PositionConstants.Group.Attackers,
                    Name = "Attackers",
                    Description = "Container for the attacking positions",
                    Order = 4
                }
            };
        }

        internal static List<Position> GetPositions(List<PositionGroup> positionGroups)
        {
            var positionIdCounter = 0;

            var gkPositionGroupId = positionGroups.GetPositionGroupIdByCode(PositionConstants.Group.Goalkeeper);
            var defPositionGroupId = positionGroups.GetPositionGroupIdByCode(PositionConstants.Group.Defenders);
            var midPositionGroupId = positionGroups.GetPositionGroupIdByCode(PositionConstants.Group.Midfielders);
            var attPositionGroupId = positionGroups.GetPositionGroupIdByCode(PositionConstants.Group.Attackers);

            return new List<Position>
            {
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = gkPositionGroupId,
                    ParentPositionId = null,
                    Order = 1,
                    Code = "GK",
                    IsRequiredForMix = false,
                    Description = "Goalkeeper"
                },

                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = null,
                    Order = 1,
                    Code = "LB",
                    IsRequiredForMix = true,
                    Description = "Left Back"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = null,
                    Order = 2,
                    Code = "LWB",
                    IsRequiredForMix = true,
                    Description = "Left Wing Back"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = null,
                    Order = 4,
                    Code = "CB",
                    IsRequiredForMix = true,
                    Description = "Centre Back"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = positionIdCounter - 1,
                    Order = 3,
                    Code = "LCB",
                    IsRequiredForMix = true,
                    Description = "Left Centre Back"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = positionIdCounter - 2,
                    Order = 5,
                    Code = "RCB",
                    IsRequiredForMix = true,
                    Description = "Right Centre Back"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = null,
                    Order = 6,
                    Code = "RB",
                    IsRequiredForMix = true,
                    Description = "Right Back"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = defPositionGroupId,
                    ParentPositionId = null,
                    Order = 7,
                    Code = "RWB",
                    IsRequiredForMix = true,
                    Description = "Right Wing Back"
                },

                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = null,
                    Order = 1,
                    Code = "LM",
                    IsRequiredForMix = true,
                    Description = "Left Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = null,
                    Order = 3,
                    Code = "DMF",
                    IsRequiredForMix = true,
                    Description = "Central Defensive Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = positionIdCounter - 1,
                    Order = 2,
                    Code = "LCDM",
                    IsRequiredForMix = true,
                    Description = "Left Central Defensive Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = positionIdCounter - 2,
                    Order = 4,
                    Code = "RCDM",
                    IsRequiredForMix = true,
                    Description = "Right Central Defensive Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = null,
                    Order = 6,
                    Code = "CM",
                    IsRequiredForMix = true,
                    Description = "Central Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = positionIdCounter - 1,
                    Order = 5,
                    Code = "LCM",
                    IsRequiredForMix = true,
                    Description = "Left Central Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = positionIdCounter - 2,
                    Order = 7,
                    Code = "RCM",
                    IsRequiredForMix = true,
                    Description = "Right Central Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = null,
                    Order = 9,
                    Code = "AMF",
                    IsRequiredForMix = true,
                    Description = "Central Attacking Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = positionIdCounter - 1,
                    Order = 8,
                    Code = "LCAM",
                    IsRequiredForMix = true,
                    Description = "Left Central Attacking Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = positionIdCounter - 2,
                    Order = 10,
                    Code = "RCAM",
                    IsRequiredForMix = true,
                    Description = "Right Central Attacking Midfielder"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = midPositionGroupId,
                    ParentPositionId = null,
                    Order = 11,
                    Code = "RM",
                    IsRequiredForMix = true,
                    Description = "Right Midfielder"
                },

                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = null,
                    Order = 1,
                    Code = "LWF",
                    IsRequiredForMix = true,
                    Description = "Left Wing Forward"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = null,
                    Order = 3,
                    Code = "SS",
                    IsRequiredForMix = true,
                    Description = "Second Striker"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = positionIdCounter - 1,
                    Order = 2,
                    Code = "LSS",
                    IsRequiredForMix = true,
                    Description = "Left Wing Forward"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = positionIdCounter - 2,
                    Order = 4,
                    Code = "RSS",
                    IsRequiredForMix = true,
                    Description = "Right Second Striker"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = null,
                    Order = 6,
                    Code = "CF",
                    IsRequiredForMix = true,
                    Description = "Central Forward"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = positionIdCounter - 1,
                    Order = 5,
                    Code = "LCF",
                    IsRequiredForMix = true,
                    Description = "Left Central Forward"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = positionIdCounter - 2,
                    Order = 7,
                    Code = "RCF",
                    IsRequiredForMix = true,
                    Description = "Right Central Striker"
                },
                new Position
                {
                    PositionId = ++positionIdCounter,
                    PositionGroupId = attPositionGroupId,
                    ParentPositionId = null,
                    Order = 8,
                    Code = "RWF",
                    IsRequiredForMix = true,
                    Description = "Right Wing Forward"
                }
            };
        }

        private static List<PositionTag> GetPositionTags(List<Position> positions)
        {
            var positionTagIdCounter = 0;

            var goalkeeperId = positions.GetPositionIdByCode("GK");
            
            var rightBackId = positions.GetPositionIdByCode("RB");
            var leftBackId = positions.GetPositionIdByCode("LB");
            var centerBackId = positions.GetPositionIdByCode("CB");
            var leftCenterBackId = positions.GetPositionIdByCode("LCB");
            var rightCenterBackId = positions.GetPositionIdByCode("RCB");

            var leftMidfielderId = positions.GetPositionIdByCode("LM");
            var rightMidfielderId = positions.GetPositionIdByCode("RM");
            var defMidfielderId = positions.GetPositionIdByCode("DMF");
            var centralMidfielderId = positions.GetPositionIdByCode("CM");
            var attMidfielderId = positions.GetPositionIdByCode("AMF");

            var leftForwardId = positions.GetPositionIdByCode("LWF");
            var rightForwardId = positions.GetPositionIdByCode("RWF");
            var secondStrikerId = positions.GetPositionIdByCode("SS");
            var centreForwardId = positions.GetPositionIdByCode("CF");

            return new List<PositionTag>
            {
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = goalkeeperId,
                    Tag = "TW"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = goalkeeperId,
                    Tag = "G"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = goalkeeperId,
                    Tag = "1"
                },

                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightBackId,
                    Tag = "DD"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightBackId,
                    Tag = "5"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftBackId,
                    Tag = "DG"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftBackId,
                    Tag = "2"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centerBackId,
                    Tag = "DC"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centerBackId,
                    Tag = "CV"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centerBackId,
                    Tag = "CH"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centerBackId,
                    Tag = "SW"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftCenterBackId,
                    Tag = "DCG"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftCenterBackId,
                    Tag = "3"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightCenterBackId,
                    Tag = "DCD"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightCenterBackId,
                    Tag = "4"
                },

                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftMidfielderId,
                    Tag = "MG"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftMidfielderId,
                    Tag = "LW"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightMidfielderId,
                    Tag = "MD"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightMidfielderId,
                    Tag = "RW"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "MDC"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "CDM"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "Pirlo"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "Vieira"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "Kante"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "DM"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = defMidfielderId,
                    Tag = "6"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centralMidfielderId,
                    Tag = "MC"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centralMidfielderId,
                    Tag = "MR"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centralMidfielderId,
                    Tag = "CMF"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centralMidfielderId,
                    Tag = "8"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "CAM"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "AM"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "AMC"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "MO"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "MOC"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "10"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "Riquelme"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = attMidfielderId,
                    Tag = "Zidane"
                },

                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftForwardId,
                    Tag = "LF"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftForwardId,
                    Tag = "Lang"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftForwardId,
                    Tag = "ATG"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = leftForwardId,
                    Tag = "7"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightForwardId,
                    Tag = "RF"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightForwardId,
                    Tag = "ATD"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = rightForwardId,
                    Tag = "11"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = secondStrikerId,
                    Tag = "F9"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = secondStrikerId,
                    Tag = "FALSE9"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = secondStrikerId,
                    Tag = "FALSE 9"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centreForwardId,
                    Tag = "9"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centreForwardId,
                    Tag = "BU"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centreForwardId,
                    Tag = "ST"
                },
                new PositionTag
                {
                    PositionTagId = ++positionTagIdCounter,
                    PositionId = centreForwardId,
                    Tag = "SP"
                }
            };
        }
    }
}
