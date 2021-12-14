using Base.Bot.Queue.Dto;
using System.Collections.Concurrent;

namespace Base.Bot.Queue
{
    public class GuildMemberUpdatedQueue
    {
        private static ConcurrentQueue<GuildMemberUpdatedDto> _queue;
        public static ConcurrentQueue<GuildMemberUpdatedDto> Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new ConcurrentQueue<GuildMemberUpdatedDto>();
                }

                return _queue;
            }
        }
    }
}
