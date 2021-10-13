using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Formation.Constants;
using WaPesLeague.Data.Entities.Formation.Extensions;
using WaPesLeague.Data.Entities.Position.Extensions;
using WaPesLeague.Data.Entities.Position.Seeders;

namespace WaPesLeague.Data.Entities.Formation.Seeders
{
    internal static class FormationSeeder
    {
        internal static void SeedFormations(this ModelBuilder modelBuilder)
        {
            var positions = PositionSeeder.GetPositions(PositionSeeder.GetPositionGroups());
            var formations = GetFormations();
            var formationPositions = GetFormationPositions(formations, positions);

            modelBuilder.Entity<Formation>().HasData(formations);
            modelBuilder.Entity<FormationPosition>().HasData(formationPositions);
        }

        private static List<Formation> GetFormations()
        {
            var formationIdCounter = 0;
            return new List<Formation>
            {
                new Formation
                {
                    FormationId = ++formationIdCounter,
                    Name = FormationConstants._Default433,
                    IsDefault = true
                },
                new Formation
                {
                    FormationId = ++formationIdCounter,
                    Name = FormationConstants._Default442,
                    IsDefault = false
                }
            };
        }

        private static List<FormationPosition> GetFormationPositions(List<Formation> formations, List<Position.Position> positions) 
        {
            var formationPositionIdCounter = 0;
            var formationWaPes433Id = formations.GetFormationIdByName(FormationConstants._Default433);
            var formationWaPes442Id = formations.GetFormationIdByName(FormationConstants._Default442);

            var gkPositionId = positions.GetPositionIdByCode("GK");
            var lbPositionId = positions.GetPositionIdByCode("LB");
            var lcbPositionId = positions.GetPositionIdByCode("LCB");
            var rcbPositionId = positions.GetPositionIdByCode("RCB");
            var rbPositionId = positions.GetPositionIdByCode("RB");
            var lmPositionId = positions.GetPositionIdByCode("LM");
            var cmPositionId = positions.GetPositionIdByCode("CM");
            var lcmPositionId = positions.GetPositionIdByCode("LCM");
            var rcmPositionId = positions.GetPositionIdByCode("RCM");
            var cdmPositionId = positions.GetPositionIdByCode("DMF");
            var camPositionId = positions.GetPositionIdByCode("AMF");
            var rmPositionId = positions.GetPositionIdByCode("RM");

            var lwfPositionId = positions.GetPositionIdByCode("LWF");
            var rwfPositionId = positions.GetPositionIdByCode("RWF");
            var cfPositionId = positions.GetPositionIdByCode("CF");

            return new List<FormationPosition>
            {
                //433
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = gkPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = lbPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = lcbPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = rcbPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = rbPositionId
                },

                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = cdmPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = cmPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = camPositionId
                },

                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = lwfPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = rwfPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes433Id,
                    PositionId = cfPositionId
                },

                //442
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = gkPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = lbPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = lcbPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = rcbPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = rbPositionId
                },

                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = lmPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = lcmPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = rcmPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = rmPositionId
                },

                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = cfPositionId
                },
                new FormationPosition
                {
                    FormationPositionId = ++formationPositionIdCounter,
                    FormationId = formationWaPes442Id,
                    PositionId = cfPositionId
                },
            };
        }
    }
}
