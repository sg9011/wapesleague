using Microsoft.Extensions.Localization;
using System;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Data.Entities.Mix
{
    public class MixPositionReservation
    {
        public int MixPositionReservationId { get; set; }
        public int MixPositionId { get; set; }
        public int UserId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool IsCaptain { get; set; }
        public string ExtraInfo { get; set; }

        public virtual MixPosition MixPosition { get; set; }
        public virtual User.User User { get; set; }

        public string ToDiscordSting(GeneralMessages generalMessages)
        {
            return string.IsNullOrWhiteSpace(ExtraInfo)
                ? $"{User?.ToGetPlatformDiscordSting(generalMessages)}"
                : $"{User?.ToGetPlatformDiscordSting(generalMessages)} - **{ExtraInfo}**";
        }
    }
}
