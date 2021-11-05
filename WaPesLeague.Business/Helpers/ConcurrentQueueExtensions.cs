using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WaPesLeague.Business.Helpers
{
    public static class ConcurrentQueueExtensions
    {
        public static List<T> TryDeQueueAll<T>(this ConcurrentQueue<T> queue)
        {
            var shouldContinue = true;
            var returnList = new List<T>();
            while (shouldContinue)
            {
                queue.TryDequeue(out var queueItem);
                if (queueItem != null)
                    returnList.Add(queueItem);
                else
                    shouldContinue = false;
            }

            return returnList;
        }
    }
}
