using System.Collections.Concurrent;

namespace WaPesLeague.Bot.Commands.Server
{
    public class ServerRequestQueue
    {
        private static ConcurrentQueue<ServerRequestDto> _queue;
        public static ConcurrentQueue<ServerRequestDto> Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new ConcurrentQueue<ServerRequestDto>();
                }

                return _queue;
            }
        }
    }
}
