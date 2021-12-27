using System.Collections.Generic;
using WaPesLeague.Business.Dto.FileImport;

namespace WaPesLeague.Business.Dto.Association
{
    public class LeagueResultLineV1Dto : BaseFileImportRecordDto
    {
        //{"N°":"4","Journée":"J01","Match":"Match 1","Equipe":"PMG Team","Adversaire":"vPestiamo","Score Dom":"3","Score Ext":"4","Nom Joueur 1":"","Poste Joueur 1":"GK","Note Joueur 1":"","Buts Joueur 1":"","Passes Joueur 1":"","Nom Joueur 2":"CdC_4rm4nd090","Poste Joueur 2":"LB","Note Joueur 2":"6","Buts Joueur 2":"","Passes Joueur 2":"","Nom Joueur 3":"PC180","Poste Joueur 3":"CB","Note Joueur 3":"4","Buts Joueur 3":"","Passes Joueur 3":"","Nom Joueur 4":"zagor_nick08","Poste Joueur 4":"CB","Note Joueur 4":"4,5","Buts Joueur 4":"","Passes Joueur 4":"","Nom Joueur 5":"Bicione76","Poste Joueur 5":"RB","Note Joueur 5":"7","Buts Joueur 5":"1","Passes Joueur 5":"","Nom Joueur 6":"Narcotor2","Poste Joueur 6":"DMF","Note Joueur 6":"5,5","Buts Joueur 6":"","Passes Joueur 6":"","Nom Joueur 7":"cheffone13","Poste Joueur 7":"CMF","Note Joueur 7":"4,5","Buts Joueur 7":"","Passes Joueur 7":"","Nom Joueur 8":"FadeToBlack1983","Poste Joueur 8":"AMF","Note Joueur 8":"6,5","Buts Joueur 8":"","Passes Joueur 8":"1","Nom Joueur 9":"Evilkain","Poste Joueur 9":"LW","Note Joueur 9":"6,5","Buts Joueur 9":"","Passes Joueur 9":"1","Nom Joueur 10":"JohnnyAce1989","Poste Joueur 10":"RW","Note Joueur 10":"4,5","Buts Joueur 10":"","Passes Joueur 10":"","Nom Joueur 11":"blackrobuf","Poste Joueur 11":"CF","Note Joueur 11":"7,5","Buts Joueur 11":"2","Passes Joueur 11":"","Nom Joueur 12":"daniel_nine","Poste Joueur 12":"LB","Note Joueur 12":"","Buts Joueur 12":"","Passes Joueur 12":"","Nom Joueur 13":"Jacoma091","Poste Joueur 13":"CMF","Note Joueur 13":"","Buts Joueur 13":"","Passes Joueur 13":"","Nom Joueur 14":"Rivuzza","Poste Joueur 14":"DMF","Note Joueur 14":"","Buts Joueur 14":"","Passes Joueur 14":"","Nom Joueur 15":"BAMBA8F","Poste Joueur 15":"SS","Note Joueur 15":"","Buts Joueur 15":"","Passes Joueur 15":"","Nom Joueur 16":"","Poste Joueur 16":"","Note Joueur 16":"","Buts Joueur 16":"","Passes Joueur 16":"","Nom Joueur 17":"","Poste Joueur 17":"","Note Joueur 17":"","Buts Joueur 17":"","Passes Joueur 17":"","Nom Joueur 18":"","Poste Joueur 18":"","Note Joueur 18":"","Buts Joueur 18":"","Passes Joueur 18":"","Nom Joueur 19":"","Poste Joueur 19":"","Note Joueur 19":"","Buts Joueur 19":"","Passes Joueur 19":"","Nom Joueur 20":"","Poste Joueur 20":"","Note Joueur 20":"","Buts Joueur 20":"","Passes Joueur 20":"","Nom Joueur 21":"","Poste Joueur 21":"","Note Joueur 21":"","Buts Joueur 21":"","Passes Joueur 21":"","Nom Joueur 22":"","Poste Joueur 22":"","Note Joueur 22":"","Buts Joueur 22":"","Passes Joueur 22":"","Email":"patapem@gmail.com","Match_":"WL1Match 1PMG TeamvPestiamo","Score Dom_":"3","Score Ext_":"4","Score":"3-4","Erreur reportée":"","Match + journée":"WL1J01Match 1PMG TeamvPestiamo"}
        //{"N°":"4","Journée":"J01","Match":"Match 1","Equipe":"PMG Team","Adversaire":"vPestiamo","Score Dom":"3","Score Ext":"4",
        //"Email":"patapem@gmail.com","Match_":"WL1Match 1PMG TeamvPestiamo","Score Dom_":"3","Score Ext_":"4","Score":"3-4","Erreur reportée":"","Match + journée":"WL1J01Match 1PMG TeamvPestiamo"}
        public string SheetRowId { get; set; } 
        public string GroupRound { get; set; }
        public int MatchNumber { get; set; }
        public string HomeTeam { get; set; } 
        public string AwayTeam { get; set; } 
        public int HomeScore { get; set; }
        public int HomeScoreConfirmed { get; set; }
        public int AwayScore { get; set; }
        public int AwayScoreConfirmed { get; set; }
        public string Score { get; set; }
        public string EmailResultPoster { get; set; }
        public string MatchIdentifierOnGoogleSheet { get; set; }
        public string ReportingError { get; set; }
        public List<LeagueResultLinePlayer> Players { get; set; }

        public LeagueResultLineV1Dto()
        {
            Players = new List<LeagueResultLinePlayer>();
        }
    }

    public class LeagueResultLinePlayer
    {
        //"Nom Joueur 1":"","Poste Joueur 1":"GK","Note Joueur 1":"","Buts Joueur 1":"","Passes Joueur 1":""
        public string PlayerName { get; set; }
        public string Position { get; set; }
        public decimal KonamiRating { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int PlayerNumberOnSheet { get; set; }
        
    }

}
