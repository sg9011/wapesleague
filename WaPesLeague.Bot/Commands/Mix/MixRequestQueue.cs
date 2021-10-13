using System.Collections.Concurrent;
namespace WaPesLeague.Bot.Commands.Mix
{
    public static class MixRequestQueue
    {
        private static ConcurrentQueue<MixRequestDto> _queue;
        public static ConcurrentQueue<MixRequestDto> Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new ConcurrentQueue<MixRequestDto>();
                }

                return _queue;
            }
        }
    }
}
