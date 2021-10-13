namespace WaPesLeague.Business.Helpers
{
    public class NotifyRequest
    {
        public ulong RequestedBy { get; set; }
        public ulong RequestedFor { get; set; }
        public ulong RequestedInChannel { get; set; }
        public ulong RequestedInServer { get; set; }

        public NotifyRequest(ulong requestedBy, ulong requestedFor, ulong requestedInChannel, ulong requestedInServer)
        {
            RequestedBy = requestedBy;
            RequestedFor = requestedFor;
            RequestedInChannel = requestedInChannel;
            RequestedInServer = requestedInServer;
        }
    }
}
