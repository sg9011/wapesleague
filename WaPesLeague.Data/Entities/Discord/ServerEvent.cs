using WaPesLeague.Data.Entities.Discord.Enums;

namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerEvent
    {
        public int ServerEventId { get; set; }
        public int ServerId { get; set; }
        public EventType EventType { get; set; }
        public string EventValue { get; set; }
        public ActionType ActionType { get; set; }
        public ActionEntity ActionEntity { get; set; }
        public string ActionValue { get; set; }

        public virtual Server Server { get; set; }
    }
}
