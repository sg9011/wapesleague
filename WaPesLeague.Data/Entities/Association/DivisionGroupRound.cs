using System;
using System.Collections.Generic;

namespace WaPesLeague.Data.Entities.Association
{
    public class DivisionGroupRound
    {
        public int DivisionGroupRoundId { get; set; }
        public int DivisionGroupId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual DivisionGroup DivisionGroup { get; set; }
        public virtual List<Match.Match> Matches { get; set; }
    }
}
