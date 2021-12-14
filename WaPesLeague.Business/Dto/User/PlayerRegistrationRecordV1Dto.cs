using System;
using System.Text.RegularExpressions;
using WaPesLeague.Business.Dto.FileImport;
using WaPesLeague.Constants;

namespace WaPesLeague.Business.Dto.User
{
    public class PlayerRegistrationRecordV1Dto : BaseFileImportRecordDto
    {
        public DateTime? JoiningDate { get; set; }
        public string GoogleImageLink { get; set; }
        public string DiscordName { get; set; }
        public string PSNName { get; set; }
        public string Status { get; set; }
        public string NationalTeam { get; set; }
        public string OriginalNation { get; set; }
        public int? Age { get; set; }
        public bool English { get; set; }
        public string Position1 { get; set; }
        public string Position2 { get; set; }
        public string Motto { get; set; }
        public string FootballStyle { get; set; }
        public string Quality1 { get; set; }
        public string Quality2 { get; set; }
        public string Quality3 { get; set; }
        public string Email { get; set; }
        public string HowDidYouFindWapes { get; set; }
        public string DiscordId { get; set; }
        public string InternalId { get; set; }

        public string GetDiscordNameWithoutDiscriminator()
        {
            var regex = new Regex(Bot.Regex.EndsWithDiscriminator);
            if ((DiscordName?.Length ?? 0) > 5 && regex.IsMatch(DiscordName))
            {
                return DiscordName.Substring(0, DiscordName.Length - 5);
            }

            return DiscordName;
        }

        public string GetDiscriminator()
        {
            var regex = new Regex(Bot.Regex.EndsWithDiscriminator);
            if ((DiscordName?.Length ?? 0) > 5 && regex.IsMatch(DiscordName))
            {
                return DiscordName.Substring(DiscordName.Length - 4);
            }

            return null;
        }
    }
}
