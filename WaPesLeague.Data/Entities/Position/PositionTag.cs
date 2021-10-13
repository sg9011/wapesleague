using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Data.Entities.Position
{
    public class PositionTag
    {
        public int PositionTagId { get; set; }
        public int PositionId { get; set; }
        public int ServerId { get; set; }
        public bool IsDisplayValue { get; set; }
        public string Tag { get; set; }

        public virtual Position Position { get; set; }
        public virtual Server Server { get; set; }
    }
}
