using Base.Bot.Queue.Dto;
using System.Collections.Concurrent;

namespace Base.Bot.Queue
{
    public static class GuildMemberAddedQueue
    {
        private static ConcurrentQueue<GuildMemberAddedDto> _queue;
        public static ConcurrentQueue<GuildMemberAddedDto> Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new ConcurrentQueue<GuildMemberAddedDto>();
                }

                return _queue;
            }
        }
    }
}
