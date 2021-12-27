using System;

namespace WaPesLeague.Data.Entities.Discord
{
    public class ServerButton
    {
        public int ServerButtonId { get; set; }
        public int ServerButtonGroupId { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public DateTime? ShowFrom { get; set; }
        public DateTime? ShowUntil { get; set; }
        

        public virtual ServerButtonGroup ButtonGroup { get; set; }

    }
}
