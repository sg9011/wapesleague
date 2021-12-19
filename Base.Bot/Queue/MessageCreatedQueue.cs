using Base.Bot.Queue.Dto;
using System.Collections.Concurrent;

namespace Base.Bot.Queue
{
    public static class MessageCreatedQueue
    {
        private static ConcurrentQueue<MessageCreatedDto> _queue;
        public static ConcurrentQueue<MessageCreatedDto> Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new ConcurrentQueue<MessageCreatedDto>();
                }

                return _queue;
            }
        }
    }
}
