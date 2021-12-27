using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class DivisionGroup
    {
        public int DivisionGroupId { get; set; }
        public int DivisionRoundId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int? GoogleSheetImportTypeId { get; set; } //This without a foreign key --> if it has a value we will have to do an extra call to load the googleSheetImportType

        public virtual DivisionRound DivisionRound { get; set;}
        public virtual List<DivisionGroupRound> DivisionGroupRounds { get; set; }

    }
}
