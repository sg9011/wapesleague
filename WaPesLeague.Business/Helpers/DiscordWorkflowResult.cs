using System.Collections.Generic;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Business.Helpers
{
    public class DiscordWorkflowResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        //Very dirty to do it this way, but it works for now
        public FollowUpParameters FollowUpParameters { get; set; }
        public List<NotifyRequest> NotifyRequests { get; set; }

        public DiscordWorkflowResult(string message, bool success = true, FollowUpParameters followUpParameters = null, List<NotifyRequest> notifyRequests = null)
        {
            Success = success;
            Message = message;
            FollowUpParameters = followUpParameters ?? new FollowUpParameters();
            NotifyRequests = notifyRequests;
        }
    }

    public class FollowUpParameters // A.K.A La Poubelle
    {
        public FollowUpParameters()
        {

        }
        public int MixSessionId { get; set; }
        public int MixChannelId { get; set; }
        public int MixGroupId { get; set; }

        public bool CheckOpenExtraChannel { get; set; }
        public bool UpdateChannelName { get; set; }
        public bool DeleteMixSuccess { get; set; }
        public string ChannelName { get; set; }
        public Server Server { get; set; }
        public ulong DiscordChannelId { get; set; }
        public ulong DiscordMessageId { get; set; }
        public string NotifyUserWithPublicMessage { get; set; }
    }


}
