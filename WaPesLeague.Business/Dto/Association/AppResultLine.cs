using System;
using System.Collections.Generic;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Business.Dto.Association
{
    public class AppResultLine
    {
        public string SheetRowId { get; set; }
        public int DivisionGroupDBId { get; set; }
        public int DivisionGroupRoundNumber { get; set; }

        public int MatchNumber { get; set; }
        public string HomeTeam { get; set; }
        public int? HomeAssociationTeamId { get; set; }
        public string AwayTeam { get; set; }
        public int? AwayAssociationTeamId { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public List<AppPlayerResultLine> HomePlayers { get; set; }
        public List<AppPlayerResultLine> AwayPlayers { get; set; }
    }

    public class AppPlayerResultLine
    {
        public string PlayerName { get; set; }
        public string Position { get; set; }
        public decimal KonamiRating { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int PlayerNumberOnSheet { get; set; }
        public Guid? UserGuid { get; set; }
        public int? AssociationPlayerTenantId { get; set; }
        public AssociationTeamPlayer AssociationTeamPlayer { get; set; }
    }
}
